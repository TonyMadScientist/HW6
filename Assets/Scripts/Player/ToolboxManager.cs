using System;
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
        none, // 0
        hoe, // 1
        wateringcan, // 2
        shovel // 3
    }
    private Tools currentTool = Tools.none;

    private FarmingAnimationController animScript;
    public GameObject PlantManager;
    private PlantManager plantingScript;

    public GameObject playerInteractPoint;
    private PlayerInteractPoint tileScript;

    public GameObject harvestManager;
    private HarvestManager harvestScript;

    public GameObject inventoryObj;
    private InventoryManager inventoryScript;

    public InventoryItemData[] toolItems;//stores a list of items' scriptable objects that can be added to inventory

    public TMP_Text seedText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        currentTool = Tools.none;
        // Finds the component with this script in our children objects
        animScript = GetComponentInChildren<FarmingAnimationController>();
        // Gets the script for these gameobjects, needed to handle tilling, watering and planting
        if (PlantManager != null)
            plantingScript = PlantManager.GetComponent<PlantManager>();
        tileScript = playerInteractPoint.GetComponent<PlayerInteractPoint>();
        if (harvestManager != null)
            harvestScript = harvestManager.GetComponent<HarvestManager>();
        inventoryObj = GameObject.Find("InventoryManager");
        if (inventoryObj != null)
            inventoryScript = inventoryObj.GetComponent<InventoryManager>();
    }

    private void Awake()
    {
        if ((FarmFund.Get().seeds > 0) && (seedText != null))
        {
            seedText.text = "Seeds: " + FarmFund.Get().seeds;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current[interactKey].wasPressedThisFrame)
            {
                Interact();
                return; // These return's make it so we don't process any other key presses this frame;
            }
            else if (Keyboard.current[equipHoeKey].wasPressedThisFrame)
            {
                if (currentTool != Tools.hoe)
                {
                    Debug.Log("Equipping Hoe");
                    currentTool = Tools.hoe; // Equip Hoe if we have something else (or nothing)
                    if ((inventoryScript != null) && (toolItems.Length > 0)) // We are using InventoryManager
                        inventoryScript.SetTool(toolItems[0]);
                } else {
                    Debug.Log("Unquipping Hoe");
                    currentTool = Tools.none; // Unequip Hoe if we have it equipped already
                    if (inventoryScript != null) // We are using InventoryManager
                        inventoryScript.RemoveTool();
                }
                    
                SwitchTool();
                return;
            }
            else if (Keyboard.current[equipWaterCanKey].wasPressedThisFrame)
            {
                if (currentTool != Tools.wateringcan)
                {
                    Debug.Log("Equipping Watering Can");
                    currentTool = Tools.wateringcan; // Equip Water Can if we have something else (or nothing)
                    if ((inventoryScript != null) && (toolItems.Length > 0)) // We are using InventoryManager
                        inventoryScript.SetTool(toolItems[1]);
                } else {
                    Debug.Log("Unequipping Watering Can");
                    currentTool = Tools.none; // Unequip Water Can if we have it equipped already
                    if (inventoryScript != null) // We are using InventoryManager
                        inventoryScript.RemoveTool();
                }
                SwitchTool();
                return;
            }
            else if (Keyboard.current[equipShovelKey].wasPressedThisFrame)
            {
                if (!allowShovel)
                {
                    Debug.Log("You don't have a Shovel yet!");
                }
                else if (currentTool != Tools.shovel)
                {
                    Debug.Log("Equipping Shovel");
                    currentTool = Tools.shovel; // Equip Shovel if we have something else (or nothing)
                    if ((inventoryScript != null) && (toolItems.Length > 0)) // We are using InventoryManager
                        inventoryScript.SetTool(toolItems[2]);
                } else {
                    Debug.Log("Unequipping Shovel");
                    currentTool = Tools.none; // Unequip Shovel if we have it equipped already
                    if (inventoryScript != null) // We are using InventoryManager
                        inventoryScript.RemoveTool();
                }
                SwitchTool();
                return;
            }
        }
    }

    // Tells our animation script to also switch its tool
    private void SwitchTool()
    {
        // Cast our enum into the animation script's enum;
        FarmingAnimationController.Tools currTool = (FarmingAnimationController.Tools)currentTool;
        // Tell our animator script to switch the tool on their end (what we are holding);
        animScript.SwitchTool(currTool);
    }

    // When interact key is pressed;
    public void Interact()
    {
        switch (currentTool)
        {
            case Tools.hoe:
                Debug.Log("Interacting with: Hoe");
                if (animScript.Plow())
                    tileScript.Interact(currentTool);
                break;
            case Tools.wateringcan:
                Debug.Log("Interacting with: Watering Can");
                if (animScript.Water())
                    tileScript.Interact(currentTool);
                break;
            case Tools.shovel:
                Debug.Log("Interacting with: Shovel");
                if (animScript.Plant() & (harvestManager != null))
                    if (!harvestScript.TryHarvest() & (PlantManager != null)) // Try to harvest first
                    { // There is a PlantManager and we didn't harvest anything this square
                        if (inventoryScript == null) // If we are in a scene WITHOUT InventoryManager
                        {
                            int seeds = FarmFund.Get().seeds;
                            if ((seeds > 0) && plantingScript.TryPlant()) // Try to plant if it says you can't harvest here (i.e. no plant there).
                            {
                                FarmFund.Get().seeds = --seeds;
                                seedText.text = "Seeds: " + FarmFund.Get().seeds;
                            }
                        } else { // We are in a scene WITH InventoryManager, so use that instead for seeds.
                            if (inventoryScript.HasItem("BerrySeed")) // Check for a seed but don't remove yet
                            {
                                // Check if we are able to plant, if the plant is made THEN remove a seed
                                if (plantingScript.TryPlant())
                                {
                                    inventoryScript.RemoveItem("BerrySeed");
                                }
                            }
                        }
                    }
                break;
            case Tools.none:
                Debug.Log("Use the 1 2 3 Keys to equip a tool first!");
                break;
            default:
                Debug.LogError("Trying to interact with an invalid tool!");
                break; // Do nothing if we have an invalid or none tool;
        }
    }

}
