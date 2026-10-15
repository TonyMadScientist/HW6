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
        if (SceneManager.GetActiveScene().buildIndex == 3) // If we directly load into scene 3
        {
            seeds = 10;
        } else if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            seeds = 16;
        } else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            money = 100;
            //storeVisits = 2; // Makes the shoes appear when loading into scene 2, skipping scenes 1 and 3.
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