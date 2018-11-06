using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopMenu : MonoBehaviour {

    public Button cart;
    public Button search;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void HideAllChildButtons(){
        foreach(GameObject item in GetComponentsInChildren<GameObject>()){
            item.SetActive(false);
        }
    }

    public void ActiveAllChildButtons(){
        foreach (GameObject item in GetComponentsInChildren<GameObject>())
        {
            item.SetActive(true);
        }
    }
}
