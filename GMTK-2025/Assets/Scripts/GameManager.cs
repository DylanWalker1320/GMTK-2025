using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerMovement player; // Reference to the player movement script
    private GameSettings gameSettings; // Reference to the game settings
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
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (levelComplete)
        {
            SwitchToSafeArea();
            SetCurrentPlayerStats();
        }
    }

    void SwitchToSafeArea()
    {

    }

    void SetCurrentPlayerStats()
    {
        if (gameSettings != null)
        {
            gameSettings.gameSettingsInfo.currentPlayerAttributes.moveForce = player.moveForce;
            gameSettings.gameSettingsInfo.currentPlayerAttributes.maxSpeed = player.maxSpeed;
            gameSettings.gameSettingsInfo.currentPlayerAttributes.maxHealth = player.maxHealth;
            gameSettings.gameSettingsInfo.currentPlayerAttributes.invincibilityFrames = player.invincibilityFrames;
            gameSettings.gameSettingsInfo.currentPlayerAttributes.experience = player.experience;
            gameSettings.gameSettingsInfo.currentPlayerAttributes.castStrength = player.CastStrength;
            gameSettings.gameSettingsInfo.currentPlayerAttributes.castSpeed = player.castSpeed;
        }
    }

    void SetExperience(float experience)
    {
        if (gameSettings != null)
        {
            gameSettings.gameSettingsInfo.currentPlayerAttributes.experience = experience;
            gameSettings.Save();
        }
    }
}
