using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using System.Collections.Generic;

public class ThreeUpgradeScreen : MonoBehaviour
{
    
    public enum UpgradeStats
    {
        Health,
        Speed,
        IFrames,
        CastSpeed,
        CastStrength
    }

    public enum Spells
    {
        Fireball,
        Waterball,
        Lightning,
        Dark
    }
    public enum Heal
    {
        Heal
    }

    private GameManager gameManager; // Reference to the GameManager script
    private PlayerMovement player; // Reference to the PlayerMovement script
    private UIManager uiManager;

    [Header("Upgrade List")]
    [SerializeField] private TextMeshProUGUI upgradeListHeader; // Header for the upgrade list

    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI upgradeHeaderOne; // Heal
    [SerializeField] private TextMeshProUGUI upgradeHeaderTwo; // Stat++
    [SerializeField] private TextMeshProUGUI upgradeHeaderThree; // Spell
    [SerializeField] private TextMeshProUGUI upgradeTextOne; 
    [SerializeField] private TextMeshProUGUI upgradeTextTwo; 
    [SerializeField] private TextMeshProUGUI upgradeTextThree; 
    [SerializeField] private Button upgradeButtonOne;
    [SerializeField] private Button upgradeButtonTwo;
    [SerializeField] private Button upgradeButtonThree;

    [Header("Upgrade Increases")]
    [SerializeField] private float healAmount;
    [SerializeField] private int healthUpgradeIncrease;
    [SerializeField] private int speedUpgradeIncrease;
    [SerializeField] private int iFramesUpgradeIncrease;
    [SerializeField] private int castSpeedUpgradeIncrease;
    [SerializeField] private int castStrengthUpgradeIncrease;

    [Header("Upgrade Index")]

    [SerializeField] private int upgradeIndexTwo; // Statas
    [SerializeField] private int upgradeIndexThree; // Spells

    [Header("Elemental Sprites")]
    [SerializeField] private Image displaySprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite lightSprite;
    [SerializeField] private Sprite darkSprite;


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
        upgradeTextOne.text = "Heal " + healAmount + " HP";

        upgradeIndexTwo = Random.Range(0, 5); // Change this according to the number of stats in the enum class
        UpdateStatDisplay();

        upgradeIndexThree = Random.Range(0, 4); // Change this according to the number of spells in the enum class
        UpdateSpellDisplay();
    }

    void UpdateStatDisplay()
    {
        switch (upgradeIndexTwo)
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
                upgradeTextTwo.text = "Cast Strength +" + castStrengthUpgradeIncrease;
                break;
            default:
                Debug.LogError("Invalid upgrade index for stats.");
                break;
        }
    }

    void UpdateSpellDisplay()
    {
        switch (upgradeIndexThree)
        {
            case 0:
                upgradeTextThree.text = "Fireball";
                gameManager.allocateSpell = Spell.SpellType.Fire;
                gameManager.spellImage = fireSprite;
                displaySprite.sprite = fireSprite;
                break;
            case 1:
                upgradeTextThree.text = "Waterball";
                gameManager.allocateSpell = Spell.SpellType.Water;
                gameManager.spellImage = waterSprite;
                displaySprite.sprite = waterSprite;
                break;
            case 2:
                upgradeTextThree.text = "Lightning";
                gameManager.allocateSpell = Spell.SpellType.Lightning;
                gameManager.spellImage = lightSprite;
                displaySprite.sprite = lightSprite;
                break;
            case 3:
                upgradeTextThree.text = "Dark";
                gameManager.allocateSpell = Spell.SpellType.Dark;
                gameManager.spellImage = darkSprite;
                displaySprite.sprite = darkSprite;
                break;
            default:
                Debug.LogError("Invalid upgrade index for spells.");
                break;
        }
    }

    public void Slot1()
    {
        if (player.health + healAmount <= player.maxHealth)
        {
            player.health += healAmount; // Heal the player by the specified amount
        }
        else
        {
            player.health = player.maxHealth;
        }
        healAmount += player.health / 3.0f;
        uiManager.SetActiveUpgradeUI();
        uiManager.SetActiveSpellUpgradeUI();

    }

    public void Slot2()
    {
        switch (upgradeIndexTwo)
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
        uiManager.SetActiveUpgradeUI();
        uiManager.SetActiveSpellUpgradeUI();
        
    }
    public void Slot3()
    {
        uiManager.SetActiveBarAllocUI();
    }
}
