using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// RectTransform을 미리 캐싱해놓고 가져오기 위한 class

[RequireComponent(typeof(RectTransform))]
public class ViewController : MonoBehaviour {

    private void Start()
    {

    }

    private RectTransform cachedRectTransform;
    public RectTransform CachedRectTransform {
        get {
            if(cachedRectTransform == null) {
                cachedRectTransform = GetComponent<RectTransform>();
            }
            return cachedRectTransform;
        }
    }

    public virtual string Title { get { return ""; } set {}}

}
