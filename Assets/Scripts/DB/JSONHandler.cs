using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class JSONHandler : MonoBehaviour {


    static string shoesPath = "DB/Shoes2";
    static string partsPath = "DB/CustomizingParts";
    static string optionsPath = "DB/CustomizingOptions";

    static List<Shoe> shoesList;
    static List<CustomizingPart> partsList;
    static List<CustomizingOption> optionsList;

    public static List<Shoe> GetAllShoesList(){ return shoesList; }
    public static Shoe GetShoeById(int id) { return shoesList.Find(shoe => shoe.id == id);}
    public static List<CustomizingPart> GetPartsListByShoeId(int shoeId){ return partsList.FindAll(part => part.shoes_id == shoeId); }
    public static List<CustomizingOption> GetOptionsListByPartId(int partId) { return optionsList.FindAll(part => part.customizingParts_id == partId); }

    public static void InitDB()
    {

        string shoesRawText = Resources.Load<TextAsset>(shoesPath).text;
        string partsRawText = Resources.Load<TextAsset>(partsPath).text;
        string optionsRawText = Resources.Load<TextAsset>(optionsPath).text;

        shoesList = JsonHelper.FromJson<Shoe>(shoesRawText);
        partsList = JsonHelper.FromJson<CustomizingPart>(partsRawText);
        optionsList = JsonHelper.FromJson<CustomizingOption>(optionsRawText);

        //LogContent();

        //Debug.Log(shoesList[0].imgPath);

    }



}