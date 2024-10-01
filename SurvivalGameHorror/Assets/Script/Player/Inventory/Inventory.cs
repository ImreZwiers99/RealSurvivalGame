using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

public class Inventory : MonoBehaviour
{
    [Header("UI")]
    public GameObject inventory;
    private List<Slot> allInventorySlots = new List<Slot>();
    public List<Slot> inventorySlots = new List<Slot>();
    public List<Slot> hotbarSlots = new List<Slot>();
    public Image crosshair;
    public TMP_Text itemHoverText;
    public static bool isOpen;

    [Header("Raycast")]
    public float raycastDistance = 5f;
    public LayerMask itemLayer;
    public Transform dropLocation;

    [Header("Drag and Drop")]
    public Image dragImageIcon;
    private Item currentDraggedItem;
    private int currentDragSlotIndex = -1;

    [Header("Equippable Items")]
    public List<GameObject> equippableItems = new List<GameObject>();
    public Transform selectedItemImage;
    private int curHotbarIndex = -1;

    [Header("Crafting")]
    public List<Recipe> itemRecipe = new List<Recipe>();

    [Header("Save/Load")]
    public List<GameObject> allItemprefabs = new List<GameObject>();
    private string saveFileName = "inventorySave.json";

    public void Start()
    {
        isOpen = false;
        ToggleInventory(false);

        allInventorySlots.AddRange(hotbarSlots);
        allInventorySlots.AddRange(inventorySlots);

        foreach (Slot uiSlot in allInventorySlots)
        {
            uiSlot.InitialiseSlot();
        }

        LoadInventory();
    }

    public void OnApplicationQuit()
    {
        SaveInventory();
    }

    public void Update()
    {
        ItemRaycast(Input.GetKeyDown(KeyCode.E));

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory(!inventory.activeInHierarchy);
        }

        if (inventory.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            DragInventoryIcon();
        }
        else if (currentDragSlotIndex != -1 && Input.GetMouseButtonUp(0) || currentDragSlotIndex != -1 && !inventory.activeInHierarchy)
        {
            DropInventoryIcon();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }

        for (int i = 1; i < hotbarSlots.Count + 1; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                Destroy(BuildingManager.ghostBuildGameobject);
                EnableHotbarItem(i - 1);

                curHotbarIndex = i - 1;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            AttemptToUseItem();
        }

