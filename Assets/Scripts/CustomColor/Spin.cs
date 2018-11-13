using UnityEngine;
using System.Collections;

// Spin class for Y spinning/transforming the shoe object when user click image from shoe list.
public class Spin : MonoBehaviour
{
    float speed = 80f;
    Vector3 originalPosition;
    Vector3 minusYVector = new Vector3(0f, 0.007f, 0f);

	private void Start()
	{
        originalPosition = transform.position;
        transform.position += new Vector3(0f, 0.2f, 0f); 
        transform.rotation = Quaternion.EulerRotation(0, 180f, 0);
	}

	void Update()
    {
        if (transform.rotation.y < 180f && transform.rotation.y>=90f)
        {
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
            speed += 10;
        } 
        else if(transform.rotation.y<90f&&transform.rotation.y>0f){
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
            speed *= 0.99f;
        }
        else {
            transform.rotation = Quaternion.EulerRotation(0, 0, 0);
        }

        if(transform.position.y>originalPosition.y) {
            transform.position -= minusYVector;
        } else {
            transform.position = originalPosition;
        }

        if(transform.rotation.y==0f&&transform.position==originalPosition) {
            transform.GetComponent<Spin>().enabled = false;
        }
    }
}