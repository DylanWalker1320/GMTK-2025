using System;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private TextAsset gameSettingsFile; // Reference to the game settings file
    private PlayerMovement player; // Reference to the player movement script
    [Header("Debug Game Settings")]
    [SerializeField] private bool firstLevelInit;
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

    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
    }
    void Start()
    {
        gameSettingsInfo = JsonUtility.FromJson<GameSettingsInfo>(gameSettingsFile.text);
        SetInitialPlayerAttributes(gameSettingsInfo.initialPlayerAttributes); // Set initial player attributes from the loaded settings   
    }

    // Update is called once per frame
    void Update()
    {

    }

    // public void Save()
    // {
    //     Debug.Log("Saving game settings...");
    //     // Save the current player attributes to the game settings file
    //     string json = JsonUtility.ToJson(gameSettingsInfo, true);
    //     System.IO.File.WriteAllText(Application.persistentDataPath + "/GameSettings.json", json);

    // }

    // public void Load()
    // {
    //     Debug.Log("Loading game settings...");
    //     // Load the game settings from the JSON file
    //     if (System.IO.File.Exists(Application.persistentDataPath + "/GameSettings.json"))
    //     {
    //         string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/GameSettings.json");
    //         gameSettingsInfo = JsonUtility.FromJson<GameSettingsInfo>(json);
    //         LoadPlayerStats(gameSettingsInfo.currentPlayerAttributes, player); // Set current player attributes from the loaded settings
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Game settings file not found, using default settings.");
    //     }
    // }
    public void LoadPlayerStats(CurrentPlayerAttributes attributes, PlayerMovement player)
    {
        if (player != null)
        {
            player.moveForce = attributes.moveForce;
            player.maxSpeed = attributes.maxSpeed;
            player.maxHealth = attributes.maxHealth;
            player.invincibilityFrames = attributes.invincibilityFrames;
            Debug.Log(attributes.experience);
            player.experience = attributes.experience;
            player.castStrength = attributes.castStrength;
            player.castSpeed = attributes.castSpeed;
        }
    }
    void SetInitialPlayerAttributes(PlayerAttributes attributes)
    {
        player.moveForce = gameSettingsInfo.initialPlayerAttributes.moveForce;
        player.maxSpeed = gameSettingsInfo.initialPlayerAttributes.maxSpeed;
        player.maxHealth = gameSettingsInfo.initialPlayerAttributes.maxHealth;
        player.invincibilityFrames = gameSettingsInfo.initialPlayerAttributes.invincibilityFrames;
        player.experience = gameSettingsInfo.initialPlayerAttributes.experience;
        player.castStrength = gameSettingsInfo.initialPlayerAttributes.castStrength;
        player.castSpeed = gameSettingsInfo.initialPlayerAttributes.castSpeed;
    }
}
