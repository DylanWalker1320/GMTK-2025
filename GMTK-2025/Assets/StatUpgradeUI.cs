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
    [SerializeField] private Button upgradeHealthButton; // Button for upgrading health
    [SerializeField] private Button upgradeSpeedButton; // Button for upgrading speed
    [SerializeField] private Button upgradeIFramesButton; // Button for upgrading invincibility frames
    [SerializeField] private Button upgradeCastSpeedButton; // Button for upgrading cast speed
    [SerializeField] private Button upgradeCastStrengthButton; // Button for upgrading cast strength
    [Header("Upgrade Costs")]
    [SerializeField] private int generalUpgradeCost;
    [SerializeField] private int generalUpgradeIncrease;

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
        castStrengthText.text = player.CastStrength.ToString();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void UpgradeHealth()
    {
        player.maxHealth += generalUpgradeIncrease;
        SubtractExperience(generalUpgradeCost); // Subtract experience for the upgrade
    }

    public void UpgradeSpeed()
    {
        player.moveForce += generalUpgradeIncrease; // Assuming moveForce is the speed attribute
        player.maxSpeed += generalUpgradeIncrease;
        SubtractExperience(generalUpgradeCost); // Subtract experience for the upgrade
    }
    public void UpgradeCastSpeed()
    {
        player.castSpeed += generalUpgradeIncrease;
        SubtractExperience(generalUpgradeCost); // Subtract experience for the upgrade
    }
    public void UpgradeCastStrength()
    {
        player.CastStrength += generalUpgradeIncrease;
        SubtractExperience(generalUpgradeCost); // Subtract experience for the upgrade
    }
    public void UpgradeIFrames()
    {
        player.invincibilityFrames += generalUpgradeIncrease;
        SubtractExperience(generalUpgradeCost); // Subtract experience for the upgrade
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
        castStrengthText.text = player.CastStrength.ToString();
    }
    
}
