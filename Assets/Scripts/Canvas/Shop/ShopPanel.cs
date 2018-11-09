using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : ViewController
{


    public UniWebView webView;
    public TopMenuShop topMenuShop;
    public string url { get; set; }

    // Use this for initialization
    void Start()
    {
        webView.Load(url);
        webView.UpdateFrame();

    }

    private void OnEnable()
    {
        topMenuShop.Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
    public void RefreshWebView(string url){
        GameObject webViewGameObject = new GameObject("UniWebView");
        webView = webViewGameObject.AddComponent<UniWebView>();
        webView.SetShowToolbar(true);
        webView.ReferenceRectTransform = this.CachedRectTransform;

        webView.Load(url);
        webView.Show();
    }
    */

}
