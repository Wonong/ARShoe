using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomWebVIew : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //this.GetComponent<UniWebView>().UpdateFrame();
    }
	
	// Update is called once per frame
	void Update () {
        if(this.GetComponent<UniWebView>().Url != null) this.GetComponent<UniWebView>().UpdateFrame();
	}
}
