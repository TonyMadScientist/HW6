using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemDrag : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    public bool isDebug=false;//turn it on in Editor if want to see debug message
    Transform parentAfterDrag;
    //called by Unity when the mouse click starts dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDebug)
            Debug.Log("Begin Drag");
        parentAfterDrag = transform.parent;

        //move the UI item to the last item of the Canvas so it appears in front of all the UI elements
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        //disable the item detecting the mouse click raycast so that the slot below it can detect 
        //the mouse hovering over it
        this.gameObject.GetComponent<Image>().raycastTarget=false;
    }

    //called by Unity when the mouse click is draging UI item
    public void OnDrag(PointerEventData eventData)
    {
        if(isDebug)
            Debug.Log("Dragging");
        transform.position = eventData.position;

    }

    //called by Unity when the mouse click finishes dragging
    public void OnEndDrag(PointerEventData eventData)
    {
        if(isDebug)
            Debug.Log("End Drag");
        transform.SetParent(parentAfterDrag);

        //enable the item detecting the mouse click raycast so that it can be picked up again
        this.gameObject.GetComponent<Image>().raycastTarget = true;
    }

    //API used by the traget slot to drop the Item into
    public void setParentAfterDrag(Transform newSlotParent)
    {
        parentAfterDrag=newSlotParent;
    }
    //API used by the traget slot to drop the Item into
    public Transform getParentAfterDrag()
    {
        return parentAfterDrag;
    }
}
