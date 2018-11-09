using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBar : MonoBehaviour {

    public Button buy, share, capture;
    public string link { get; set; }

	// Use this for initialization
	void Start () {
        buy.onClick.AddListener(BuyButtonClick);
        share.onClick.AddListener(ShareButtonClick);
        capture.onClick.AddListener(CaptureButtonClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void BuyButtonClick(){
        UIManager.Instance.shopPanel.RefreshWebView(link);
        UIManager.Instance.navigationView.Push(UIManager.Instance.shopPanel);
    }

    void ShareButtonClick(){

    }

    void CaptureButtonClick(){

    }
}
