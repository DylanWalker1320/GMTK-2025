using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class HatScrollUI : MonoBehaviour
{
    [SerializeField] private GameObject scrollUI;
    [SerializeField] private GameObject scrollButton;
    [SerializeField] private GameObject hatPrizeUI;
    [SerializeField] private TextMeshProUGUI hatRarityText;
    [SerializeField] private TextMeshProUGUI hatNameText;
    [SerializeField] private Image HatSpriteImage;
    [SerializeField] private TextMeshProUGUI hatStatsText;
    [SerializeField] private UnityEvent unityEvent;

    void Update()
    {
        if (hatPrizeUI.activeSelf && Input.GetKeyDown(KeyCode.F))
        {
            unityEvent.Invoke();
        }
        if(scrollUI.GetComponent<HatScroll>()._speed == 0 && scrollUI.GetComponent<HatScroll>()._hasScrolled == true)
        {
            scrollUI.GetComponent<HatScroll>()._hasScrolled = false;
            ToggleScrollUI();
            ToggleHatPrize(scrollUI.GetComponent<HatScroll>().GetTargetHatData());
        }
    }

    public void ToggleScrollUI()
    {
        scrollUI.SetActive(!scrollUI.activeSelf);
        scrollButton.SetActive(!scrollButton.activeSelf);
    }

    public void ToggleHatPrize(GeneratedHat hatData)
    {
        if(hatData != null)
        {
            hatNameText.text = hatData.hatName;
            HatSpriteImage.sprite = hatData.hatSprite;
            hatRarityText.text = hatData.rarity.ToString();
            hatStatsText.text = hatData.PrintStatsOnly();
        }
        hatPrizeUI.SetActive(!hatPrizeUI.activeSelf);
    }
}
