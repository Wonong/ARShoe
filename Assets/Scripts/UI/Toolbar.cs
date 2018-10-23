using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toolbar : MonoBehaviour
{

    //public InputField searchField;
    public Button shopping, back, list, search, liked, arBut, arAttachBut;

    // Use this for initialization
    void Start() {
        back.onClick.AddListener(BackButtonClick);
        arBut.onClick.AddListener(ARButtonClick);
        arAttachBut.onClick.AddListener(ARAttachButtonClick);
        shopping.onClick.AddListener(ShoppingButtonClick);
    }

    // Update is called once per frame
    void Update() {

    }


    // 현재 화면에 따라 toolbar 상태 변경
    public void ChangeActivation(string status){

        // 자기자신 제외하고 모든 obj를 deactivate
        foreach(MonoBehaviour obj in GetComponentsInChildren<MonoBehaviour>())
        {

            if(obj.name != "Toolbar") obj.gameObject.SetActive(false);
        }

        switch (status) {
            case "PanelList":
                search.gameObject.SetActive(true);
                list.gameObject.SetActive(true);
                break;

            case "PanelItem":
                shopping.gameObject.SetActive(true);
                back.gameObject.SetActive(true);
                arAttachBut.gameObject.SetActive(true);
                arBut.gameObject.SetActive(true);
                break;

            case "PanelAR":
                back.gameObject.SetActive(true);
                shopping.gameObject.SetActive(true);
                break;

            case "PanelShop":
                back.gameObject.SetActive(true);
                break;
        }
    }

    private void BackButtonClick()
    {
        UIManager.Instance.navigationView.Pop();

    }

    private void ARButtonClick()
    {
        SceneChanger.ChangeToWatchingShoes();
    }

    private void ARAttachButtonClick()
    {
        SceneChanger.ChangeToAttachShoes();
    }

    public void ShoppingButtonClick(){
        UIManager.Instance.navigationView.Push(UIManager.Instance.shopPanel);
    }
}
