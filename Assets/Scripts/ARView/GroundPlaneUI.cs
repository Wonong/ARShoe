/*==============================================================================
Copyright (c) 2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
==============================================================================*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GroundPlaneUI : MonoBehaviour
{
    #region PUBLIC_MEMBERS
    [Header("UI Buttons")]
    public Button m_ResetButton;
    public Button m_LockButton;
    public Button m_BackButton;
    public Button m_CaptureButton;
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    ARController m_ARController;
    AudioSource shoePuttingSound;
    GraphicRaycaster[] m_GraphicRayCasters;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        m_ARController = FindObjectOfType<ARController>();
        m_GraphicRayCasters = FindObjectsOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();
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
        m_ARController.IsPlaced = true;
        m_ARController.FixShoe();
        m_LockButton.image.sprite = Resources.Load<Sprite>("Sprites/Icons/UI_Icon_LockLocked");
    }

    private void SetShoeMovable()
    {
        m_ARController.IsPlaced = false;
        m_ARController.MoveShoe();
        m_LockButton.image.sprite = Resources.Load<Sprite>("Sprites/Icons/UI_Icon_LockUnlocked");
    }

    void ClickResetButton()
    {
        m_ARController.ResetAR();
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
            = m_CaptureButton.interactable = m_ARController.DoesShoeActive;
    }
#endregion // MONOBEHAVIOUR_METHODS
}
