using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractPoint : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStamina playerStamina;

    [Header("Action Costs")]
    [SerializeField] private float farmTileStaminaCost = 5f;

    private GameObject curTile;
    private InputAction interactAction;

    private void Start()
    {
        interactAction = InputSystem.actions.FindAction("Jump");

        if (interactAction != null)
            interactAction.performed += OnTileInteractPerformed;
    }

    private void OnDestroy()
    {
        if (interactAction != null)
            interactAction.performed -= OnTileInteractPerformed;
    }

    private void OnTileInteractPerformed(InputAction.CallbackContext context)
    {
        if (curTile == null)
            return;

        if (playerStamina == null)
        {
            Debug.LogError("PlayerStamina has not been assigned.");
            return;
        }

        if (!playerStamina.TryUseStamina(farmTileStaminaCost))
        {
            Debug.Log("Not enough stamina.");
            return;
        }

        Debug.Log("Space is pressed.");

        FarmTileControl farmTile = curTile.GetComponent<FarmTileControl>();

        if (farmTile != null)
            farmTile.InteractWithFarmTile();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Farm Tile"))
            return;

        FarmTileControl farmTile = other.GetComponent<FarmTileControl>();

        if (farmTile == null)
            return;

        Debug.Log("Player is interacting with Tile " + other.name);

        farmTile.EnterTile();
        curTile = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Farm Tile"))
            return;

        FarmTileControl farmTile = other.GetComponent<FarmTileControl>();

        if (farmTile != null)
            farmTile.ExitTile();

        if (curTile == other.gameObject)
            curTile = null;
    }
}