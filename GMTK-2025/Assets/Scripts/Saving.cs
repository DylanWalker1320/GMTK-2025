using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saving : MonoBehaviour
{
    private TextAsset gameSettingsFile; // Reference to the game settings file
    private PlayerMovement player; // Reference to the player movement script

    [System.Serializable]
    public class GameSettingsInfo
    {
        public PlayerAttributes playerAttributes; // Initial spells for the player
    }

    [System.Serializable]
    public class PlayerAttributes
    {
        public float moveForce;
        public float maxSpeed;
        public float health;
        public float maxHealth;
        public int invincibilityFrames;
        public float experience;
        public float castStrength;
        public float castSpeed;
    }

    public GameSettingsInfo gameSettingsInfo = new GameSettingsInfo(); // Game settings loaded from the JSON file
                                                                       // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private bool debugMode = false;
    private string debugPrefix = "<color=#55AAFF>[Saving]</color>";

    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
    }
    void Start()
    {
        GetSaveValues();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Save();
        } 
        if (Input.GetKeyDown(KeyCode.F9))
        {
            Load();
        }
    }

    public void GetSaveValues()
    {
        if (player != null)
        {
            gameSettingsInfo.playerAttributes = new PlayerAttributes
            {
                moveForce = player.moveForce,
                maxSpeed = player.maxSpeed,
                health = player.health,
                maxHealth = player.maxHealth,
                invincibilityFrames = player.invincibilityFrames,
                experience = player.experience,
                castStrength = player.castStrength,
                castSpeed = player.castSpeed
            };
        }
    }

    public void Save()
    {
        if (debugMode) Debug.Log($"{debugPrefix} Saving game settings...");

        GetSaveValues();

        // Save the current player attributes to the game settings file
        string json = JsonUtility.ToJson(gameSettingsInfo, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/GameSettings.json", json);

        if (debugMode)
        {
            Debug.Log($"{debugPrefix} Saved Data: {json}");
        }
    }

    public void Load()
    {
        if (debugMode) Debug.Log($"{debugPrefix} Loading game settings...");
        // Load the game settings from the JSON file
        if (System.IO.File.Exists(Application.persistentDataPath + "/GameSettings.json"))
        {
            string json = System.IO.File.ReadAllText(Application.persistentDataPath + "/GameSettings.json");
            gameSettingsInfo = JsonUtility.FromJson<GameSettingsInfo>(json);
            LoadPlayerStats(gameSettingsInfo.playerAttributes, player); // Set current player attributes from the loaded settings
        }
        else
        {
            Debug.LogWarning($"{debugPrefix} Game settings file not found, using default settings.");
        }

        if (debugMode)
        {
            Debug.Log($"{debugPrefix} Loaded Data: {JsonUtility.ToJson(gameSettingsInfo, true)}");
        }
    }

    public void LoadPlayerStats(PlayerAttributes attributes, PlayerMovement player)
    {
        if (player != null)
        {
            player.moveForce = attributes.moveForce;
            player.maxSpeed = attributes.maxSpeed;
            player.health = attributes.health;
            player.maxHealth = attributes.maxHealth;
            player.invincibilityFrames = attributes.invincibilityFrames;
            player.experience = attributes.experience;
            player.castStrength = attributes.castStrength;
            player.castSpeed = attributes.castSpeed;
        }

        player.UpdateUI();
    }
}
