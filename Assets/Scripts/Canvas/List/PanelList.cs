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
        CurrentCustomShoe.SetCurrentCustomShoe(1);
        topView.link = JSONHandler.GetShoeById(1).link;

        // navigation view의 첫번째 뷰로 설정
        if (navigationView != null && UIManager.Instance.viewStack.Count == 0)
        {
            navigationView.Push(this);
        }


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
