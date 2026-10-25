using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public static GameObject InstanceOBJ;
    public bool isDebug = true;
    public GameObject[] inventorySlots;
    public GameObject inventoryMain;//will hide it after grabbing all the inventory slots in start
    public GameObject inventoryItemPrefab;
    private List<GameObject> itemInBagList = new List<GameObject>();
    public GameObject toolSlot;
    public GameObject seedSlot;
        // Task 3 - remembers which actual inventory tool is equipped
    // and which inventory slot it came from.
    private GameObject equippedToolItem;
    private Transform equippedToolHomeSlot;



    // for AddItems()
    private int lastUsedIndex; // Used to keep track of the last item slot filled
    private int numLeft; // Quantity left to add
    private InventoryItemData itemDataToAdd; // Item left to add


    private void Awake()
    {
        // only keep one InventoryManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public static InventoryManager Get()
    {
        if (Instance == null)
        {
            return null; // Don't create the InventoryManager because you need the whole prefab
            //new GameObject("InventoryManager").AddComponent<InventoryManager>();
        }
        return Instance;
    }
    public static bool Active()
    {
        return (Instance != null);
    }
    private void Start()
    {
        inventoryMain.SetActive(true);//temporary turn on the Main bag so that all the slots gets stored in the list
        inventorySlots = GameObject.FindGameObjectsWithTag("InventorySlot");
        inventorySlots = inventorySlots.OrderBy(go => go.name).ToArray();
        inventoryMain.SetActive(false);
    }
    public bool AddItem(InventoryItemData itemData)
    {
        if (isDebug)
            Debug.Log("InventoryManager adding item checking if existed: " + itemData.itemName);
        for (int index = 0; index < itemInBagList.Count; index++)//if this item already exist in the bag, just refresh counter
        {
            Debug.Log(index.ToString() + itemInBagList[index].name);
            if (itemInBagList[index].name == itemData.itemName)
            {
                AddExistingItem(index, itemData);
                return true;
            }
        }
        return AddNewItem(itemData);//if this item is a new item, find a empty slot to add it in
    }
    private void AddExistingItem(int index, InventoryItemData itemData)
    {
        itemInBagList[index].GetComponent<InventoryItem>().AddOneItem();
        lastUsedIndex = index; // Saves the last used item slot
    }
    private bool AddNewItem(InventoryItemData itemData)
    {
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
    private void SpawnANewItem(
        InventoryItemData itemData,
        GameObject emptySlot)
    {
        GameObject newItem =
            Instantiate(
                inventoryItemPrefab,
                emptySlot.transform
            );

        InventoryItem inventoryItem =
            newItem.GetComponent<InventoryItem>();

        if (inventoryItem == null)
        {
            Debug.LogError(
                "Inventory prefab is missing InventoryItem."
            );

            return;
        }

        inventoryItem.itemData = itemData;

        Button itemButton =
            newItem.GetComponentInChildren<Button>(true);

        if (itemButton == null)
        {
            Debug.LogError(
                "Inventory prefab is missing a Button."
            );
        }
        else
        {
            itemButton.onClick.AddListener(
                inventoryItem.SelectItem
            );

            Debug.Log(
                "Selection button connected for: " +
                itemData.itemName
            );
        }

        itemData.amountInBag = 0;
        itemInBagList.Add(newItem);
        lastUsedIndex = itemInBagList.Count - 1;
    }

    //API for other parts of the program to remove an item from the inventory, like selling or planting
    public bool RemoveItem(string itemName)
    {
        Debug.Log(
    "REMOVING ITEM: " +
    itemName +
    "\nCALL STACK:\n" +
    System.Environment.StackTrace
);
        if (isDebug)
            Debug.Log("InventoryManager removing item: " + itemName);
        for (int index = 0; index < itemInBagList.Count; index++)//if this item already exist in the bag, just refresh counter
        {
            if (itemInBagList[index].name == itemName)
            {
                GameObject item = itemInBagList[index];
                bool successfulRemove = item.GetComponent<InventoryItem>().RemoveOneItem();

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

    public bool RemoveItems(string itemName, int num)
    {
        for (int index = 0; index < itemInBagList.Count; index++)//if this item already exist in the bag, just refresh counter
        {
            if (itemInBagList[index].name == itemName)
            {
                GameObject item = itemInBagList[index];
                bool successfulRemove = item.GetComponent<InventoryItem>().RemoveItems(num);
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
        if (AddItem(itemData)) // If we successfully add the item (not all slots full)
        {
            if (HasItem(itemData.itemName))
                num--; // There's a weird off-by-one error if the item isn't yet in the inventory (making a new slot).
            for (int i = num; i > 0; i--)
            {
                //Debug.Log(itemDataToAdd.itemName + i.ToString() + "." + lastUsedIndex.ToString());
                AddExistingItem(lastUsedIndex, itemData);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

        public void SetTool(InventoryItemData itemData)
    {
        if (itemData == null || toolSlot == null)
        {
            Debug.LogError("SetTool is missing itemData or toolSlot.");
            return;
        }

        // Find the ACTUAL inventory item already stored in the bag/bar.
        GameObject requestedTool = null;

        foreach (GameObject item in itemInBagList)
        {
            if (item == null)
                continue;

            InventoryItem inventoryItem = item.GetComponent<InventoryItem>();

            if (inventoryItem != null &&
                inventoryItem.itemData != null &&
                inventoryItem.itemData.itemName == itemData.itemName)
            {
                requestedTool = item;
                break;
            }
        }

        if (requestedTool == null)
        {
            Debug.LogWarning(
                "Could not equip " + itemData.itemName +
                " because it was not found in the inventory."
            );
            return;
        }

        // Already equipped.
        if (equippedToolItem == requestedTool)
            return;

        // Put the previously equipped tool back first.
        RemoveTool();

        // Remember where this tool came from.
        equippedToolItem = requestedTool;
        equippedToolHomeSlot = requestedTool.transform.parent;

        // Move the existing tool into the active tool slot.
        requestedTool.transform.SetParent(toolSlot.transform, false);

        RectTransform rect = requestedTool.GetComponent<RectTransform>();

        if (rect != null)
        {
            rect.anchoredPosition = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        if (isDebug)
            Debug.Log("Moved " + itemData.itemName + " into the active tool slot.");
    }

    public void SetSeed(InventoryItemData itemData)
        {
            GameObject newItem = Instantiate(inventoryItemPrefab, seedSlot.transform);
            newItem.GetComponent<InventoryItem>().itemData = itemData;
        }

        public void RemoveTool()
    {
        if (equippedToolItem == null)
            return;

        if (equippedToolHomeSlot != null)
        {
            // Return the actual tool to the inventory slot it originally came from.
            equippedToolItem.transform.SetParent(equippedToolHomeSlot, false);

            RectTransform rect = equippedToolItem.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
            }

            if (isDebug)
                Debug.Log(
                    "Returned " + equippedToolItem.name +
                    " to its inventory slot."
                );
        }

        equippedToolItem = null;
        equippedToolHomeSlot = null;
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
