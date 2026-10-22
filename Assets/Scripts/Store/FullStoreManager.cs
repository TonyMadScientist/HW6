using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
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


    //public string plantSceneName = "Scene3-Plant"; // NEW: scene for the first store visit
    //public string level4SceneName = "Scene4-Shoes";
    //public string level5SceneName = "Scene5-Inventory";

    private InventoryManager inventoryManager;

#if UNITY_EDITOR
    // NEW: drag scene files here in the Inspector (editor only, so builds still work)
    //public UnityEditor.SceneAsset plantScene;   // first store visit -> Scene3-Plant
    //public UnityEditor.SceneAsset level4Scene;  // second store visit -> Scene4-Shoes
    //public UnityEditor.SceneAsset level5Scene;  // third store visit -> Scene5-Inventory

    private void OnValidate()
    {
        //if (plantScene != null) plantSceneName = plantScene.name;
        //if (level4Scene != null) level4SceneName = level4Scene.name;
    }
#endif

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
        fundText.GameObject().SetActive(false);
        upgradePanel.SetActive(false);
        storeMessage.text = "SUPER STORE\r\nWith the tutorial over, you now have full store access.";
        buttonHolder.SetActive(true);
        FarmFund farm = FarmFund.Get();
        farm.SpendMoney(bagPrice);
        InitInventory();
        if (farm.money <= 0)
            farm.money = 225; // Fallback if there is problems with "DontDestroyOnLoad"
        inventoryManager.AddItems(goldItem, farm.money);
        foreach (InventoryItemData tool in tools)
        {
            inventoryManager.AddItem(tool);
        }
        inventoryManager.AddItems(seedItem, farm.seeds);
        inventoryManager.AddItems(harvestItem, farm.harvestedCount);
        farm.seeds = 0;
        farm.harvestedCount = 0;
    }

    public void StartNextLevel()
    {
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