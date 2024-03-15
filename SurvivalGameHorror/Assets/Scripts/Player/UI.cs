using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Slider staminaSlider = default;

    private void OnEnable()
    {
        FirstPersonController.staminaChange += UpdateStamina;
    }

    private void OnDisable()
    {
        FirstPersonController.staminaChange -= UpdateStamina;
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
