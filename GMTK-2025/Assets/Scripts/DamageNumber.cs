using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private float lifetime = 0.5f;
    [SerializeField] private float fadeStartTime = 0.3f;
    [SerializeField] private float moveDistance = 1f;
    [SerializeField] private float damageSizeDivisor = 100f; // Divisor to scale damage to size, can be adjusted as needed
    [SerializeField] private float minFontSize = 0.4f; // Minimum font size for damage numbers
    [SerializeField] private float maxFontSize = 1f; // Maximum font size for damage numbers
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float elapsedTime = 0f;
    
    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + Vector3.up * moveDistance;
    }
    
    void Update()
    {
        elapsedTime += Time.deltaTime;
        
        // Move upward
        float moveProgress = elapsedTime / lifetime;
        transform.position = Vector3.Lerp(startPosition, targetPosition, moveProgress);
        
        // Handle fading
        if (elapsedTime >= fadeStartTime)
        {
            float fadeProgress = (elapsedTime - fadeStartTime) / (lifetime - fadeStartTime);
            Color textColor = damageText.color;
            textColor.a = 1f - fadeProgress;
            damageText.color = textColor;
        }
        
        // Destroy when lifetime is up
        if (elapsedTime >= lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    public void SetDamageAmount(float damage)
    {
        if (damageText != null)
        {
            damageText.text = damage.ToString("F0");
        }
    }

    public void SetDamageObjectSize(float damage)
    {
        if (damageText != null)
        {
            float size = Mathf.Clamp(damage / damageSizeDivisor, minFontSize, maxFontSize); // Adjust the divisor and clamp values as needed
            damageText.fontSize = size; 
        }
    }
    
    public void SetColor(Color color)
    {
        if (damageText != null)
        {
            damageText.color = color;
        }
    }
} 