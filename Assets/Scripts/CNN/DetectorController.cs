using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using Vuforia;
using TFClassify;
using TensorFlow;
using System;
using System.IO;

public class DetectorController : MonoBehaviour
{

    #region PUBLIC_MEMBERS
    public TextAsset modelFile; // .pb or .bytes file    

    public static bool isDebug = false;
    public RawImage testImage;
    public Text angleText;
    public float shoeScale = 1f;
    #endregion // PUBLIC_MEMBERS

    #region PRIVATE_MEMBERS
    private int capturedImageWidth = 256;
    private static int CNN_INPUT_SIZE = 256;

    private AngleDetector angleDetector;
    private Texture2D capturedImage = null;
    private float outputAngle = -1f;

    private bool mAccessCameraImage = true;

    private static Texture2D boxOutlineTexture;
    private GameObject copyShoe;
    private GameObject transparentShader;
    #endregion // PRIVATE_MEMBERS

    public static string debugStr = "";


#if UNITY_EDITOR
    //private Vuforia.Image.PIXEL_FORMAT mPixelFormat = Vuforia.Image.PIXEL_FORMAT.GRAYSCALE;
    private Vuforia.Image.PIXEL_FORMAT mPixelFormat = Vuforia.Image.PIXEL_FORMAT.RGBA8888;
#elif UNITY_ANDROID
   private Vuforia.Image.PIXEL_FORMAT mPixelFormat =  Vuforia.Image.PIXEL_FORMAT.RGB888;
#elif UNITY_IOS
    private Vuforia.Image.PIXEL_FORMAT mPixelFormat =  Vuforia.Image.PIXEL_FORMAT.RGB888;
#endif
    private bool mFormatRegistered = false;

    public void ClickCapture()
    {
        if(!isDebug)
        {
            StartCoroutine(ScreenshotPreview.CaptureAndShowPreviewImage());
        }
        else
        {
            StartCoroutine(ScreenshotPreview.CaptureAndShowPreviewImage(capturedImage));
        }
    }

    public void ClickBackButton()
    {
        SceneChanger.ChangeToListScene();
    }

    private void Awake()
    {
        InitializeShoe();
    }

    private void InitializeShoe()
    {
        CurrentCustomShoe.shoes.GetComponent<Swiper>().enabled = false;
        CurrentCustomShoe.shoes.GetComponent<Spin>().enabled = false;
        copyShoe = Instantiate(CurrentCustomShoe.shoes);
        copyShoe.name = "CopyShoe";
        copyShoe.transform.position = new Vector3(0, 0, 0);
        copyShoe.GetComponentsInChildren<Transform>()[1].localRotation = Quaternion.Euler(0, -90, 15);
        copyShoe.SetActive(false);
        SetShoeScale();
        transparentShader = Instantiate(Resources.Load("Prefabs/AttachingAR/TransparentShader") as GameObject);
        transparentShader.transform.SetParent(copyShoe.GetComponentsInChildren<Transform>()[1]);
        transparentShader.transform.localPosition = new Vector3(0.027f, 0.053f, 0.003f);
        transparentShader.transform.localRotation = Quaternion.Euler(-7.48f, 84.37f, -2.88f);
    }

    public void SetShoeScale()
    {
        copyShoe.transform.localScale = new Vector3(shoeScale, shoeScale, shoeScale);
        Debug.Log(shoeScale);
    }

    void Start()
    {
        //copyShoe.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0f);

        // load tensorflow model
        LoadWorker();

        // Register Vuforia life-cycle callbacks:
        Vuforia.VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        Vuforia.VuforiaARController.Instance.RegisterOnPauseCallback(OnPause);
        Vuforia.VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
    }

