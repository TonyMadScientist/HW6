using UnityEngine;

public class SpriteHealthBar : MonoBehaviour
{
    [SerializeField] private Transform bar;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = bar.localScale;
    }

    public void SetHealth(float currentHealth, float maxHealth)
    {
        float percentage = Mathf.Clamp01(currentHealth / maxHealth);

        Vector3 newScale = originalScale;
        newScale.x = originalScale.x * percentage;

        bar.localScale = newScale;
    }
}