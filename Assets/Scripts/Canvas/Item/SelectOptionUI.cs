using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectOptionUI : ViewController {

    bool isDark;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CheckOpt(){
        transform.Find("InnerCircle").gameObject.SetActive(true);
    }

    public void UncheckOpt(){
        transform.Find("InnerCircle").gameObject.SetActive(false);
    }

    public void Init(){
        // initialize color of inner circle
        Color32 rgb = GetComponent<RawImage>().color;
        Debug.Log(rgb);

        if ((rgb.r * 299 + rgb.g * 587 + rgb.b * 114) / 1000 >= 125)
        {
            transform.Find("InnerCircle").GetComponent<RawImage>().color = Color.black;
        }
        else
        {
            transform.Find("InnerCircle").GetComponent<RawImage>().color = Color.white;
        }

    }
}
