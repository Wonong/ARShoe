using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo {
    public string title;
    public RawImage icon;
    public string company, price;
}

public class ItemPanel : ViewController {

    public Text title, price, company;
    public RawImage icon;
    public CustomizeMenu Customize;
    public Button ARView, ARExp;

    public override string Title { get { return title.ToString(); } }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
    }

    // initialize information of panel by information of chosen item
    public void Init(Shoe currentShoe) {
        //title.text = currentItem.name;
        //price.text = currentItem.price.ToString();
        //company.text = currentItem.company;
        icon.texture = currentShoe.GetIconAsTexture();
        UIManager.Instance.shopPanel.url = currentShoe.link;

        CurrentCustomShoe.SetCurrentCustomShoe(currentShoe.id);



        if (currentShoe.isCustomizable){
            Customize.DeleteSelectParts();
            Customize.AddSelectParts(JSONHandler.GetPartsListByShoeId(currentShoe.id));
            Customize.gameObject.SetActive(true);

            // 각 파트별로 첫번쨰 옵션으로 초기화.
            //foreach (CustomizingPart part in JSONHandler.GetPartsListByShoeId(currentShoe.id)){
            //    part.GetMaterial().mainTexture = JSONHandler.GetOptionsListByPartId(part.id)[0].GetTexture();
            //}


        }
    }
}
