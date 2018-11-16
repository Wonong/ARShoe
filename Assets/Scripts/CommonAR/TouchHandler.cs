using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.EventSystems;

static class TouchHandler
{
    # region public members
    public static bool isFirstFrameWithTwoTouches;
    /// <summary>
    ///   The delta of the angle between two touch points
    /// </summary>
    public static float turnAngleDelta;
    /// <summary>
    ///   The angle between two touch points
    /// </summary>
    public static float turnAngle;
    #endregion

    #region private members
    static float speed = 5f;
    #endregion

    public static void InteractSingleFinger(GameObject shoes, TrackableHit hit, Touch[] touches)
    {
        if (!shoes.activeSelf) // Initialize shoe.
        {
            shoes.SetActive(true);
            shoes.transform.position = hit.Pose.position;
            shoes.transform.position += Vector3.up * 0.2f;
            var anchor = hit.Trackable.CreateAnchor(hit.Pose);
            shoes.transform.parent = anchor.transform;
            isFirstFrameWithTwoTouches = true;
        }
        else if (isFirstFrameWithTwoTouches && (touches[0].phase == TouchPhase.Moved)) // Drag and make shoe move.
        {
            shoes.transform.position = hit.Pose.position;
            shoes.transform.position += Vector3.up * 0.2f;
            isFirstFrameWithTwoTouches = true;
        }
        else if (!isFirstFrameWithTwoTouches && touches[0].phase == TouchPhase.Began) // Set the bool value true for moving shoe.
        {
            isFirstFrameWithTwoTouches = true;
        }
    }

    public static void InteractDoubleFinger(GameObject shoes, Touch[] touches)
    {
        Vector3 rotationDeg = Vector3.zero;
        rotationDeg.y = -turnAngleDelta;
        shoes.transform.rotation *= Quaternion.Euler(rotationDeg);

        // Initialize angle, angleDelta, and touch boolean.
        if (isFirstFrameWithTwoTouches) 
        {
            turnAngle = turnAngleDelta = 0;
            isFirstFrameWithTwoTouches = false;
        }
        // If at least one of them moved, than user can rotate the shoe.
        if (touches[0].phase == TouchPhase.Moved || touches[1].phase == TouchPhase.Moved)
        {
            // Check the delta angle between touches.
            turnAngle = Angle(touches[0].position, touches[1].position);
            float prevTurn = Angle(touches[0].position - touches[0].deltaPosition,
                                   touches[1].position - touches[1].deltaPosition);
            turnAngleDelta = Mathf.DeltaAngle(prevTurn, turnAngle);

            // If it's greater than zero, it's a turn!
            if (Mathf.Abs(turnAngleDelta) > 0)
            {
                turnAngleDelta *= speed;
            }
            else
            {
                turnAngle = turnAngleDelta = 0;
            }
        }
    }

    static float Angle(Vector2 pos1, Vector2 pos2)
    {
        Vector2 from = pos2 - pos1;
        Vector2 to = new Vector2(1, 0);

        float result = Vector2.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        if (cross.z > 0)
        {
            result = 360f - result;
        }

        return result;
    }
}