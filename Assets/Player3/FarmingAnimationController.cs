using UnityEngine;
using UnityEngine.InputSystem;

public class FarmingAnimationController : MonoBehaviour
{
    private Animator animator;

    public GameObject farmingToolbox;
    public GameObject toolHoe;
    public GameObject toolWaterCan;
    public GameObject toolShovel;

    public enum Tools
    {
        none, // 0
        hoe, // 1
        wateringcan, // 2
        shovel // 3
    }
    public Tools currentTool = Tools.none;

    private bool toolEquipped;
    private bool isUnequipping;

    private void Start()
    {
        animator = GetComponent<Animator>();

        //toolEquipped = false;
        //isUnequipping = false;
        //animator.SetBool("isEquipped", false);
        currentTool = Tools.none;

        if ((farmingToolbox == null) | (toolHoe == null) | (toolWaterCan == null))
        {
            Debug.LogError("ERROR! Must set GameObjects in FarmingAnimationController!");
        } else {
            SwitchTool();
        }
    }

    public void SwitchTool()
    {
        toolHoe.SetActive(false);
        animator.SetBool("equipHoe", false);
        toolWaterCan.SetActive(false);
        animator.SetBool("equipWaterCan", false);
        toolShovel.SetActive(false);
        animator.SetBool("equipShovel", false);
        switch (currentTool)
        {
            case Tools.hoe:
                farmingToolbox.SetActive(true);
                toolHoe.SetActive(true);
                animator.SetBool("equipHoe", true);
                break;
            case Tools.wateringcan:
                farmingToolbox.SetActive(true);
                toolWaterCan.SetActive(true);
                animator.SetBool("equipWaterCan", true);
                break;
            case Tools.shovel:
                farmingToolbox.SetActive(true);
                toolShovel.SetActive(true);
                animator.SetBool("equipShovel", true);
                break;
            case Tools.none:
            default:
                farmingToolbox.SetActive(false);
                break;
        }
    }
    public void SwitchTool(Tools toolenum)
    {
        currentTool = toolenum;
        SwitchTool();
    }
    public void SwitchTool(string str)
    {
        switch (str)
        {
            case "hoe":
                currentTool = Tools.hoe;
                break;
            case "wateringcan":
                currentTool = Tools.wateringcan;
                break;
            case "shovel":
                currentTool = Tools.shovel;
                break;
            default:
                currentTool = Tools.none;
                break;
        }
        SwitchTool();
    }

    public Tools GetTool()
    {
        return currentTool;
    }
    public bool Plow()
    {
        if (currentTool == Tools.hoe)
        {
            animator.SetTrigger("isPlowing");
            return true;
        }
        else
            return false;
    }
    public bool Water()
    {
        if (currentTool == Tools.wateringcan)
        {
            animator.SetTrigger("isWatering");
            return true;
        }
        else
            return false;
    }

    public bool Plant()
    {
        if (currentTool == Tools.shovel)
        {
            animator.SetTrigger("isPlanting");
            return true;
        }
        else
            return false;
    }
/*
    private void Update()
    {
        return;
        if (Keyboard.current == null)
        {
            return;
        }

        // E equips or begins unequipping.
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!toolEquipped)
            {
                EquipTool();
            }
            else
            {
                BeginUnequip();
            }
        }

        // Space performs one plow.
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Plow();
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            Plant();
        }
    }

    private void EquipTool()
    {
        return;
        if (toolEquipped || isUnequipping)
        {
            return;
        }

        toolEquipped = true;
        animator.SetBool("isEquipped", true);

        if (farmingToolbox != null)
        {
            farmingToolbox.SetActive(true);
        }
    }

    private void BeginUnequip()
    {
        return;
        if (!toolEquipped || isUnequipping)
        {
            return;
        }

        isUnequipping = true;
        animator.SetBool("isEquipped", false);
    }

    // Called by an Animation Event at the end of the Stop clip.
    public void FinishUnequip()
    {
        Debug.LogError("FinishUnequip() Was Triggered in FarmingAnimController!");
        return;
        // Do nothing when StopPlow is only being used between plows.
        if (!isUnequipping)
        {
            return;
        }

        toolEquipped = false;
        isUnequipping = false;

        if (farmingToolbox != null)
        {
            farmingToolbox.SetActive(false);
        }
    }
*/
}