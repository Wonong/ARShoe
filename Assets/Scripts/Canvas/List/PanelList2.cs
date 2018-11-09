using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelList2 : ViewController {

    public Button backBut;
    public Text titleLabel;
    public Transform content;
    public List<Shoe> shoeList;
    GameObject tempShoeRow;

    // ToDo: 카테고리 각 항목에 따라 다른 아이템 표시 기능

	// Use this for initialization
	void Start () {

        backBut.onClick.AddListener(() => { UIManager.Instance.navigationView.Pop(); });

        shoeList = JSONHandler.GetAllShoesList();

        for (int i = 0; i < shoeList.Count; i++){
            if(i%2 == 0){
                tempShoeRow = UIManager.Instance.listShoeRowObjPool.GetObject();
                tempShoeRow.transform.SetParent(content, false);
            }
            GameObject item = UIManager.Instance.listShoeItemObjPool.GetObject();
            item.transform.SetParent(tempShoeRow.transform, false);
            item.GetComponent<ShoeItem>().Setup(shoeList[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
