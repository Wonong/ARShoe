using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeMenu : ViewController {

    //public SelectPartUI selectPartUI;
    public Button randBut;
    List<SelectPartUI> parts = new List<SelectPartUI>();

	// Use this for initialization
	void Start () {
        randBut.onClick.AddListener(() =>{
            RandCustomize();
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // 파트별 Object 생성
    public void AddSelectParts(List<CustomizingPart> partsList){
        foreach(CustomizingPart part in partsList)
        {
            GameObject selectPart = UIManager.Instance.selectPartObjPool.GetObject();
            selectPart.transform.SetParent(CachedRectTransform, false);

            SelectPartUI selectPartUI = selectPart.GetComponent<SelectPartUI>();
            selectPartUI.Setup(part);

            parts.Add(selectPartUI);
        }
    }

    public void DeleteSelectParts(){
        foreach(Transform child in this.transform){
            if(child.GetComponent<SelectPartUI>() != null) Destroy(child.gameObject);
        }
    }

    public void RandCustomize(){
        foreach(SelectPartUI part in parts){
            part.RandOptClick();
        }
    }
}
