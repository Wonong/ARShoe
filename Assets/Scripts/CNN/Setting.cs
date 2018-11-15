using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Setting : MonoBehaviour
{
    public GameObject settingPanel;
    public InputField sizeInputField;
    public InputField lightIntensityField;
    public Button sizeConfirmButton;
    public Button lightIntensityButton;
    public Button dismissButton;

    public Slider m_LowerHSlider, m_UpperHSlider;
    public Slider m_LowerSSlider, m_UpperSSlider;
    public Slider m_LowerVSlider, m_UpperVSlider;
    public Text m_HSVRangeText;
    public Button m_HSVConfirmButton;

    public Button m_ToggleDebugButton;
    
    public Button m_ToggleRepeatButton;
    private bool m_IsRepeat = false;

    DetectorController m_DetectController;
    ARLight arLight;

    public InputField m_RepeatRateField;
    public Button m_RepeatRateConfirmButton;
    private float m_RepeatRate = 0.04f;

    private ShoeController m_ShoeController;

    public Button m_ForwardConfirmButton;
    public InputField m_ForwardInputField;

    public Button m_DistanceConfirmButton;
    public InputField m_DistanceInputField;

    public Button m_SaveSettingButton;

    private void Start()
    {
        m_DetectController = FindObjectOfType<DetectorController>();
        m_ShoeController = FindObjectOfType<ShoeController>();
        arLight = FindObjectOfType<ARLight>();

        lightIntensityField.text = arLight.minusLightIntensity.ToString();
        sizeInputField.text = m_ShoeController.shoeScale.ToString();
        m_RepeatRateField.text = m_RepeatRate.ToString();
        m_DistanceInputField.text = m_DetectController.m_CameraShoeDistance.ToString();
        m_ForwardInputField.text = m_DetectController.m_ForwardDistance.ToString();

        m_LowerHSlider.value = (float)m_DetectController.lowerHSV.val[0];
        m_LowerSSlider.value = (float)m_DetectController.lowerHSV.val[1];
        m_LowerVSlider.value = (float)m_DetectController.lowerHSV.val[2];

        m_UpperHSlider.value = (float)m_DetectController.upperHSV.val[0];
        m_UpperSSlider.value = (float)m_DetectController.upperHSV.val[1];
        m_UpperVSlider.value = (float)m_DetectController.upperHSV.val[2];

        m_LowerHSlider.onValueChanged.AddListener(OnSliderChange);
        m_UpperHSlider.onValueChanged.AddListener(OnSliderChange);

        m_LowerSSlider.onValueChanged.AddListener(OnSliderChange);
        m_UpperSSlider.onValueChanged.AddListener(OnSliderChange);

        m_LowerVSlider.onValueChanged.AddListener(OnSliderChange);
        m_UpperVSlider.onValueChanged.AddListener(OnSliderChange);

        m_HSVConfirmButton.onClick.AddListener(ClickHSVConfirmButton);

        m_ToggleDebugButton.onClick.AddListener(ClickToggleDebugButton);
        m_ToggleRepeatButton.onClick.AddListener(ClickToggleRepeatButton);

        m_RepeatRateConfirmButton.onClick.AddListener(ClickRepeatRateConfirmButton);

        sizeConfirmButton.onClick.AddListener(ClickSizeConfirmButton);

        m_ForwardConfirmButton.onClick.AddListener(ClickForwardConfirmButton);

        m_DistanceConfirmButton.onClick.AddListener(ClickDistanceConfirmButton);

        m_SaveSettingButton.onClick.AddListener(ClickSaveSettingButton);

        LoadSetting();
    }

    public void ClickSetting()
    {
        settingPanel.SetActive(true);
        //sizeInputField.text = detector.shoeScale.ToString();
        lightIntensityField.text = arLight.minusLightIntensity.ToString();
    }

    public void ClickSizeConfirmButton()
    {
        var scale = float.Parse(sizeInputField.text);
        m_ShoeController.ChangeScale(scale);
    }

    public void ClickLightConfirmButton()
    {
        arLight.minusLightIntensity = float.Parse(lightIntensityField.text);
    }

    public void ClickDismissButton()
    {
        settingPanel.SetActive(false);
    }

    public void OnSliderChange(float value)
    {
        var lowerHSV = new OpenCVForUnity.Scalar(
            m_LowerHSlider.value,
            m_LowerSSlider.value,
            m_LowerVSlider.value
            );

        var upperHSV = new OpenCVForUnity.Scalar(
            m_UpperHSlider.value,
            m_UpperSSlider.value,
            m_UpperVSlider.value
            );

        m_DetectController.lowerHSV = lowerHSV;
        m_DetectController.upperHSV = upperHSV;
    }

    public void ClickHSVConfirmButton()
    {
        OnSliderChange(0);
        m_DetectController.ClickResetButton();
    }

    public void ClickToggleDebugButton()
    {
        m_DetectController.m_IsDebug = !m_DetectController.m_IsDebug;
    }

    public void ClickToggleRepeatButton()
    {
        m_IsRepeat = !m_IsRepeat;

        var func = nameof(ClickHSVConfirmButton);

        if (m_IsRepeat)
        {
            InvokeRepeating(func, 0, m_RepeatRate);
        } else
        {
            CancelInvoke(func);
        }
    }

    public void ClickRepeatRateConfirmButton()
    {
        m_RepeatRate = float.Parse(m_RepeatRateField.text);
    }

    public void ClickForwardConfirmButton()
    {
        float forwardDistance = float.Parse(m_ForwardInputField.text);
        m_DetectController.m_ForwardDistance = forwardDistance;
    }

    public void ClickDistanceConfirmButton()
    {
        float cameraShoeDistance = float.Parse(m_DistanceInputField.text);
        m_DetectController.m_CameraShoeDistance = cameraShoeDistance;
    }

    void Update()
    {
        m_HSVRangeText.text = string.Format("{0}/{1}/{2} \n {3}/{4}/{5}",
            m_LowerHSlider.value,
            m_LowerSSlider.value,
            m_LowerVSlider.value,

            m_UpperHSlider.value,
            m_UpperSSlider.value,
            m_UpperVSlider.value
        );
    }

    public void ClickSaveSettingButton()
    {
        SaveSetting();
    }

    public void SaveSetting()
    {
        PlayerPrefs.SetFloat("LowerH", (float)m_DetectController.lowerHSV.val[0]);
        PlayerPrefs.SetFloat("UpperH", (float)m_DetectController.upperHSV.val[0]);

        PlayerPrefs.SetFloat("LowerS", (float)m_DetectController.lowerHSV.val[1]);
        PlayerPrefs.SetFloat("UpperS", (float)m_DetectController.upperHSV.val[1]);

        PlayerPrefs.SetFloat("LowerV", (float)m_DetectController.lowerHSV.val[2]);
        PlayerPrefs.SetFloat("UpperV", (float)m_DetectController.upperHSV.val[2]);

        PlayerPrefs.SetFloat("RepeatRate", m_RepeatRate);

        PlayerPrefs.SetFloat("CameraShoeDistance", m_DetectController.m_CameraShoeDistance);

        PlayerPrefs.SetFloat("ShoeScale", m_ShoeController.shoeScale);

        PlayerPrefs.Save();

        Debug.Log("Save Setting");
    }

    public void LoadSetting()
    {
        Debug.Log("Load Setting");
        m_LowerHSlider.value = PlayerPrefs.GetFloat("LowerH", m_LowerHSlider.value);
        m_UpperHSlider.value = PlayerPrefs.GetFloat("UpperH", m_UpperHSlider.value);

        m_LowerSSlider.value = PlayerPrefs.GetFloat("LowerS", m_LowerSSlider.value);
        m_UpperSSlider.value = PlayerPrefs.GetFloat("UpperS", m_UpperSSlider.value);

        m_LowerVSlider.value = PlayerPrefs.GetFloat("LowerV", m_LowerVSlider.value);
        m_UpperVSlider.value = PlayerPrefs.GetFloat("UpperV", m_UpperVSlider.value);

        OnSliderChange(0);

        m_RepeatRateField.text = PlayerPrefs.GetFloat("RepeatRate", m_RepeatRate).ToString();

        ClickRepeatRateConfirmButton();

        m_DistanceInputField.text = PlayerPrefs.GetFloat("CameraShoeDistance", m_DetectController.m_CameraShoeDistance).ToString();

        ClickDistanceConfirmButton();

        sizeInputField.text = PlayerPrefs.GetFloat("ShoeScale", m_ShoeController.shoeScale).ToString();

        ClickSizeConfirmButton();
    }
}
