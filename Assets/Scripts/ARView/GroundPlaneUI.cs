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
    public Button m_ConfirmButton;
    public Button m_BackButton;
    public Button m_CaptureButton;
    public Button m_ListUpDown;
    public Button m_ShoeLeftRightTextButton;
    public Button m_SceneChangeButton;
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
        m_ConfirmButton.onClick.AddListener(ClickConfirmButton);
        m_ResetButton.onClick.AddListener(ClickResetButton);
        m_CaptureButton.onClick.AddListener(ClickCaptureButton);
        m_ResetButton.interactable = m_ConfirmButton.interactable = false;
        m_ResetButton.image.enabled = m_ConfirmButton.image.enabled = true;
    }

    void ClickBackButton()
    {
        CurrentCustomShoe.shoe.GetComponent<Swiper>().enabled = true;
        SceneChanger.ChangeToShoeListScene();
    }

    void ClickConfirmButton()
    {
        if (m_ConfirmButton.image.enabled)
        {
            SetShoeStopped();
        }
    }

    private void SetShoeStopped()
    {
        m_ARController.IsPlaced = true;
        m_ARController.FixShoe();
        m_ConfirmButton.image.enabled = false;
        ChangeButtonStatus();
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

    void ClickListUpDownButton() {
        if(m_ListUpDown.image.name.Equals("up-arrow")) {
            m_ListUpDown.image.sprite = Resources.Load<Sprite>("Sprites/Icons/down-arrow");
        }
        else {
            m_ListUpDown.image.sprite = Resources.Load<Sprite>("Sprites/Icons/up-arrow");
        }
    }

    void ClickShoeLeftRightTextButton() {
        if(m_ShoeLeftRightTextButton.GetComponent<Text>().Equals("R")) {
            m_ShoeLeftRightTextButton.GetComponent<Text>().text = "L";
        }
        else {
            m_ShoeLeftRightTextButton.GetComponent<Text>().text = "R";
        }
    }

    void ClickSceneChangeButton() {
        SceneChanger.ChangeToAttachShoes();
    }

    public void SetShoeMovable()
    {
        m_ARController.IsPlaced = false;
        m_ARController.MoveShoe();
        m_ConfirmButton.image.enabled = true;
        ChangeButtonStatus();
    }

    // Change button's clickability and visualization.
    // Return true: If shoe object does not placed and vuforia detect floor, or shoe object placed.
    public void ChangeButtonStatus() {
        m_ResetButton.interactable = m_CaptureButton.interactable = m_ConfirmButton.interactable = m_ARController.DoesShoeActive;
        m_ConfirmButton.image.enabled = m_ARController.DoesShoeActive && !m_ARController.IsPlaced;
    }
#endregion // MONOBEHAVIOUR_METHODS
}
