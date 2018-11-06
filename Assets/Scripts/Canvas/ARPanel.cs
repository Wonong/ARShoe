using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARPanel : ViewController {

	// Use this for initialization
	void Start () {
        // 기존 view들을 투명화
        Color32 canvasBg = UIManager.Instance.canvas.GetComponent<Image>().color;
        canvasBg.a = 0;
        UIManager.Instance.canvas.GetComponent<Image>().color = canvasBg;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
