using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutManager : MonoBehaviour
{
    public List<GameObject> potentialItems; // List of items to store in the inventory
    public List<GameObject> items; // List of items to store in the inventory
    private List<GameObject> shortcuts = new List<GameObject>(); // List to store shortcuts

    // Start is called before the first frame update
    private void Start()
    {
        UpdateShortcuts();
    }

    // Update is called once per frame
    void Update()
    {
        // Checking for shortcut inputs
        for (int i = 0; i < shortcuts.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                UseShortcut(i);
            }
        }
    }
    void UpdateShortcuts()
    {
        // Assigning items to shortcuts
        for (int i = 0; i < Mathf.Min(items.Count, 123); i++)
        {
            shortcuts.Add(items[i]);
        }
    }
    public void AddItem(string compareItem)
    {
       
        for (int i = 0; i < potentialItems.Count; i++)
        {
            if (potentialItems[i].name.Contains(compareItem))
            {
                items.Add(potentialItems[i]);
                potentialItems.Remove(potentialItems[i]);
                UpdateShortcuts();
            }
        }
    }

    // Function to use the shortcut
    void UseShortcut(int index)
    {
        // Checking if the shortcut is valid and the item is available
        if (index >= 0 && index < shortcuts.Count && shortcuts[index] != null)
        {
            // Toggle the active state of the item
            shortcuts[index].SetActive(!shortcuts[index].activeSelf);
        }
    }
}
