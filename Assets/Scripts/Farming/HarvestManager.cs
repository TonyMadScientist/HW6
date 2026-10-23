using NUnit.Framework.Interfaces;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HarvestManager : MonoBehaviour
// Control is H for harvesting, press near a plant
{
    public Transform player;          
    public TMP_Text harvestText;      
    //public Key harvestKey = Key.H;
    public float harvestRange = 1f;

    private InventoryManager inventoryScript;

    public InventoryItemData[] harvestItems;//stores a list of items' scriptable objects that can be added to inventory

    private void Start()
    {
        FarmFund.Get().harvestedCount = 0; // each level starts its harvest count at 0
        UpdateText();
        if (InventoryManager.Active())
            inventoryScript = InventoryManager.Get(); // Only initialize if it was already created.
    }

    private void Update()
    {
        /*
        if (Keyboard.current != null && Keyboard.current[harvestKey].wasPressedThisFrame)
        {
            TryHarvest();
        }
        */
    }

    public bool TryHarvest()
    {
        // find the closest plant to the player
        Plant closest = null;
        float closestDist = harvestRange;

        foreach (Plant plant in FindObjectsByType<Plant>())
        {
            // compare only left right and forward back distance
            Vector3 a = plant.transform.position;
            Vector3 b = player.position;
            a.y = 0;
            b.y = 0;
            float dist = Vector3.Distance(a, b);

            if (dist < closestDist)
            {
                closest = plant;
                closestDist = dist;
            }
        }

        if (closest == null)
        {
            Debug.Log("No plant close enough to harvest.");
            return false;
        }

        if (!closest.IsMature())
        {
            Debug.Log("This plant isn't ready yet.");
            return false;
        }

        // remove the plant to count it
        Destroy(closest.gameObject);
        if (inventoryScript == null) // If we are in a scene WITHOUT InventoryManager
        {
            FarmFund.Get().harvestedCount++;
            UpdateText();
            Debug.Log("Harvested! Total: " + FarmFund.Get().harvestedCount);
        } else { // We are in a scene WITH InventoryManager, so use that instead for harvest berries.
            int index = 0; // Add the first item in the script. Hardcoded value.
            // THIS NEEDS TO BE CHANGED LATER when we are detecting which plant we are removing.
            if (index < harvestItems.Length)
            {
                inventoryScript.AddItem(harvestItems[index]);
            } else
                Debug.LogError("HarvestManager script needs to be given InventoryItemData array, out of bounds!");
        }
        return true;
    }

    private void UpdateText()
    {
        harvestText.text = "Harvested: " + FarmFund.Get().harvestedCount;
    }
}
