using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CustomScroll : ScrollRect
{

    private bool routeToParent = false;
    public ScrollRect parentTransform;

    // Use this for initialization
    void Start () {
        parentTransform = this.GetComponent<Temp>().parentTransform;	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (routeToParent)
            parentTransform.OnDrag(eventData);
        else
            base.OnDrag(eventData);
    }

    public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {

        if (Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
        {
            routeToParent = true;
        }
        routeToParent = false;

    }
}


