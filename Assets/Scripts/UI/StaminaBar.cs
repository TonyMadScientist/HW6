using UnityEngine;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private PlayerStamina playerStamina;
    [SerializeField] private Transform fillBar;

    private Vector3 originalScale;

    private void Start()
    {
        if (playerStamina == null)
        {
            Debug.LogError("Player Stamina is not assigned.");
            return;
        }

        if (fillBar == null)
        {
            Debug.LogError("Fill Bar is not assigned.");
            return;
        }

        originalScale = fillBar.localScale;

        playerStamina.OnStaminaChanged += UpdateBar;
        UpdateBar(playerStamina.StaminaPercent);
    }

    private void OnDestroy()
    {
        if (playerStamina != null)
            playerStamina.OnStaminaChanged -= UpdateBar;
    }

    private void UpdateBar(float percentage)
    {
        percentage = Mathf.Clamp01(percentage);

        Vector3 newScale = originalScale;
        newScale.x = originalScale.x * percentage;

        fillBar.localScale = newScale;
    }
}