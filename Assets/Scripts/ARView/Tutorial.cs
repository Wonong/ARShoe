using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {
    public GameObject tutorialPanel;
    public GameObject smartphone;
    public Button okButton;

    RectTransform smartphoneRectTransform;
    Image smartphoneImage;

    Vector2 startVector = new Vector2(-300f, 0);
    Vector2 midVector = new Vector2(0f, 200f);
    Vector2 endVector = new Vector2(300f, 0);
    Vector2 centerVector = new Vector2(0, 0);

	// Use this for initialization
	void Start () {
        tutorialPanel.SetActive(true);
        smartphoneRectTransform = smartphone.GetComponent<RectTransform>();
        smartphoneImage = smartphone.GetComponent<Image>();
        StartCoroutine(StartTutorial());
        okButton.onClick.AddListener(EndTutorial);
	}

    IEnumerator StartTutorial()
    {
        float currentTime = 0f;
        float timeOver = 1f;

        while (currentTime < timeOver)
        {
            currentTime += Time.deltaTime;
            float normalizedValue = currentTime / timeOver; // we normalize our time 

            smartphoneRectTransform.anchoredPosition = Vector2.Lerp(startVector, midVector, normalizedValue);
            yield return null;
        }

        currentTime = 0f;
        while (currentTime < timeOver)
        {
            currentTime += Time.deltaTime;
            float normalizedValue = currentTime / timeOver; // we normalize our time 

            smartphoneRectTransform.anchoredPosition = Vector2.Lerp(midVector, endVector, normalizedValue);
            yield return null;
        }

        currentTime = 0f;
        while (currentTime < timeOver)
        {
            currentTime += Time.deltaTime;
            float normalizedValue = currentTime / timeOver; // we normalize our time 

            smartphoneRectTransform.anchoredPosition = Vector2.Lerp(endVector, centerVector, normalizedValue);
            yield return null;
        }
    }

    void EndTutorial()
    {
        tutorialPanel.SetActive(false);
    }
}
