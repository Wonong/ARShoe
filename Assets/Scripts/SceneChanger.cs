using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {
    public static void ChangeToListScene() {
        SceneManager.LoadScene("List");
        UIManager.Instance.gameObject.SetActive(true);
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
