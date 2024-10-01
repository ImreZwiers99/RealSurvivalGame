using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortcutManager : MonoBehaviour
{
    public List<GameObject> potentialItems; 
    public List<GameObject> items; 
    private List<GameObject> shortcuts = new List<GameObject>(); 

    private void Start()
    {
        UpdateShortcuts();
    }

    void Update()
    {
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

    void UseShortcut(int index)
    {
        if (index >= 0 && index < shortcuts.Count && shortcuts[index] != null)
        {
            shortcuts[index].SetActive(!shortcuts[index].activeSelf);
        }
    }
}
