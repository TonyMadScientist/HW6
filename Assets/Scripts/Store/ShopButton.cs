using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public InventoryItemData itemData;
    public TMP_Text textObj;
    public Image spriteObj;
    public FullStoreManager store;
    public AudioSource audioSource;
    public AudioClip clickSound;

    private void Start()
    {
        audioSource = transform.parent.GetComponent<AudioSource>();
        if (store == null)
            store = GameObject.Find("FullStoreManager").GetComponent<FullStoreManager>();
        spriteObj.sprite = itemData.itemSprite;
        string actiontext = "";
        string itemtext = itemData.itemName;
        string pricetext = "";
        switch (itemData.interactionType) {
            case (InteractionType.sell):
                actiontext = "Sell ";
                pricetext = " (+$" + itemData.sellPrice.ToString() + ")";
                break;
            default:
            case (InteractionType.buy):
                actiontext = "Buy ";
                pricetext = " (-$" + itemData.buyPrice.ToString() + ")";
                break;        
        }
        textObj.text = actiontext + itemtext + pricetext;
    }
    // See FullStoreManager.cs for actual logic
    public void ButtonClick()
    {
        audioSource.PlayOneShot(clickSound);
        if (store != null)
        {
            store = GameObject.Find("FullStoreManager").GetComponent<FullStoreManager>();
        }
        store.ClickItem(itemData);
    }
}
