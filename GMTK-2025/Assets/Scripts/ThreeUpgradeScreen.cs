using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

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

    public UnityEvent<float, float> updateHealthUI;

    private GameManager gameManager; // Reference to the GameManager script
    private PlayerMovement player; // Reference to the PlayerMovement script
    private UIManager uiManager;
    private float restoreHealthHandicap = 3;

    [SerializeField] private UnityEvent unityEvent;

    [Header("Upgrade List")]
    [SerializeField] private TextMeshProUGUI upgradeListHeader; // Header for the upgrade list

    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI upgradeHeaderOne; // Heal
    [SerializeField] private TextMeshProUGUI upgradeHeaderTwo; // Stat++
    [SerializeField] private TextMeshProUGUI upgradeHeaderThree; // Spell
    [SerializeField] private TextMeshProUGUI upgradeTextOne; 
    [SerializeField] private TextMeshProUGUI upgradeTextTwo; 
    [SerializeField] private TextMeshProUGUI upgradeTextThree; 
    [SerializeField] private Animator boxOneAnimator;
    [SerializeField] private Animator boxTwoAnimator;
    [SerializeField] private Animator boxThreeAnimator;

    [Header("Upgrade Increases")]
    public float healAmount;
    public int healthUpgradeIncrease;
    public int speedUpgradeIncrease;
    public int iFramesUpgradeIncrease;
    public float castSpeedUpgradeIncrease;
    public float castStrengthUpgradeIncrease;

    [Header("Upgrade Index")]

    public StatIncreaseType upgradeStatType; // Statas
    public SpriteType upgradeSpriteType; // Spells

    [Header("Elemental Sprites")]
    [SerializeField] private Image displaySprite;
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite lightSprite;
    [SerializeField] private Sprite darkSprite;

    public enum StatIncreaseType
    {
        Health = 0,
        Speed = 1,
        IFrames = 2,
        CastSpeed = 3,
        CastStrength = 4
    }
    public enum SpriteType
    {
        Fire = 0,
        Water = 1,
        Lightning = 2,
        Dark = 3
    }


    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        gameManager = FindFirstObjectByType<GameManager>();
        uiManager = FindFirstObjectByType<UIManager>();
    }

    public void UpdateDisplays()
    {
        RestartAnimators();
        upgradeTextOne.text = "Heal " + healAmount + " HP";

        upgradeStatType = (StatIncreaseType) UnityEngine.Random.Range(0, Enum.GetValues(typeof(StatIncreaseType)).Length); // Change this according to the number of stats in the enum class
        UpdateStatDisplay();

        upgradeSpriteType = (SpriteType) UnityEngine.Random.Range(0, Enum.GetValues(typeof(SpriteType)).Length); // Change this according to the number of spells in the enum class
        UpdateSpellDisplay();
    }

    void UpdateStatDisplay()
    {
        switch (upgradeStatType)
        {
            case StatIncreaseType.Health:
                upgradeTextTwo.text = "Health +" + healthUpgradeIncrease;
                break;
            case StatIncreaseType.Speed:
                upgradeTextTwo.text = "Speed +" + speedUpgradeIncrease;
                break;
            case StatIncreaseType.IFrames:
                upgradeTextTwo.text = "IFrames +" + iFramesUpgradeIncrease;
                break;
            case StatIncreaseType.CastSpeed:
                upgradeTextTwo.text = "Cast Speed +" + castSpeedUpgradeIncrease;
                break;
            case StatIncreaseType.CastStrength:
                upgradeTextTwo.text = "Cast Strength + " + 100 * castStrengthUpgradeIncrease + "%";
                break;
            default:
                Debug.LogError("Invalid upgrade index for stats.");
                break;
        }
    }

    void UpdateSpellDisplay()
    {
        switch (upgradeSpriteType)
        {
            case SpriteType.Fire:
                upgradeTextThree.text = "Fireball";
                gameManager.allocateSpell = Spell.SpellType.Fire;
                gameManager.spellImage = fireSprite;
                displaySprite.sprite = fireSprite;
                break;
            case SpriteType.Water:
                upgradeTextThree.text = "Waterball";
                gameManager.allocateSpell = Spell.SpellType.Water;
                gameManager.spellImage = waterSprite;
                displaySprite.sprite = waterSprite;
                break;
            case SpriteType.Lightning:
                upgradeTextThree.text = "Lightning";
                gameManager.allocateSpell = Spell.SpellType.Lightning;
                gameManager.spellImage = lightSprite;
                displaySprite.sprite = lightSprite;
                break;
            case SpriteType.Dark:
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

    public void SlotOne()
    {
        if (player.health + healAmount <= player.maxHealth)
        {
            player.health += healAmount; // Heal the player by the specified amount
        }
        else
        {
            player.health = player.maxHealth;
        }
        healAmount += Mathf.Round(player.health / restoreHealthHandicap);

        updateHealthUI.Invoke(player.health, player.maxHealth);
        DisableUpgradeScreen();

    }

    public void SlotTwo()
    {
        switch (upgradeStatType)
        {
            case StatIncreaseType.Health:
                player.maxHealth += healthUpgradeIncrease; // Upgrade health
                player.health += healthUpgradeIncrease;
                if(player.health > player.maxHealth)
                {
                    player.health = player.maxHealth;
                }
                updateHealthUI.Invoke(player.health, player.maxHealth);
                break;
            case StatIncreaseType.Speed:
                player.moveForce += speedUpgradeIncrease; // Upgrade speed
                player.maxSpeed += speedUpgradeIncrease;
                break;
            case StatIncreaseType.IFrames:
                player.invincibilityFrames += iFramesUpgradeIncrease; // Upgrade invincibility frames
                break;
            case StatIncreaseType.CastSpeed:
                player.castSpeed += castSpeedUpgradeIncrease; // Upgrade cast speed
                break;
            case StatIncreaseType.CastStrength:
                player.castStrength += castStrengthUpgradeIncrease; // Upgrade cast strength
                break;
            default:
                Debug.LogError("Invalid upgrade index for stats.");
                break;
        }
        DisableUpgradeScreen();
    }
    public void SlotThree()
    {
        DisableUpgradeScreen(false);
        // Send to spell allocation UI
        uiManager.SetActiveBarAllocUI();
    }

    private void DisableUpgradeScreen(bool unityEventInvoke = true)
    {
        PreserveAnimStateOnDisable();
        TooltipManager._instance.HideTooltip();
        if(unityEventInvoke)
        {
            unityEvent.Invoke();   
        }
    }

    private void PreserveAnimStateOnDisable()
    {
        boxOneAnimator.keepAnimatorStateOnDisable = true;
        boxTwoAnimator.keepAnimatorStateOnDisable = true;
        boxThreeAnimator.keepAnimatorStateOnDisable = true;
    }
    private void RestartAnimators()
    {
        boxOneAnimator.SetTrigger("Normal");
        boxOneAnimator.Update(0f);
        boxTwoAnimator.SetTrigger("Normal");
        boxTwoAnimator.Update(0f);
        boxThreeAnimator.SetTrigger("Normal");
        boxThreeAnimator.Update(0f);
    }
    
}
