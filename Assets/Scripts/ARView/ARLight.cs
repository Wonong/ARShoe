using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class ARLight : MonoBehaviour {
    public Light directionalLight;
    public float minusLightIntensity = 0.4f;

	// Update is called once per frame
	void Update () {
        // Normalize pixel intensity by middle gray in gamma space.
        const float middleGray = 0.566f;
        float normalizedIntensity = Frame.LightEstimate.PixelIntensity / middleGray;

        // Apply color correction along with normalized pixel intensity in gamma space.
        directionalLight.color = Frame.LightEstimate.ColorCorrection * normalizedIntensity;
        directionalLight.intensity = normalizedIntensity - minusLightIntensity;
    }
}
