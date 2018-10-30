using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelList : MonoBehaviour {

    public Camera cam;
    public Vector2 minPos, maxPos;

	// Use this for initialization
	void Start () {
        GetComponent<ScrollRect>().onValueChanged.AddListener(OnScrollChanged);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnScrollChanged(Vector2 scrollPos){
        //Debug.Log(scrollPos.ToString("F4"));
        cam.transform.position = Vector2.Lerp(minPos, maxPos, scrollPos.y);
    }
}
