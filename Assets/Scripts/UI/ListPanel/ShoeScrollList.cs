//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoeScrollList : MonoBehaviour {
    
    public List<Shoe> itemList;
    public Transform contentPanel;


    // Use this for initialization
    void Start () {
        RefreshDisplay();
	}

    // Update is called once per frame
    void Update() {

    }

    public void RefreshDisplay() {
        RemoveButtons();
        AddRows();
    }
    
    private void RemoveButtons() {
        while (contentPanel.childCount > 0) {
            GameObject toRemove = transform.GetChild(0).gameObject;
            UIManager.Instance.itemObjPool.ReturnObject(toRemove);
        }
    }


    // row 자동 생성
    private void AddRows() {
        itemList = UIManager.Instance.itemList;
        for (int i = 0; i < itemList.Count; i+=3) {
            GameObject newRow = UIManager.Instance.rowObjPool.GetObject();
            newRow.transform.SetParent(contentPanel, false);

            SampleRow sampleRow = newRow.GetComponent<SampleRow>();

            int endIdx = (itemList.Count - 1) < (i + 2) ? (itemList.Count - 1) : (i + 2);
            sampleRow.AddItems(i, endIdx);
        }
    }
	
	
}
