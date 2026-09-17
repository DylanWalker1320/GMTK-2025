using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSpellEntry : MonoBehaviour
{
    [Header("UI Properties")]
    [SerializeField] private Image spellIcon;
    [SerializeField] private Image spellBorder;
    [SerializeField] private TextMeshProUGUI damageText;

    [Header("Animation Tweening Values")]
    [SerializeField] private int totalDamage;
    [SerializeField] private float delayAnimationSpeed = 2f;
    [SerializeField] private int damageBuildUpNumber = 0;
    [SerializeField] private int damageTextTime = 5;


    public void Setup(Sprite sprite, int damage, Color color)
    {
        totalDamage = damage;
        spellIcon.sprite = sprite;
        spellBorder.color = color;

        transform.position = Vector3.zero;
        StartCoroutine(FromZeroToTotalDamageAnimation(damageBuildUpNumber, totalDamage, damageTextTime));
    }

    // For some reason if we don't use this the text won't update properly
    private IEnumerator FromZeroToTotalDamageAnimation(int start, int end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime * delayAnimationSpeed;

            float t = elapsed / duration;

            int currentValue = Mathf.RoundToInt(Mathf.Lerp(start, end, t));
            damageText.text = currentValue.ToString();

            yield return null;
        }

        damageText.text = end.ToString();
    }
}
