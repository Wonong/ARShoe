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

    //public ItemPanel itemPanel;
    public CustomizePanel customizePanel;
    public PanelList listPanel;
    public ShopPanel shopPanel;



    // Object pools for auto creating objects
    public SimpleObjectPool itemObjPool;
    public SimpleObjectPool rowObjPool;
    public SimpleObjectPool selectPartObjPool;
    public SimpleObjectPool selectOptionObjPool;

    // Shoes item list
    public List<Shoe> itemList;

    // Data 초기화
    private void SetShoesData()
    {
        itemList = JSONHandler.GetAllShoesList();
    }


    private void Awake()
    {
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
    }
}
