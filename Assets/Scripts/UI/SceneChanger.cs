using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {
    public static void ChangeToShoeListScene() {
        SceneManager.LoadScene("ShoeList");
        UIManager.Instance.gameObject.SetActive(true);
        CurrentCustomShoe.shoe.GetComponent<Swiper>().enabled = true;
        CurrentCustomShoe.shoe.GetComponent<Spin>().enabled = true;
        //UIManager.Instance.navigationView.Pop();
    }

    public static void ChangeToWatchingShoes()
    {
        SceneManager.LoadScene("WatchingShoes");
        UIManager.Instance.gameObject.SetActive(false);
    }

    public static void ChangeToAttachShoes()
    {
        SceneManager.LoadScene("AttachingShoes");
        UIManager.Instance.gameObject.SetActive(false);
    }
}
