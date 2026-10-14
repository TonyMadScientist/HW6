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
    public string level4SceneName = "Scene4"; 

    private void Start()
    { 
        FarmFund.Get().storeVisits++;
        bool secondVisit = FarmFund.Get().storeVisits >= 2;
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
  
        if (FarmFund.Get().storeVisits >= 2)
            SceneManager.LoadScene(level4SceneName);
        else
            SceneManager.LoadScene("Scene3-Plant");
    }
    private void RefreshUI()
    {
        quantityText.text = quantity.ToString();
        totalText.text = "Total: $" + (seedPrice * quantity);
        fundText.text = "Fund: $" + FarmFund.Get().money;
    }
}