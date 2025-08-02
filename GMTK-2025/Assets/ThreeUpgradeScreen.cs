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
        Lightning
    }
    public enum Heal
    {
        Heal
    }

    private GameManager gameManager; // Reference to the GameManager script
    private PlayerMovement player; // Reference to the PlayerMovement script

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

    [SerializeField] private List<int> upgradePoolIndexList;

    private int upgradeIndexOne;
    private int upgradeIndexTwo;
    private int upgradeIndexThree;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        upgradeTextOne.text = "Heal " + healAmount + " HP";

        upgradeIndexTwo = Random.Range(0, 5); // Change this according to the number of stats in the enum class
        UpdateStatDisplay();

        upgradeIndexTwo = Random.Range(0, 3); // Change this according to the number of spells in the enum class
        UpdateSpellDisplay();
    }

    // Update is called once per frame
    void Update()
    {

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
                break;
            case 1:
                upgradeTextThree.text = "Waterball";
                break;
            case 2:
                upgradeTextThree.text = "Lightning";
                break;
            default:
                Debug.LogError("Invalid upgrade index for spells.");
                break;
        }
    }

    void Slot1()
    {
        player.health += healAmount; // Heal the player by the specified amount
        healAmount += player.health / 3.0f;
        // Turn Canvas Off

    }

    void Slot2()
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
    }
    void Slot3()
    {

    }
}
