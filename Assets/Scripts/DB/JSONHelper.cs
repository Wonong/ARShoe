using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(FixJSON(json)); 
        return new List<T>(wrapper.Items);
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    // json format handling
    private static string FixJSON(string rawJSON)
    {
        rawJSON = "{\"Items\":" + rawJSON + "}";
        return rawJSON;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