    private void OnVuforiaStarted()
    {
        // Try register camera image format
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            debugStr = "OnVuforiaStarted error\n" + debugStr;
            Debug.LogError("Failed to register pixel format " + mPixelFormat.ToString() +
                "\n the format may be unsupported by your device;" +
                "\n consider using a different pixel format.");
            mFormatRegistered = false;
        }

    }

    private void OnTrackablesUpdated()
    {
        if (mFormatRegistered && mAccessCameraImage && capturedImage == null)
        {
            TakeCapture();
        }
    }

    private void TakeCapture()
    {
        // Get image from camera
        var snap = TakeTextureSnap();

        if (snap == null)
        {
            return;
        }

        var rotated = Rotate(snap);

        var croped = TextureTools.CropWithRect(
                    snap,
                    //rotated,
                    new Rect(0, 0, capturedImageWidth, capturedImageWidth),
                    //TextureTools.RectOptions.BottomLeft,
                    TextureTools.RectOptions.Center,
                    0, 0);

        if (testImage != null && isDebug)
        {
            testImage.texture = rotated;
        }

        capturedImage = Scale(rotated, CNN_INPUT_SIZE);
    }
    /// <summary>
    /// Called when app is paused / resumed
    /// </summary>
    private void OnPause(bool paused)
    {
        if (paused)
        {
            Debug.Log("App was paused");
            UnregisterFormat();
        }
        else
        {
            Debug.Log("App was resumed");
            RegisterFormat();
        }
    }

    /// <summary>
    /// Unregister the camera pixel format (e.g. call this when app is paused)
    /// </summary>
    private void UnregisterFormat()
    {
        Debug.Log("Unregistering camera pixel format " + mPixelFormat.ToString());
        CameraDevice.Instance.SetFrameFormat(mPixelFormat, false);
        mFormatRegistered = false;
    }

    private void RegisterFormat()
    {
        if (CameraDevice.Instance.SetFrameFormat(mPixelFormat, true))
        {
            Debug.Log("Successfully registered camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = true;
        }
        else
        {
            Debug.LogError("Failed to register camera pixel format " + mPixelFormat.ToString());
            mFormatRegistered = false;
        }
    }

    // Update is called once per frame
    public void CallDetect()
    {
        Debug.Log("CallDetect");

        if (isDebug)
        {
            TFDetect();
        } else
        {
            var func = nameof(TFDetect);
            InvokeRepeating(func, 0, 1.0f);
        }
        
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!ScreenshotPreview.previewGameObject.activeSelf && Input.GetKey(KeyCode.Escape))
            {
                ClickBackButton();
            }
        }

        if (outputAngle != -1)
        {
            if (!copyShoe.activeSelf)
            {
                copyShoe.SetActive(true);
            }
            copyShoe.transform.localRotation = Quaternion.Euler(0f, outputAngle, 0f);
            Debug.Log(string.Format("output angle: {0}", outputAngle));
            debugStr = string.Format("output angle: {0}\n", outputAngle);
            outputAngle = -1;
        }

        if (isDebug)
        {
            testImage.gameObject.SetActive(true);
            angleText.gameObject.SetActive(true);
            angleText.text = debugStr;

            var func = nameof(TFDetect);
            CancelInvoke(func);
        }
        else
        {
            testImage.gameObject.SetActive(false);
            angleText.gameObject.SetActive(false);
        }

    }

    private void LoadWorker()
    {
        try
        {
            this.angleDetector = new AngleDetector(this.modelFile.bytes, CNN_INPUT_SIZE);
        }
        catch (TFException ex)
        {
            Debug.Log("Error: TFException. Make sure you model trained with same version of TensorFlow as in Unity plugin.");
            Debug.Log(ex.ToString());
        }
    }

    private async void TFDetectAsync()
    {
        if (capturedImage != null)
        {
            var scaled = Scale(capturedImage, CNN_INPUT_SIZE);

            outputAngle = await this.angleDetector.DetectAsync(scaled.GetPixels32());
            capturedImage = null;
        }
    }

    private void TFDetect()
    {
        Color32[] colors = capturedImage.GetPixels32();

        new System.Threading.Thread(() => {
            //outputAngle = this.angleDetector.Detect(capturedImage.GetPixels32());
            outputAngle = this.angleDetector.Detect(colors);
            capturedImage = null;
        }).Start();
    }

    private Texture2D Scale(Texture2D texture, int imageSize)
    {
        var scaled = TextureTools.scaled(texture, imageSize, imageSize, FilterMode.Bilinear);
        //var scaled = TextureTools.test(texture, imageSize, imageSize);

        return scaled;
    }

    /*
     * Get snaped image from camera
     */
    private Texture2D TakeTextureSnap()
    {
        Vuforia.Image captured = CameraDevice.Instance.GetCameraImage(mPixelFormat);

        if (captured == null || !captured.IsValid())
        {
            return null;
        }

        byte[] pixels = captured.Pixels;

        if (pixels == null || pixels.Length <= 0)
        {
            return null;
        }

        // Make temperate Texture2D to copy camera pixel data
        Texture2D tmp = new Texture2D(captured.Width, captured.Height, TextureFormat.RGB24, false);
        captured.CopyToTexture(tmp);

        /*
         * TODO: Change captureImageWidth to be proportional to Screen.width
         */
        return TextureTools.CropWithRect(
                        tmp,
                        //new Rect(0, 0, Mathf.Min(tmp.width, Screen.width), Mathf.Min(tmp.height, Screen.height)),
                        //new Rect(0, 0, 610, 1280),
                        new Rect(0, 0, capturedImageWidth, capturedImageWidth),
                        TextureTools.RectOptions.Center,
                        0, 0);
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

    public static void DrawRectangle(Rect area, int frameWidth, Color color)
    {
        // Create a one pixel texture with the right color
        if (boxOutlineTexture == null)
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            boxOutlineTexture = texture;
        }

        Rect lineArea = area;
        lineArea.height = frameWidth;
        GUI.DrawTexture(lineArea, boxOutlineTexture); // Top line

        lineArea.y = area.yMax - frameWidth;
        GUI.DrawTexture(lineArea, boxOutlineTexture); // Bottom line

        lineArea = area;
        lineArea.width = frameWidth;
        GUI.DrawTexture(lineArea, boxOutlineTexture); // Left line

        lineArea.x = area.xMax - frameWidth;
        GUI.DrawTexture(lineArea, boxOutlineTexture); // Right line
    }

    public void OnGUI()
    {
        if (capturedImage != null && isDebug)
        {
            var a = new Rect(
                    (Screen.width / 2) - capturedImageWidth / 2,
                    ((Screen.height / 2) - capturedImageWidth / 2),
                    capturedImageWidth,
                    capturedImageWidth);
            GUI.Box(
                a,
                GUIContent.none);
        }
    }
}
