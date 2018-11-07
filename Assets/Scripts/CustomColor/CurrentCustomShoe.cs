using UnityEngine;
using System.Collections;

// Item Panel 상태에서 CustomShoe 오브젝트를 유일하게 유지하기 위한 클래스
public class CurrentCustomShoe : MonoBehaviour
{
    public static GameObject shoes;
    public static GameObject shoeLeft;
    public static GameObject shoeRight;
    public static int currentShoeId;

    public static void SetCurrentCustomShoe(int shoeId)
	{
        if (shoes != null)
        {
            Destroy(shoes);
        }
        currentShoeId = shoeId;

        shoes = Instantiate(JSONHandler.GetShoeById(shoeId).GetObjectAsGameObject());
        shoeLeft = GameObject.Find("Shoe_Left");
        shoeRight = GameObject.Find("Shoe_Right");
        InitializeShoe();
        DontDestroyOnLoad(shoes);
	}

    static void InitializeShoe() {
        shoes.GetComponent<Spin>().enabled = true;
        shoes.GetComponent<Swiper>().enabled = true;
        shoeLeft.transform.localRotation = Quaternion.Euler(0, 0, 45);
        shoeLeft.transform.localPosition = new Vector3(0, 0, 0);
        //shoeRight.transform.localRotation = Quaternion.Euler(0, 0, 45);
        shoeRight.transform.localPosition = new Vector3(0, 0, 0);
        shoeRight.SetActive(false);
        foreach (MeshRenderer mesh in shoes.GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = true;
        }
    }
}
