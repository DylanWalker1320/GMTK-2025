using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;

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
    private AudioManager audioManager;
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
    private Animator animator;

    [Header("Upgrade Increases")]
    public float healAmount;
    public int healthUpgradeIncrease;
    public int speedUpgradeIncrease;
    public int xpPullRangeUpgradeIncrease;
    public int dashCooldownUpgradeIncrease;
    public int dashStrengthUpgradeIncrease;
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
        Health,
        Speed,
        IFrames,
        CastSpeed,
        CastStrength,
        XpPullRange,
        DashCooldown,
        DashStrength
    }
    public enum SpriteType
    {
        Fire,
        Water,
        Lightning,
        Dark,
    }


    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        gameManager = FindFirstObjectByType<GameManager>();
        uiManager = FindFirstObjectByType<UIManager>();
        audioManager = FindAnyObjectByType<AudioManager>();
        animator = GetComponent<Animator>();
    }

    public void UpdateDisplays()
    {
        upgradeTextOne.text = $"Heal {healAmount} HP";

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
                upgradeTextTwo.text = $"Health +{healthUpgradeIncrease}";
                break;
            case StatIncreaseType.Speed:
                upgradeTextTwo.text = $"Speed +{speedUpgradeIncrease}";
                break;
            case StatIncreaseType.IFrames:
                upgradeTextTwo.text = $"IFrames +{iFramesUpgradeIncrease}";
                break;
            case StatIncreaseType.CastSpeed:
                upgradeTextTwo.text = $"Cast Speed +{100 * castSpeedUpgradeIncrease}%";
                break;
            case StatIncreaseType.CastStrength:
                upgradeTextTwo.text = $"Cast Strength +{100 * castStrengthUpgradeIncrease}%";
                break;
            case StatIncreaseType.DashStrength:
                upgradeTextTwo.text = $"Dash Strength +{dashStrengthUpgradeIncrease}";
                break;
            case StatIncreaseType.XpPullRange:
                upgradeTextTwo.text = $"XP Pull Range +{xpPullRangeUpgradeIncrease}%";
                break;
            case StatIncreaseType.DashCooldown:
                upgradeTextTwo.text = $"Dash Cooldown -{dashCooldownUpgradeIncrease}%";
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

        audioManager.Play("HealOption");
        updateHealthUI.Invoke(player.health, player.maxHealth);
        DisableUpgradeScreen();

    }

    public void SlotTwo()
    {
        audioManager.Play("UIInteraction" + UnityEngine.Random.Range(1, 3));
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
            case StatIncreaseType.DashStrength:
                player.dashStrength += PlayerMovement.baseDashStrength * dashStrengthUpgradeIncrease / 100f;
                break;
            case StatIncreaseType.DashCooldown:
                player.dashCooldown = Mathf.Max(0.1f, player.dashCooldown - dashCooldownUpgradeIncrease / 100f); // Ensure cooldown doesn't go below 0.1 seconds
                break;
            case StatIncreaseType.XpPullRange:
                player.xpParticleSystem.endRange += PlayerMovement.baseXpPullRange * xpPullRangeUpgradeIncrease / 100f;
                break;
            default:
                Debug.LogError("Invalid upgrade index for stats.");
                break;
        }
        uiManager.updateStatTrackerUI();
        DisableUpgradeScreen();
    }
    public void SlotThree()
    {
        FindAnyObjectByType<AudioManager>().Play("UIInteraction" + UnityEngine.Random.Range(1, 3));
        DisableUpgradeScreen(false);
        // Send to spell allocation UI
        uiManager.SetActiveBarAllocUI(InteractableLoopBar.LoopBarType.SpellCombination);
    }

    private void DisableUpgradeScreen(bool unityEventInvoke = true)
    {
        StartCoroutine(DisableUpgradeScreenCoroutine(unityEventInvoke));
    }

    IEnumerator DisableUpgradeScreenCoroutine(bool unityEventInvoke = true)
    {
        TooltipManager._instance.HideTooltip();
        if(unityEventInvoke)
        {
            animator.SetTrigger("ExitThreeUpgradeScreen");
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
            unityEvent.Invoke();
        }
    }
    
}
