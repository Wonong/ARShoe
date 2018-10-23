using UnityEngine;
using System.Collections;

// Item Panel 상태에서 CustomShoe 오브젝트를 유일하게 유지하기 위한 클래스
public class CurrentCustomShoe : MonoBehaviour
{
    public static GameObject shoe;
    public static GameObject shoeComponentsGameObject;

    public static void SetCurrentCustomShoe(int ShoeId)
	{
        if (shoe != null)
        {
            Destroy(shoe);
        }
        shoeComponentsGameObject = JSONHandler.GetShoeById(ShoeId).GetShoeComponent();
        shoe = Instantiate(JSONHandler.GetShoeById(ShoeId).GetObjectAsGameObject());
        InitializeShoe();
        DontDestroyOnLoad(shoe);
	}

    static void InitializeShoe() {
        shoe.GetComponent<Swiper>().enabled = true;
        foreach (MeshRenderer mesh in shoe.GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = true;
        }
        shoeComponentsGameObject.transform.localRotation = Quaternion.Euler(0, 0, 45);
        shoeComponentsGameObject.transform.localPosition = new Vector3(0, 0, 0);
    }
}
