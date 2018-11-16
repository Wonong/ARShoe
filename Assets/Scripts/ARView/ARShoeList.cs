using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ARShoeList : MonoBehaviour {
    public GameObject contents;
    public GameObject contentPrefab;
    public ShoeController shoeController;
    public GameObject indicatorParent;
    public GameObject indicators;
    public GroundPlaneUI groundPlaneUI;

    RectTransform contentsRectTransform;
    List<RawImage> shoeRawImageList;
    List<Button> shoeButtonList;
    Button shoeButton;
    List<Shoe> shoesList;
    float shoeImageWidth = 300f;
    float shoeImageSpace = 60f;
    float scrollContentsWidth;

    // Use this for initialization
    void Start () {
        contentsRectTransform = contents.GetComponent<RectTransform>();
        shoesList = JSONHandler.GetAllShoesList();
        scrollContentsWidth = (shoeImageSpace + shoeImageWidth) * shoesList.Count;
        for (int i = 0; i < shoesList.Count; i++)
        {
            GameObject shoesImage = Instantiate(contentPrefab);
            shoesImage.transform.SetParent(contents.transform);
        }

        shoeRawImageList = new List<RawImage>(contents.GetComponentsInChildren<RawImage>());
        shoeButtonList = new List<Button>(contents.GetComponentsInChildren<Button>());

        for (int i = 0; i < shoeRawImageList.Count; i++)
        {
            ShoeAR shoeAR = shoeButtonList[i].gameObject.GetComponent<ShoeAR>();
            shoeAR.id = shoesList[i].id;
            shoeAR.link = shoesList[i].link;
            shoeRawImageList[i].texture = Resources.Load<Texture>(shoesList[i].imgPath);
            shoeButtonList[i].onClick.AddListener(delegate { ClickShoeImage(shoeAR.id); });
        }
    }

    // Update is called once per frame
    void Update () {
        if(contentsRectTransform.offsetMin.x <= -(scrollContentsWidth - Screen.width))
        {
            contentsRectTransform.offsetMin = new Vector2(-(scrollContentsWidth - Screen.width), contentsRectTransform.offsetMin.y);
            contentsRectTransform.offsetMax = new Vector2(67f , contentsRectTransform.offsetMax.y);
        }
        else if(contentsRectTransform.offsetMin.x >= 67f)
        {
            contentsRectTransform.offsetMin = new Vector2(67f, contentsRectTransform.offsetMin.y);
            contentsRectTransform.offsetMax = new Vector2(0f, contentsRectTransform.offsetMax.y);
        }
    }

    private void ClickShoeImage(int id)
    {
        if (id != CurrentCustomShoe.currentShoeId)
        {
            if (indicatorParent != null)
            {
                indicators.transform.SetParent(indicatorParent.transform);
            }
            Vector3 position = shoeController.shoes.transform.localPosition;
            Quaternion rotation = shoeController.shoes.transform.localRotation;
            Vector3 scale = shoeController.shoes.transform.localScale;

            Destroy(shoeController.shoes);
            CurrentCustomShoe.SetCurrentCustomShoe(id);
            shoeController.CreateShoe();
            shoeController.InitShoe();
            if (indicatorParent != null)
            {
                indicators.transform.SetParent(shoeController.shoes.transform);
                indicators.transform.localPosition = new Vector3(0, 0, 0);
                indicators.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else 
            {
                shoeController.shoes.SetActive(true);
                shoeController.shoes.transform.localPosition = position;
                shoeController.shoes.transform.localRotation = rotation;
                shoeController.shoes.transform.localScale = scale;
            }
            groundPlaneUI.CheckCustomizingOK();

            // customzie menu 초기화
            UIManager.Instance.customizePanel.RefreshCustomizeMenu(id);
        }
    }
}
