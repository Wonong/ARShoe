using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Categories : MonoBehaviour {

    public Button cancelBut, allBut, newBut, bestBut;

    // ToDo: button click function

	// Use this for initialization
	void Start () {
        cancelBut.onClick.AddListener(CancelButtonClick);
        allBut.onClick.AddListener(AllButtonClick);
        newBut.onClick.AddListener(NewButtonClick);
        bestBut.onClick.AddListener(BestButtonClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void CancelButtonClick(){
        UIManager.Instance.categories.gameObject.SetActive(false);
        UIManager.Instance.topMenu.ActiveAllChildButtons();
    }

    void AllButtonClick(){
        CancelButtonClick();
        UIManager.Instance.navigationView.Push(UIManager.Instance.listPanel2);
    }

    void NewButtonClick(){

    }

    void BestButtonClick(){

    }
}
