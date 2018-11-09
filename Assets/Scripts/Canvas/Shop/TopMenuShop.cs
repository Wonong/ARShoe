using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopMenuShop : MonoBehaviour {

    public Button backBut;
    public Text title;

	// Use this for initialization
	void Start () {
        backBut.onClick.AddListener(BackButtonClicked);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void BackButtonClicked(){
        UIManager.Instance.navigationView.Pop();
        UIManager.Instance.topMenu.gameObject.SetActive(true);
    }

    public void Init(){
        UIManager.Instance.topMenu.gameObject.SetActive(false);
        title.text = JSONHandler.GetShoeById(CurrentCustomShoe.currentShoeId).name;
    }
}
