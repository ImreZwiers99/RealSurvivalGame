using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeHealth : MonoBehaviour
{
    [SerializeField] private int currentHealth = 10;
    [SerializeField] private List<ItemDrop> itemDrops = new List<ItemDrop>();
    public float rotationAmount = 15f;
    public bool hasRotated = false;
    //public GameObject treeStump;
    //[SerializeField] private float damageCooldown = 0.5f;
    //private float newTime = 0.5f;

    private void Update()
    {
        if (currentHealth <= 0 && !hasRotated)
        {
            StartCoroutine(TreeDelete());
            Rigidbody rb = gameObject.AddComponent<Rigidbody>(); 
            rb.mass = 50;
            rb.drag = 3;

            Vector3 rotationDirection = Camera.main.transform.right;
            rb.AddTorque(rotationDirection * rotationAmount, ForceMode.Impulse);
            rotationDirection.y = 0; 

            hasRotated = true;
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

            //Destroy(gameObject);
        }
    }

    private IEnumerator TreeDelete()
    {
        yield return new WaitForSeconds(8f);
        Destroy(gameObject);
    }
}

[System.Serializable]

public class ItemDrop
{
    public GameObject itemToDrop;
    public int minQuantityDrop = 2;
    public int maxQuantityDrop = 4;
}