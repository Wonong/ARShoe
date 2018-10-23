using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListPanel : ViewController {

    private NavigationViewController navigationView;

    public override string Title { get { return "ARShoe"; } }

    // Use this for initialization
    void Start () {
    
        navigationView = UIManager.Instance.navigationView;

        // navigation view의 첫번째 뷰로 설정
        if (navigationView != null && UIManager.Instance.viewStack.Count == 0) {
            navigationView.Push(this);
        }	
	}
	
	// Update is called once per frame
	void Update () {
    }
}
