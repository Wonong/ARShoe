using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelList : ViewController {

    private NavigationViewController navigationView;
    public Camera cam;
    public Vector2 minPos, maxPos;
    public ShoeRow newRow, bestRow;
    public TopView topView;

	// Use this for initialization
	void Start () {

        navigationView = UIManager.Instance.navigationView;
        //GetComponent<ScrollRect>().onValueChanged.AddListener(OnScrollChanged);
        AddShoeItemsIntoRow();

        // TopView 의 신발 설정(현재는 리스트의 첫번쨰 신발)
        Debug.Log("shoe id : " + CurrentCustomShoe.currentShoeId);
        if(CurrentCustomShoe.shoes == null){
            CurrentCustomShoe.SetCurrentCustomShoe(1);
        } 
        topView.link = JSONHandler.GetShoeById(CurrentCustomShoe.currentShoeId).link;
	}
	
	// Update is called once per frame
	void Update () {

    }

    void AddShoeItemsIntoRow(){
        UIManager.Instance.itemList.ForEach((item) =>
        {
            if(item.isNew) newRow.AddNewShoe(item);
            if(item.isBest) bestRow.AddNewShoe(item);
        });
        newRow.content.GetComponent<ContentSizeFitter>().enabled = true;
        bestRow.content.GetComponent<ContentSizeFitter>().enabled = true;

        newRow.shoeScroll.horizontalNormalizedPosition = 0;
        bestRow.shoeScroll.horizontalNormalizedPosition = 0;
    }
}
