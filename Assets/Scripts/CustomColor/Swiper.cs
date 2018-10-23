using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Swiper class for rotating the shoe object by Mouse input/Touch input.
public class Swiper : MonoBehaviour {
    GameObject shoe;
    Vector3 mouseInitPosition;
    Vector3 mousePreviousPosition;
    Vector3 mouseEndPosiotion;

    private float rotY;
    private Vector3 origRot;
    float dir = -1;
    float deltaY;
    float rotSpeed;
    float maxSpeed;
    float minSpeed;
    bool isMouseUp;

	void Start ()
    {
        shoe = transform.gameObject;
        origRot = shoe.transform.eulerAngles;
        rotY = origRot.y;
        isMouseUp = false;
        minSpeed = 0f;
        maxSpeed = 15f;
	}

	private void Update()
	{
        if (Input.mousePosition.y > 2.5 * Screen.height / 9 && Input.mousePosition.y < 8 * Screen.height / 9) // Prevent to swipe the object using y position.
        {
            if (Input.GetMouseButtonDown(0) == true)
            {
                mouseInitPosition = Input.mousePosition;
                mousePreviousPosition = mouseInitPosition;
                rotSpeed = 10f;
                isMouseUp = false;
            }
            else if (Input.GetMouseButton(0) == true)
            {
                if (Mathf.Abs(mousePreviousPosition.x - Input.mousePosition.x) > Screen.width / 100)
                {
                    deltaY = mousePreviousPosition.x - Input.mousePosition.x;
                    rotY -= deltaY * Time.deltaTime * dir * rotSpeed;
                    shoe.transform.eulerAngles = new Vector3(0f, rotY, 0f);
                    if (rotSpeed < maxSpeed)
                    {
                        rotSpeed += 0.5f;
                    }
                    mousePreviousPosition = Input.mousePosition;
                }
                else if (rotSpeed > minSpeed && mouseInitPosition.x - Input.mousePosition.x != 0)
                {
                    rotY -= deltaY * Time.deltaTime * rotSpeed * dir;
                    shoe.transform.eulerAngles = new Vector3(0f, rotY, 0f);
                    rotSpeed -= 1.5f;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isMouseUp = true;
                mouseEndPosiotion = Input.mousePosition;
            }
            else if (isMouseUp && rotSpeed > minSpeed && mouseInitPosition.x - mouseEndPosiotion.x != 0)
            {
                rotY -= deltaY * Time.deltaTime * rotSpeed * dir;
                shoe.transform.eulerAngles = new Vector3(0f, rotY, 0f);
                rotSpeed -= 1.5f;
            }
        }
	}
}
