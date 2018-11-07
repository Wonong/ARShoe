using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopMenu : MonoBehaviour {

    public Button myMenu, categoriesBut, searchBut;

	// Use this for initialization
	void Start () {
        categoriesBut.onClick.AddListener(CategoriesButtonOnClick);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CategoriesButtonOnClick(){
        Debug.Log("categori button clicked");
        UIManager.Instance.categories.gameObject.SetActive(true);
        this.HideAllChildButtons();
    }

    public void HideAllChildButtons(){
        foreach(Transform child in transform){
            child.gameObject.SetActive(false);
        }
    }

    public void ActiveAllChildButtons(){
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
