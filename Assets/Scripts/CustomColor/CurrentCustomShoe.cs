using UnityEngine;
using System.Collections;

// Item Panel 상태에서 CustomShoe 오브젝트를 유일하게 유지하기 위한 클래스
public class CurrentCustomShoe : MonoBehaviour
{
    public static GameObject shoes, shoeLeft, shoeRight, shoeParent;
    public static int currentShoeId;
    public static Shoe currentShoeInfo;

    public static void SetCurrentCustomShoe(int shoeId)
	{
        if (shoes != null)
        {
            Destroy(shoes);
        }

        // 현재 인스턴스화된 신발 정보 저장
        currentShoeId = shoeId;
        currentShoeInfo = JSONHandler.GetShoeById(shoeId);

        shoes = Instantiate(currentShoeInfo.GetObjectAsGameObject());
        shoeParent = GameObject.Find("ShoeParent");
        shoes.transform.SetParent(shoeParent.transform);
        shoeLeft = GameObject.Find("Shoe_Left");
        shoeRight = GameObject.Find("Shoe_Right");
        InitializeShoe();
        DontDestroyOnLoad(shoeParent);
	}

    static void InitializeShoe() {
        shoes.GetComponent<Spin>().enabled = true;
        shoes.GetComponent<Swiper>().enabled = true;
        shoes.transform.position = new Vector3(-913.4f, 7.07f, -9.9f);
        shoeLeft.transform.localPosition = new Vector3(0, 0, 0);
        shoeRight.transform.localPosition = new Vector3(0, 0, 0);
        shoeRight.SetActive(false);
        foreach (MeshRenderer mesh in shoes.GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = true;
        }
    }

    public static Vector3 GetShoesPosition(){
        return shoes.transform.position;
    }

    public static void SetShoesPosition(Vector3 newPos){
        shoes.transform.position = newPos;
    }
}