        dragImageIcon.transform.position = Input.mousePosition;
    }

    private void DropItem()
    {
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];
            if (curSlot.hovered && curSlot.HasItem())
            {
                curSlot.GetItem().gameObject.SetActive(true);
                curSlot.GetItem().transform.position = dropLocation.position;
                curSlot.SetItem(null);
                break;
            }
        }
    }

    private void DropInventoryIcon()
    {
        dragImageIcon.sprite = null;
        dragImageIcon.color = new Color(1, 1, 1, 0);

        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];
            if (curSlot.hovered)
            {
                if (curSlot.HasItem())
                {
                    Item itemToSwap = curSlot.GetItem();
                    curSlot.SetItem(currentDraggedItem);
                    allInventorySlots[currentDragSlotIndex].SetItem(itemToSwap);
                    ResetDragVariables();
                    return;
                }
                else
                {
                    curSlot.SetItem(currentDraggedItem);
                    ResetDragVariables();
                    return;
                }
            }    
        }
        allInventorySlots[currentDragSlotIndex].SetItem(currentDraggedItem);
        ResetDragVariables();
    }

    private void ResetDragVariables()
    {
        currentDraggedItem = null;
        currentDragSlotIndex = -1;
    }

    private void DragInventoryIcon()
    {
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];

            if (curSlot.hovered && curSlot.HasItem())
            {
                currentDragSlotIndex = i;
                currentDraggedItem = curSlot.GetItem();
                dragImageIcon.sprite = currentDraggedItem.icon;
                dragImageIcon.color = new Color (1, 1, 1, 1);
                curSlot.SetItem(null);
            }
        }
    }

    private void ItemRaycast(bool hasClicked = false)
    {
        itemHoverText.text = "";
        Ray ray = Camera.main.ScreenPointToRay(crosshair.transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, itemLayer))
        {
            if (hit.collider != null)
            {
                if (hasClicked)
                {
                    Item newItem = hit.collider.GetComponent<Item>();
                    if (newItem)
                    {
                        AddItemToInventory(newItem);
                    }
                }
                else
                {
                    Item newItem = hit.collider.GetComponent<Item>();

                    if (newItem)
                    {
                        itemHoverText.text = newItem.name;
                    }
                }
            }
        }
    }

    public void AddItemToInventory(Item itemToAdd, int overrideIndex = -1)
    {
        if (overrideIndex != -1)
        {
            allInventorySlots[overrideIndex].SetItem(itemToAdd);
            itemToAdd.gameObject.SetActive(false);
            allInventorySlots[overrideIndex].UpdateData();
            return;
        }

        int leftoverQuantity = itemToAdd.currentQuantity;
        Slot openSlot = null;
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Item heldItem = allInventorySlots[i].GetItem();

            if (heldItem != null && itemToAdd.name == heldItem.name)
            {
                int freeSpaceInSlot = heldItem.maxQuantity - heldItem.currentQuantity;

                if (freeSpaceInSlot >= leftoverQuantity)
                {
                    heldItem.currentQuantity += leftoverQuantity;
                    Destroy(itemToAdd.gameObject);
                    allInventorySlots[i].UpdateData();
                    return;
                }
                else
                {
                    heldItem.currentQuantity = heldItem.maxQuantity;
                    leftoverQuantity -= freeSpaceInSlot;
                }
            }
            else if (heldItem == null)
            {
                if (!openSlot)
                    openSlot = allInventorySlots[i];
            }

            allInventorySlots[i].UpdateData();
        }

        if (leftoverQuantity > 0 && openSlot)
        {
            openSlot.SetItem(itemToAdd);
            itemToAdd.currentQuantity = leftoverQuantity;
            itemToAdd.gameObject.SetActive(false);
        }
        else
        {
            itemToAdd.currentQuantity = leftoverQuantity;
        }
    }

    private void ToggleInventory(bool enable)
    {
        if (FirstPersonController.isMenuActive == false)
        {
            isOpen = enable;
            inventory.SetActive(enable);

            Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = enable;
        }
    }

    private void EnableHotbarItem(int hotbarIndex)
    {
        foreach (GameObject a in equippableItems)
        {
            a.SetActive(false);
        }
        Slot hotbarSlot = hotbarSlots[hotbarIndex];
        selectedItemImage.transform.position = hotbarSlots[hotbarIndex].transform.position;
        if (hotbarSlot.HasItem())
        {
            if (hotbarSlot.GetItem().equippableItemIndex != -1)
            {
                equippableItems[hotbarSlot.GetItem().equippableItemIndex].SetActive(true);
                Destroy(BuildingManager.ghostBuildGameobject);
            }
        }
    }

    public void CraftItem(string itemName)
    {
        foreach (Recipe recipe in itemRecipe)
        {
            if (recipe.createdItemPrefab.GetComponent<Item>().name == itemName)
            {
                bool haveAllIngredients = true;
                for (int i = 0; i < recipe.requiredIngredients.Count; i++)
                {
                    if (haveAllIngredients)
                    {
                        haveAllIngredients = haveIngredient(recipe.requiredIngredients[i].itemName, recipe.requiredIngredients[i].requiredQuantity);
                    }
                }

                if (haveAllIngredients)
                {
                    for (int i = 0; i < recipe.requiredIngredients.Count; i++)
                    {
                        RemoveIngredient(recipe.requiredIngredients[i].itemName, recipe.requiredIngredients[i].requiredQuantity);
                    }

                    GameObject craftedItem = Instantiate(recipe.createdItemPrefab, dropLocation.position, Quaternion.identity);
                    craftedItem.GetComponent<Item>().currentQuantity = recipe.quantityProduced;
                    AddItemToInventory(craftedItem.GetComponent<Item>());
                }
                break;
            }
        }
    }

    private void RemoveIngredient(string itemName, int quantity)
    {
        if (!haveIngredient(itemName, quantity))
        {
            return;
        }

        int remainingQuatity = quantity;

        foreach (Slot curSlot in allInventorySlots)
        {
            Item item = curSlot.GetItem();

            if (item != null && item.name == itemName)
            {
                if (item.currentQuantity >= remainingQuatity)
                {
                    item.currentQuantity -= remainingQuatity;
                    if (item.currentQuantity == 0)
                    {
                        curSlot.SetItem(null);
                    }
                    return;
                }
                else
                {
                    remainingQuatity -= item.currentQuantity;
                    curSlot.SetItem(null);
                }
            }
        }
    }

    private bool haveIngredient(string itemName, int quantity)
    {
        int foundQuantity = 0;
        foreach (Slot curSlot in allInventorySlots)
        {
            if (curSlot.HasItem() && curSlot.GetItem().name == itemName)
            {
                foundQuantity += curSlot.GetItem().currentQuantity;

                if (foundQuantity >= quantity)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void SaveInventory()
    {
        InventoryData data = new InventoryData();

        foreach (Slot slot in allInventorySlots)
        {
            Item item = slot.GetItem();
            if (item != null)
            {
                ItemData itemData = new ItemData(item.name, item.currentQuantity, allInventorySlots.IndexOf(slot));
                data.slotData.Add(itemData);
            }
        }

        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(saveFileName, jsonData);
    }

    private void LoadInventory()
    {
        if (File.Exists(saveFileName))
        {
            string jsonData = File.ReadAllText(saveFileName);
            InventoryData data = JsonUtility.FromJson<InventoryData>(jsonData);

            foreach (ItemData itemData in data.slotData)
            {
                GameObject itemPrefab = allItemprefabs.Find(prefab => prefab.GetComponent<Item>().name == itemData.itemName);

                if (itemPrefab != null)
                {
                    GameObject createdItem = Instantiate(itemPrefab, dropLocation.position, Quaternion.identity);
                    Item item = createdItem.GetComponent<Item>();
                    item.currentQuantity = itemData.quantity;
                    AddItemToInventory(item, itemData.slotIndex);
                }
            }
        }

        foreach (Slot slot in allInventorySlots)
        {
            slot.UpdateData();
        }
    }

    private void ClearInventory()
    {
        foreach (Slot slot in allInventorySlots)
        {
            slot.SetItem(null);
        }
    }

    private void AttemptToUseItem()
    {
        if (curHotbarIndex == -1)
        {
            return;
        }

        Item curItem = hotbarSlots[curHotbarIndex].GetItem();

        if (curItem)
        {
            curItem.UseItem();

            if (curItem.currentQuantity != 0)
            {
                hotbarSlots[curHotbarIndex].UpdateData();
            }
            else
            {
                hotbarSlots[curHotbarIndex].SetItem(null);
            }

            EnableHotbarItem(curHotbarIndex);
        }
    }
}

[System.Serializable]

public class ItemData
{
    public string itemName;
    public int quantity;
    public int slotIndex;

    public ItemData (string itemName, int quantity, int slotIndex)
    {
        this.itemName = itemName;
        this.quantity = quantity;
        this.slotIndex = slotIndex;
    }
}

[System.Serializable]

public class InventoryData
{
    public List<ItemData> slotData = new List<ItemData>();
}
