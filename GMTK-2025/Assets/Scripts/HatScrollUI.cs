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
    private HatGenerator hatGenerator;
    private GameObject hatObject;

    void Update()
    {
        if (hatPrizeUI.activeSelf && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Apply Hat Stats");
            scrollUI.GetComponent<HatScroll>().ApplyPrizeHatStats();
            unityEvent.Invoke();
        }
        if(scrollUI.GetComponent<HatScroll>()._speed == 0 && scrollUI.GetComponent<HatScroll>()._hasScrolled == true)
        {
            scrollUI.GetComponent<HatScroll>()._hasScrolled = false;
            ToggleScrollUI();
            ToggleHatPrize(scrollUI.GetComponent<HatScroll>().GetTargetHatData());
        }
    }

    public void ToggleScrollUI(bool newInitialization = false)
    {
        if (newInitialization)
        {
            scrollUI.GetComponent<HatScroll>().Initialize();
        }
        hatPrizeUI.SetActive(false);
        scrollUI.SetActive(!scrollUI.activeSelf);
        scrollButton.SetActive(!scrollButton.activeSelf);
        
    }

    public void ToggleHatPrize(GeneratedHat hatData)
    {
        if(hatData != null) // Toggle On
        {
            // Make a new hat object to display
            if (hatGenerator == null)
                hatGenerator = FindFirstObjectByType<HatGenerator>();
            
            GameObject hatObject = hatGenerator.GenerateHatWithStats(hatData);
            hatObject.transform.SetParent(HatSpriteImage.transform);
            hatObject.transform.localPosition = Vector3.zero;

            hatNameText.text = hatData.hatName;
            hatRarityText.text = hatData.rarity.ToString();
            hatStatsText.text = hatData.PrintStatsOnly();
        } else // Toggle Off
        {
            Destroy(hatObject);
            hatObject = null;
        }
        hatPrizeUI.SetActive(!hatPrizeUI.activeSelf);
    }
}
