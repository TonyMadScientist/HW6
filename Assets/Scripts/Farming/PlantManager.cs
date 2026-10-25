using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public PlantData selectedPlantData;
    public Transform player;
    public DayNightControl dayNightControl;

    public InventoryItemData selectedSeedItem;

    private Dictionary<FarmTileControl, GameObject> plants =
        new Dictionary<FarmTileControl, GameObject>();

    public void SetSelectedPlant(PlantData plantData)
    {
        if (plantData == null)
        {
            Debug.LogError("The selected seed has no PlantData.");
            return;
        }

        selectedPlantData = plantData;

        Debug.Log(
            "Selected plant: " +
            selectedPlantData.plantName
        );
    }

    public bool TryPlant()
    {

       
        if (selectedPlantData == null)
        {
            Debug.LogError("No plant seed has been selected.");
            return false;
        }

        if (dayNightControl == null)
        {
            Debug.LogError("DayNightControl is not assigned.");
            return false;
        }

        DayNightControl.Season currentSeason =
            dayNightControl.GetCurrentSeason();

        if (!selectedPlantData.CanBePlantedInSeason(currentSeason))
        {
            Debug.Log(
                selectedPlantData.plantName +
                " cannot be planted during " +
                currentSeason
            );

            return false;
        }

        if (selectedPlantData.plantPrefab == null)
        {
            Debug.LogError(
                selectedPlantData.plantName +
                " has no plant prefab assigned."
            );

            return false;
        }

        if (player == null)
        {
            Debug.LogError("Player slot is empty.");
            return false;
        }

        RaycastHit[] hits = Physics.RaycastAll(
            player.position + Vector3.up * 2f,
            Vector3.down,
            6f
        );

        FarmTileControl tile = null;
        Collider tileCollider = null;

        foreach (RaycastHit hit in hits)
        {
            tile =
                hit.collider.GetComponentInParent<FarmTileControl>();

            if (tile != null)
            {
                tileCollider = hit.collider;
                break;
            }
        }

        if (tile == null)
        {
            Debug.Log("No farm tile found.");
            return false;
        }

        if (tile.tileCond != FarmTileControl.FarmTileCond.Watered)
        {
            Debug.Log("The tile must be watered.");
            return false;
        }

        if (plants.ContainsKey(tile) && plants[tile] != null)
        {
            Debug.Log("This tile already has a plant.");
            return false;
        }

        Vector3 position = tile.transform.position;
        position.y = tileCollider.bounds.max.y;

        GameObject newPlant = Instantiate(
            selectedPlantData.plantPrefab,
            position,
            Quaternion.identity,
            transform
        );

        Plant plantScript =
            newPlant.GetComponent<Plant>();

        if (plantScript != null)
        {
            plantScript.plantData = selectedPlantData;
        }

 

        Debug.Log(
            selectedPlantData.plantName +
            " planted during " +
            currentSeason
        );

        return true;
    }

    public void SetSelectedSeed(InventoryItemData seedItem)
    {
        if (seedItem == null)
        {
            Debug.LogError("Selected seed is null.");
            return;
        }

        if (seedItem.plantData == null)
        {
            Debug.LogError(
                seedItem.itemName +
                " has no PlantData assigned."
            );

            return;
        }

        selectedSeedItem = seedItem;
        selectedPlantData = seedItem.plantData;

        Debug.Log(
            "Selected seed: " +
            seedItem.itemName +
            " → " +
            selectedPlantData.plantName
        );
    }

}