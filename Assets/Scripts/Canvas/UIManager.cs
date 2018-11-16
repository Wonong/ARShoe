using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
   
    public static UIManager Instance { get; private set; }

    // Navigation View Stack
    public Stack<ViewController> viewStack = new Stack<ViewController>();
    public ViewController currentView = null;

    // UI components
    public Canvas canvas;
    public NavigationViewController navigationView;
    public TopMenu topMenu;
    public Categories categories;

    // Panels
    //public ItemPanel itemPanel;
    public CustomizePanel customizePanel;
    public PanelList listPanel;
    public ShopPanel shopPanel;
    public PanelList2 listPanel2;



    // Object pools for auto creating objects
    public SimpleObjectPool itemObjPool;
    public SimpleObjectPool listShoeRowObjPool;
    public SimpleObjectPool listShoeItemObjPool;
    public SimpleObjectPool selectPartObjPool;
    public SimpleObjectPool selectOptionObjPool;

    // Shoes item list
    public List<Shoe> itemList;

    // Data 초기화
    private void SetShoesData()
    {
        itemList = JSONHandler.GetAllShoesList();
    }

    public void SetShopUrl(string link){
        this.shopPanel.url = link;
    }


    private void Awake()
    {
        Debug.Log("UiManager Awake");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        JSONHandler.InitDB();
        SetShoesData();

        // navigation view의 첫번째 뷰를 리스트로 설정
        if (navigationView != null && viewStack.Count == 0)
        {
            navigationView.Push(listPanel);
        }
    }
}
