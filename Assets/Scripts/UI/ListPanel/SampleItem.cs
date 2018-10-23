using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleItem : MonoBehaviour {

    public Button buttonComponent;
    public RawImage icon;
    public Text nameLabel, priceLabel;

    private Shoe item;

    private ShoeScrollList scrollList;

	// Use this for initialization
	void Start () {
        buttonComponent.onClick.AddListener(ClickFunc);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Setup(Shoe currentItem){
        item = currentItem;
        icon.texture = item.GetIconAsTexture();
        nameLabel.text = item.name;
        priceLabel.text = item.price.ToString();


    }

    public void ClickFunc() {
        UIManager.Instance.itemPanel.Init(item);
        UIManager.Instance.navigationView.Push(UIManager.Instance.itemPanel);

    }
}

