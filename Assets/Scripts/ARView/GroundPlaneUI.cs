using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class GroundPlaneUI : MonoBehaviour
{
    #region PUBLIC_MEMBERS
    [Header("UI Buttons")]
    public Button m_ResetButton;
    public Button m_ConfirmButton;
    public Button m_BackButton;
    public Button m_CaptureButton;
    public Button m_ListUpDown;
    public Button m_ShoeListDownUp;
    public Button m_ShoeLeftRightTextButton;
    public Button m_SceneChangeButton;
    public Button m_HeartButton;
    public Button m_SocialShareButton;
    public Button m_BuyButton;

    [Header("UI Panels")]
    public RectTransform m_CustomListRectTransform;
    public RectTransform m_BottomMidToolbarRectTrnasform;
    public RectTransform m_ShoeListRectTransform;
    public RectTransform m_TopMidToolbarRectTransform;

    public static String leftRightName;
    #endregion // PUBLIC_MEMBERS


    #region PRIVATE_MEMBERS
    ShoeController m_ShoeController;
    DetectorController m_DetectController;
    ARController m_ARController;
    AudioSource shoePuttingSound;
    GraphicRaycaster[] m_GraphicRayCasters;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    GameObject m_CustomScrollView;
    GameObject shopWebView;
    float customizingListSize = 670f;
    float shoeListSize = 280f;
    #endregion // PRIVATE_MEMBERS

    #region MONOBEHAVIOUR_METHODS
    void Start()
    {
        m_ARController = FindObjectOfType<ARController>();
        m_ShoeController = FindObjectOfType<ShoeController>();
        m_DetectController = FindObjectOfType<DetectorController>();
        m_GraphicRayCasters = FindObjectsOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();
        InitializeButtons();
        ChangeButtonStatus();

        CheckCustomizingOK();
    }

    void Update()
    {
        ChangeButtonStatus();
        // If user click android back button, then call click back button method.
        if (Application.platform == RuntimePlatform.Android && Input.GetKey(KeyCode.Escape))
        {
            if (!ScreenshotPreview.previewGameObject.activeSelf && shopWebView==null)
            {
                ClickBackButton();
            }
            else if (ScreenshotPreview.previewGameObject.activeSelf)
            {
                ScreenshotPreview.previewGameObject.SetActive(false);
            }
            else if(shopWebView!=null) 
            {
                Destroy(shopWebView);
            }
        }
    }

    /// <summary>
    /// Sets the custom scroll view into Canvas.
    /// </summary>
    private void SetCustomScrollView()
    {
        m_CustomScrollView = UIManager.Instance.customizePanel.customize.gameObject;
        UIManager.Instance.customizePanel.customize.transform.SetParent(m_CustomListRectTransform.gameObject.transform);


        // scroll rect content initialize
        m_CustomListRectTransform.GetComponent<ScrollRect>().content = m_CustomScrollView.GetComponent<RectTransform>();

        RectTransform customScrollViewRectTransform = m_CustomScrollView.GetComponent<RectTransform>();
        customScrollViewRectTransform.anchorMax = new Vector2(1f, 1f);
        customScrollViewRectTransform.anchorMin = new Vector2(0f, 0f);
        customScrollViewRectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
        customScrollViewRectTransform.offsetMin = new Vector2Int(67, 0);
        customScrollViewRectTransform.offsetMax = new Vector2Int(-67, 0);
        customScrollViewRectTransform.localPosition += Vector3.down*240;
    }

    /// <summary>
    /// Add listener to each button.
    /// </summary>
    void InitializeButtons()
    {
        m_BackButton.onClick.AddListener(ClickBackButton);
        m_ResetButton.onClick.AddListener(ClickResetButton);
        m_CaptureButton.onClick.AddListener(ClickCaptureButton);
        m_ListUpDown.onClick.AddListener(ClickListUpDownButton);
        m_ShoeListDownUp.onClick.AddListener(ClickShoeListDownUpButton);
        m_ShoeLeftRightTextButton.onClick.AddListener(ClickShoeLeftRightTextButton);
        m_SceneChangeButton.onClick.AddListener(ClickSceneChangeButton);
        m_HeartButton.onClick.AddListener(ClickHeartButton);
        m_SocialShareButton.onClick.AddListener(ClickSocialShareButton);
        m_BuyButton.onClick.AddListener(ClickBuyButton);
        if (SceneManager.GetActiveScene().name.Equals("WatchingShoes"))
        {
            m_ConfirmButton.onClick.AddListener(ClickConfirmButton);
        }

        #region DEBUG
        var m_Setting = FindObjectOfType<Setting>();
        m_HeartButton.onClick.AddListener(()=>
        {
            if (m_Setting.settingPanel.activeSelf)
            {
                m_Setting.ClickDismissButton();
            } else
            {
                m_Setting.ClickSetting();
            }
        });
        #endregion
    }

    void ClickBackButton()
    {
        CurrentCustomShoe.shoes.GetComponent<Swiper>().enabled = true;
        CurrentCustomShoe.shoes.GetComponent<Spin>().enabled = false;
        SceneChanger.ChangeToListScene();
        UIManager.Instance.customizePanel.customize.transform.SetParent(UIManager.Instance.customizePanel.contentObj.transform);
        m_ShoeController.shoes.transform.SetParent(CurrentCustomShoe.shoeParent.transform);
        m_ShoeController.shoes.SetActive(true);
        m_ShoeController.shoes.transform.localScale = new Vector3(1f, 1f, 1f);
        m_ShoeController.shoeLeft.SetActive(true);
        m_ShoeController.shoeRight.SetActive(false);
        CurrentCustomShoe.shoeParent.transform.localPosition = new Vector3(0, 0, 0);
        m_ShoeController.shoes.transform.position = new Vector3(-913.4f, 7.07f, -9.9f);
        m_ShoeController.shoeLeft.transform.localRotation = Quaternion.Euler(0, 0, 45);
        if(m_ARController!=null)
        {
            m_ARController.DeleteIndicators();
        }
        else 
        {
            switch(CurrentCustomShoe.currentShoeId) {
                case 1:
                    Destroy(GameObject.Find("TransparentPrefab(Clone)"));
                    break;
                case 2:
                    Destroy(GameObject.Find("TransparentPrefab2(Clone)"));
                    break;
            }
        }
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
        m_ShoeController.IsPlaced = true;
        m_ShoeController.PlaceShoe();
        m_ConfirmButton.image.enabled = false;
        ChangeButtonStatus();
    }

    void ClickResetButton()
    {
        if(SceneManager.GetActiveScene().name.Equals("WatchingShoes"))
        {
            m_ShoeController.ResetAR();
           
            ChangeButtonStatus();
        }
        else
        {
            // Attaching scene에서 reset 호출 시의 코드 작성.
            m_DetectController.ClickRepeatButton();
        }
    }

    /// <summary>
    /// Capture and save image, the image can be shared optionally.
    /// </summary>
    void ClickCaptureButton()
    {
        StartCoroutine(ScreenshotPreview.CaptureAndShowPreviewImage()); // Start coroutine for screenshot function.
    }

    /// <summary>
    /// Clicks the list up and down button, then UIs are changed.
    /// </summary>
    void ClickListUpDownButton()
    {
        Vector2 originalPanelVector = m_CustomListRectTransform.anchoredPosition;
        Vector2 goalPanelVector;
        Vector2 originalToolbarVector = m_BottomMidToolbarRectTrnasform.anchoredPosition;
        Vector2 goalToolbarVector;
        if (m_ListUpDown.image.sprite.name.Equals("arrow_up"))
        {
            m_ListUpDown.image.sprite = Resources.Load<Sprite>("Sprites/Arshoe/arrow_down");
            goalPanelVector = new Vector2(m_CustomListRectTransform.anchoredPosition.x, 0f);
            goalToolbarVector = new Vector2(m_BottomMidToolbarRectTrnasform.anchoredPosition.x, m_BottomMidToolbarRectTrnasform.anchoredPosition.y + customizingListSize);
            StartCoroutine(ListUpOrDownAnimation(originalPanelVector, goalPanelVector, originalToolbarVector, goalToolbarVector, m_CustomListRectTransform, m_BottomMidToolbarRectTrnasform));
        }
        else
        {
            m_ListUpDown.image.sprite = Resources.Load<Sprite>("Sprites/Arshoe/arrow_up");
            goalPanelVector = new Vector2(m_CustomListRectTransform.anchoredPosition.x, -customizingListSize);
            goalToolbarVector = new Vector2(m_BottomMidToolbarRectTrnasform.anchoredPosition.x, m_BottomMidToolbarRectTrnasform.anchoredPosition.y - customizingListSize);
            StartCoroutine(ListUpOrDownAnimation(originalPanelVector, goalPanelVector, originalToolbarVector, goalToolbarVector, m_CustomListRectTransform, m_BottomMidToolbarRectTrnasform));
        }
    }

    void ClickShoeListDownUpButton()
    {
        Vector2 originalPanelVector = m_ShoeListRectTransform.anchoredPosition;
        Vector2 goalPanelVector;
        Vector2 originalToolbarVector = m_TopMidToolbarRectTransform.anchoredPosition;
        Vector2 goalToolbarVector;

        if (m_ShoeListDownUp.image.sprite.name.Equals("arrow_up"))
        {
            m_ShoeListDownUp.image.sprite = Resources.Load<Sprite>("Sprites/Arshoe/arrow_down");
            goalPanelVector = new Vector2(m_ShoeListRectTransform.anchoredPosition.x, shoeListSize);
            goalToolbarVector = new Vector2(m_TopMidToolbarRectTransform.anchoredPosition.x, 0);
            StartCoroutine(ListUpOrDownAnimation(originalPanelVector, goalPanelVector, originalToolbarVector, goalToolbarVector, m_ShoeListRectTransform, m_TopMidToolbarRectTransform));
        }
        else
        {
            m_ShoeListDownUp.image.sprite = Resources.Load<Sprite>("Sprites/Arshoe/arrow_up");
            goalPanelVector = new Vector2(m_ShoeListRectTransform.anchoredPosition.x, 0);
            goalToolbarVector = new Vector2(m_TopMidToolbarRectTransform.anchoredPosition.x, -shoeListSize);
            StartCoroutine(ListUpOrDownAnimation(originalPanelVector, goalPanelVector, originalToolbarVector, goalToolbarVector, m_ShoeListRectTransform, m_TopMidToolbarRectTransform));
        }
    }

    /// <summary>
    /// Extends or shrink the Panel height, change toolbar's anchor y position.
    /// </summary>
    /// <returns>The or shrink height.</returns>
    /// <param name="originalPanelVector">Original panel vector.</param>
    /// <param name="goalPanelVector">Goal vector.</param>
    /// <param name="originalToolbarVector">Original toolbar vector.</param>
    /// <param name="goalToolbarVector">Goal toolbar vector.</param>
    IEnumerator<RectTransform> ListUpOrDownAnimation(Vector2 originalPanelVector, Vector2 goalPanelVector, Vector2 originalToolbarVector, Vector2 goalToolbarVector,
                                                     RectTransform listRectTransform, RectTransform toolbarRectTransform)
    {
        float currentTime = 0f;
        float timeOver = 0.3f;

        while (currentTime < timeOver)
        {
            currentTime += Time.deltaTime;
            float normalizedValue = currentTime / timeOver; // we normalize our time 

            listRectTransform.anchoredPosition = Vector2.Lerp(originalPanelVector, goalPanelVector, normalizedValue);
            toolbarRectTransform.anchoredPosition = Vector2.Lerp(originalToolbarVector, goalToolbarVector, normalizedValue);
            yield return null;
        }
    }

    /// <summary>
    /// Change shoe object to left or right
    /// </summary>
    void ClickShoeLeftRightTextButton()
    {
        if (m_ShoeLeftRightTextButton.image.sprite.name.Equals("right"))
        {
            m_ShoeLeftRightTextButton.image.sprite = Resources.Load<Sprite>("Sprites/Arshoe/left");
        }
        else
        {
            m_ShoeLeftRightTextButton.image.sprite = Resources.Load<Sprite>("Sprites/Arshoe/right");
        }
        m_ShoeController.ChangeLeftRight(leftRightName=m_ShoeLeftRightTextButton.image.sprite.name);
    }

    void ClickSceneChangeButton()
    {
        if (SceneManager.GetActiveScene().name.Equals("WatchingShoes"))
        {
            SceneChanger.ChangeToAttachShoes();
        }
        else
        {
            SceneChanger.ChangeToWatchingShoes();
        }
    }

    void ClickHeartButton()
    {
        if (m_HeartButton.image.sprite.name.Equals("heart"))
        {
            m_HeartButton.image.sprite = Resources.Load<Sprite>("Sprites/Arshoe/heart_red");
            ColorBlock colorBlock = m_HeartButton.colors;
            colorBlock.highlightedColor = new Color32(255, 255, 255, 255);
            colorBlock.normalColor = new Color32(255, 255, 255, 255);
            m_HeartButton.colors = colorBlock;
            AndroidController._ShowAndroidToastMessage("관심 상품에 추가되었습니다.");
        }
        else
        {
            m_HeartButton.image.sprite = Resources.Load<Sprite>("Sprites/Arshoe/heart");
            ColorBlock colorBlock = m_HeartButton.colors;
            colorBlock.highlightedColor = new Color32(0, 0, 0, 255);
            colorBlock.normalColor = new Color32(0, 0, 0, 255);
            m_HeartButton.colors = colorBlock;
            AndroidController._ShowAndroidToastMessage("관심 상품에서 제외되었습니다.");
        }
    }

    /// <summary>
    /// Share link or itme info, etc.
    /// </summary>
    void ClickSocialShareButton()
    {
        // Solved(원영): 아래 "text"에 공유할 링크 CurrentCustomShoe.링크멤버변수 할당.
        #if UNITY_ANDROID
        new NativeShare().SetText(JSONHandler.GetShoeById(CurrentCustomShoe.currentShoeId).link).Share();
        #elif UNITY_IOS
        new NativeShare().SetText(JSONHandler.GetShoeById(CurrentCustomShoe.currentShoeId).link).Share();
        #endif
    }

    void ClickBuyButton() {
        UIManager.Instance.SetShopUrl(JSONHandler.GetShoeById(CurrentCustomShoe.currentShoeId).link);
        UIManager.Instance.navigationView.Push(UIManager.Instance.shopPanel);
    }

    public void SetShoeMovable()
    {
        m_ShoeController.IsPlaced = false;
        m_ShoeController.MoveShoe();
        m_ConfirmButton.image.enabled = true;
        ChangeButtonStatus();
    }

    // Change button's clickability and visualization.
    // Return true: If shoe object does not placed and arcore detect floor, or shoe object placed.
    public void ChangeButtonStatus() {
        m_ResetButton.interactable = m_CaptureButton.interactable = m_ConfirmButton.interactable = m_ShoeController.DoesShoeActive;
        m_ConfirmButton.image.enabled = m_ShoeController.DoesShoeActive && !m_ShoeController.IsPlaced;
    }

    public void CheckCustomizingOK()
    {
        // 커스터마이징 가능 여부에따라 UI 변화
        if (JSONHandler.GetShoeById(CurrentCustomShoe.currentShoeId).isCustomizable)
        {
            SetCustomScrollView();
            m_ListUpDown.gameObject.SetActive(true);
        }
        else
        {
            m_ListUpDown.gameObject.SetActive(false);
        }
    }
#endregion // MONOBEHAVIOUR_METHODS
}
