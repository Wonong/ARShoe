/*============================================================================== 
Copyright (c) 2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.   
==============================================================================*/

using UnityEngine;
using Vuforia;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProductPlacement : MonoBehaviour
{

    #region PUBLIC_MEMBERS
    public bool IsPlaced { get; set; }

    [Header("Placement Controls")]
    public GameObject m_TranslationIndicator;
    public GameObject m_RotationIndicator;
    public Transform Floor;

    [Header("Placement Augmentation Size Range")]
    [Range(1f, 2.0f)]
    public float ProductSize = 1f;
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    MeshRenderer[] shoeRenderers;
    [SerializeField]
    //    MeshRenderer shadowRenderer;

    const string EmulatorGroundPlane = "Emulator Ground Plane";

    GroundPlaneUI m_GroundPlaneUI;
    Camera mainCamera;
    Ray cameraToPlaneRay;
    RaycastHit hit;
    GameObject copyShoe;
    List<MeshCollider> meshColliders = new List<MeshCollider>();
    bool doesHitShoe;

    float m_PlacementAugmentationScale;
    Vector3 ProductScaleVector;
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        m_GroundPlaneUI = FindObjectOfType<GroundPlaneUI>();

        // Enable floor collider if running on device; Disable if running in PlayMode
        Floor.gameObject.SetActive(!VuforiaRuntimeUtilities.IsPlayMode());

        mainCamera = Camera.main;

        m_PlacementAugmentationScale = VuforiaRuntimeUtilities.IsPlayMode() ? 1f : ProductSize;

        ProductScaleVector =
            new Vector3(m_PlacementAugmentationScale,
                        m_PlacementAugmentationScale,
                        m_PlacementAugmentationScale);

        gameObject.transform.localScale = ProductScaleVector;

        copyShoe = GameObject.Find("CopyShoe");
        shoeRenderers = copyShoe.GetComponentsInChildren<MeshRenderer>();
        doesHitShoe = false;
        meshColliders.AddRange(copyShoe.GetComponentsInChildren<MeshCollider>());
    }


    void Update()
    {
        if (PlaneManager.planeMode == PlaneManager.PlaneMode.PLACEMENT)
        {
            // shadowRenderer.enabled = 
            foreach (Renderer shoeRenderer in shoeRenderers)
            {
                shoeRenderer.enabled = (IsPlaced || PlaneManager.GroundPlaneHitReceived || PlaneManager.indicator);
            }
            foreach (MeshCollider meshCollider in meshColliders)
            {
                meshCollider.enabled = !doesHitShoe;
            }
        }
        else
        {
            foreach (Renderer shoeRenderer in shoeRenderers)
            {
                shoeRenderer.enabled = !IsPlaced;
            }
        }

        if (PlaneManager.planeMode == PlaneManager.PlaneMode.PLACEMENT && !IsPlaced)
        {
            m_RotationIndicator.SetActive(Input.touchCount == 2);
            if (m_RotationIndicator.activeSelf)
            {
                m_RotationIndicator.transform.position = copyShoe.transform.position;
                m_RotationIndicator.transform.position -= Vector3.up * 0.2f;
            }

            m_TranslationIndicator.SetActive(
                !m_GroundPlaneUI.IsCanvasUIPressed() && (TouchHandler.IsSingleFingerDragging || TouchHandler.IsSingleFingerStationary) && doesHitShoe);
            if (m_TranslationIndicator.activeSelf)
            {
                m_TranslationIndicator.transform.position = copyShoe.transform.position;
                m_TranslationIndicator.transform.position -= Vector3.up * 0.2f;
            }

            if (!m_GroundPlaneUI.IsCanvasUIPressed())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    cameraToPlaneRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(cameraToPlaneRay, out hit))
                    {
                        if (hit.collider.name.Equals(VuforiaRuntimeUtilities.IsPlayMode() ? EmulatorGroundPlane : Floor.name))
                        {
                            doesHitShoe = false;
                        }
                        else
                        {
                            doesHitShoe = true;
                        }
                    }
                }
                else if (TouchHandler.IsSingleFingerDragging || (VuforiaRuntimeUtilities.IsPlayMode() && Input.GetMouseButton(0)))
                {
                    cameraToPlaneRay = mainCamera.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(cameraToPlaneRay, out hit))
                    {
                        if (hit.collider.gameObject.name ==
                            (VuforiaRuntimeUtilities.IsPlayMode() ? EmulatorGroundPlane : Floor.name) && doesHitShoe)
                        {
                            gameObject.PositionAt(hit.point);
                        }
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    doesHitShoe = false;
                }
            }
        }
        else
        {
            m_RotationIndicator.SetActive(false);
            m_TranslationIndicator.SetActive(false);
        }

    }
    #endregion // MONOBEHAVIOUR_METHODS


    #region PUBLIC_METHODS
    public void Reset()
    {
        transform.position = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = ProductScaleVector;
    }

    public void SetProductAnchor(Transform transform)
    {
        IsPlaced = false;

        if (transform)
        {
            gameObject.transform.SetParent(transform);
            gameObject.transform.localPosition = Vector3.zero;
        }
        else
        {
            gameObject.transform.SetParent(null);
            UtilityHelper.RotateTowardCamera(gameObject);
        }
    }
    #endregion // PUBLIC_METHODS
}
