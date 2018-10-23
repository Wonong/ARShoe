/*==============================================================================
Copyright (c) 2017-2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/

using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class PlaneManager : MonoBehaviour
{
    public enum PlaneMode
    {
        PLACEMENT
    }

    #region PUBLIC_MEMBERS
    public PlaneFinderBehaviour m_PlaneFinderBehaviour;
    public GameObject m_PlaneFinder;
    //public MidAirPositionerBehaviour m_MidAirPositioner;

    [Header("Placement Augmentations")]
    GameObject m_PlacementAugmentation;
    public static bool GroundPlaneHitReceived;
    public static PlaneMode planeMode = PlaneMode.PLACEMENT;
    public static GameObject indicator;

    public static bool AnchorExists
    {
        get { return anchorExists; }
        private set { anchorExists = value; }
    }

    GameObject copyShoe;
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    const string unsupportedDeviceTitle = "Unsupported Device";
    const string unsupportedDeviceBody =
        "This device has failed to start the Positional Device Tracker. " +
        "Please check the list of supported Ground Plane devices on our site: " +
        "\n\nhttps://library.vuforia.com/articles/Solution/ground-plane-supported-devices.html";

    StateManager m_StateManager;
    SmartTerrain m_SmartTerrain;
    PositionalDeviceTracker m_PositionalDeviceTracker;
    ContentPositioningBehaviour m_ContentPositioningBehaviour;
    TouchHandler m_TouchHandler;
    ProductPlacement m_ProductPlacement;
    GroundPlaneUI m_GroundPlaneUI;
    AnchorBehaviour m_PlacementAnchor;
    GameObject shadowPlane;
    int AutomaticHitTestFrameCount;
    int m_AnchorCounter;
    bool uiHasBeenInitialized;
    static bool anchorExists; // backs public AnchorExists property
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    private void Awake()
    {
        CurrentCustomShoe.shoe.GetComponent<Swiper>().enabled = false;
        copyShoe = Instantiate(CurrentCustomShoe.shoe);
        copyShoe.name = "CopyShoe";
    }

    void Start()
    {
        VuforiaARController.Instance.RegisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.RegisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.RegisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);

        m_PlaneFinderBehaviour.HitTestMode = HitTestMode.AUTOMATIC;

        m_ProductPlacement = FindObjectOfType<ProductPlacement>();
        m_TouchHandler = FindObjectOfType<TouchHandler>();
        m_GroundPlaneUI = FindObjectOfType<GroundPlaneUI>();

        copyShoe.transform.SetParent(m_ProductPlacement.gameObject.transform);
        #if (UNITY_IOS||!UNITY_ANDROID)
            copyShoe.GetComponentInChildren<Transform>().transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        #elif UNITY_ANDROID
            copyShoe.GetComponentInChildren<Transform>().transform.localScale = new Vector3(6f, 6f, 6f);
        #endif
        copyShoe.GetComponentsInChildren<Transform>()[1].localRotation = Quaternion.Euler(0, 0, 0);

        m_PlacementAugmentation = copyShoe.transform.parent.gameObject;

        m_PlacementAnchor = m_PlacementAugmentation.GetComponentInParent<AnchorBehaviour>();

        UtilityHelper.EnableRendererColliderCanvas(m_PlacementAugmentation, false);
        m_PlaneFinder.SetActive(false);
        shadowPlane = GameObject.Find("ShadowPlane");
        shadowPlane.transform.SetParent(copyShoe.transform);
        shadowPlane.transform.localPosition = new Vector3(0, 0, 0);
        #if (UNITY_IOS || !UNITY_ANDROID)
            shadowPlane.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            shadowPlane.transform.position -= Vector3.up * 0.2f;
        #elif UNITY_ANDROID
            shadowPlane.transform.localScale = new Vector3(0.22f, 0.22f, 0.22f);
            shadowPlane.transform.position -= Vector3.up * 0.4f;
        #endif
        shadowPlane.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        m_ProductPlacement.SetProductAnchor(null);
    }

    void Update()
    {
        copyShoe.transform.localPosition = new Vector3(0, 0, 0);
        shadowPlane.transform.localRotation = Quaternion.Euler(90f, 0, 0);
        if (!VuforiaRuntimeUtilities.IsPlayMode() && !AnchorExists)
        {
            AnchorExists = DoAnchorsExist();
        }

        if (m_ProductPlacement.enabled&&!m_ProductPlacement.IsPlaced) {
            m_PlaneFinder.SetActive(true);
            m_PlaneFinderBehaviour.enabled = true;
        } else {
            m_PlaneFinder.SetActive(false);
        }

        GroundPlaneHitReceived = (AutomaticHitTestFrameCount == Time.frameCount);

        SetSurfaceIndicatorVisible(
            GroundPlaneHitReceived &&
             (planeMode == PlaneMode.PLACEMENT && Input.touchCount == 0));
    }

    void OnDestroy()
    {
        Debug.Log("OnDestroy() called.");

        VuforiaARController.Instance.UnregisterVuforiaStartedCallback(OnVuforiaStarted);
        VuforiaARController.Instance.UnregisterOnPauseCallback(OnVuforiaPaused);
        DeviceTrackerARController.Instance.UnregisterTrackerStartedCallback(OnTrackerStarted);
        DeviceTrackerARController.Instance.UnregisterDevicePoseStatusChangedCallback(OnDevicePoseStatusChanged);
    }

#endregion // MONOBEHAVIOUR_METHODS


#region GROUNDPLANE_CALLBACKS

    public void HandleAutomaticHitTest(HitTestResult result)
    {
        AutomaticHitTestFrameCount = Time.frameCount;

        //if (!uiHasBeenInitialized)
        //{
        //    uiHasBeenInitialized = m_GroundPlaneUI.InitializeUI();
        //}
        if (indicator == null)
        {
            indicator = GameObject.Find("Indicator").gameObject;
            indicator.transform.localScale = new Vector3(1f, 1f, 1f);
            indicator.transform.position -= Vector3.up * 0.3f;
        }
        else if (!m_ProductPlacement.IsPlaced && indicator.activeSelf)
        {
            m_PlacementAugmentation.transform.rotation = Quaternion.Euler(0, 0, 0);
            SetSurfaceIndicatorVisible(false);
            m_ProductPlacement.SetProductAnchor(null);
            m_PlacementAugmentation.transform.position = m_PlaneFinder.GetComponentsInChildren<Transform>()[1].position;
            //m_PlacementAugmentation.transform.position += Vector3.up * 0.1f;
            //shadowPlane.transform.position = m_PlacementAugmentation.transform.position - Vector3.up * 0.2f;
        }
    }

    public void HandleInteractiveHitTest(HitTestResult result)
    {
        if (result == null)
        {
            //Debug.LogError("Invalid hit test result!");
            return;
        }

        if (!m_GroundPlaneUI.IsCanvasUIPressed())
        {
            if (indicator.activeSelf)
            {
                indicator.SetActive(false);
            }
            m_ContentPositioningBehaviour = m_PlaneFinderBehaviour.GetComponent<ContentPositioningBehaviour>();
            m_ContentPositioningBehaviour.DuplicateStage = false;

            if (!m_ProductPlacement.IsPlaced || TouchHandler.DoubleTap)
            {
                m_ContentPositioningBehaviour.AnchorStage = m_PlacementAnchor;
                m_ContentPositioningBehaviour.PositionContentAtPlaneAnchor(result);
                UtilityHelper.EnableRendererColliderCanvas(m_PlacementAugmentation, true);
            }

            if (!m_ProductPlacement.IsPlaced)
            {
                //m_ProductPlacement.SetProductAnchor(m_PlacementAnchor.transform);
                m_TouchHandler.enableRotation = true;
            }
        }
    }

#endregion // GROUNDPLANE_CALLBACKS


#region PUBLIC_BUTTON_METHODS

    public void SetPlacementMode(bool active)
    {
        if (active)
        {
            planeMode = PlaneMode.PLACEMENT;
            m_PlaneFinderBehaviour.enabled = true;
            m_TouchHandler.enableRotation = m_PlacementAugmentation.activeInHierarchy;
        }
    }

    public void ResetScene()
    {
        //Debug.Log("ResetScene() called.");

        indicator.SetActive(true);
        m_ProductPlacement.Reset();
        UtilityHelper.EnableRendererColliderCanvas(m_PlacementAugmentation, false);

        DeleteAnchors();
        m_ProductPlacement.SetProductAnchor(null);
        m_TouchHandler.enableRotation = false;
        m_PlaneFinderBehaviour.enabled = false;
    }

    public void ResetTrackers()
    {
        //Debug.Log("ResetTrackers() called.");

        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();
        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();

        // Stop and restart trackers
        m_SmartTerrain.Stop(); // stop SmartTerrain tracker before PositionalDeviceTracker
        m_PositionalDeviceTracker.Reset();
        m_SmartTerrain.Start(); // start SmartTerrain tracker after PositionalDeviceTracker
    }
#endregion // PUBLIC_BUTTON_METHODS


#region PRIVATE_METHODS

    void DeleteAnchors()
    {
        m_PlacementAnchor.UnConfigureAnchor();
        AnchorExists = DoAnchorsExist();
    }

    void SetSurfaceIndicatorVisible(bool isVisible)
    {
        Renderer[] renderers = m_PlaneFinderBehaviour.PlaneIndicator.GetComponentsInChildren<Renderer>(true);
        Canvas[] canvas = m_PlaneFinderBehaviour.PlaneIndicator.GetComponentsInChildren<Canvas>(true);

        foreach (Canvas c in canvas)
            c.enabled = isVisible;

        foreach (Renderer r in renderers)
            r.enabled = isVisible;
    }

    bool DoAnchorsExist()
    {
        if (m_StateManager != null)
        {
            IEnumerable<TrackableBehaviour> trackableBehaviours = m_StateManager.GetActiveTrackableBehaviours();

            foreach (TrackableBehaviour behaviour in trackableBehaviours)
            {
                if (behaviour is AnchorBehaviour)
                {
                    return true;
                }
            }
        }
        return false;
    }

#endregion // PRIVATE_METHODS


#region VUFORIA_CALLBACKS

    void OnVuforiaStarted()
    {
        //Debug.Log("OnVuforiaStarted() called.");

        m_StateManager = TrackerManager.Instance.GetStateManager();

        // Check trackers to see if started and start if necessary
        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (m_PositionalDeviceTracker != null && m_SmartTerrain != null)
        {
            if (!m_PositionalDeviceTracker.IsActive)
                m_PositionalDeviceTracker.Start();
            if (m_PositionalDeviceTracker.IsActive && !m_SmartTerrain.IsActive)
                m_SmartTerrain.Start();
        }
        else
        {
            if (m_PositionalDeviceTracker == null)
                Debug.Log("PositionalDeviceTracker returned null. GroundPlane not supported on this device.");
            if (m_SmartTerrain == null)
                Debug.Log("SmartTerrain returned null. GroundPlane not supported on this device.");

            MessageBox.DisplayMessageBox(unsupportedDeviceTitle, unsupportedDeviceBody, false, null);
        }
    }

    void OnVuforiaPaused(bool paused)
    {
        Debug.Log("OnVuforiaPaused(" + paused.ToString() + ") called.");

        if (paused)
            ResetScene();
    }

#endregion // VUFORIA_CALLBACKS


#region DEVICE_TRACKER_CALLBACKS

    void OnTrackerStarted()
    {
        //Debug.Log("OnTrackerStarted() called.");

        m_PositionalDeviceTracker = TrackerManager.Instance.GetTracker<PositionalDeviceTracker>();
        m_SmartTerrain = TrackerManager.Instance.GetTracker<SmartTerrain>();

        if (m_PositionalDeviceTracker != null)
        {
            if (!m_PositionalDeviceTracker.IsActive)
                m_PositionalDeviceTracker.Start();

            //Debug.Log("PositionalDeviceTracker is Active?: " + m_PositionalDeviceTracker.IsActive +
                      //"\nSmartTerrain Tracker is Active?: " + m_SmartTerrain.IsActive);
        }
    }

    void OnDevicePoseStatusChanged(TrackableBehaviour.Status status, TrackableBehaviour.StatusInfo statusInfo)
    {
        //Debug.Log("OnDevicePoseStatusChanged(" + status + ", " + statusInfo + ")");

        //if (status == TrackableBehaviour.Status.TRACKED)
        //{
        //}
    }

#endregion // DEVICE_TRACKER_CALLBACK_METHODS
}
