using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerMovement player; // Reference to the player movement script
    private EnemySpawner enemySpawner; // Reference to the enemy spawner script
    private GameSettings gameSettings; // Reference to the game settings
    private bool isGamePaused = false; // Flag to check if the game is paused
    [SerializeField] private Transform levelCompletePosition; // Position to move the player when the level is complete
    public bool isInSafeArea = false; // Flag to check if the player is in a safe area
    public bool isSaved = true; // Flag to reset the save state
    public bool levelComplete = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        if (player == null)
        {
            Debug.LogError("PlayerMovement not found in the scene.");
        }

        gameSettings = FindFirstObjectByType<GameSettings>();
        if (gameSettings == null)
        {
            Debug.LogError("GameSettings not found in the scene.");
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
            SwitchToSafeArea();
            SetExperience(player.experience); // Save player's experience when level is complete
        }
        else if (enemySpawner != null)
        {
            if (enemySpawner.maxWavePopulation == 0 && enemySpawner.currentEnemies == 0 && !isInSafeArea)
            {
                levelComplete = true; // Set level complete when all enemies are defeated
                isInSafeArea = true; // Switch to safe area when all enemies are defeated
            }
        }
    }

    void SwitchToSafeArea()
    {
        player.transform.position = levelCompletePosition.position; // Move player to safe area
    }

    void SaveCurrentPlayerStats() // Sets current 
    {
        if (gameSettings != null)
        {
            gameSettings.gameSettingsInfo.currentPlayerAttributes.maxSpeed = player.maxSpeed;
            gameSettings.gameSettingsInfo.currentPlayerAttributes.maxHealth = player.maxHealth;
            gameSettings.gameSettingsInfo.currentPlayerAttributes.invincibilityFrames = player.invincibilityFrames;
            gameSettings.gameSettingsInfo.currentPlayerAttributes.experience = player.experience;
            gameSettings.gameSettingsInfo.currentPlayerAttributes.castStrength = player.CastStrength;
            gameSettings.gameSettingsInfo.currentPlayerAttributes.castSpeed = player.castSpeed;
            gameSettings.Save(); // Save the current player stats
        }
    }

    void SetExperience(float experience) // Only sets experience and saves it
    {
        if (gameSettings != null)
        {
            gameSettings.gameSettingsInfo.currentPlayerAttributes.experience = experience;
            gameSettings.Save();
        }
    }
}
