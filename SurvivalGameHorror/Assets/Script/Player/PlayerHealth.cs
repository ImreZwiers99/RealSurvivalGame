using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static float health = 90;
    public static float food = 90;
    public static float water = 90;

    public float editorFood;
    public float editorWater;
    public float editorHealth;

    public float foodDrain = 0.1f;
    public float waterDrain = 0.1f;
    public float healthDrain = 1f;

    public GameObject Camera;
    public GameObject viewCam;
    public GameObject UI;
    public GameObject Items;
    //public Animator animator;

    public void Start()
    {
        viewCam.GetComponent<Animator>().enabled = false;
    }

    private void Update()
    {

        if (health >= 100)
            health = 100;

        if (food >= 100)
            food = 100;

        if (water >= 100)
            water = 100;

        if (editorHealth >= 100)
            editorHealth = 100;

        if (editorFood >= 100)
            editorFood = 100;

        if (editorWater >= 100)
            editorWater = 100;

        if (health <= 0 && editorHealth <=  0)
        {
            gameObject.GetComponent<FirstPersonController>().enabled = false;
            Camera.GetComponent<CameraLook>().enabled = false;
            viewCam.GetComponent<Animator>().enabled = true;
            UI.SetActive(false);
            Items.SetActive(false);
        }
        else
        {
            viewCam.GetComponent<Animator>().enabled = false;
        }
            
        Debug.Log(health);

        health = editorHealth;
        food = editorFood -= foodDrain * Time.deltaTime;
        water = editorWater -= waterDrain * Time.deltaTime;
        if (water > 0)
            health = health;
        else if (water <=0)
            health = editorHealth -= healthDrain * Time.deltaTime;

        if (food > 0)
            health = health;
        else if (food <= 0)
            health = editorHealth -= healthDrain * Time.deltaTime;
    }

    public void TakeDamage(int damageAmount)
    {
        editorHealth -= damageAmount;
    }

    public void AddHealth(int addedHealth)
    {
        if (health < 100)
            health += addedHealth;
    }

    public void AddFood(int addedFood)
    {
        if (food < 100)
            editorFood += addedFood;
    }

    public void AddWater(int addedWater)
    {
        if (water < 100)
            editorWater += addedWater;
    }
}
