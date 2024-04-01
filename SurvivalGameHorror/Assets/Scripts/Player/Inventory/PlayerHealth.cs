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

    public float foodDrain = 0.1f;
    public float waterDrain = 0.1f;

    private void Update()
    {
        if (health >= 100)
            health = 100;

        if (food >= 100)
            food = 100;

        if (water >= 100)
            water = 100;

        Debug.Log(health);
        //Debug.Log(water);
        //Debug.Log(food);

        food = editorFood -= foodDrain * Time.deltaTime;
        water = editorWater -= waterDrain * Time.deltaTime;
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
