using UnityEngine;
using UnityEngine.SceneManagement;

public class FarmFund : MonoBehaviour // Player fund and purchases to be live across all scene changes 
{
    public static FarmFund Instance;

    public int money = 0;         
    public int seeds = 0;           
    public int harvestedCount = 0;
    public int storeVisits = 0;
    // upgrades bought in the store
    public bool hasFastShoes = false;
   // public bool hasNewSeed = false; // just a bonus item if you do want to added, we woud just need a new looking plant or we can take it out and keep it simple ;p 

    private void Awake()
    
    {
        // only keep one Farm Fund 
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // Set debug data if we start the game from a specific stage instead of from the start of Scene0
        switch (SceneManager.GetActiveScene().buildIndex) {
            case 3: // If we directly load into scene 3 (Plant)
                seeds = 10;
                break;
            case 4: // Loading into Scenes 4 (Shoes)
                seeds = 16;
                hasFastShoes = true;
                break;
            case 5: // Scene 5 (Inventory)
                hasFastShoes = true;
                GameObject inventoryObj = GameObject.Find("InventoryManager");
                InventoryManager inventoryScript;
                if (inventoryObj != null)
                {
                    inventoryScript = inventoryObj.GetComponent<InventoryManager>();
                }
                break;
            case 2: // Scene 2 (shop)
                money = 100;
                storeVisits = 2; // Makes the shoes appear when loading into scene 2, skipping scenes 1 and 3.
                break;
        }
    }

    // for FarmFund Get() it fixes crashes if there's no wallet added yet, get end up creates one on the spot


    public static FarmFund Get()
    {
        if (Instance == null)
        {
            new GameObject("Farm Fund").AddComponent<FarmFund>();
        }
        return Instance;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log("Fund +" + amount + " -> $" + money);
    }

    // returns false if the player can't afford it
    public bool SpendMoney(int amount)
    {
        if (amount > money) return false;
        money -= amount;
        Debug.Log("Fund -" + amount + " -> $" + money);
        return true;
    }
}