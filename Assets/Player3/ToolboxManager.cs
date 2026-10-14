using UnityEngine;
using UnityEngine.InputSystem;

public class ToolboxManager : MonoBehaviour
{
    public Key equipHoeKey = Key.Digit1;
    public Key equipWaterCanKey = Key.Digit2;
    public Key equipShovelKey = Key.Digit3;
    public Key interactKey = Key.Space;

    public enum Tools
    {
        none, // 0
        hoe, // 1
        wateringcan, // 2
        shovel // 3
    }
    public Tools currentTool = Tools.none;

    private FarmingAnimationController animScript;
    public GameObject PlantManager;
    private PlantManager plantingScript;
    public GameObject playerInteractPoint;
    private PlayerInteractPoint tileScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        currentTool = Tools.none;
        // Finds the component with this script in our children objects
        animScript = GetComponentInChildren<FarmingAnimationController>();
        // Gets the script for these gameobjects, needed to handle tilling, watering and planting
        plantingScript = PlantManager.GetComponent<PlantManager>();
        tileScript = playerInteractPoint.GetComponent<PlayerInteractPoint>();
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
                } else {
                    Debug.Log("Unquipping Hoe");
                    currentTool = Tools.none; // Unequip Hoe if we have it equipped already
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
                } else {
                    Debug.Log("Unequipping Watering Can");
                    currentTool = Tools.none; // Unequip Water Can if we have it equipped already
                }
                SwitchTool();
                return;
            }
            else if (Keyboard.current[equipShovelKey].wasPressedThisFrame)
            {
                if (currentTool != Tools.shovel)
                {
                    Debug.Log("Equipping Shovel");
                    currentTool = Tools.shovel; // Equip Shovel if we have something else (or nothing)
                } else {
                    Debug.Log("Unequipping Shovel");
                    currentTool = Tools.none; // Unequip Shovel if we have it equipped already
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
                if (animScript.Plant())
                    plantingScript.TryPlant();
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
