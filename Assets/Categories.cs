using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Categories : MonoBehaviour {

    public Button cancelBut, allBut, newBut, bestBut;

    // ToDo: button click function

	// Use this for initialization
	void Start () {
        cancelBut.onClick.AddListener(() => { 
            UIManager.Instance.categories.gameObject.SetActive(false);
            UIManager.Instance.topMenu.ActiveAllChildButtons();
        });

        allBut.onClick.AddListener(AllButtonClick);
        newBut.onClick.AddListener(NewButtonClick);
        bestBut.onClick.AddListener(BestButtonClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void AllButtonClick(){
    }

    void NewButtonClick(){

    }

    void BestButtonClick(){

    }
}
