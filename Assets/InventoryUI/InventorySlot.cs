using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour,IDropHandler
{
    public bool isDebug = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (isDebug)
            Debug.Log("Drop to slot"+transform.name);
        //the dragged Item that is dropped to the slot is now saved to the variable
        GameObject droppedItem = eventData.pointerDrag;

        if (transform.childCount == 1)//if this slot is occupied by another item

        {
            //move the existing object to the dragged Item's original slot
            Transform droppedItemOriginalParent = droppedItem.GetComponent<InventoryItemDrag>().getParentAfterDrag();
            transform.GetChild(0).SetParent(droppedItemOriginalParent);
        }
        
        //drop this Item to the empty slot
        droppedItem.GetComponent<InventoryItemDrag>().setParentAfterDrag(transform);

    }
}
