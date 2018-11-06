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

    DetectorController detector;
    ARLight arLight;

    private void Start()
    {
        detector = FindObjectOfType<DetectorController>();
        arLight = FindObjectOfType<ARLight>();
    }

    public void ClickSetting()
    {
        settingPanel.SetActive(true);
        sizeInputField.text = detector.shoeScale.ToString();
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
}
