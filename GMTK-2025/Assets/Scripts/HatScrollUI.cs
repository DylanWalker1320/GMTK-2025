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
    [SerializeField] private TextMeshProUGUI[] hatStatsTexts;
    [SerializeField] private UnityEvent unityEvent;
    private HatGenerator hatGenerator;
    private static GameObject hatObject;

    void Update()
    {
        if (hatPrizeUI.activeSelf && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("Apply Hat Stats");
            scrollUI.GetComponent<HatScroll>().ApplyPrizeHatStats();
            unityEvent.Invoke();
        }
        if(scrollUI.GetComponent<HatScroll>()._speed == 0 && scrollUI.GetComponent<HatScroll>()._hasScrolled == true) // Click
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
        Debug.Log("hat data received: " + hatData);
        if(hatData != null) // Toggle On
        {
            // Make a new hat object to display
            if (hatGenerator == null)
                hatGenerator = FindFirstObjectByType<HatGenerator>();
            
            hatObject = hatGenerator.GenerateHatWithStats(hatData);
            hatObject.transform.SetParent(HatSpriteImage.transform);
            hatObject.transform.localPosition = Vector3.zero;
            hatObject.GetComponent<HatComponentManager>().DisableShadow();

            hatNameText.text = hatData.hatName;
            hatRarityText.text = hatData.rarity.ToString();
            for (int i = 0; i < hatStatsTexts.Length; i++)
            {
                if (i < hatData.stats.Count)
                {
                    hatStatsTexts[i].text = hatData.stats[i].ToString();
                    hatStatsTexts[i].color = HatColors.GetStatTypeColor(hatData.stats[i].type);
                }
                else
                {
                    hatStatsTexts[i].text = "";
                }
            }

            // Colour the UI based on rarity
            Color rarityColor = HatColors.GetRarityColor(hatData.rarity);
            hatNameText.color = rarityColor;
            hatRarityText.color = rarityColor;
            HatSpriteImage.color = rarityColor;
            
        }
        hatPrizeUI.SetActive(!hatPrizeUI.activeSelf);
    }

    public static void DestroyTargetHat()
    {
        if (hatObject != null)
        {
            Debug.Log("Destroying hat object" + hatObject.name);
            Destroy(hatObject);
            hatObject = null;
        }
    }
}
