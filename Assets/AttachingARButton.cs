using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttachingARButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Button>().onClick.AddListener(SceneChanger.ChangeToAttachShoes);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
