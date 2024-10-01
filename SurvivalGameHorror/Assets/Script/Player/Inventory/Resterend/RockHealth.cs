using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth = 10;
    [SerializeField] private List<ItemDrop> itemDrops = new List<ItemDrop>();
    //public GameObject treeStump;
    //[SerializeField] private float damageCooldown = 0.5f;
    //private float newTime = 0.5f;

    private void Update()
    {
        if (currentHealth <= 0)
        {
             Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage, GameObject player)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            foreach (ItemDrop item in itemDrops)
            {
                int quantityToDrop = Random.Range(item.minQuantityDrop, item.maxQuantityDrop);

                if (quantityToDrop == 0)
                {
                    return;
                }

                Item droppedItem = Instantiate(item.itemToDrop, transform.position, Quaternion.identity).GetComponent<Item>();
                droppedItem.currentQuantity = quantityToDrop;

                player.GetComponent<Inventory>().AddItemToInventory(droppedItem);
            }
        }
    }

}

[System.Serializable]

public class ItemDropRock
{
    public GameObject itemToDrop;
    public int minQuantityDrop = 2;
    public int maxQuantityDrop = 4;
}