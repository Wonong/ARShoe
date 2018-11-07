using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizePanel : ViewController {

    public Text name, price, company;
    public MenuBar menuBar;
    public CustomizeMenu customize;
    public GameObject contentObj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(Shoe currentShoe){
        name.text = currentShoe.name;
        price.text = currentShoe.price + "";
        company.text = currentShoe.company;
        menuBar.link = currentShoe.link;

        CurrentCustomShoe.SetCurrentCustomShoe(currentShoe.id);

        // Customize menu 초기화
        customize.DeleteSelectParts();
        customize.AddSelectParts(JSONHandler.GetPartsListByShoeId(currentShoe.id));
        customize.gameObject.SetActive(true);

        // Reinitialize height of parent object of customize by content size fitter
        contentObj.GetComponent<ContentSizeFitter>().enabled = true;
    }
}
