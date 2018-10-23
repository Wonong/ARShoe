using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockButton : MonoBehaviour {

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClickFunction);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnClickFunction()
    {
        Debug.Log("LockOnClick");
        DetectorController.isDebug = !DetectorController.isDebug;
    }
}
