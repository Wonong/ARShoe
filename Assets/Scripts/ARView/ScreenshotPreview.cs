using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using NatShareU;
using System.Collections;

public class ScreenshotPreview : MonoBehaviour
{
    public static GameObject previewGameObject;
    public static Image screenshotImage;
    public const string androidGalleryPath = "/storage/emulated/0/DCIM/ARShoe/";
    public const string windowsImageSavePath = "/ARShoe/";
    public const string galleryFolderName = "ARShoe";
    static string screenshotImagePath;
#if UNITY_IOS
    static Texture2D iosShareImage;
#endif
    public Button backButton;
    public Button shareButton;

	private void Start()
	{
        previewGameObject = gameObject;
        screenshotImage = gameObject.GetComponentsInChildren<Image>()[1];
        previewGameObject.SetActive(false);
        backButton.onClick.AddListener(ClickBackButton);
        shareButton.onClick.AddListener(ClickShareButton);
	}

    public static IEnumerator CaptureAndShowPreviewImage(Texture2D imageTexture=null)
    {
        yield return new WaitForEndOfFrame(); // Wait for rendering.
        byte[] bytes = imageTexture==null? GetBytesOfScreenshot():imageTexture.EncodeToPNG();
        string path;
        string imageName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".png"; // Set image name.

#if UNITY_ANDROID
        if (!GoogleARCore.AndroidPermissionsManager.IsPermissionGranted("android.permission.WRITE_EXTERNAL_STORAGE"))
        {
            var task = GoogleARCore.AndroidPermissionsManager.RequestPermission("android.permission.WRITE_EXTERNAL_STORAGE");
            yield return task.WaitForCompletion();
        }

        path = androidGalleryPath;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        SaveImageToAndroidGallery(path + imageName, bytes); // Write bytes and refresh android gallery.
        previewGameObject.SetActive(true);
        ShowPreview(path + imageName);
#elif UNITY_IOS
        path = Application.persistentDataPath+"/";
        NativeGallery.SaveImageToGallery(bytes, galleryFolderName, imageName); // User plugin.
        previewGameObject.SetActive(true);
        ShowPreview(path + imageName);
#elif UNITY_EDITOR
        path = windowsImageSavePath;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        SaveImageToWindow(path + imageName, bytes);
#endif
    }

    public static byte[] GetBytesOfScreenshot() // Screenshot image -> bytes for file writing.
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        RenderTexture rt = new RenderTexture(screenWidth, screenHeight, 24);
        Camera.main.targetTexture = rt;
        Texture2D screenshot = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
        Rect rec = new Rect(0, 0, screenshot.width, screenshot.height);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
        screenshot.Apply();
#if UNITY_IOS // if runtime platform is ios, then set screenshot image for shareing.
        iosShareImage = screenshot;
#endif
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        return screenshot.EncodeToPNG();
    }

    public static void SaveImageToAndroidGallery(string location, byte[] bytes) // Refresh android gallery.
    {
        File.WriteAllBytes(location, bytes);
        AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2] { "android.intent.action.MEDIA_SCANNER_SCAN_FILE", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + location) });
        objActivity.Call("sendBroadcast", objIntent);
    }

    public static void SaveImageToWindow(string location, byte[] bytes)
    {
        File.WriteAllBytes(location, bytes);
    }

    public static void ShowPreview(string imagePath) // Get saved screenshot image and show preview.
    {
        if(imagePath!=null)
        {
            screenshotImagePath = imagePath;
        }
        Texture2D texture = null;
#if UNITY_ANDROID
        {
            texture = GetScreenshotImage();
        }
#elif UNITY_IOS
            texture = NativeGallery.LoadImageAtPath(screenshotImagePath);
#endif
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        screenshotImage.sprite = sprite;
    }

    public static Texture2D GetScreenshotImage() // Get screenshot image from path;
    {
        Texture2D texture = null;
        byte[] fileBytes;
        if(File.Exists(screenshotImagePath))
        {
            fileBytes = File.ReadAllBytes(screenshotImagePath);
            texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
            texture.LoadImage(fileBytes);
        }
        return texture;
    }

    void ClickBackButton() // Delete preview image.
    {
        previewGameObject.SetActive(false);
    }

    void ClickShareButton() // Share screenshot image.
    {
#if UNITY_ANDROID
        new NativeShare().AddFile(screenshotImagePath).Share();
#elif UNITY_IOS
        NatShare.ShareImage(iosShareImage);
#endif
    }
}
