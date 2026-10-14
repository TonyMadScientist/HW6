using UnityEngine;
using UnityEngine.InputSystem;

public class FarmingAnimationController : MonoBehaviour
{
    private Animator animator;

    public GameObject farmingTool;

    private bool toolEquipped;
    private bool isUnequipping;

    private void Start()
    {
        animator = GetComponent<Animator>();

        toolEquipped = false;
        isUnequipping = false;

        animator.SetBool("isEquipped", false);

        if (farmingTool != null)
        {
            farmingTool.SetActive(false);
        }
    }

    private void Update()
    {
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
        if (toolEquipped || isUnequipping)
        {
            return;
        }

        toolEquipped = true;
        animator.SetBool("isEquipped", true);

        if (farmingTool != null)
        {
            farmingTool.SetActive(true);
        }
    }

    private void Plow()
    {
        if (!toolEquipped || isUnequipping)
        {
            return;
        }

        animator.SetTrigger("isPlowing");
    }

    private void Plant()
    {
        if (isUnequipping)
        {
            return;
        }

        animator.SetTrigger("isPlanting");
    }

    private void BeginUnequip()
    {
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
        // Do nothing when StopPlow is only being used between plows.
        if (!isUnequipping)
        {
            return;
        }

        toolEquipped = false;
        isUnequipping = false;

        if (farmingTool != null)
        {
            farmingTool.SetActive(false);
        }
    }
}