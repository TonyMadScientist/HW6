using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public Button buySeedButton;
    public Button startNextLevelButton;

    private bool seedPurchased = false;

    private void Start()
    {
        // Player cannot start the next level until they buy a seed
        if (startNextLevelButton != null)
        {
            startNextLevelButton.interactable = false;
        }
    }

    public void BuySeed()
    {
        seedPurchased = true;

        Debug.Log("Seed purchased!");

        // Enable Start Next Level after buying the seed
        if (startNextLevelButton != null)
        {
            startNextLevelButton.interactable = true;
        }

        // Prevent buying the same seed repeatedly
        if (buySeedButton != null)
        {
            buySeedButton.interactable = false;
        }
    }

    public void StartNextLevel()
    {
        if (seedPurchased)
        {
            Debug.Log("Starting next level!");
            SceneManager.LoadScene("Scene3-Plant");

        }
    }
}