/*============================================================================== 
Copyright (c) 2018 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.   
==============================================================================*/

using UnityEngine;
using UnityEngine.EventSystems;

public class TouchHandler : MonoBehaviour
{
    #region PUBLIC_MEMBERS

    Transform m_AugmentationObject;
    ProductPlacement m_ProductPlacement;

    [HideInInspector]
    public bool enableRotation = false;
    public bool enablePinchScaling = false;

    public static bool DoubleTap
    {
        get { return (Input.touchSupported) && Input.touches[0].tapCount == 2; }
    }

    public static bool IsSingleFingerStationary
    {
        get { return IsSingleFingerDown() && (Input.touches[0].phase == TouchPhase.Stationary); }
    }

    public static bool IsSingleFingerDragging
    {
        get { return IsSingleFingerDown() && (Input.touches[0].phase == TouchPhase.Moved); }
    }

    #endregion // PUBLIC MEMBERS


    #region PRIVATE_MEMBERS
    const float scaleRangeMin = 0.0025f;
    const float scaleRangeMax = 2.0f;

    Touch[] touches;
    static int lastTouchCount;
    bool isFirstFrameWithTwoTouches;
    float cachedTouchAngle;
    float cachedTouchDistance;
    float cachedAugmentationScale;
    Vector3 cachedAugmentationRotation;
    #endregion // PRIVATE_MEMBERS


    #region MONOBEHAVIOUR_METHODS

    void Start()
    {
        m_AugmentationObject = GameObject.Find("CopyShoe").transform;
        cachedAugmentationScale = m_AugmentationObject.localScale.x;
        cachedAugmentationRotation = m_AugmentationObject.localEulerAngles;
        m_ProductPlacement = FindObjectOfType<ProductPlacement>();
    }

    void Update()
    {
        touches = Input.touches;

        if (Input.touchCount == 2 && !m_ProductPlacement.IsPlaced)
        {
            float currentTouchDistance = Vector2.Distance(touches[0].position, touches[1].position);
            float diff_y = touches[0].position.y - touches[1].position.y;
            float diff_x = touches[0].position.x - touches[1].position.x;
            float currentTouchAngle = Mathf.Atan2(diff_y, diff_x) * Mathf.Rad2Deg;

            if (isFirstFrameWithTwoTouches)
            {
                cachedTouchDistance = currentTouchDistance;
                cachedTouchAngle = currentTouchAngle;
                isFirstFrameWithTwoTouches = false;
            }

            float angleDelta = currentTouchAngle - cachedTouchAngle;
            float scaleMultiplier = (currentTouchDistance / cachedTouchDistance);
            float scaleAmount = cachedAugmentationScale * scaleMultiplier;
            float scaleAmountClamped = Mathf.Clamp(scaleAmount, scaleRangeMin, scaleRangeMax);

            if (enableRotation)
            {
                m_AugmentationObject.localEulerAngles = cachedAugmentationRotation - new Vector3(0f, angleDelta * 3, 0f);
            }
            if (enableRotation && enablePinchScaling)
            {
                // Optional Pinch Scaling can be enabled via Inspector for this Script Component
                m_AugmentationObject.localScale = new Vector3(scaleAmountClamped, scaleAmountClamped, scaleAmountClamped);
            }

        }
        else if (Input.touchCount < 2)
        {
            cachedAugmentationScale = m_AugmentationObject.localScale.x;
            cachedAugmentationRotation = m_AugmentationObject.localEulerAngles;
            isFirstFrameWithTwoTouches = true;
        }
        else if (Input.touchCount == 6)
        {
            // enable runtime testing of pinch scaling
            //enablePinchScaling = true;
        }
        else if (Input.touchCount == 5)
        {
            // disable runtime testing of pinch scaling
            enablePinchScaling = false;
        }
    }

    #endregion // MONOBEHAVIOUR_METHODS


    #region PRIVATE_METHODS

    static bool IsSingleFingerDown()
    {
        if (Input.touchCount == 0 || Input.touchCount >= 2)
            lastTouchCount = Input.touchCount;

        return (
            Input.touchCount == 1 &&
            Input.touches[0].fingerId == 0 &&
            lastTouchCount == 0);
    }

    #endregion // PRIVATE_METHODS

}