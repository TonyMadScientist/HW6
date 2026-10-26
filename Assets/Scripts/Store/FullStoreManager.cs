using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class FullStoreManager : MonoBehaviour
{
    //public Button buySeedButton;
    public Button startNextLevelButton;

    // for text on screen
    public TMP_Text fundText;
    public TMP_Text storeMessage;

    public GameObject upgradePanel;   //  for INVENTORY BAG upgrade 
    public GameObject buttonHolder;

    public GameObject[] buttons;

    public InventoryItemData goldItem;
    public InventoryItemData[] tools;
    public InventoryItemData seedItem;
    public InventoryItemData harvestItem;

    public GameObject inventoryManagerPrefab;
    private int storeVisits;

    public AudioClip clickSound;
    public AudioClip bagSound;

    private InventoryManager inventoryManager;

    private void Start()
    {
        storeVisits = ++FarmFund.Get().storeVisits;
        Debug.Log("STORE VISITS = " + storeVisits);
        // Can't leave until the player buys seeds
        startNextLevelButton.interactable = false;

        InitInventory();
        buttons = GameObject.FindGameObjectsWithTag("ShopButton");
        buttons = buttons.OrderBy(go => go.transform.GetSiblingIndex().ToString("D2")).ToArray(); // Sort our buttons by the hierarchy order

        // Only display the amount of buttons equal to the amount of storeVisits we have done, subtracted by 1.
        // I.e. the first time you visit this store is on Visit 3, where it shows 2 buttons. 4th visit it shows 3 buttons.
        for (int i = 0; i < buttons.Count(); i++)
        {
            if (i >= (storeVisits - 1))
            {
                buttons[i].SetActive(false);
            }
        }

        if (FarmFund.Get().storeVisits <= 3) // If this is the first time entering the full store.
        {
            fundText.GameObject().SetActive(true);
            upgradePanel.SetActive(true);
            storeMessage.text = "STORE\r\nPurchase the bag to access the full store feature.";
            buttonHolder.SetActive(false);
        }            
        RefreshUI();
    }
    private void InitInventory()
    {
        if (!InventoryManager.Active())
        {
            GameObject.Instantiate(inventoryManagerPrefab);
        }
        inventoryManager = InventoryManager.Get();
    }

    public void BuyingBag(int bagPrice)
    {
		GetComponent<AudioSource>().PlayOneShot(bagSound);
        fundText.gameObject.SetActive(false);
        upgradePanel.SetActive(false);

        storeMessage.text =
            "SUPER STORE\r\nWith the tutorial over, you now have full store access.";

        buttonHolder.SetActive(true);

        FarmFund farm = FarmFund.Get();

        farm.SpendMoney(bagPrice);

        InitInventory();

        if (farm.money <= 0)
        {
            farm.money = 225; // Prevent possibility of softlock if loading from scene (debugging) instead of from scene0
        }

        inventoryManager.AddItems(goldItem, farm.money);

        foreach (InventoryItemData tool in tools)
        {
            inventoryManager.AddItem(tool);
        }

        inventoryManager.AddItems(seedItem, farm.seeds);
        inventoryManager.AddItems(harvestItem, farm.harvestedCount);

        farm.seeds = 0;
        farm.harvestedCount = 0;

        RefreshUI();
    }

    public void StartNextLevel()
    {
        GameObject soundObj = new GameObject("ClickSound");
        DontDestroyOnLoad(soundObj);
        soundObj.AddComponent<AudioSource>().PlayOneShot(clickSound);
        Destroy(soundObj, clickSound.length);
        FarmFund.Get().NextLevel();
    }
    private void RefreshUI()
    {
        fundText.text = "Fund: $" + FarmFund.Get().money;
    }

    public void ClickItem(InventoryItemData itemData)
    {
        if (goldItem == null)
            return;

        switch (itemData.interactionType)
        {
            case (InteractionType.sell):
                if (inventoryManager.RemoveItem(itemData.itemName))
                    inventoryManager.AddItems(goldItem, itemData.sellPrice);
                break;
            default:
            case (InteractionType.buy):
                if (inventoryManager.RemoveItems(goldItem.itemName, itemData.buyPrice))
                    inventoryManager.AddItem(itemData);
                break;
        }

        // This activates the Next Level button
        startNextLevelButton.interactable = true;
    }
}