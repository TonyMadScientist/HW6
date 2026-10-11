using System;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float startingStamina = 100f;
    [SerializeField] private float regenerationDelay = 2f;
    [SerializeField] private float regenerationRate = 10f;

    public float CurrentStamina { get; private set; }
    public float MaxStamina => maxStamina;
    public float StaminaPercent => CurrentStamina / maxStamina;

    public event Action<float> OnStaminaChanged;

    private float timeSinceLastUse;

    private void Start()
    {
        CurrentStamina = Mathf.Clamp(startingStamina, 0f, maxStamina);
        OnStaminaChanged?.Invoke(StaminaPercent);
    }

    private void Update()
    {
        if (CurrentStamina >= maxStamina)
            return;

        timeSinceLastUse += Time.deltaTime;

        if (timeSinceLastUse >= regenerationDelay)
        {
            CurrentStamina += regenerationRate * Time.deltaTime;
            CurrentStamina = Mathf.Clamp(CurrentStamina, 0f, maxStamina);

            OnStaminaChanged?.Invoke(StaminaPercent);
        }
    }

    public bool TryUseStamina(float amount)
    {
        if (amount <= 0f)
            return true;

        if (CurrentStamina < amount)
            return false;

        CurrentStamina -= amount;
        timeSinceLastUse = 0f;

        OnStaminaChanged?.Invoke(StaminaPercent);
        return true;
    }

    public void RestoreStamina(float amount)
    {
        CurrentStamina = Mathf.Clamp(
            CurrentStamina + amount,
            0f,
            maxStamina
        );

        OnStaminaChanged?.Invoke(StaminaPercent);
    }
}