using UnityEngine;
using System.Collections;

// Spin class for Y spinning/transforming the shoe object when user click image from shoe list.
public class Spin : MonoBehaviour
{
    Vector3 originalPosition;
    Vector3 goalPosition;
    Quaternion originalRotation;
    Quaternion goalRotation;
    private float startTime;
    private float journeyLength;
    private float positionSpeed = 0.2f;
    private float rotationSpeed = 0.1f;

    private void Start()
	{
        originalPosition = transform.position + Vector3.up * 0.2f;
        originalRotation = Quaternion.Euler(0, 180f, 0);
        goalPosition = new Vector3(-913.4f, 7.1f, -9.9f);
        goalRotation = Quaternion.Euler(0, 0, 0);
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        startTime = Time.time;
        journeyLength = Vector3.Distance(originalPosition, goalPosition);
    }

    void Update()
    {
        float distPositionCovered = (Time.time - startTime) * positionSpeed;
        float distRotationCovered = (Time.time - startTime) * rotationSpeed;
        float fracPositionJourney = distPositionCovered / journeyLength;
        float fracRotationJourney = distRotationCovered / journeyLength;
        transform.position =  Vector3.Lerp(originalPosition, goalPosition, fracPositionJourney);
        transform.rotation = Quaternion.Lerp(originalRotation, goalRotation, fracRotationJourney);
       
        if (transform.rotation == goalRotation || Input.GetMouseButtonDown(0))
        {
            transform.GetComponent<Spin>().enabled = false;
            transform.position = goalPosition;
        }
    }
}