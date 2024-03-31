using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float health = 0;
    public float food = 0;
    public float water = 0;

    public void AddHealth(int addedHealth)
    {
        health += addedHealth;
    }

    public void AddFood(int addedFood)
    {
        food += addedFood;
    }

    public void AddWater(int addedWater)
    {
        water += addedWater;
    }
}
