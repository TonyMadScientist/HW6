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
    public bool enableTestingCanvas = false;

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
                storeVisits = 1;
                break;
            case 4: // Loading into Scenes 4 (Shoes)
                seeds = 18;
                hasFastShoes = true;
                storeVisits = 2;
                break;
            case 5: // Scene 5 (Inventory)
                hasFastShoes = true;
                enableTestingCanvas = true;
                storeVisits = 3;
                break;
            case 6: // Scene 6 (NewStore)
                hasFastShoes = true;
                money = 350;
                harvestedCount = 16;
                seeds = 10;
                storeVisits = 2;
                break;
            case 2: // Scene 2 (shop)
                money = 250;
                storeVisits = 1; // Makes the shoes appear when loading into scene 2, skipping scenes 1 and 3.
                break;
            case 0:
            case 1:
                enableTestingCanvas = false;
                break;
            default:
                enableTestingCanvas = true;
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

    public void NextLevel()
    {
        switch (SceneManager.GetActiveScene().buildIndex) {
            case 0:
                SceneManager.LoadScene(2);
                break;
            case 1:
            case 3:
                SceneManager.LoadScene(2); // Go to first store after levels 1 and 3
                break;                
            case 6:
                int sceneCount = SceneManager.sceneCountInBuildSettings;
                if (storeVisits == 3 || sceneCount == 7)
                    SceneManager.LoadScene(5); // Scene6-FullStore uniquely comes after Scene5-Inventory, cycles if Scene7 doesn't exist
                else {
                    int destinationScene = storeVisits + 3; // All future level loads follow this formula
                    if (destinationScene < sceneCount)
                        SceneManager.LoadScene(destinationScene); // Go to the next level, 3 more than the number of store visits.
                    else
                        SceneManager.LoadScene(sceneCount - 1); // Loop the last scene infinitely
                }
                break;
            default: // Scenes 4, 5
                SceneManager.LoadScene(6); // Go to the new store that uses the inventory
                break;
        }
    } 
    // 1>3, 2>4, 3>5, 4>7 (has 8), 5>8, 6>9
}