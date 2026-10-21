using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public InventoryItemData itemData;//use the scriptable object
    public TMP_Text counterText;

    public void Start()
    {
        gameObject.GetComponent<Image>().sprite = itemData.itemSprite;
        transform.name=itemData.itemName;
        itemData.amountInBag = 1;
    }

    //API called by the inventory manager when adding an item that is already stored in the bag
    public void AddOneItem()
    {   
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
    private void RefreshUICounter() {
        counterText.text=itemData.amountInBag.ToString();
    }
}
