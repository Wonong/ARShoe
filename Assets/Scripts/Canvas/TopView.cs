using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopView : MonoBehaviour {

    public Button shopBut, arExpBut, arViewBut;
    public int shoeId;
    public string link;

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
        link = JSONHandler.GetShoeById(shoeId).link;
        UIManager.Instance.SetShopUrl(link);
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

    void Init(int id){
        shoeId = id;
        CurrentCustomShoe.SetCurrentCustomShoe(id);
    }
}
