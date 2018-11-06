using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeMenu : ViewController {

    //public SelectPartUI selectPartUI;

	// Use this for initialization
	void Start () {
		
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
        }
    }

    public void DeleteSelectParts(){
        foreach(Transform child in this.transform){
            Destroy(child.gameObject);
        }
    }
}
