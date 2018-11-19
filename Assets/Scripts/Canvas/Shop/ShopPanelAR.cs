using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelAR : ViewController
{


    public UniWebView webView;
    public string url { get; set; }
    //public Button backBut;
    public Button sceneBackBut;
    //public Text title;

    // Use this for initialization
    void Start()
    {
        Debug.Log("shop panel ar start");
        //backBut.onClick.AddListener(BackButtonClicked);
        webView.Load(url);
        webView.UpdateFrame();

    }

    private void OnEnable()
    {
        //title.text = CurrentCustomShoe.currentShoeInfo.name;
        //sceneBackBut.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        //sceneBackBut.gameObject.SetActive(true);
    }
    

    void BackButtonClicked(){
        Debug.Log("back but click");
        this.gameObject.SetActive(false);
        gameObject.SetActive(false);
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
