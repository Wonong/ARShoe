using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Shoe
{
    public int id;
    new public string name;
    public string company;
    public int price;
    public bool isCustomizable, isNew, isBest;
    public string link;
    public string objPath, imgPath;      // 커스터마이징,AR에 사용되는 obj의 위치 / 리스트에 보여지는 이미지의 위치

    // not from json
    private GameObject shoeCompontnt = null;
    private Texture icon = null;
    private List<Material> materialsList = null;
    private GameObject obj = null;

    // Getters
    public Texture GetIconAsTexture()
    {
        if (icon == null) icon = Resources.Load<Texture>(imgPath);
        return icon;
    }
    public GameObject GetObjectAsGameObject()
    {
        if (obj == null) obj = Resources.Load<GameObject>(objPath);
        return obj;
    }
    public GameObject GetShoeComponent()
    {
        if (shoeCompontnt == null) shoeCompontnt = GetObjectAsGameObject().transform.GetChild(0).gameObject;
        return shoeCompontnt;
    }
    public List<Material> GetMaterialsList()
    {
        if (materialsList == null)
        {
            materialsList = new List<Material>();
            foreach (MeshRenderer i in GetObjectAsGameObject().GetComponentsInChildren<MeshRenderer>())
            {
                materialsList.AddRange(i.sharedMaterials);
            }
        }
        return materialsList;
    }

}


[System.Serializable]
public class CustomizingPart{
    public int id;
    new public string name;
    public string objName;      // 해당 part가 속하는 신발 object의 child object 중 이 part의 object 이름
    public int materialIdx;  // 해당 part의 Material index
    public int shoes_id;

    public Material GetMaterial(){ return JSONHandler.GetShoeById(shoes_id).GetMaterialsList()[materialIdx]; }

}

[System.Serializable]
public class CustomizingOption{
    public int id;
    new public string name;
    public string texturePath;      // 해당 option의 texture 위치
    public byte rgb_r, rgb_g, rgb_b; // 해당 option이 커스터마이징 메뉴에서 보여지는 색깔
    public int customizingParts_id;

    // not from json
    private Texture texture = null;

    public Color32 GetColorByRGB() { return new Color32(rgb_r, rgb_g, rgb_b, 100); }
    public Texture GetTexture() {
        if (texture == null) texture = Resources.Load<Texture>(texturePath);
        return texture;
    }

}