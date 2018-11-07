using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : ViewController
{


    public UniWebView webView;
    public string url { get; set; }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void RefreshWebView(string url){
        GameObject webViewGameObject = new GameObject("UniWebView");
        webView = webViewGameObject.AddComponent<UniWebView>();
        webView.transform.SetParent(CachedRectTransform, false);
        webView.ReferenceRectTransform = this.CachedRectTransform;

        webView.Frame = new Rect(0,0, CachedRectTransform.rect.width, CachedRectTransform.rect.height);
        webView.Load(url);
        webView.Show();
    }

}
