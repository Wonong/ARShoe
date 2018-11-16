using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShoeRow : MonoBehaviour {

    public Transform content;
    public ScrollRect shoeScroll;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void AddNewShoe(Shoe shoeInfo){
        GameObject newShoe = UIManager.Instance.itemObjPool.GetObject();
        newShoe.transform.SetParent(content, false);

        ShoeItem shoeItem = newShoe.GetComponent<ShoeItem>();

        shoeItem.Setup(shoeInfo);


    }
}
