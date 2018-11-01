using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class ShoeController : MonoBehaviour {
    public bool IsPlaced
    {
        get
        {
            return isPlaced;
        }
        set
        {
            isPlaced = value;
        }
    }

    public bool DoesShoeActive
    {
        get
        {
            return shoe != null && shoe.activeSelf;
        }
    }

    public GameObject shoe;

    // For checking shoe object is placed.
    bool isPlaced = false;


    // Size values.
    float shoeHeight = 0.15f;
    float shoeScale = 1.4f;

    void Awake()
    {
        CurrentCustomShoe.shoe.GetComponent<Swiper>().enabled = false;
        shoe = Instantiate(CurrentCustomShoe.shoe);
        shoe.GetComponent<Spin>().enabled = false;
        shoe.name = "CopyShoe";
        shoe.transform.localScale = new Vector3(shoeScale, shoeScale, shoeScale);
        shoe.GetComponentsInChildren<Transform>()[1].localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void Start()
    {
        MoveShoe(); // Shoe object is movable at very first.
        shoe.SetActive(false);
    }

    #region public methods
    /// <summary>
    /// Place shoe to the plane.
    /// </summary>
    public void PlaceShoe()
    {
        shoe.transform.position -= Vector3.up * shoeHeight;
        GameObject.Find("PuttingSound").GetComponent<AudioSource>().Play();
        isPlaced = true;
    }

    /// <summary>
    /// Move shoe on the plane.
    /// </summary>
    public void MoveShoe()
    {
        shoe.transform.position += Vector3.up * shoeHeight;
        isPlaced = false;
    }

    /// <summary>
    /// Reset object, anchor, and make shoe object movable.
    /// </summary>
    public void ResetAR()
    {
        Destroy(FindObjectOfType<Anchor>());
        shoe.SetActive(false);
        isPlaced = false;
        MoveShoe();
    }
    #endregion
}
