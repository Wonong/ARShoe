using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBar : MonoBehaviour {

    public Button buy, share;
    public string link;

	// Use this for initialization
	void Start () {
        buy.onClick.AddListener(BuyButtonClick);
        share.onClick.AddListener(ShareButtonClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void BuyButtonClick(){
        UIManager.Instance.SetShopUrl(CurrentCustomShoe.currentShoeInfo.link);
        //UIManager.Instance.shopPanel.webView.Load(CurrentCustomShoe.currentShoeInfo.link);
        UIManager.Instance.navigationView.Push(UIManager.Instance.shopPanel);
    }

    void ShareButtonClick(){

    }
}
