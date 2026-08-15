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
    [SerializeField] private TextMeshProUGUI hatNameText;
    [SerializeField] private Image HatSpriteImage;
    [SerializeField] private TextMeshProUGUI[] hatStatsTexts;
    [SerializeField] private UnityEvent unityEvent;
    
    [Header("Prize Visuals")]
    [SerializeField] private GameObject prizeHatFront;
    [SerializeField] private GameObject prizeHatBack;
    [SerializeField] private GameObject prizeHatOutline;

    // Misc
    private HatGenerator hatGenerator;
    private AudioManager audioManager;
    [SerializeField] private static GameObject hatObject;

    void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    void Update()
    {
        if (hatPrizeUI.activeSelf && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Escape))) // Apply Hat Stats after clicking off prize menu
        {
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

    IEnumerator WaitForAnimationToFinish()
    {
        GetComponent<Animator>().SetTrigger("ExitHatRoll");
        yield return new WaitUntil(() => GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        yield return new WaitWhile(() => GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        unityEvent.Invoke();
    }
}
