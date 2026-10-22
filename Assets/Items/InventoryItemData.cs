using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemData", menuName = "Scriptable Objects/InventoryItemData")]
public class InventoryItemData : ScriptableObject
{
    [Header ("Game Play")]
    public string itemName;//will be used as ID

    public int buyPrice;
    public int sellPrice;
    public int amountInBag = 0;

    public ItemType itemType;
    public InteractionType interactionType;

    [Header("UI")]
    //sprites used by the inventory
    public Sprite itemSprite;
    public bool stackable = true;
    
}
//A plant can only be planted in the right season and all plants disappear in winter
public enum ItemType
{
    tool,seed,harvest,currency
}

//you can change the usetool type to different type of interactions for different tools
public enum InteractionType
{
    usetool,buy,sell,currency
}
