using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class TestingCanvas : MonoBehaviour
{

    //this script controls the testing canvas.
    public InventoryItemData[] itemsData;//stores a list of items' scriptable objects that can be added to inventory
    public InventoryManager inventoryManager;
    public GameObject inventoryItemPrefab;
    public GameObject Fund;
    public bool isDebug = false;

    private bool removeItemSuccess=false;
    private List<GameObject> test3DObjects = new List<GameObject>();

    public void Start()
    {
        if (!FarmFund.Get().enableTestingCanvas)
            gameObject.SetActive(false); // Disable the canvas if we are told we don't need it.
        else { 
            inventoryManager = InventoryManager.Get();
        }
    }

    public void AddItem(int index)
    {
        //Call the inventory Manager and add the item with the right itemData 
        if (index < itemsData.Length)
        {
            if(isDebug)
                Debug.Log("AddItem " + itemsData[index].itemName);
            inventoryManager.AddItem(itemsData[index]);
        }
    }
    public void RemoveItem(string itemName) {
        //Call the inventory Manager to remove an item
        removeItemSuccess = inventoryManager.RemoveItem(itemName);
        if (!removeItemSuccess)
        {
            Debug.LogWarning(itemName + " cannot be removed from the inventory.");
        }
    }

    //Only for test so it only has a Cube representing Wheat 3D model
    public void SpawnItemTo3DWhenPlanting()
    {
        //a testing script to spawn an object to 3D to show the user just planted a small plant to 3D that was in the bag UI
        if (removeItemSuccess) {
            GameObject newItem = Instantiate(inventoryItemPrefab, new Vector3(Random.Range(-8,8), 0, Random.Range(-1, 8)), Quaternion.identity);
            test3DObjects.Add(newItem);
        }
    }
    //Only for test so it only has a Cube representing Wheat 3D model
    public void HarvestItem()
    {
        if (test3DObjects.Count > 0)
        {
            GameObject item = test3DObjects[0];
            test3DObjects.RemoveAt(0);
            Destroy(item);
            AddItem(3);//add 1 to the Wheat L UI sprite
        }
    }
    //This is testing only so the Wheat L is hardcoded to test if the Inventory Manager works
    public void SellItem(string itemName)
    {
        RemoveItem(itemName);
        if (removeItemSuccess) { //if remove item is successful, add the fund
            Fund.GetComponent<Fund>().AddFund(8);
        }
    }
}
