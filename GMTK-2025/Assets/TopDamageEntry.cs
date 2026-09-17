using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopDamageEntry : MonoBehaviour
{
    [Header("UI Properties")]
    [SerializeField] private Image spellIcon;
    [SerializeField] private Slider damageDoneSlider;
    [SerializeField] private Image sliderFillColor;
    [SerializeField] private TextMeshProUGUI damageText;
    [Header("Animation Tweening Values")]
    [SerializeField] private int totalDamage;
    [SerializeField] private float targetSliderValue;
    [SerializeField] private float delayAnimationSpeed;
    [SerializeField] private float animationSpeed = 5f;
    [SerializeField] private int damageBuildUpNumber = 0;

    private void Update()
    {
        if(damageDoneSlider.value < targetSliderValue)
        {
            damageDoneSlider.value = Mathf.Lerp(damageDoneSlider.value, targetSliderValue, Time.unscaledDeltaTime * delayAnimationSpeed);
        }
    }

    public void Setup(Sprite sprite, int damage, float overallDamagePercentage, Color color, int rank)
    {
        spellIcon.sprite = sprite;
        totalDamage = damage;
        sliderFillColor.color = color;
        targetSliderValue = overallDamagePercentage;
        delayAnimationSpeed = animationSpeed - rank; // rank 1 -> 5 - 0, rank 2 -> 5 - 1, ... , rank 5 -> 5 - 4.

        transform.position = Vector3.zero;
        StartCoroutine(FromZeroToTotalDamageAnimation(damageBuildUpNumber, totalDamage, rank + 1));
    }

    // For some reason if we don't use this the text won't update properly
    private IEnumerator FromZeroToTotalDamageAnimation(int start, int end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = elapsed / duration;

            int currentValue = Mathf.RoundToInt(Mathf.Lerp(start, end, t));
            damageText.text = currentValue.ToString();

            yield return null;
        }

        damageText.text = end.ToString();
    }

}
