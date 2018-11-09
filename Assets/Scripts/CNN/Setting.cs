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

    DetectorController m_DetectController;
    ARLight arLight;

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

    }

    public void ClickSetting()
    {
        settingPanel.SetActive(true);
        //sizeInputField.text = detector.shoeScale.ToString();
        lightIntensityField.text = arLight.minusLightIntensity.ToString();
    }

    public void ClickSizeConfirmButton()
    {
        Debug.Log("Click confirm");
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
