using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class StoreManager : MonoBehaviour
{
    public Button buySeedButton;
    public Button startNextLevelButton;

    // for text on screen
    public TMP_Text fundText;
    public TMP_Text priceText;
    public TMP_Text quantityText;
    public TMP_Text totalText;
    public TMP_Text storeMessage;

    public int seedPrice = 10;
    private int quantity = 1;

    public GameObject upgradePanel;   //  for upgrade 
    public Button buyShoesButton;     // for BuyShoes
    public int shoesPrice = 50;
    
    public string plantSceneName = "Scene3-Plant"; // NEW: scene for the first store visit
    public string level4SceneName = "Scene4-Shoes";
    public string level5SceneName = "Scene5-Inventory";

#if UNITY_EDITOR
    // NEW: drag scene files here in the Inspector (editor only, so builds still work)
    public UnityEditor.SceneAsset plantScene;   // first store visit -> Scene3-Plant
    public UnityEditor.SceneAsset level4Scene;  // second store visit -> Scene4-Shoes
    public UnityEditor.SceneAsset level5Scene;  // third store visit -> Scene5-Inventory

    private void OnValidate()
    {
        if (plantScene != null) plantSceneName = plantScene.name;
        if (level4Scene != null) level4SceneName = level4Scene.name;
    }
#endif

    private void Start()
    { 
        FarmFund.Get().storeVisits++;
        bool secondVisit = FarmFund.Get().storeVisits >= 2;
        if (upgradePanel != null)
            upgradePanel.SetActive(secondVisit);
        buyShoesButton.interactable = !FarmFund.Get().hasFastShoes; // prevent from buying twice

        // Can't leave until the player buy seeds
        startNextLevelButton.interactable = FarmFund.Get().seeds > 0;
        priceText.text = "Seed: $" + seedPrice + " each";
        RefreshUI();
    }

    public void IncreaseQuantity()

    {
        quantity++;
        RefreshUI();
    }

    public void DecreaseQuantity()

    {
        if (quantity > 1) quantity--;
        RefreshUI();
    }

    public void BuySeed()
    {
        int cost = seedPrice * quantity;

        if (FarmFund.Get().SpendMoney(cost)) // SpendMoney returns false if the player can't afford it  
        {
            FarmFund.Get().seeds += quantity;
            storeMessage.text = "Bought " + quantity + " seed(s)! You have " + FarmFund.Get().seeds + " seeds.";
            startNextLevelButton.interactable = true;
        }

        else
        {
            storeMessage.text = "Not enough money!";
        }

        RefreshUI();
    }
    public void BuyShoes()
    {
        if (FarmFund.Get().SpendMoney(shoesPrice))
        {
            FarmFund.Get().hasFastShoes = true;
            buyShoesButton.interactable = false; // one-time purchase
            storeMessage.text = "You bought SUPER DUPER Fast Shoes! You'll walk faster next level.";
        }
        else
        {
            storeMessage.text = "Not enough money for Fast Shoes!";
        }
        RefreshUI();
    }


    public void StartNextLevel()
    {

        switch (FarmFund.Get().storeVisits)
        {
            case 3:
                SceneManager.LoadScene(level5SceneName); // Go to scene 5 if this is the 3rd time shopping
                break;
            case 2:
                SceneManager.LoadScene(level4SceneName); // Scene 4 if shopped two times.
                break;
            case 1:
            default:
                SceneManager.LoadScene(plantSceneName); // CHANGED: was hardcoded "Scene3-Plant"
                break;
        }
    }
    private void RefreshUI()
    {
        quantityText.text = quantity.ToString();
        totalText.text = "Total: $" + (seedPrice * quantity);
        fundText.text = "Fund: $" + FarmFund.Get().money;
    }
}