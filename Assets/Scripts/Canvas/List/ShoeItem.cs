using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoeItem : MonoBehaviour {

    public Button buttonComponent;
    public RawImage icon;
    public Text nameLabel, priceLabel;

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
        nameLabel.text = item.company + "_" + item.name;
        priceLabel.text = item.price.ToString();
    }

    public void ClickFunc() {
        if(item.isCustomizable){
            UIManager.Instance.customizePanel.Init(item);
            UIManager.Instance.navigationView.Push(UIManager.Instance.customizePanel);
        }
        else{
            //UIManager.Instance.itemPanel.Init(item);
            //UIManager.Instance.navigationView.Push(UIManager.Instance.itemPanel);
        }

    }
}
