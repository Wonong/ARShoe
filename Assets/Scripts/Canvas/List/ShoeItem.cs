using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoeItem : MonoBehaviour {

    public Button buttonComponent;
    public RawImage icon;
    public Text companyLabel, nameLabel, priceLabel;

    private Shoe item;

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
        companyLabel.text = item.company;
        nameLabel.text = item.name;
        priceLabel.text = item.price.ToString();
    }

    public void ClickFunc() {
        UIManager.Instance.customizePanel.Init(item);
        UIManager.Instance.navigationView.Push(UIManager.Instance.customizePanel);
    }
}
