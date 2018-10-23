using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Spin class for Y spinning/transforming the shoe object when user click image from shoe list.
public class FallingLeave : MonoBehaviour
{
    public GameObject grassObject;
    List<Transform> leave = new List<Transform>();
    float originalPositionY;
    float speed;
    private void Start()
    {
        speed = Random.value;
        originalPositionY = transform.position.y;
        foreach(Transform leaf in gameObject.GetComponentsInChildren<Transform>())
        {
            leave.Add(leaf);
        }
    }

    void Update()
    {
        foreach(Transform leaf in leave)
        {
            if (leaf.position.y > grassObject.transform.position.y)
            {
                leaf.position -= Vector3.up * speed * 0.005f * Random.Range(1f, 10f);
                leaf.eulerAngles -= Vector3.back * Time.deltaTime * Random.Range(1f, 10f);
            }
            else
            {
                leaf.position = new Vector3(leaf.position.x, originalPositionY, leaf.position.z);
            }
        }
    }
}