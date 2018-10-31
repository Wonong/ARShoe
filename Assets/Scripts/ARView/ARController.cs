using System;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

/// <summary>
/// Control ar view class.
/// </summary>
public class ARController : MonoBehaviour
{
    # region PUBLIC_MEMBERS
    /// <summary>
    /// The main camera being used to render the passthrough camera image (i.e. AR background).
    /// </summary>
    public Camera mainCamera;

    /// <summary>
    /// A prefab for tracking and visualizing detected planes.
    /// </summary>
    public GameObject detectedPlanePrefab;

    public bool IsPlaced {
        get 
        {
            return isPlaced;
        }
        set 
        {
            isPlaced = value;
        }
    }

    public bool DoesShoeActive {
        get
        {
            return shoe!=null && shoe.activeSelf;
        }
    }

    public GameObject shadowPlaneIndicator;
    public GameObject rotationIndicator;
    public GameObject translationIndicator;
    public GameObject defaultIndicator;
    public GameObject planeGenerator;
    public GameObject pointCloud;
    #endregion

    # region PRIVATE_MEMBERS
    /// <summary>
    /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
    /// the application to avoid per-frame allocations.
    /// </summary>
    List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

    /// <summary>
    /// A model to place when a raycast from a user touch hits a plane.
    /// </summary>
    GameObject shoe;

    GroundPlaneUI m_GroundPlaneUI;
    /// <summary>
    /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
    /// </summary>
    bool m_IsQuitting = false;

    // For checking shoe object is placed.
    bool isPlaced = false;

    // Size values.
    float shoeHeight = 0.15f;
    float shoeScale = 1.4f;
    float indicatorHeight = 0.2f;
    float shadowMovedHeight = 0.15f;
    float shadowFixedHeight = 0.05f;
    float indicatorScale = 0.25f;
    float shadowMovedScale = 0.2f;
    float shadowFixedScaled = 0.18f;
    #endregion

    void Awake()
    {
        CurrentCustomShoe.shoe.GetComponent<Swiper>().enabled = false;
        shoe = Instantiate(CurrentCustomShoe.shoe);
        shoe.GetComponent<Spin>().enabled = false;
        shoe.name = "CopyShoe";
        shoe.transform.localScale = new Vector3(shoeScale, shoeScale, shoeScale);
        shoe.GetComponentsInChildren<Transform>()[1].localRotation = Quaternion.Euler(0, 0, 0);
    }

    void Start()
    {
        m_GroundPlaneUI = FindObjectOfType<GroundPlaneUI>();
        InitializeIndicators();
        MoveShoe(); // Shoe object is movable at very first.
        shoe.SetActive(false);
    }

    /// <summary>
    /// The Unity Update() method.
    /// </summary>
    void Update()
    {
        _UpdateApplicationLifecycle();
        Session.GetTrackables<DetectedPlane>(m_AllPlanes);
        SetIndicators(); // Set indicators children of shoe.
        shadowPlaneIndicator.SetActive(shoe.activeSelf); // Change shadow activity by shoe's activity.
        ChangePlanesVisualizer(); // Change visualizing of planes by status of shoe placed. 

        // If the player has not touched the screen, we are done with this update.
        Touch[] touches = Input.touches;
        if (Input.touchCount < 1 || (touches[0] = Input.GetTouch(0)).phase == TouchPhase.Ended)
        {
            return;
        }

        if(isPlaced && !EventSystem.current.IsPointerOverGameObject(0))
        {
            isPlaced = false;
            m_GroundPlaneUI.SetShoeMovable();
        }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Input.touches[0].phase == TouchPhase.Ended)
        {
            TouchHandler.isFirstFrameWithTwoTouches = true;
        }

