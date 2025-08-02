using System;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private TextAsset gameSettingsFile; // Reference to the game settings file
    [System.Serializable]
    public class GameSettingsInfo
    {
        public PlayerAttributes initialPlayerAttributes; // Initial spells for the player
        public CurrentPlayerAttributes currentPlayerAttributes; // Current spells for the player
    }
    [System.Serializable]
    public class PlayerAttributes
    {
        public float moveForce;
        public float maxSpeed;
        public float maxHealth;
        public int invincibilityFrames;
        public float experience;
        public float castStrength;
        public float castSpeed;
    }
    [System.Serializable]
    public class CurrentPlayerAttributes
    {
        public float moveForce;
        public float maxSpeed;
        public float maxHealth;
        public int invincibilityFrames;
        public float experience;
        public float castStrength;
        public float castSpeed;
    }
    public GameSettingsInfo gameSettingsInfo = new GameSettingsInfo(); // Game settings loaded from the JSON file
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameSettingsInfo = JsonUtility.FromJson<GameSettingsInfo>(gameSettingsFile.text);
        SetInitialPlayerAttributes(gameSettingsInfo.initialPlayerAttributes); // Set initial player attributes from the loaded settings
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Save()
    {
        // Save the current player attributes to the game settings file
        SetCurrentPlayerAttributes(gameSettingsInfo.currentPlayerAttributes);
        string json = JsonUtility.ToJson(gameSettingsInfo, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/GameSettings.json", json);
        Debug.Log("Game settings saved to " + Application.persistentDataPath + "/GameSettings.json");
    }
    void SetInitialPlayerAttributes(PlayerAttributes attributes)
    {

        attributes.moveForce = gameSettingsInfo.initialPlayerAttributes.moveForce;
        attributes.maxSpeed = gameSettingsInfo.initialPlayerAttributes.maxSpeed;
        attributes.maxHealth = gameSettingsInfo.initialPlayerAttributes.maxHealth;
        attributes.invincibilityFrames = gameSettingsInfo.initialPlayerAttributes.invincibilityFrames;
        attributes.experience = gameSettingsInfo.initialPlayerAttributes.experience;
        attributes.castStrength = gameSettingsInfo.initialPlayerAttributes.castStrength;
        attributes.castSpeed = gameSettingsInfo.initialPlayerAttributes.castSpeed;
    }
    void SetCurrentPlayerAttributes(CurrentPlayerAttributes attributes)
    {
        attributes.moveForce = gameSettingsInfo.currentPlayerAttributes.moveForce;
        attributes.maxSpeed = gameSettingsInfo.currentPlayerAttributes.maxSpeed;
        attributes.maxHealth = gameSettingsInfo.currentPlayerAttributes.maxHealth;
        attributes.invincibilityFrames = gameSettingsInfo.currentPlayerAttributes.invincibilityFrames;
        attributes.experience = gameSettingsInfo.currentPlayerAttributes.experience;
        attributes.castStrength = gameSettingsInfo.currentPlayerAttributes.castStrength;
        attributes.castSpeed = gameSettingsInfo.currentPlayerAttributes.castSpeed;
    }
}
