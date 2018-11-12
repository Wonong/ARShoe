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
    private float m_RepeatRate = 0.1f;

    private ShoeController m_ShoeController;
    
    private void Start()
    {
        m_DetectController = FindObjectOfType<DetectorController>();
        arLight = FindObjectOfType<ARLight>();

        m_LowerHSlider.onValueChanged.AddListener(OnSliderChange);
        m_UpperHSlider.onValueChanged.AddListener(OnSliderChange);

        m_LowerSSlider.onValueChanged.AddListener(OnSliderChange);
        m_UpperSSlider.onValueChanged.AddListener(OnSliderChange);

        m_LowerVSlider.onValueChanged.AddListener(OnSliderChange);
        m_UpperVSlider.onValueChanged.AddListener(OnSliderChange);

        m_HSVConfirmButton.onClick.AddListener(ClickHSVConfirmButton);

        m_LowerHSlider.value = 0;
        m_LowerSSlider.value = 40;
        m_LowerVSlider.value = 125;

        m_UpperHSlider.value = 179;
        m_UpperSSlider.value = 255;
        m_UpperVSlider.value = 255;

        m_ToggleDebugButton.onClick.AddListener(ClickToggleDebugButton);
        m_ToggleRepeatButton.onClick.AddListener(ClickToggleRepeatButton);

        m_RepeatRateConfirmButton.onClick.AddListener(ClickRepeatRateConfirmButton);

        m_ShoeController = FindObjectOfType<ShoeController>();
        sizeConfirmButton.onClick.AddListener(ClickSizeConfirmButton);
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
        var lower = new OpenCVForUnity.Scalar(
            m_LowerHSlider.value,
            m_LowerSSlider.value,
            m_LowerVSlider.value
            );

        var upper = new OpenCVForUnity.Scalar(
            m_UpperHSlider.value,
            m_UpperSSlider.value,
            m_UpperVSlider.value
            );

        m_DetectController.SetHSVRange(lower, upper);
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

        if (m_IsRepeat)
        {
            ClickToggleRepeatButton();
            ClickToggleRepeatButton();
        }
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
}
