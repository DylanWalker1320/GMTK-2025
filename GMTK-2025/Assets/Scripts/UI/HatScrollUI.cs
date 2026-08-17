using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections;

public class HatScrollUI : MonoBehaviour
{
    [SerializeField] private GameObject scrollUI;
    [SerializeField] private GameObject scrollButton;
    [SerializeField] private GameObject hatPrizeUI;
    [SerializeField] private TextMeshProUGUI hatRarityText;
    [SerializeField] private TextMeshProUGUI hatRarityShade;
    [SerializeField] private TextMeshProUGUI hatNameText;
    [SerializeField] private TextMeshProUGUI hatNameShade;
    [SerializeField] private Image HatSpriteImage;
    [SerializeField] private TextMeshProUGUI[] hatStatsTexts;
    [SerializeField] private TextMeshProUGUI[] hatStatShades;
    [SerializeField] private UnityEvent unityEvent;
    
    [Header("Prize Hat Visuals")]
    [SerializeField] private GameObject prizeHatFront;
    [SerializeField] private GameObject prizeHatBack;
    [SerializeField] private GameObject prizeHatOutline;
    [SerializeField] private ParticleSystem prizeHatParticles;
    [Header("Prize UI Color Visuals")]
    private Color originalPrizeBackgroundColor;
    private Color originalPrizeAccentColor;
    [SerializeField] private Color commonColor;
    [SerializeField] private Color commonAccentColor;
    [SerializeField] private Color unCommonColor;
    [SerializeField] private Color unCommonAccentColor;
    [SerializeField] private Color rareColor;
    [SerializeField] private Color rareAccentColor;
    [SerializeField] private Color epicColor;
    [SerializeField] private Color epicAccentColor;
    [SerializeField] private Color legendaryColor;
    [SerializeField] private Color legendaryAccentColor;
    [SerializeField] private Image mainPrizeBackGround;
    [SerializeField] private Image mainPrizeAccent;

    // Misc
    private HatGenerator hatGenerator;
    private AudioManager audioManager;
    [SerializeField] private static GameObject hatObject;

    void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        originalPrizeBackgroundColor = mainPrizeBackGround.color;
        originalPrizeAccentColor = mainPrizeAccent.color;
    }

    void Update()
    {
        if (hatPrizeUI.activeSelf && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Escape))) // Apply Hat Stats after clicking off prize menu
        {
            prizeHatParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            scrollUI.GetComponent<HatScroll>().ApplyPrizeHatStats();
            FindAnyObjectByType<UIManager>().updateStatTrackerUI();
            DisableHatPrizeUI();
        }
        if(scrollUI.GetComponent<HatScroll>()._speed == 0 && scrollUI.GetComponent<HatScroll>()._hasScrolled == true) // Click
        {
            scrollUI.GetComponent<HatScroll>()._hasScrolled = false;
            ToggleScrollUI();
            ToggleHatPrize(scrollUI.GetComponent<HatScroll>().GetTargetHatData());
        }
        if(scrollUI.activeSelf && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Escape)) && scrollUI.GetComponent<HatScroll>().GetIsScrolling())
        {
            scrollUI.GetComponent<HatScroll>()._hasScrolled = false;
            GetComponent<Animator>().SetTrigger("HatRollPrize");
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
        scrollUI.GetComponent<RectTransform>().localPosition = new Vector2(1080, 0);
        scrollUI.GetComponent<HatScroll>()._speed = 0;
        scrollUI.GetComponent<HatScroll>().SetIsScrolling(false);
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
            
            ConvertHatPrizeSpriteToUI(hatData);

            hatNameText.text = hatData.hatName;
            hatNameShade.text = hatData.hatName;
            hatRarityText.text = hatData.rarity.ToString();
            hatRarityShade.text = hatData.rarity.ToString();
            for (int i = 0; i < hatStatsTexts.Length; i++)
            {
                if (i < hatData.stats.Count)
                {
                    hatStatsTexts[i].text = hatData.stats[i].ToString();
                    hatStatShades[i].text = hatData.stats[i].ToString();
                    hatStatsTexts[i].color = HatColors.GetStatTypeColor(hatData.stats[i].type);
                }
                else
                {
                    hatStatsTexts[i].text = "";
                    hatStatShades[i].text = "";
                }
            }

            // Colour the UI based on rarity
            Color rarityColor = HatColors.GetRarityColor(hatData.rarity);
            hatNameText.color = rarityColor;
            hatRarityText.color = rarityColor;
            HatSpriteImage.color = rarityColor;
            DeterminePrizeUIColors(hatData.rarity.ToString());

            
        }
        prizeHatParticles.Play();
        audioManager.Play("NewHatGet!");
    }

    public static void DestroyTargetHat()
    {
        if (hatObject != null)
        {
            Destroy(hatObject);
            hatObject = null;
        }
    }

    private void DisableHatPrizeUI()
    {
        StartCoroutine(WaitForAnimationToFinish());
    }

    private void ConvertHatPrizeSpriteToUI(GeneratedHat hatData)
    {
        hatObject = hatGenerator.GenerateHatWithStats(hatData);
        prizeHatFront.GetComponent<Image>().sprite = hatObject.GetComponent<HatComponentManager>().front.GetComponent<SpriteRenderer>().sprite;
        prizeHatFront.GetComponent<Image>().color = hatData.components.color;
        prizeHatBack.GetComponent<Image>().sprite = hatObject.GetComponent<HatComponentManager>().back.GetComponent<SpriteRenderer>().sprite;
        prizeHatOutline.GetComponent<Image>().sprite = hatObject.GetComponent<HatComponentManager>().outline.GetComponent<SpriteRenderer>().sprite;
        hatObject.GetComponent<HatComponentManager>().DisableShadow();
    }

    private void DeterminePrizeUIColors(string rarity)
    {
        switch(rarity)
        {
            case "Common":
                mainPrizeBackGround.color = commonColor;
                mainPrizeAccent.color = commonAccentColor;
                break;
            case "Uncommon":
                mainPrizeBackGround.color = unCommonColor;
                mainPrizeAccent.color = unCommonAccentColor;
                break;
            case "Rare":
                mainPrizeBackGround.color = rareColor;
                mainPrizeAccent.color = rareAccentColor;
                break;
            case "Epic":
                mainPrizeBackGround.color = epicColor;
                mainPrizeAccent.color = epicAccentColor;
                break;
            case "Legendary":
                mainPrizeBackGround.color = legendaryColor;
                mainPrizeAccent.color = legendaryAccentColor;
                break;
            default:
                mainPrizeBackGround.color = originalPrizeBackgroundColor;
                mainPrizeAccent.color = originalPrizeAccentColor;
                break;
        }
    }

    IEnumerator WaitForAnimationToFinish()
    {
        GetComponent<Animator>().SetTrigger("ExitHatRoll");
        yield return new WaitUntil(() => GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        yield return new WaitWhile(() => GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        DeterminePrizeUIColors("Default");
        unityEvent.Invoke();
    }
}
