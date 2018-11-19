using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizePanel : ViewController {

    public int shoeId;
    public Text name, price, company;
    public MenuBar menuBar;
    public CustomizeMenu customize;
    public GameObject contentObj;

	// Use this for initialization
	void Start () {

    }

    private void OnEnable()
    {
        if(CurrentCustomShoe.currentShoeId != null){
            ResetShoeInfo(JSONHandler.GetShoeById(CurrentCustomShoe.currentShoeId));
        }
    }

    private void OnDisable()
    {

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Init(Shoe currentShoe){

        ResetShoeInfo(currentShoe);
        CurrentCustomShoe.SetCurrentCustomShoe(currentShoe.id);

        if (currentShoe.isCustomizable)
        {
            RefreshCustomizeMenu(currentShoe.id);
        }
        else {
            customize.gameObject.SetActive(false);
        }

    }

    public void RefreshCustomizeMenu(int shoeId){
        // Customize menu 초기화
        customize.DeleteSelectParts();
        customize.AddSelectParts(JSONHandler.GetPartsListByShoeId(shoeId));
        customize.gameObject.SetActive(true);

        // Reinitialize height of parent object of customize by content size fitter
        contentObj.GetComponent<ContentSizeFitter>().enabled = true;
    }

    public void ResetShoeInfo(Shoe shoe){
        shoeId = shoe.id;
        name.text = shoe.name;
        price.text = shoe.GetPriceAsString();
        company.text = shoe.company;
        menuBar.link = shoe.link;
    }
}
