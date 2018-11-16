using GoogleARCore;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            return shoes != null && shoes.activeSelf;
        }
    }

    public GameObject shoes;
    public GameObject shoeLeft;
    public GameObject shoeRight;
    // For checking shoe object is placed.
    bool isPlaced = false;
    bool isShoeLeft = false;

    // Size values.
    public float shoeHeight = 0.15f;
    public float shoeScale = 1.8f;

    private void Awake()
    {
        CreateShoe();
    }

    private void Start()
    {
        InitShoe();
    }

    public void CreateShoe()
    {
        shoes = CurrentCustomShoe.shoes;
        shoes.GetComponent<Spin>().enabled = false;
    }

    public void InitShoe()
    {
        shoes.GetComponent<Swiper>().enabled = false;
        shoes.transform.localScale = new Vector3(shoeScale, shoeScale, shoeScale);
        int componentsLength = shoes.GetComponents<Transform>().Length;
        shoeLeft = shoes.transform.GetChild(0).gameObject;
        shoeRight = shoes.transform.GetChild(1).gameObject;
        if (SceneManager.GetActiveScene().name.Equals("WatchingShoes"))
        {
            shoeLeft.transform.localRotation = Quaternion.Euler(0, 0, 0);
            shoeRight.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            //shoeLeft.transform.localRotation = Quaternion.Euler(180, -90, 90);
            shoes.transform.position = new Vector3(0, 0, 0);
            shoes.transform.rotation = Quaternion.Euler(0, 0, 0);
            shoeLeft.transform.localRotation = Quaternion.Euler(0, 90, 0);
            shoeRight.transform.localRotation = Quaternion.Euler(0, 90, 0);
            Instantiate(Resources.Load<GameObject>("Prefabs/AttachingAR/TransparentPrefab")).transform.SetParent(shoes.transform);
        }
        shoes.SetActive(false);
        MoveShoe(); // Shoe object is movable at very first.
        ChangeLeftRight(GroundPlaneUI.leftRightName);
    }

    public void ChangeLocalAngle()
    {

    }

    public void ChangeScale(float scale)
    {
        shoeScale = scale;
        shoes.transform.localScale = new Vector3(shoeScale, shoeScale, shoeScale);
    }

    public void ResetPosition(Vector3 initPosition)
    {
        if (initPosition == null)
        {
            initPosition = new Vector3(0, 0, 0.55f);
        }

        Quaternion initRotation = Quaternion.Euler(0, 0, 0);
        shoes.transform.rotation = initRotation;

        shoes.transform.parent = GameObject.Find("First Person Camera").transform;
        shoes.transform.localPosition = initPosition;

        shoes.transform.parent = null;
    }

    public void ActivateShoe()
    {
        shoes.SetActive(true);
    }

    public void DeactivateShoe()
    {
        shoes.SetActive(false);
    }

    #region public methods
    /// <summary>
    /// Place shoe to the plane.
    /// </summary>
    public void PlaceShoe()
    {
        shoes.transform.position -= Vector3.up * shoeHeight;
        GameObject.Find("PuttingSound").GetComponent<AudioSource>().Play();
        isPlaced = true;
    }

    /// <summary>
    /// Move shoe on the plane.
    /// </summary>
    public void MoveShoe()
    {
        shoes.transform.position += Vector3.up * shoeHeight;
        isPlaced = false;
    }

    /// <summary>
    /// Reset object, anchor, and make shoe object movable.
    /// </summary>
    public void ResetAR()
    {
        Destroy(FindObjectOfType<Anchor>());
        shoes.SetActive(false);
        isPlaced = false;
        MoveShoe();
    }

    public void ChangeLeftRight(string name)
    {
        if(name == null || name.Equals("left"))
        {
            shoeLeft.SetActive(true);
            shoeRight.SetActive(false);
            isShoeLeft = true;
        }
        else if (name.Equals("right"))
        {
            shoeRight.SetActive(true);
            shoeLeft.SetActive(false);
            isShoeLeft = false;
        }
    }
    #endregion
}
