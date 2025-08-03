using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerMovement player; // Reference to the player movement script
    private EnemySpawner enemySpawner; // Reference to the enemy spawner script
    private bool isGamePaused = false; // Flag to check if the game is paused
    public bool isInSafeArea = false; // Flag to check if the player is in a safe area
    public bool levelComplete = false;
    public bool loopComplete = false;
    public int loopsCompleted;
    public int wavesCompleted;

    // Reference for Upgrade Screen & Slot Allocation
    public Spell.SpellType allocateSpell;
    public Sprite spellImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        if (player == null)
        {
            Debug.LogError("PlayerMovement not found in the scene.");
        }

        if (!isInSafeArea)
        {
            enemySpawner = FindFirstObjectByType<EnemySpawner>();
        }

    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (levelComplete)
        {
            levelComplete = false; // Reset level complete flag
            // SetExperience(player.experience); // Save player's experience when level is complete
        }
        else if (enemySpawner != null)
        {
            if (enemySpawner.maxWavePopulation <= 0 && enemySpawner.currentEnemies <= 0 && !isInSafeArea)
            {
                levelComplete = true; // Set level complete when all enemies are defeated
                isInSafeArea = true; // Switch to safe area when all enemies are defeated
                wavesCompleted++;
                if (wavesCompleted % 4 == 0)
                {
                    loopComplete = true;
                    loopsCompleted++;
                }
            }
        }
    }


    // void SetExperience(float experience) // Only sets experience and saves it
    // {
    //     if (gameSettings != null)
    //     {
    //         gameSettings.gameSettingsInfo.currentPlayerAttributes.experience = experience;
    //     }
    // }
    
    public void ToggleSafeArea(bool isInSafeArea)
    {
        this.isInSafeArea = isInSafeArea; // Set the safe area flag
    }

}
