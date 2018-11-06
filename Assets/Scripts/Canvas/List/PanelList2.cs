using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelList2 : MonoBehaviour {

    public Button backBut;
    public Text titleLabel;
    public List<Shoe> shoeList;

    // ToDo: 카테고리 각 항목에 따라 다른 아이템 표시 기능

	// Use this for initialization
	void Start () {

        backBut.onClick.AddListener(() => { UIManager.Instance.viewStack.Pop(); });

        shoeList = JSONHandler.GetAllShoesList();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
