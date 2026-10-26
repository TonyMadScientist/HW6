using TMPro;
using UnityEngine;

public class HarvestManager : MonoBehaviour
{
    public Transform player;
    public TMP_Text harvestText;
    public float harvestRange = 1f;

    private InventoryManager inventoryScript;

    private void Start()
    {
        FarmFund.Get().harvestedCount = 0;

        UpdateText();

        if (InventoryManager.Active())
        {
            inventoryScript = InventoryManager.Get();
        }
    }

    public bool TryHarvest()
    {
        Plant closest = null;
        float closestDist = harvestRange;

        foreach (Plant plant in FindObjectsByType<Plant>())
        {
            Vector3 plantPosition = plant.transform.position;
            Vector3 playerPosition = player.position;

            plantPosition.y = 0f;
            playerPosition.y = 0f;

            float distance =
                Vector3.Distance(
                    plantPosition,
                    playerPosition
                );

            if (distance < closestDist)
            {
                closest = plant;
                closestDist = distance;
            }
        }

        if (closest == null)
        {
            Debug.Log("No plant close enough to harvest.");
            return false;
        }

        if (!closest.IsMature())
        {
            Debug.Log("This plant is not mature.");
            return false;
        }

        if (closest.plantData == null)
        {
            Debug.LogError(
                "The plant has no PlantData assigned."
            );

            return false;
        }

        InventoryItemData harvestItem =
            closest.plantData.harvestItem;

        if (inventoryScript == null &&
            InventoryManager.Active())
        {
            inventoryScript = InventoryManager.Get();
        }

        if (inventoryScript != null)
        {
            if (harvestItem == null)
            {
                Debug.LogError(
                    closest.plantData.plantName +
                    " has no harvest item assigned."
                );

                return false;
            }

            inventoryScript.AddItem(harvestItem);

            Debug.Log(
                "Harvested " +
                closest.plantData.plantName +
                " and added " +
                harvestItem.itemName
            );
        }
        else
        {
            FarmFund.Get().harvestedCount++;
            UpdateText();

            Debug.Log(
                "Harvested! Total: " +
                FarmFund.Get().harvestedCount
            );
        }

        Destroy(closest.gameObject);

        return true;
    }

    private void UpdateText()
    {
        if (harvestText != null)
        {
            harvestText.text =
                "Harvested: " +
                FarmFund.Get().harvestedCount;
        }
    }
}