using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string ItemName;

    public bool playerInRange;

    public string GetItemName()
    {
        return ItemName;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerInRange)
        {
            if (!InventorySystem.Instance.CheckIfFull())
            {
                Destroy(gameObject);
                InventorySystem.Instance.AddToInventory(ItemName);
            }
            else
            {
                Debug.Log("inv is full");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}