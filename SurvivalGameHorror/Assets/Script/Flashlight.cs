using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public Light flashlight; // Reference to the flashlight light component
    public AudioSource flashlightSource;
    public AudioClip on;
    public AudioClip off;
    private bool isOn = false; // Flag to track the state of the flashlight

    // Start is called before the first frame update
    void Start()
    {
        flashlight.enabled = false; // Ensure the flashlight is initially turned off
    }

    // Update is called once per frame
    void Update()
    {
        // Check for mouse button 1 (left mouse button) click
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(isOn)
            {
                flashlightSource.clip = off;
                flashlightSource.Play();
            }
            else
            {
                flashlightSource.clip = on;
                flashlightSource.Play();
            }
            // Toggle the flashlight state
            isOn = !isOn;

            // Turn the flashlight on or off based on the state
            flashlight.enabled = isOn;
        }
    }
}
