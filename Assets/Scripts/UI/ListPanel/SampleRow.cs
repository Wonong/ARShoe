using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleRow : MonoBehaviour {

    public GameObject sampleItem;
    public Transform selfTransform;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Add items in a row
    public void AddItems(int startIdx, int endIdx){
        // Load from itemList in UIManager
        for (int i = startIdx; i <= endIdx; i++){
            Shoe item = UIManager.Instance.itemList[i];
            GameObject newButton = UIManager.Instance.itemObjPool.GetObject();
            newButton.transform.SetParent(selfTransform, false);
            SampleItem sampleButton = newButton.GetComponent<SampleItem>();

            // Initialize informations of each shoe button
            sampleButton.Setup(item);
        }
    }
}
