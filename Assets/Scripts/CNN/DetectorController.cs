using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

using GoogleARCore;
using GoogleARCore.Examples.Common;
using TFClassify;
using TensorFlow;
using OpenCVForUnity;


#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class DetectorController : MonoBehaviour
{

    #region PUBLIC_MEMBERS
    public TextAsset modelFile; // .pb or .bytes file    
    public int cropMargin = 0;

    //public Scalar lowerHSV { get; set; } = new Scalar(0, 40, 125);
    //public Scalar upperHSV { get; set; } = new Scalar(179, 255, 255);
    public Scalar lowerHSV { get; set; } = new Scalar(135, 30, 140);
    public Scalar upperHSV { get; set; } = new Scalar(218, 100, 235);
    public float m_ForwardDistance { get; set; } = 0;
    public float m_CameraShoeDistance { get; set; } = -0.05f;
    public float m_RepeatRate { get; set; } = 0.05f;
    public Boolean m_IsRepeating { get; set; } = false;
    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    private bool mAccessCameraImage = true;

    private static int INPUT_CNN_SIZE = 416;
    private int inputCameraWidth = -1, inputCameraHeight = -1;
    private FootDetector footDetector;
    private List<BoxOutline> boxOutlines;
    private int footPosX, footPosY;
    private float footAngleDegree = 90;
    private bool useTFDetect = false;
    private ShoeController m_ShoeController;
    #endregion // PRIVATE_MEMBERS

    public GameObject TrackedPlanePrefab;

    #region FOR_DEBUG
    public bool m_IsDebug = false;
    public RawImage m_DebugImage1;
    public RawImage m_DebugImage2;
    public UnityEngine.UI.Text m_DebugText;
    public static string m_DebugStr = "";
    private List<GameObject> m_PlaneObjects = new List<GameObject>();

    #endregion // FOR_DEBUG

    public static Texture2D LoadImage(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(INPUT_CNN_SIZE, INPUT_CNN_SIZE);
            tex.LoadImage(fileData);
        }
        return tex;
    }

    void Start()
    {
        m_ShoeController = FindObjectOfType<ShoeController>();

        // load tensorflow model
        LoadWorker();
    }

    public void AttachShoe()
    {
        GuessAngle();
        ResetShoePosition();
        m_ShoeController.ActivateShoe();
    }

    public void ClickRepeatButton()
    {
        m_IsRepeating = !m_IsRepeating;

        var func = nameof(AttachShoe);

        if (m_IsRepeating)
        {
            CancelInvoke(func);
            InvokeRepeating(func, 0, m_RepeatRate);
        }
        else
        {
            CancelInvoke(func);
        }
    }

    private Vector3 ChangeScreenPosToWorldPos(float screenPosX, float screenPosY, float distance)
    {
        // BottomLeft가 (0,0)인 것을 Center가 (0,0)이 되도록 변경
        screenPosX = screenPosX - (inputCameraWidth / 2);
        screenPosY = screenPosY - (inputCameraHeight / 2);

        float clippingWidth, clippingHeight, far;
        float fieldOfView = 30 * (Mathf.PI / 180);

        // World에서 Screen의 크기 계산
        far = 200;
        clippingHeight = Mathf.Tan(fieldOfView) * far * 2;
        clippingWidth = clippingHeight * ((float)inputCameraWidth / inputCameraHeight);

        float shoeX, shoeY, shoeZ = distance;

        shoeX = screenPosX * (clippingWidth / inputCameraWidth);
        shoeY = screenPosY * (clippingHeight / inputCameraHeight);

        shoeX = shoeX * shoeZ / far;
        shoeY = shoeY * shoeZ / far;

        return new Vector3(shoeX, shoeY, shoeZ);
    }

    private void ResetShoePosition()
    {
        float shoeDistance = 0.55f;

        var cameraObject = GameObject.Find("First Person Camera");
        var cameraCenterObject = GameObject.Find("Camera Center");
        var shoeDecoyObject = GameObject.Find("Shoe Decoy");
        var shoeObject = m_ShoeController.shoes;

        shoeObject.transform.parent = null;

        var cameraPosition = cameraObject.transform.position;
        var centerPosition = cameraCenterObject.transform.position;
        var cameraToPlaneVector = (centerPosition - cameraPosition).normalized;

        // Check to find ground plane
        List<DetectedPlane> allPlanes = new List<DetectedPlane>();
        Session.GetTrackables<DetectedPlane>(allPlanes, TrackableQueryFilter.All);

        // If ground plane exist, calculate z value of shoes object at camera coordinate
        if (allPlanes.Count > 0 && allPlanes[0] != null)
        {
            var planeCenter = allPlanes[0].CenterPose.position;

            var intersectionY = planeCenter.y;
            float t = (intersectionY - cameraPosition.y) / cameraToPlaneVector.y;
            var intersectionX = cameraToPlaneVector.x * t + cameraPosition.x;
            var intersectionZ = cameraToPlaneVector.z * t + cameraPosition.z;

            float distanceCameraToPlane = Vector3.Distance(cameraPosition, new Vector3(intersectionX, intersectionY, intersectionZ));

            Debug.Log(string.Format("distance: {0}", distanceCameraToPlane));
            shoeDistance = distanceCameraToPlane;
        }

        #region DEBUG
        shoeDistance += m_CameraShoeDistance;
        #endregion

        // Change detected foot position to world position
        Vector3 shoePos = ChangeScreenPosToWorldPos(footPosX, footPosY, shoeDistance);

        if (inputCameraWidth == -1)
        {
            shoePos.x = 0;
            shoePos.y = 0;
        }

        // Reset decoy object position and angle
        Quaternion initRotation = Quaternion.Euler(0, 0, 0);
        shoeDecoyObject.transform.rotation = initRotation;

        shoeDecoyObject.transform.parent = cameraObject.transform;
        shoeDecoyObject.transform.localPosition = shoePos;

        shoeDecoyObject.transform.parent = null;

        // First, make decoy shoe and camera to be parallels
        var cameraObjectForward = cameraCenterObject.transform.forward;
        var shoeOjbectForward = shoeDecoyObject.transform.forward;

        cameraObjectForward.y = 0; // Project to x-z plane
        shoeOjbectForward.y = 0; // Project to x-z plane

        bool angleDirectionIsUp = Vector3.Cross(cameraObjectForward, shoeOjbectForward).y > 0 ? true : false;
        var cameraShoeAngle = Vector3.Angle(cameraObjectForward, shoeOjbectForward);

        if (angleDirectionIsUp)
        {
            cameraShoeAngle = -cameraShoeAngle;
        }

        Debug.Log(string.Format("Camera-Shoe Angle: {0}", cameraShoeAngle));
        Debug.Log(string.Format("Diff Angle: {0}", (90 - footAngleDegree - 90)));

        // Second, Rotate shoe about detect angle
        cameraShoeAngle = cameraShoeAngle + (footAngleDegree - 90);
        shoeDecoyObject.transform.rotation = Quaternion.Euler(0, cameraShoeAngle, 0);
        shoeDecoyObject.transform.position = (shoeDecoyObject.transform.position - shoeDecoyObject.transform.forward * m_ForwardDistance);

        // Finally, Project decoy object position and rotation to shoe object
        shoeObject.transform.rotation = shoeDecoyObject.transform.rotation;
        shoeObject.transform.position = shoeDecoyObject.transform.position;
    }

    void Update()
    {
        if (m_IsDebug)
        {
            m_DebugImage1.gameObject.SetActive(true);
            m_DebugImage2.gameObject.SetActive(true);
            //m_DebugText.gameObject.SetActive(true);
            //m_DebugText.text = m_DebugStr;
        }
        else
        {
            m_DebugImage1.gameObject.SetActive(false);
            m_DebugImage2.gameObject.SetActive(false);
            m_DebugText.gameObject.SetActive(false);
        }

        List<DetectedPlane> newPlanes = new List<DetectedPlane>();
        Session.GetTrackables<DetectedPlane>(newPlanes, TrackableQueryFilter.New);

        // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
        foreach (var currentPlane in newPlanes)
        {
            var planeObject = Instantiate(TrackedPlanePrefab, Vector3.zero, Quaternion.identity,
                transform);
            planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(currentPlane);

            // Apply a random color and grid rotation.
            planeObject.GetComponent<Renderer>().material.SetColor("_GridColor", new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f)));
            planeObject.GetComponent<Renderer>().material.SetFloat("_UvRotation", UnityEngine.Random.Range(0.0f, 360.0f));

            m_PlaneObjects.Add(planeObject);
        }

        foreach (var planeObject in m_PlaneObjects)
        {
            if (planeObject == null)
            {
                continue;
            }

            if (m_IsDebug)
            {
                planeObject.SetActive(true);
            }
            else
            {
                planeObject.SetActive(false);
            }
        }
    }

    private void LoadWorker()
    {
        try
        {
            this.footDetector = new FootDetector(this.modelFile.bytes, INPUT_CNN_SIZE);
        }
        catch (TFException ex)
        {
            Debug.Log("Error: TFException. Make sure you model trained with same version of TensorFlow as in Unity plugin.");
            Debug.Log(ex.ToString());
        }
    }

    private async void TFDetect()
    {
        var captured = GetImageFromCamera();

        if (captured == null)
        {
            return;
        }

        if (inputCameraWidth == -1)
        {
            inputCameraWidth = captured.width;
            inputCameraHeight = captured.height;

            this.footDetector.SetInputImageSize(inputCameraWidth, inputCameraHeight);
        }

        var resized = ResizeTexture(captured, INPUT_CNN_SIZE, INPUT_CNN_SIZE);
        //var rotated = TextureTools.RotateTexture(resized, 180);

        Color32[] colors = resized.GetPixels32();

        TimeSpan time;
        DateTime start;

        start = DateTime.Now;
        boxOutlines = await footDetector.DetectAsync(colors);

        time = DateTime.Now - start;
        Debug.Log(string.Format("DetectAsync time: {0}", time.TotalSeconds));
    }

    private async void GuessAngle()
    {
        TimeSpan time;
        DateTime start;

        start = DateTime.Now;

        var captured = GetImageFromCamera();

        #region DEBUG
        m_DebugImage2.texture = captured;
        #endregion

        if (captured == null)
        {
            Debug.Log("captured is null");
            return;
        }

        if (inputCameraWidth == -1)
        {
            inputCameraWidth = captured.width;
            inputCameraHeight = captured.height;
            Debug.Log(string.Format("captured w/h: {0} {1}", inputCameraWidth, inputCameraHeight));
        }

        if (useTFDetect && (boxOutlines == null || boxOutlines.Count <= 0))
        {
            return;
        }

        Texture2D snap;
        int left, bottom, width, height;

        // If usedetect is true, Find foot in image and crop that area
        if (useTFDetect)
        {
            var boxOutline = boxOutlines[0];

            cropMargin = 50;
            left = boxOutline.left - cropMargin;
            bottom = boxOutline.bottom - cropMargin;
            width = (boxOutline.right - boxOutline.left) + cropMargin * 2;
            height = (boxOutline.top - boxOutline.bottom) + cropMargin * 2;

            Debug.Log(string.Format("w/h {0} {1}", width, height));

            var rect = new UnityEngine.Rect(
                Math.Max(left, 0),
                Math.Max(bottom, 0),
                Math.Min(width, inputCameraWidth - boxOutline.left),
                Math.Min(height, inputCameraHeight - boxOutline.bottom)
            );
            snap = TextureTools.CropWithRect(captured, rect, TextureTools.RectOptions.BottomLeft, 0, 0);
        }
        else
        {
            left = 0;
            bottom = 0;

            snap = captured;
        }

        // Change Texture2D to Opencv.Mat
        Mat src = new Mat(snap.height, snap.width, CvType.CV_8UC4);
        Utils.texture2DToMat(snap, src);

        Point cntr = new Point();
        Point vec = new Point();

        if (m_IsDebug)
        {
            GetPCA(src, cntr, vec);
        }
        else
        {
            await GetPCAAsync(src, cntr, vec);
        }

        // Change PCA result to angle
        footPosX = (int)cntr.x + left;
        //footPosY = Math.Max(inputCameraHeight - ((int)cntr.y + bottom), 0);
        footPosY = (int)cntr.y + bottom;

        float footAngleRadian = Mathf.Atan2((float)vec.y, (float)vec.x);
        footAngleDegree = footAngleRadian * (180.0f / Mathf.PI);
        if (footAngleDegree < 0)
        {
            footAngleDegree = 180 + footAngleDegree;
        }

        #region DEBUG
        if (m_IsDebug)
        {
            DrawCircle(captured, footPosX, footPosY, 5);
        } else
        {
            Destroy(captured);
        }
        time = DateTime.Now - start;
        Debug.Log(string.Format("GuessAngle time: {0}", time.TotalSeconds));
        Debug.Log(string.Format("GuessAngle: {0}", footAngleDegree));
        #endregion

        // Release used Mat variable
        src.Dispose();
    }

    private Task GetPCAAsync(Mat src, Point cntr, Point vec)
    {
        return Task.Run(() =>
        {
            GetPCA(src, cntr, vec);
        });
    }

    private void GetPCA(Mat src, Point cntr, Point vec)
    {
        //Convert image to hsv
        Mat hsv = new Mat();
        Imgproc.cvtColor(src, hsv, Imgproc.COLOR_RGB2HSV_FULL);

        Mat mask = new Mat();
        Core.inRange(hsv, lowerHSV, upperHSV, mask);

        Mat hierarchy = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Imgproc.findContours(mask, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_NONE);

        double largestValue = 0;
        int largestIndex = -1;

        // Find largest contour
        for (int i = 0; i < contours.Count; ++i)
        {
            // Calculate the area of each contour
            double area = Imgproc.contourArea(contours[i]);
            // Ignore contours that are too small
            if (area < 1e2)
            {
                continue;
            }

            if (area > largestValue)
            {
                largestIndex = i;
                largestValue = area;
            }
        }

        #region DEBUG
        if (m_IsDebug)
        {
            Texture2D texture = new Texture2D(mask.cols(), mask.rows(), TextureFormat.RGBA32, false);
            Utils.matToTexture2D(mask, texture);
            Destroy(m_DebugImage1.texture);
            m_DebugImage1.texture = texture;
        }
        #endregion

        if (largestIndex == -1)
        {
            return;
        }

        // Find PCA value
        //Construct a buffer used by the pca analysis
        List<Point> pts = contours[largestIndex].toList();
        int sz = pts.Count;
        Mat data_pts = new Mat(sz, 2, CvType.CV_64FC1);
        for (int p = 0; p < data_pts.rows(); ++p)
        {
            data_pts.put(p, 0, pts[p].x);
            data_pts.put(p, 1, pts[p].y);
        }

        Mat mean = new Mat();
        Mat eigenvectors = new Mat();
        Core.PCACompute(data_pts, mean, eigenvectors, 2);

        Debug.Log("mean: " + mean.dump());
        Debug.Log("eginvectors: " + eigenvectors.dump());

        vec.x = eigenvectors.get(0, 0)[0];
        vec.y = eigenvectors.get(0, 1)[0];

        RotatedRect boundRect = Imgproc.minAreaRect(new MatOfPoint2f(contours[largestIndex].toArray()));
        cntr.x = boundRect.center.x;
        cntr.y = src.rows() - boundRect.center.y;

        // Release used Mat variable
        hsv.Dispose();
        mask.Dispose();
        hierarchy.Dispose();

        data_pts.Dispose();
        mean.Dispose();
        eigenvectors.Dispose();
    }

    /*
     * Get snaped image from camera
     */
    private Texture2D GetImageFromCamera()
    {
        m_ShoeController.DeactivateShoe();
        foreach (var planeObject in m_PlaneObjects)
        {
            if (planeObject == null)
            {
                continue;
            }

            planeObject.SetActive(false);
        }
        var captured = ScreenshotPreview.GetTexture2DOfScreenshot();
        m_ShoeController.ActivateShoe();

        return captured;
    }

    private Texture2D ResizeTexture(Texture2D input, int width, int height)
    {
        //TextureTools.scale(input, width, height);

        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, true);
        Color[] pixels = result.GetPixels(0);
        float incX = (1.0f / (float)width);
        float incY = (1.0f / (float)height);

        for (int px = 0; px < pixels.Length; px++)
        {
            pixels[px] = input.GetPixelBilinear(
                incX * ((float)px % width),
                incY * ((float)Mathf.Floor(px / height)));
        }
        result.SetPixels(pixels, 0);
        result.Apply();
        return result;
    }

    private Task<Texture2D> AsyncRotate(Texture2D texture)
    {
        return Task.Run(() => {
            texture = TextureTools.RotateTexture(texture, -90);
            return texture;
        });
    }

    private Texture2D Rotate(Texture2D texture)
    {
        var ret = TextureTools.ReflectTexture(texture);
#if !UNITY_EDITOR
        ret = TextureTools.RotateTexture(ret, -90);
#endif

        return ret;
    }

    /*
     * Drwa circle at image for debug
     */
    public void DrawCircle(Texture2D tex, int x, int y, int r, int c = 0)
    {
        Color32 color = Color.red;

        if (c != 0)
        {
            color = Color.blue;
        }

        float rSquared = r * r;

        for (int u = 0; u < tex.width; u++)
        {
            for (int v = 0; v < tex.height; v++)
            {
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                {
                    tex.SetPixel(u, v, color);
                }
            }
        }

        tex.Apply();

    }
}
