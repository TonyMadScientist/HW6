using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public bool isDebug=true;
    public GameObject[] inventorySlots;
    public GameObject inventoryMain;//will hide it after grabbing all the inventory slots in start
    public GameObject inventoryItemPrefab;
    private List<GameObject> itemInBagList= new List<GameObject>();
    public GameObject toolSlot;
    public GameObject seedSlot;

    public void Start()
    {
        inventoryMain.SetActive(true);//temporary turn on the Main bag so that all the slots gets stored in the list
        inventorySlots = GameObject.FindGameObjectsWithTag("InventorySlot");
        inventorySlots = inventorySlots.OrderBy(go => go.name).ToArray();
        inventoryMain.SetActive(false);
    }
    public bool AddItem(InventoryItemData itemData) {
        if (isDebug)
            Debug.Log("InventoryManager adding item checking if existed: " + itemData.itemName);
        for (int index=0;index<itemInBagList.Count;index++)//if this item already exist in the bag, just refresh counter
        {
            if (itemInBagList[index].name == itemData.itemName)
            {
                AddExistingItem(index,itemData);
                return true;
            }
        }
        return AddNewItem(itemData);//if this item is a new item, find a empty slot to add it in
    }
    private void AddExistingItem(int index,InventoryItemData itemData)
    {
        itemInBagList[index].GetComponent<InventoryItem>().AddOneItem();
    }
    private bool AddNewItem(InventoryItemData itemData) {
        //find an empty slot in the list of inventory slots.
        foreach (var slot in inventorySlots)
        {
            if (slot.transform.childCount == 0)//no child object means an empty slot
            {
                if (isDebug)
                    Debug.Log("Add item to empty slot " + slot.name);
                SpawnANewItem(itemData, slot);
                return true;//stop the loop once 1 empty slot is found
            }
        }
        return false;//the slots are all full, give user a warning message
    }
    private void SpawnANewItem(InventoryItemData itemData, GameObject emptySlot) {
        GameObject newItem = Instantiate(inventoryItemPrefab, emptySlot.transform);
        newItem.GetComponent<InventoryItem>().itemData = itemData;
        //add to the list of items 
        itemInBagList.Add(newItem);
    }

    //API for other parts of the program to remove an item from the inventory, like selling or planting
    public bool RemoveItem(string itemName) {
        if (isDebug)
            Debug.Log("InventoryManager removing item: " + itemName);
        for (int index = 0; index < itemInBagList.Count; index++)//if this item already exist in the bag, just refresh counter
        {
            if (itemInBagList[index].name == itemName)
            {
                GameObject item= itemInBagList[index];
                bool successfulRemove=item.GetComponent<InventoryItem>().RemoveOneItem();

                int amountInBag = item.GetComponent<InventoryItem>().itemData.amountInBag;
                if (amountInBag == 0) //item count==0, also remove the sprite from the slot
                {
                    Destroy(item);
                    itemInBagList.RemoveAt(index);
                }
                return successfulRemove;
           
            }
        }
        return false;//the bag does not contain the item to be removed
    }

    public bool HasItem(string itemName)
    {
        if (isDebug)
            Debug.Log("InventoryManager checking for item: " + itemName);
        for (int index = 0; index < itemInBagList.Count; index++)//if this item already exist in the bag, just refresh counter
        {
            if (itemInBagList[index].name == itemName)
            {
                return true;
            }
        }
        return false;
    }

    public bool AddItems(InventoryItemData itemData, int num)
    {
        for (int i=0; i<num; i++)
        {
            if (!AddItem(itemData))
            {
                return false;
            }
        }
        return true;
    }

    public void SetTool(InventoryItemData itemData)
    {
        RemoveTool();
        GameObject newItem = Instantiate(inventoryItemPrefab, toolSlot.transform);
        newItem.GetComponent<InventoryItem>().itemData = itemData;
    }

    public void SetSeed(InventoryItemData itemData)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, seedSlot.transform);
        newItem.GetComponent<InventoryItem>().itemData = itemData;
    }

    public void RemoveTool()
    {
        // Safe backwards loop to destroy all children
        for (int i = toolSlot.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(toolSlot.transform.GetChild(i).gameObject);
        }
    }
    public void RemoveSeedSlot()
    {
        // Safe backwards loop to destroy all children
        for (int i = seedSlot.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(seedSlot.transform.GetChild(i).gameObject);
        }
    }
}
