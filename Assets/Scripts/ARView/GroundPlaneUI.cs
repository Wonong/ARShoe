/*==============================================================================
Copyright (c) 2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections;
using System;

public class GroundPlaneUI : MonoBehaviour
{
    #region PUBLIC_MEMBERS
    [Header("UI Elements")]
    public CanvasGroup m_ScreenReticle;

    [Header("UI Buttons")]
    public Button m_ResetButton;
    public Button m_LockButton;
    public Button m_BackButton;
    public Button m_CaptureButton;
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    GraphicRaycaster[] m_GraphicRayCasters;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    ProductPlacement m_ProductPlacement;

    AudioSource shoePuttingSound;
    GameObject shadowPlane;
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        m_ProductPlacement = FindObjectOfType<ProductPlacement>();
        m_GraphicRayCasters = FindObjectsOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();

        m_ProductPlacement.enabled = true;
        shoePuttingSound = GameObject.Find("ShoeParent").GetComponent<AudioSource>();
        shadowPlane = GameObject.Find("ShadowPlane");

        InitializeButtons();
        ChangeButtonStatus();
    }

    void Update()
    {
        ChangeButtonStatus();
        // If user click android back button, then call click back button method.
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!ScreenshotPreview.previewGameObject.activeSelf && Input.GetKey(KeyCode.Escape))
            {
                ClickBackButton();
            }
        }
    }

    void InitializeButtons()
    {
        m_BackButton.onClick.AddListener(ClickBackButton);
        m_LockButton.onClick.AddListener(ClickLockButton);
        m_ResetButton.onClick.AddListener(ClickResetButton);
        m_CaptureButton.onClick.AddListener(ClickCaptureButton);
        m_ResetButton.interactable = m_LockButton.interactable = false;
        m_ResetButton.image.enabled = m_LockButton.image.enabled = true;
    }

    void ClickBackButton()
    {
        CurrentCustomShoe.shoe.GetComponent<Swiper>().enabled = true;
        SceneChanger.ChangeToShoeListScene();
    }

    void ClickLockButton()
    {
        if (m_LockButton.GetComponent<Image>().sprite.name.Equals("UI_Icon_LockUnlocked"))
        {
            SetShoeStopped();
        }
        else
        {
            SetShoeMovable();
        }
        ChangeButtonStatus();
    }

    private void SetShoeStopped()
    {
        m_ProductPlacement.IsPlaced = true;
        GameObject.Find("CopyShoe").transform.parent.position -= Vector3.up * 0.1f;
        shadowPlane.transform.localPosition = new Vector3(0, 0, 0);
        #if (UNITY_IOS || !UNITY_ANDROID)
            shadowPlane.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
            shadowPlane.transform.position -= Vector3.up * 0.1f;
        #elif UNITY_ANDROID
            shadowPlane.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            shadowPlane.transform.position -= Vector3.up * 0.2f;
        #endif
        m_LockButton.image.sprite = Resources.Load<Sprite>("Sprites/Icons/UI_Icon_LockLocked");
        shoePuttingSound.Play();
    }

    private void SetShoeMovable()
    {
        m_ProductPlacement.IsPlaced = false;
        GameObject.Find("CopyShoe").transform.parent.position += Vector3.up * 0.1f;
        shadowPlane.transform.localPosition = new Vector3(0, 0, 0);
        #if (UNITY_IOS || !UNITY_ANDROID)
            shadowPlane.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            shadowPlane.transform.position -= Vector3.up * 0.2f;
        #elif UNITY_ANDROID
            shadowPlane.transform.localScale = new Vector3(0.22f, 0.22f, 0.22f);
            shadowPlane.transform.position -= Vector3.up * 0.3f;
        #endif
        m_LockButton.image.sprite = Resources.Load<Sprite>("Sprites/Icons/UI_Icon_LockUnlocked");
        if (PlaneManager.indicator != null)
        {
            PlaneManager.indicator.SetActive(true); // Find floor for setting shoe.
        }
    }

    void ClickResetButton()
    {
        SetShoeMovable();
        ChangeButtonStatus();
    }

    void ClickCaptureButton()
    {
        StartCoroutine(ScreenshotPreview.CaptureAndShowPreviewImage()); // Start coroutine for screenshot function.
    }

    // Change button's clickability.
    // Return true: If shoe object does not placed and vuforia detect floor, or shoe object placed.
    public void ChangeButtonStatus() {
        m_ResetButton.interactable = m_LockButton.interactable
            = m_CaptureButton.interactable = (!m_ProductPlacement.IsPlaced && PlaneManager.GroundPlaneHitReceived)||m_ProductPlacement.IsPlaced;
    }

    void LateUpdate()
    {
        if (PlaneManager.GroundPlaneHitReceived || PlaneManager.indicator==null || PlaneManager.indicator.activeSelf || m_ProductPlacement.IsPlaced)
        {
            m_ScreenReticle.alpha = 0;
        }
        else
        {
            // No automatic hit test, so set alpha based on which plane mode is active
            m_ScreenReticle.alpha =
                (PlaneManager.planeMode == PlaneManager.PlaneMode.PLACEMENT) ? 1 : 0;
        }
    }

#endregion // MONOBEHAVIOUR_METHODS


#region PUBLIC_METHODS
    public bool IsCanvasUIPressed()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        bool resultIsButton = false;

        foreach(GraphicRaycaster graphicRayCaster in m_GraphicRayCasters)
        {
            graphicRayCaster.Raycast(m_PointerEventData, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponent<Button>() ||
                    result.gameObject.name == "UpperToolbar" ||
                    result.gameObject.name == "UnderToolbar")
                {
                    resultIsButton = true;
                    break;
                }
                Debug.Log(result.gameObject.name);
            }
            if (resultIsButton) break;
        }
        return resultIsButton;
    }
#endregion // PUBLIC_METHODS
}
