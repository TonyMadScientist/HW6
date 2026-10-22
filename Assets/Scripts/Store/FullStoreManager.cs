using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
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
    public GameObject inventoryManagerPrefab;


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
        FarmFund.Get().storeVisits++;
        // Can't leave until the player buys seeds
        startNextLevelButton.interactable = false;

        if (!InventoryManager.Active())
        {
            GameObject.Instantiate(inventoryManagerPrefab);
        }
        inventoryManager = InventoryManager.Get();

        if (FarmFund.Get().storeVisits < 3) // If this is the first time entering the full store.
        {
            fundText.GameObject().SetActive(true);
            upgradePanel.SetActive(true);
            storeMessage.text = "STORE\r\nPurchase the bag to access the full store feature.";
            buttonHolder.SetActive(false);
            //inventoryManager.GameObject().SetActive(false);
        }
        RefreshUI();
    }

    public void BuyingBag(int bagPrice)
    {
        fundText.GameObject().SetActive(false);
        upgradePanel.SetActive(false);
        storeMessage.text = "SUPER STORE\r\nWith the tutorial over, you now have full store access.";
        buttonHolder.SetActive(true);
        FarmFund farm = FarmFund.Get();
        farm.SpendMoney(bagPrice);
        inventoryManager.GameObject().SetActive(true);
        if (farm.money <= 0)
            farm.money = 225; // Fallback if there is problems with "DontDestroyOnLoad"
        inventoryManager.AddItems(goldItem, farm.money);
    }

    public void StartNextLevel()
    {

        switch (FarmFund.Get().storeVisits)
        {
            default:
                SceneManager.LoadScene(5); // CHANGE THIS. ALWAYS GOES TO SCENE 5
                break;
        }
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
                Debug.Log("buy click");
                if (inventoryManager.RemoveItems(goldItem.itemName, itemData.buyPrice))
                    inventoryManager.AddItem(itemData);
                break;
        }

    }
}