        if (Input.touchCount == 1 && Frame.Raycast(touches[0].position.x, touches[0].position.y, raycastFilter, out hit))
        {
            TouchHandler.InteractSingleFinger(shoe, hit, touches);
        }
        else if (Input.touchCount == 2)
        {
            TouchHandler.InteractDoubleFinger(shoe, touches);
        }
    }

    #region public methods
    /// <summary>
    /// Place shoe to the plane.
    /// </summary>
    public void PlaceShoe()
    {
        shoe.transform.position -= Vector3.up * shoeHeight;
        shadowPlaneIndicator.transform.localPosition = new Vector3(0, 0, 0);
#if (UNITY_IOS || !UNITY_ANDROID)
            shadowPlaneIndicator.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
            shadowPlaneIndicator.transform.position -= Vector3.up * 0.1f;
#elif UNITY_ANDROID
        shadowPlaneIndicator.transform.localScale = new Vector3(shadowFixedScaled, shadowFixedScaled, shadowFixedScaled);
        shadowPlaneIndicator.transform.position -= Vector3.up * shadowFixedHeight;
        #endif
        GameObject.Find("PuttingSound").GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// Move shoe on the plane.
    /// </summary>
    public void MoveShoe()
    {
        shoe.transform.position += Vector3.up * shoeHeight;
        shadowPlaneIndicator.transform.localPosition = new Vector3(0, 0, 0);
#if (UNITY_IOS || !UNITY_ANDROID)
            shadowPlaneIndicator.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            shadowPlaneIndicator.transform.position -= Vector3.up * 0.2f;
#elif UNITY_ANDROID
        shadowPlaneIndicator.transform.localScale = new Vector3(shadowMovedScale, shadowMovedScale, shadowMovedScale);
        shadowPlaneIndicator.transform.position -= Vector3.up * shadowMovedHeight;
        #endif
    }

    /// <summary>
    /// Reset object, anchor, and make shoe object movable.
    /// </summary>
    public void ResetAR()
    {
        Destroy(FindObjectOfType<Anchor>());
        shoe.SetActive(false);
        isPlaced = false;
        MoveShoe();
    }
    #endregion

    /// <summary>
    /// Initialize indicator's'
    /// </summary>
    void InitializeIndicators()
    {
        shadowPlaneIndicator.transform.SetParent(shoe.transform);
        shadowPlaneIndicator.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        InitializeIndicator(rotationIndicator);
        InitializeIndicator(translationIndicator);
        InitializeIndicator(defaultIndicator);
    }

    /// <summary>
    /// Initialize each indicator.
    /// </summary>
    void InitializeIndicator(GameObject indicator)
    {
        indicator.transform.SetParent(shoe.transform);
        indicator.transform.localRotation = Quaternion.Euler(90f, -90f, 0f);
        indicator.transform.localScale = new Vector3(indicatorScale, indicatorScale, indicatorScale);
    }

    /// <summary>
    /// Set indicators hierachy under shoe object's transform.
    /// </summary>
    void SetIndicators()
    {
        rotationIndicator.SetActive(Input.touchCount == 2 && !isPlaced);
        if (rotationIndicator.activeSelf)
        {
            rotationIndicator.transform.position = shoe.transform.position;
            rotationIndicator.transform.position -= Vector3.up * indicatorHeight;
        }

        translationIndicator.SetActive(Input.touchCount == 1 && !isPlaced && !EventSystem.current.IsPointerOverGameObject(0));
        if (translationIndicator.activeSelf)
        {
            translationIndicator.transform.position = shoe.transform.position;
            translationIndicator.transform.position -= Vector3.up * indicatorHeight;
        }

        defaultIndicator.SetActive(Input.touchCount == 0 && shoe.activeSelf && !isPlaced);
        if (defaultIndicator.activeSelf)
        {
            defaultIndicator.transform.position = shoe.transform.position;
            defaultIndicator.transform.position -= Vector3.up * indicatorHeight;
        }
    }

    /// <summary>
    // If shoe object is placed, then hide plane visualizer. Else, show plane visualizer.
    /// </summary>
    private void ChangePlanesVisualizer()
    {
        Transform[] planes = planeGenerator.GetComponentsInChildren<Transform>();
        if (planes.Length < 1) return;
        for (int i = 1; i < planes.Length; i++)
        {
            planes[i].gameObject.SetActive(!isPlaced || !isActiveAndEnabled);
        }

        pointCloud.SetActive(!isPlaced || !isActiveAndEnabled);
    }


    /// <summary>
    /// Check and update the application lifecycle.
    /// </summary>
    void _UpdateApplicationLifecycle()
    {
        // Only allow the screen to sleep when not tracking.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
        }
        else
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        if (m_IsQuitting)
        {
            return;
        }

        // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
        if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
        {
            _ShowAndroidToastMessage("Camera permission is needed to run this application.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError())
        {
            _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
            m_IsQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    /// <summary>
    /// Actually quit the application.
    /// </summary>
    void _DoQuit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Show an Android toast message.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    void _ShowAndroidToastMessage(string message)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
