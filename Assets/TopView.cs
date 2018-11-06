using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopView : MonoBehaviour {

    public Button shopBut, arExpBut, arViewBut;

    // Use this for initialization
    void Start () {
        shopBut.onClick.AddListener(ShopButtonClick);
        arExpBut.onClick.AddListener(ARExpButtonClick);
        arViewBut.onClick.AddListener(ARViewButtonClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void ShopButtonClick()
    {
        // ToDo : shop panel link activate
        //UIManager.Instance.shopPanel.url = link;
        UIManager.Instance.navigationView.Push(UIManager.Instance.shopPanel); 
    }

    void ARExpButtonClick()
    {
        SceneChanger.ChangeToAttachShoes();
    }

    void ARViewButtonClick()
    {
        SceneChanger.ChangeToWatchingShoes();
    }
}
