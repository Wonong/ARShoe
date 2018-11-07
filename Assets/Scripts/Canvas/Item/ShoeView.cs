using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoeView : MonoBehaviour {

    public Button backBut, arExpBut, arViewBut;

	// Use this for initialization
	void Start () {
        backBut.onClick.AddListener(BackButtonClick);
        arExpBut.onClick.AddListener(ARExpButtonClick);
        arViewBut.onClick.AddListener(ARViewButtonClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void BackButtonClick(){
        UIManager.Instance.navigationView.Pop();
    }

    void ARExpButtonClick(){
        SceneChanger.ChangeToAttachShoes();
    }

    void ARViewButtonClick(){
        SceneChanger.ChangeToWatchingShoes();
    }
}
