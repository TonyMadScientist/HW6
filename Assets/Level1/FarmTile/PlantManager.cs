using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlantManager : MonoBehaviour
{
    [Header("References")]
    public GameObject plantPrefab;
    public Transform player;

    [Header("Plant Input")]
    public Key plantKey = Key.P;

    [Header("Planting Ray")]
    public float distanceAhead = 1.5f;
    public float rayHeight = 2f;
    public float rayDistance = 6f;

    // Remembers which tiles already have a plant.
    private Dictionary<FarmTileControl, GameObject> plants =
        new Dictionary<FarmTileControl, GameObject>();

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current[plantKey].wasPressedThisFrame)
        {
            TryPlant();
        }
    }

    private Vector3 GetRayOrigin()
    {
        return player.position +
               player.forward * distanceAhead +
               Vector3.up * rayHeight;
    }

    private void TryPlant()
    {
        if (player == null)
        {
            Debug.LogError("Player slot is empty!");
            return;
        }

        if (plantPrefab == null)
        {
            Debug.LogError("Plant Prefab slot is empty!");
            return;
        }

        Vector3 rayOrigin = GetRayOrigin();

        RaycastHit[] hits = Physics.RaycastAll(
            rayOrigin,
            Vector3.down,
            rayDistance
        );

        // Remains visible for two seconds after pressing P.
        Debug.DrawRay(
            rayOrigin,
            Vector3.down * rayDistance,
            Color.red,
            2f
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
            Debug.Log(
                "No farm tile was found underneath the planting ray."
            );

            return;
        }

        // Only watered tiles can be planted on.
        if (tile.tileCond !=
            FarmTileControl.FarmTileCond.Watered)
        {
            Debug.Log("Can only plant on a watered tile.");
            return;
        }

        // Only one plant can exist on each tile.
        if (plants.ContainsKey(tile) &&
            plants[tile] != null)
        {
            Debug.Log("This tile already has a plant.");
            return;
        }

        // Spawn at the center and on top of the tile.
        Vector3 plantPosition = tile.transform.position;
        plantPosition.y = tileCollider.bounds.max.y;

        GameObject plant = Instantiate(
            plantPrefab,
            plantPosition,
            Quaternion.identity,
            transform
        );

        plants[tile] = plant;

        Debug.Log(
            "Plant created on " + tile.gameObject.name
        );
    }

    // Draws the planting ray in the Scene window.
    private void OnDrawGizmosSelected()
    {
        if (player == null)
        {
            return;
        }

        Vector3 rayOrigin = GetRayOrigin();

        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            rayOrigin,
            rayOrigin + Vector3.down * rayDistance
        );

        // Shows where the ray starts.
        Gizmos.DrawSphere(rayOrigin, 0.1f);
    }
}