using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour {
    public static void ChangeToListScene() {
        SceneManager.LoadScene("List");
        UIManager.Instance.gameObject.SetActive(true);
        CurrentCustomShoe.shoes.GetComponent<Swiper>().enabled = true;
        CurrentCustomShoe.shoes.GetComponent<Spin>().enabled = true;

        // 해당 신발의 커스터마이즈 패널(또는 일반정보 패널) 보여줌
        //UIManager.Instance.navigationView.Push(UIManager.Instance.listPanel);
        UIManager.Instance.navigationView.Push(UIManager.Instance.customizePanel);
        UIManager.Instance.customizePanel.Init(CurrentCustomShoe.currentShoeInfo);
    }

    public static void ChangeToWatchingShoes()
    {
        SceneManager.LoadScene("WatchingShoes");
        UIManager.Instance.gameObject.SetActive(false);


        //customize panel 있으면 해당 씬으로 move
    }

    public static void ChangeToAttachShoes()
    {
        SceneManager.LoadScene("AttachingShoes");
        UIManager.Instance.gameObject.SetActive(false);
    }
}
