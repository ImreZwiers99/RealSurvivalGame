using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Slider staminaSlider = default;
    [SerializeField] private Slider foodSlider = default;
    [SerializeField] private Slider waterSlider = default;

    private void OnEnable()
    {
        FirstPersonController.staminaChange += UpdateStamina;
    }

    private void OnDisable()
    {
        FirstPersonController.staminaChange -= UpdateStamina;
    }

    private void Update()
    {
        foodSlider.value = PlayerHealth.food;
        waterSlider.value = PlayerHealth.water;
    }

    private void Start()
    {
        UpdateStamina(100);
    }

    private void UpdateStamina(float currentStamina)
    {
        staminaSlider.value = currentStamina;
    }
}
