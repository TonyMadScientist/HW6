using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class HarvestManager : MonoBehaviour
// Control is H for harvesting, press near a plant
{
    public Transform player;          
    public TMP_Text harvestText;      
    //public Key harvestKey = Key.H;
    public float harvestRange = 1f;   

    private void Start()
    {
        UpdateText();
    }

    private void Update()
    {
        /*
        if (Keyboard.current != null && Keyboard.current[harvestKey].wasPressedThisFrame)
        {
            TryHarvest();
        }
        */
    }

    public bool TryHarvest()
    {
        // find the closest plant to the player
        Plant closest = null;
        float closestDist = harvestRange;

        foreach (Plant plant in FindObjectsByType<Plant>())
        {
            // compare only left right and forward back distance
            Vector3 a = plant.transform.position;
            Vector3 b = player.position;
            a.y = 0;
            b.y = 0;
            float dist = Vector3.Distance(a, b);

            if (dist < closestDist)
            {
                closest = plant;
                closestDist = dist;
            }
        }

        if (closest == null)
        {
            Debug.Log("No plant close enough to harvest.");
            return false;
        }

        if (!closest.IsMature())
        {
            Debug.Log("This plant isn't ready yet.");
            return false;
        }

        // remove the plant to count it
        Destroy(closest.gameObject);
        FarmFund.Get().harvestedCount++;
        UpdateText();
        Debug.Log("Harvested! Total: " + FarmFund.Get().harvestedCount);
        return true;
    }

    private void UpdateText()
    {
        harvestText.text = "Harvested: " + FarmFund.Get().harvestedCount;
    }
}
