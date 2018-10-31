using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBar : MonoBehaviour {

    public Button buy, share;
    public string link { get; set; }

	// Use this for initialization
	void Start () {
        buy.onClick.AddListener(BuyButtonClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void BuyButtonClick(){
        //UIManager.Instance.shopPanel.Init(link);
        UIManager.Instance.shopPanel.url = link;
        UIManager.Instance.navigationView.Push(UIManager.Instance.shopPanel);
    }
}
