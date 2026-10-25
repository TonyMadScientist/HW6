using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolboxManager : MonoBehaviour
{
    public Key equipHoeKey = Key.Digit1;
    public Key equipWaterCanKey = Key.Digit2;
    public Key equipShovelKey = Key.Digit3;
    public Key interactKey = Key.Space;

    public bool allowShovel = true;

    public enum Tools
    {
        none,
        hoe,
        wateringcan,
        shovel
    }

    private Tools currentTool = Tools.none;

    private FarmingAnimationController animScript;

    public GameObject PlantManager;
    private PlantManager plantingScript;

    public GameObject playerInteractPoint;
    private PlayerInteractPoint tileScript;

    public GameObject harvestManager;
    private HarvestManager harvestScript;

    private InventoryManager inventoryScript;

    public InventoryItemData[] toolItems;

    public TMP_Text seedText;

    private void Awake()
    {
        if (FarmFund.Get().seeds > 0 && seedText != null)
        {
            seedText.text =
                "Seeds: " +
                FarmFund.Get().seeds;
        }
    }

    private void Start()
    {
        currentTool = Tools.none;

        animScript =
            GetComponentInChildren<FarmingAnimationController>();

        if (PlantManager != null)
        {
            plantingScript =
                PlantManager.GetComponent<PlantManager>();
        }

        tileScript =
            playerInteractPoint.GetComponent<PlayerInteractPoint>();

        if (harvestManager != null)
        {
            harvestScript =
                harvestManager.GetComponent<HarvestManager>();
        }

        if (InventoryManager.Active())
        {
            inventoryScript =
                InventoryManager.Get();
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current[interactKey].wasPressedThisFrame)
        {
            Interact();
            return;
        }

        if (Keyboard.current[equipHoeKey].wasPressedThisFrame)
        {
            if (currentTool != Tools.hoe)
            {
                Debug.Log("Equipping Hoe");

                currentTool = Tools.hoe;

                if (inventoryScript != null &&
                    toolItems.Length > 0)
                {
                    inventoryScript.SetTool(toolItems[0]);
                }
            }
            else
            {
                Debug.Log("Unequipping Hoe");

                currentTool = Tools.none;

                if (inventoryScript != null)
                {
                    inventoryScript.RemoveTool();
                }
            }

            SwitchTool();
            return;
        }

        if (Keyboard.current[equipWaterCanKey].wasPressedThisFrame)
        {
            if (currentTool != Tools.wateringcan)
            {
                Debug.Log("Equipping Watering Can");

                currentTool = Tools.wateringcan;

                if (inventoryScript != null &&
                    toolItems.Length > 1)
                {
                    inventoryScript.SetTool(toolItems[1]);
                }
            }
            else
            {
                Debug.Log("Unequipping Watering Can");

                currentTool = Tools.none;

                if (inventoryScript != null)
                {
                    inventoryScript.RemoveTool();
                }
            }

            SwitchTool();
            return;
        }

        if (Keyboard.current[equipShovelKey].wasPressedThisFrame)
        {
            if (!allowShovel)
            {
                Debug.Log("You do not have a Shovel yet!");
            }
            else if (currentTool != Tools.shovel)
            {
                Debug.Log("Equipping Shovel");

                currentTool = Tools.shovel;

                if (inventoryScript != null &&
                    toolItems.Length > 2)
                {
                    inventoryScript.SetTool(toolItems[2]);
                }
            }
            else
            {
                Debug.Log("Unequipping Shovel");

                currentTool = Tools.none;

                if (inventoryScript != null)
                {
                    inventoryScript.RemoveTool();
                }
            }

            SwitchTool();
            return;
        }
    }

    private void SwitchTool()
    {
        FarmingAnimationController.Tools currentAnimationTool =
            (FarmingAnimationController.Tools)currentTool;

        animScript.SwitchTool(currentAnimationTool);
    }

    public void Interact()
    {
        switch (currentTool)
        {
            case Tools.hoe:

                Debug.Log("Interacting with: Hoe");

                if (animScript.Plow())
                {
                    tileScript.Interact(currentTool);
                }

                break;

            case Tools.wateringcan:

                Debug.Log("Interacting with: Watering Can");

                if (animScript.Water())
                {
                    tileScript.Interact(currentTool);
                }

                break;

            case Tools.shovel:

                Debug.Log("Interacting with: Shovel");

                if (!animScript.Plant())
                    return;

                if (harvestManager == null)
                    return;

                // Try harvesting first
                bool harvested =
                    harvestScript.TryHarvest();

                if (harvested)
                    return;

                if (PlantManager == null ||
                    plantingScript == null)
                {
                    return;
                }

                // Scenes without InventoryManager
                if (inventoryScript == null)
                {
                    int currentSeeds =
                        FarmFund.Get().seeds;

                    if (currentSeeds > 0 &&
                        plantingScript.TryPlant())
                    {
                        FarmFund.Get().seeds =
                            currentSeeds - 1;

                        if (seedText != null)
                        {
                            seedText.text =
                                "Seeds: " +
                                FarmFund.Get().seeds;
                        }
                    }

                    return;
                }

                // Scenes with InventoryManager
                InventoryItemData selectedSeed =
                    plantingScript.selectedSeedItem;

                if (selectedSeed == null)
                {
                    Debug.Log(
                        "No seed is selected. " +
                        "Click a seed in the inventory first."
                    );

                    return;
                }

                if (selectedSeed.plantData == null)
                {
                    Debug.LogError(
                        selectedSeed.itemName +
                        " has no PlantData assigned."
                    );

                    return;
                }

                Debug.Log(
                    "Trying to plant using: " +
                    selectedSeed.itemName
                );

                // Only remove the exact selected seed
                if (inventoryScript.HasItem(
                        selectedSeed.itemName))
                {
                    bool planted =
                        plantingScript.TryPlant();

                    if (planted)
                    {
                        inventoryScript.RemoveItem(
                            selectedSeed.itemName
                        );

                        Debug.Log(
                            "Removed planted seed: " +
                            selectedSeed.itemName
                        );
                    }
                }
                else
                {
                    Debug.Log(
                        "The inventory does not contain: " +
                        selectedSeed.itemName
                    );
                }

                break;

            case Tools.none:

                Debug.Log(
                    "Use the 1, 2, or 3 keys to equip a tool first."
                );

                break;

            default:

                Debug.LogError(
                    "Trying to interact with an invalid tool."
                );

                break;
        }
    }
}