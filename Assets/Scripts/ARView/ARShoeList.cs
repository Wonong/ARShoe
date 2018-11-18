using System.Collections.Generic;
using UnityEngine;
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
    List<GameObject> shoeCheckImageList = new List<GameObject>();
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
            GameObject checkImage = shoesImage.transform.GetChild(0).gameObject;
            shoeCheckImageList.Add(checkImage);
            if (shoesList[i].id == CurrentCustomShoe.currentShoeId) {
                checkImage.SetActive(true);
            }
            else {
                checkImage.SetActive(false);
            }
        }

        shoeRawImageList = new List<RawImage>(contents.GetComponentsInChildren<RawImage>());
        shoeButtonList = new List<Button>(contents.GetComponentsInChildren<Button>());

        for (int i = 0; i < shoeRawImageList.Count; i++)
        {
            ShoeAR shoeAR = shoeButtonList[i].gameObject.GetComponent<ShoeAR>();
            shoeAR.id = shoesList[i].id;
            shoeAR.link = shoesList[i].link;
            shoeAR.shoeCheckImage = shoeCheckImageList[i];
            shoeRawImageList[i].texture = Resources.Load<Texture>(shoesList[i].imgPath);
            shoeButtonList[i].onClick.AddListener(delegate { ClickShoeImage(shoeAR); });
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

    private void ClickShoeImage(ShoeAR shoeAR)
    {
        if (shoeAR.id != CurrentCustomShoe.currentShoeId)
        {
            if (indicatorParent != null)
            {
                indicators.transform.SetParent(indicatorParent.transform);
            }

            Transform shoeParent = shoeController.shoes.transform.parent;
            Vector3 position = shoeController.shoes.transform.localPosition;
            Quaternion rotation = shoeController.shoes.transform.localRotation;
            Vector3 scale = shoeController.shoes.transform.localScale;

            Destroy(shoeController.shoes);

            CurrentCustomShoe.SetCurrentCustomShoe(shoeAR.id);
            shoeController.CreateShoe();
            shoeController.InitShoe();

            if (indicatorParent != null)
            {
                shoeController.IsPlaced = true;
                indicators.transform.SetParent(shoeController.shoes.transform);
                indicators.transform.localPosition = new Vector3(0, 0, 0);
                indicators.transform.localRotation = Quaternion.Euler(0, 0, 0);
                indicators.SetActive(!shoeController.IsPlaced);
                shoeController.shoes.transform.SetParent(shoeParent);
                shoeController.shoes.transform.localPosition = position;
                shoeController.shoes.transform.localRotation = rotation;
                shoeController.shoes.SetActive(true);
            }
            else 
            {
                shoeController.shoes.SetActive(true);
                shoeController.shoes.transform.localPosition = position;
                shoeController.shoes.transform.localRotation = rotation;
                shoeController.shoes.transform.localScale = scale;
            }
            groundPlaneUI.CheckCustomizingOK();

            // Set Check Image.
            foreach (GameObject shoeCheckImage in shoeCheckImageList)
            {
                shoeCheckImage.SetActive(false);
            }
            shoeAR.shoeCheckImage.SetActive(true);

            // customzie menu 초기화
            UIManager.Instance.customizePanel.RefreshCustomizeMenu(shoeAR.id);
        }
    }
}
