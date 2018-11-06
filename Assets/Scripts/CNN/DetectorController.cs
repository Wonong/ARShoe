using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

//using Vuforia;
using GoogleARCore;
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
    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    private bool mAccessCameraImage = true;

    private static int INPUT_CNN_SIZE = 416;
    private int inputCameraWidth = -1, inputCameraHeight = -1;
    private FootDetector footDetector;
    private List<BoxOutline> boxOutlines;

    private ShoeController m_ShoeController;
    
    private int footPosX, footPosY;
    private float footAngleDegree;
    private bool findFoot = false;
    private bool useTFDetect = false;
    #endregion // PRIVATE_MEMBERS

    #region FOR_DEBUG
    public static bool isDebug = false;
    public RawImage m_DebugImage1;
    public RawImage m_DebugImage2;
    public UnityEngine.UI.Text m_DebugText;
    public int tmp = 0;
    public static string debugStr = "";
    public int x, y, z;
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
        // load tensorflow model
        LoadWorker();

        m_ShoeController = FindObjectOfType<ShoeController>();

        ResetShoePosition();
    }

    // Update is called once per frame
    public void ClickResetButton()
    {
        GuessAngle();
        ResetShoePosition();
    }

    private void ResetShoePosition()
    {
        m_ShoeController.ResetPosition(new Vector3(0, 0, 0.55f), Quaternion.Euler(0, 0, footAngleDegree - 90));
    }

    void Update()
    {
        if (isDebug)
        {
            m_DebugImage1.gameObject.SetActive(true);
            m_DebugImage2.gameObject.SetActive(true);
            m_DebugText.gameObject.SetActive(true);
            m_DebugText.text = debugStr;
        }
        else
        {
            m_DebugImage1.gameObject.SetActive(false);
            m_DebugImage2.gameObject.SetActive(false);
            m_DebugText.gameObject.SetActive(false);
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
        Debug.Log(string.Format("DetectAsync time: {0}.{1}", time.Seconds, time.Milliseconds));
    }

    private async void GuessAngle()
    {
        TimeSpan time;
        DateTime start;

        start = DateTime.Now;

        var captured = GetImageFromCamera();

        #region DEBUG
        //var captured = LoadImage("Assets\\test_416.jpg");
        //var captured = LoadImage("Assets\\test.png");
        #endregion

        if (captured == null)
        {
            return;
        }

        if (useTFDetect && (boxOutlines == null || boxOutlines.Count <= 0))
        {
            return;
        }

        Texture2D snap;
        int left, bottom, width, height;

        if (useTFDetect)
        {
            var boxOutline = boxOutlines[tmp];
    
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
        } else
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
        await GetPCAAsync(src, cntr, vec);
        //GetPCA(src, cntr, vec);

        footPosX = (int)cntr.x + left;
        footPosY = (int)cntr.y + bottom;
        float footAngleRadian = Mathf.Atan2((float)vec.y, (float)vec.x);
        footAngleDegree = footAngleRadian * (180.0f / Mathf.PI);
        if (footAngleDegree < 0)
        {
            footAngleDegree = 180 + footAngleDegree;
        }
        findFoot = true;

        #region DEBUG
        //DrawCircle(snap, (int)cntr.x, (int)cntr.y, 5);
        //testImage.texture = snap;

        //int cx = (int)cntr.x + left;
        //int cy = (int)cntr.y + bottom;

        //DrawCircle(captured, cx, cy, 5);
        //testImage2.texture = captured;

        time = DateTime.Now - start;
        Debug.Log(string.Format("GuessAngle time: {0}.{1}", time.Seconds, time.Milliseconds));
        Debug.Log(string.Format("GuessAngle: {0}", footAngleDegree));
        #endregion

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
        Core.inRange(hsv, new Scalar(0, 40, 125), new Scalar(179, 255, 255), mask);

        Mat hierarchy = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Imgproc.findContours(mask, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_NONE);

        double largestValue = 0;
        int largestIndex = -1;

        // find largest contour
        for (int i = 0; i < contours.Count; ++i)
        {
            // Calculate the area of each contour
            double area = Imgproc.contourArea(contours[i]);
            // Ignore contours that are too small
            if (area < 1e2 )
                continue;

            if (area > largestValue)
            {
                largestIndex = i;
                largestValue = area;
            }
        }

        if (largestIndex == -1)
        {
            return;
        }

        #region DEBUG
        //Texture2D texture = new Texture2D(mask.cols(), mask.rows(), TextureFormat.RGBA32, false);
        //Utils.matToTexture2D(mask, texture);
        //testImage2.texture = texture;

        //Imgproc.drawContours(src, contours, largestIndex, new Scalar(255, 0, 0), 2);
        //Texture2D texture = new Texture2D(src.cols(), src.rows(), TextureFormat.RGBA32, false);
        //Utils.matToTexture2D(src, texture);
        //testImage2.texture = texture;
        #endregion

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

        cntr.x = mean.get(0, 0)[0];
        cntr.y = src.rows() - mean.get(0, 1)[0];

        vec.x = eigenvectors.get(0, 0)[0];
        vec.y = eigenvectors.get(0, 1)[0];


        data_pts.Dispose();
        mean.Dispose();
        eigenvectors.Dispose();
    }

    /*
     * Get snaped image from camera
     */
    private Texture2D GetImageFromCamera()
    {
        var captured = (Texture2D)GoogleARCore.Frame.CameraImage.Texture;

        if (captured == null)
        {
            return null;
        }

        var pixels = captured.GetPixels32();

        if (pixels == null || pixels.Length <= 0)
        {
            return null;
        }

        Texture2D result = new Texture2D((int)captured.width, (int)captured.height);
        result.SetPixels(captured.GetPixels());
        result.Apply();
        return result;
    }

    private Texture2D ResizeTexture(Texture2D input, int width, int height)
    {
        //TextureTools.scale(input, width, height);

        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, true);
        Color[] pixels = result.GetPixels(0);
        float incX = (1.0f / (float)width);
        float incY = (1.0f / (float)height);

        for(int px = 0; px < pixels.Length; px++)
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
        return Task.Run(() =>
        {
            texture = TextureTools.ReflectTexture(texture);
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

    public void DrawCircle(Texture2D tex, int x, int y, int r)
    {
        Color32 color = Color.red;
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
