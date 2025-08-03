using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class NewGameLoopMenu : MonoBehaviour
{
    private GameManager gameManager; // Reference to the GameManager script
    private PlayerMovement player; // Reference to the PlayerMovement script
    private UIManager uiManager;
    [SerializeField] private UnityEvent unityEvent;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI upgradeTextOne;
    [SerializeField] private TextMeshProUGUI upgradeTextTwo;
    [SerializeField] private TextMeshProUGUI upgradeTextThree;

    [Header("Upgrade Increases")]
    [SerializeField] private int upgradeIndex;
    [SerializeField] private int healthUpgradeIncrease;
    [SerializeField] private int speedUpgradeIncrease;
    [SerializeField] private int iFramesUpgradeIncrease;
    [SerializeField] private float castSpeedUpgradeIncrease;
    [SerializeField] private float castStrengthUpgradeIncrease;

    [Header("BIG EXP BOOM BOOM")]
    [SerializeField] private int experienceGain;
    [Header("Elements")]
    [SerializeField] private int spellTypeIndex;


    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        gameManager = FindFirstObjectByType<GameManager>();
        uiManager = FindFirstObjectByType<UIManager>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateDisplays()
    {
        upgradeIndex = Random.Range(0, 5);
        spellTypeIndex = Random.Range(0, 4);
        upgradeTextOne.text = "GAIN " + experienceGain + "SOULS!";

        UpdateStatDisplay();
        UpdateSpellDisplay();
    }

    void UpdateStatDisplay()
    {
        switch (upgradeIndex)
        {
            case 0:
                upgradeTextTwo.text = "Health +" + healthUpgradeIncrease;
                break;
            case 1:
                upgradeTextTwo.text = "Speed +" + speedUpgradeIncrease;
                break;
            case 2:
                upgradeTextTwo.text = "IFrames +" + iFramesUpgradeIncrease;
                break;
            case 3:
                upgradeTextTwo.text = "Cast Speed +" + castSpeedUpgradeIncrease;
                break;
            case 4:
                upgradeTextTwo.text = "Cast Strength + " + 100 * castStrengthUpgradeIncrease + "%";
                break;
            default:
                Debug.LogError("Invalid upgrade index for stats.");
                break;
        }
    }

    void UpdateSpellDisplay()
    {
        switch (spellTypeIndex)
        {
            case 0:
                upgradeTextThree.text = "30% Fire Buff";
                break;
            case 1:
                upgradeTextThree.text = "30% Water Buff";
                break;
            case 2:
                upgradeTextThree.text = "30% Lightning Buff";
                break;
            case 3:
                upgradeTextThree.text = "30% Dark Buff";
                break;

        }
    }

    public void SlotOne()
    {
        player.experience += experienceGain;
        unityEvent.Invoke();
    }

    public void SlotTwo()
    {
        switch (upgradeIndex)
        {
            case 0:
                player.maxHealth += healthUpgradeIncrease; // Upgrade health
                break;
            case 1:
                player.moveForce += speedUpgradeIncrease; // Upgrade speed
                player.maxSpeed += speedUpgradeIncrease;
                break;
            case 2:
                player.invincibilityFrames += iFramesUpgradeIncrease; // Upgrade invincibility frames
                break;
            case 3:
                player.castSpeed += castSpeedUpgradeIncrease; // Upgrade cast speed
                break;
            case 4:
                player.castStrength += castStrengthUpgradeIncrease; // Upgrade cast strength
                break;
            default:
                Debug.LogError("Invalid upgrade index for stats.");
                break;
        }
        unityEvent.Invoke();
    }

    public void SlotThree()
    {
        switch (spellTypeIndex)
        {
            case 0:
                Spell.UpgradeModifier(Spell.SpellType.Fire, 0.3f);
                break;
            case 1:
                Spell.UpgradeModifier(Spell.SpellType.Water, 0.3f);
                break;
            case 2:
                Spell.UpgradeModifier(Spell.SpellType.Lightning, 0.3f);
                break;
            case 3:
                Spell.UpgradeModifier(Spell.SpellType.Lightning, 0.3f);
                break;

        }
        unityEvent.Invoke();
    }

}
