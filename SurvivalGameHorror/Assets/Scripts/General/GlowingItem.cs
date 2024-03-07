using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowingItem : MonoBehaviour
{
    public Material[] glowMaterials; // Reference to the materials using your shader
    public float minIntensity = 3f; // Minimum intensity value
    public float maxIntensity = 4f; // Maximum intensity value
    public float lerpDuration = 1f; // Duration for lerping between intensities

    private float currentTime = 0f; // Current time since lerping started
    private bool increasing = true; // Flag to determine if intensity is increasing or decreasing

    // Update is called once per frame
    void Update()
    {
        // Increment or decrement time based on whether intensity is increasing or decreasing
        if (increasing)
            currentTime += Time.deltaTime;
        else
            currentTime -= Time.deltaTime;

        // Check if time exceeds duration, then toggle increasing flag and reset time
        if (currentTime >= lerpDuration)
        {
            currentTime = lerpDuration;
            increasing = false;
        }
        else if (currentTime <= 0)
        {
            currentTime = 0;
            increasing = true;
        }

        // Calculate lerp value between min and max intensity based on current time
        float t = currentTime / lerpDuration;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);

        // Set the intensity value in the shader for each material
        for (int i = 0; i < glowMaterials.Length; i++)
        {
            glowMaterials[i].SetFloat("_GowIntecity", intensity);
        }
    }
}
