using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Setting : MonoBehaviour
{
    public GameObject settingPanel;
    public InputField inputField;
    public DetectorController detector;
    public Button confirmButton;

    private void Start()
    {
        detector = FindObjectOfType<DetectorController>();
    }

    public void ClickSetting()
    {
        settingPanel.SetActive(true);
    }

    public void ClickConfirmButton()
    {
        Debug.Log("Click confirm");
    }
}
