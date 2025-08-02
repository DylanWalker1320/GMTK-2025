using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class StatUpgradeUI : MonoBehaviour
{
    private GameManager gameManager; // Reference to the GameManager script
    private PlayerMovement player; // Reference to the PlayerMovement script
    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI healthText; // Text component for displaying health
    [SerializeField] private TextMeshProUGUI speedText; // Text component for displaying speed
    [SerializeField] private TextMeshProUGUI iFramesText; // Text component for displaying invincibility frames
    [SerializeField] private TextMeshProUGUI castSpeedText; // Text component for displaying cast speed
    [SerializeField] private TextMeshProUGUI castStrengthText; // Text component for displaying cast strength
    [SerializeField] private TextMeshProUGUI experienceText; // Text component for displaying experience
    [SerializeField] private Button upgradeHealthButton; // Button for upgrading health
    [SerializeField] private Button upgradeSpeedButton; // Button for upgrading speed
    [SerializeField] private Button upgradeIFramesButton; // Button for upgrading invincibility frames
    [SerializeField] private Button upgradeCastSpeedButton; // Button for upgrading cast speed
    [SerializeField] private Button upgradeCastStrengthButton; // Button for upgrading cast strength
    [Header("Upgrade Costs")]
    [SerializeField] private int healthUpgradeCost;
    [SerializeField] private int speedUpgradeCost;
    [SerializeField] private int iFramesUpgradeCost;
    [SerializeField] private int castSpeedUpgradeCost;
    [SerializeField] private int castStrengthUpgradeCost;
    [SerializeField] private int healthUpgradeIncrease;
    [SerializeField] private int speedUpgradeIncrease;
    [SerializeField] private int iFramesUpgradeIncrease;
    [SerializeField] private int castSpeedUpgradeIncrease;
    [SerializeField] private int castStrengthUpgradeIncrease;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
        player = FindFirstObjectByType<PlayerMovement>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthText.text = player.maxHealth.ToString();
        speedText.text = player.maxSpeed.ToString();
        iFramesText.text = player.invincibilityFrames.ToString();
        castSpeedText.text = player.castSpeed.ToString();
        castStrengthText.text = player.castStrength.ToString();
        experienceText.text = player.experience.ToString();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpgradeHealth()
    {
        if (CheckExperience(healthUpgradeCost))
        {
            player.maxHealth += healthUpgradeIncrease;
            SubtractExperience(healthUpgradeCost); // Subtract experience for the upgrade           
        }
    }

    public void UpgradeSpeed()
    {
        if (CheckExperience(speedUpgradeCost))
        {
            player.moveForce += speedUpgradeIncrease; // Assuming moveForce is the speed attribute
            player.maxSpeed += speedUpgradeIncrease;
            SubtractExperience(speedUpgradeCost); // Subtract experience for the upgrade
        }
    }

    public void UpgradeCastSpeed()
    {
        if (CheckExperience(castSpeedUpgradeCost))
        {
            player.castSpeed += castSpeedUpgradeIncrease;
            SubtractExperience(castSpeedUpgradeCost); // Subtract experience for the upgrade
        }
    }

    public void UpgradeCastStrength()
    {
        if (CheckExperience(castStrengthUpgradeCost))
        {
            player.castStrength += castStrengthUpgradeIncrease;
            SubtractExperience(castStrengthUpgradeCost); // Subtract experience for the upgrade
        }
    }

    public void UpgradeIFrames()
    {
        if (CheckExperience(iFramesUpgradeCost))
        {
            player.invincibilityFrames += iFramesUpgradeIncrease;
            SubtractExperience(iFramesUpgradeCost); // Subtract experience for the upgrade
        }
    }

    void SubtractExperience(int experience)
    {
        player.experience -= experience; // Subtract experience from the player
        UpdateText(); // Update the UI text to reflect the new values
    }

    void UpdateText()
    {
        healthText.text = player.maxHealth.ToString();
        speedText.text = player.maxSpeed.ToString();
        iFramesText.text = player.invincibilityFrames.ToString();
        castSpeedText.text = player.castSpeed.ToString();
        castStrengthText.text = player.castStrength.ToString();
    }

    private bool CheckExperience(int cost)
    {
        if (player.experience - cost < 0 )
        {
            return false;
        }
        return true; // Player has enough experience for the upgrade
    }
    
}
