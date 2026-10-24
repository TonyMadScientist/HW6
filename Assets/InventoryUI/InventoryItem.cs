using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public InventoryItemData itemData;//use the scriptable object
    public TMP_Text counterText;
    private bool quantityUpdated = false;

    public void Start()
    {
        gameObject.GetComponent<Image>().sprite = itemData.itemSprite;
        transform.name=itemData.itemName;
        if (!quantityUpdated)
            itemData.amountInBag = 1;
        RefreshUICounter(); // RefreshUI to remove counter if it is not stackable (I.E. tools)
    }

    //API called by the inventory manager when adding an item that is already stored in the bag
    public void AddOneItem()
    {
        quantityUpdated = true;
        itemData.amountInBag++;
        RefreshUICounter();
    }
    //API called by the inventory manager when removing an item that is already stored in the bag
    public bool RemoveOneItem()
    {
        if (itemData.amountInBag > 0)
        {
            itemData.amountInBag--;
            RefreshUICounter();
            return true;
        }
        else 
            return false;
    }

    public bool RemoveItems(int num)
    {
        if (itemData.amountInBag >= num)
        {
            itemData.amountInBag -= num;
            RefreshUICounter();
            return true;
        }
        else
            return false;
    }



    private void RefreshUICounter()
    {
        if (itemData.stackable)
        {
            //Debug.Log(itemData.amountInBag);
            counterText.text = itemData.amountInBag.ToString(); // If Stackable, show quantity
        }
        else // Hide and disable quantity
        {
            counterText.text = "";
            counterText.GameObject().SetActive(false);
        }
    }

    public void SelectItem()
    {
        if (itemData == null)
        {
            Debug.LogError("Inventory item has no ItemData.");
            return;
        }

        if (itemData.itemType != ItemType.seed)
        {
            return;
        }

        if (itemData.plantData == null)
        {
            Debug.LogError(
                itemData.itemName +
                " has no PlantData assigned."
            );

            return;
        }

        PlantManager plantManager =
            FindFirstObjectByType<PlantManager>();

        if (plantManager == null)
        {
            Debug.LogError("PlantManager was not found.");
            return;
        }

        // This updates BOTH:
        // selectedSeedItem
        // selectedPlantData
        plantManager.SetSelectedSeed(itemData);

        Debug.Log(
            "Selected seed: " +
            itemData.itemName +
            " → " +
            itemData.plantData.plantName
        );
    }

    private void Update()
    {
        //RefreshUICounter();
    }
}
