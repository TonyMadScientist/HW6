using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlantManager : MonoBehaviour
{
    public GameObject plantPrefab;
    public Transform player;
    //public Key plantKey = Key.P;

    public DayNightControl dayNightControl;

    // Remembers which tiles already have a plant
    private Dictionary<FarmTileControl, GameObject> plants =
        new Dictionary<FarmTileControl, GameObject>();

    private void Update()
    {
        /*
        if (Keyboard.current != null && Keyboard.current[plantKey].wasPressedThisFrame)
        {
            TryPlant();
        }
        */
    }

    public bool TryPlant()
    {
        if (dayNightControl != null && dayNightControl.GetCurrentSeason() == DayNightControl.Season.Winter)
            {
                Debug.Log(
                    "Cannot plant during Winter."
                );

                return false;
            }

        if (player == null) { Debug.LogError("Player slot is empty!"); return false; }
        if (plantPrefab == null) { Debug.LogError("Plant Prefab slot is empty!"); return false; }

        // Ray straight down from above the player; RaycastAll so the
        // player's own collider doesn't block finding the tile
        RaycastHit[] hits = Physics.RaycastAll(player.position + Vector3.up * 2f, Vector3.down, 6f);

        FarmTileControl tile = null;
        Collider tileCollider = null;
        foreach (RaycastHit hit in hits)
        {
            tile = hit.collider.GetComponentInParent<FarmTileControl>();
            if (tile != null) { tileCollider = hit.collider; break; }
        }
        if (tile == null) return false;

        // Only watered tiles can be planted on
        if (tile.tileCond != FarmTileControl.FarmTileCond.Watered)
        {
            Debug.Log("Can only plant on a watered tile.");
            return false;
        }

        // One plant per tile
        if (plants.ContainsKey(tile) && plants[tile] != null)
        {
            Debug.Log("This tile already has a plant.");
            return false;
        }

        // Spawn on the top surface of the tile
        Vector3 pos = tile.transform.position;
        pos.y = tileCollider.bounds.max.y;

        Debug.Log("PLANTING on frame " + Time.frameCount);
          //" | P held: " + Keyboard.current[Key.P].isPressed
          //" | plantKey: " + plantKey + "\n" + System.Environment.StackTrace);
          
        GameObject plant = Instantiate(plantPrefab, pos, Quaternion.identity, transform);
        plants[tile] = plant;
        return true;
    }
}