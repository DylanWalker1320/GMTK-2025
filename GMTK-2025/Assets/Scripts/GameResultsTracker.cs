using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using TMPro;

public class SpellEntryStats
{
    [SerializeField] private Image icon;
}

public class GameResultsTracker : MonoBehaviour
{
    public static GameResultsTracker _instance { get; private set; }

    private GameManager gameManager;

    [Header("Game Results")]
    [SerializeField] private float totalTimePlayed = 0f;
    [SerializeField] private int totalEnemiesKilled = 0;
    [SerializeField] private int wavesCompleted = 0;
    [SerializeField] private int loopsCompleted = 0;
    [SerializeField] private int soulsEarned = 0;
    [Header("Final Stats")]
    [SerializeField] private int playerLevel = 1;
    [SerializeField] private int playerHealth = 100;
    [SerializeField] private int playerSpeed = 5;
    [SerializeField] private float playercastStrength = 1f;
    [SerializeField] private float playercastSpeed = 1f;
    [SerializeField] private float playerdashStrength = 1f;
    [SerializeField] private float playerXPPullRange = 1f;
    [Header("Spell Damage Stats")]
    [SerializeField] private float fireBallSpellDamage = 0f;
    [SerializeField] private float waterBallSpellDamage = 0f; 
    [SerializeField] private float lightningDamage = 0f;
    [SerializeField] private float darkDamage = 0f;
    [SerializeField] private float explosiveShotDamage = 0f;
    [SerializeField] private float steamVentDamage = 0f;
    [SerializeField] private float fissureFlareDamage = 0f;
    [SerializeField] private float ghostFlameDamage = 0f;
    [SerializeField] private float waveDamage = 0f;
    [SerializeField] private float chainLightningDamage = 0f;
    [SerializeField] private float poisonPuddleDamage = 0f;
    [SerializeField] private float stormDamage = 0f;
    [SerializeField] private float blackFlashDamage = 0f;
    [SerializeField] private float blackHoleDamage = 0f;

    [Header("UI Elements")]
    [SerializeField] private GameObject resultsMenu;
    [SerializeField] private float menuTransitionTime;
    [SerializeField] TextMeshProUGUI timeSurvivedText;
    [SerializeField] TextMeshProUGUI totalEnemiesKilledText;
    [SerializeField] TextMeshProUGUI wavesCompletedText;
    [SerializeField] TextMeshProUGUI loopsCompletedText;
    [SerializeField] TextMeshProUGUI soulsEarnedText;
    [SerializeField] TextMeshProUGUI playerStatsText;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        gameManager = FindAnyObjectByType<GameManager>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(resultsMenu.activeSelf == true && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.JoystickButton1)))
        {
            resultsMenu.SetActive(false);
            gameManager.ResetGame();
        }
        else if(resultsMenu.activeSelf == true && resultsMenu.transform.localScale != Vector3.one)
        {
            resultsMenu.transform.localScale = Vector3.Lerp(resultsMenu.transform.localScale, Vector3.one, Time.unscaledDeltaTime * menuTransitionTime);
        }
    }

    public void ActivateResultsMenu()
    {
        resultsMenu.SetActive(true);
        resultsMenu.transform.localScale = Vector3.zero;

        RecordGeneralResults();
        UpdateDisplay();
    }

    public void RecordFinalPlayerStats(int level, int health, int speed, float castStrength, float castSpeed, float dashStrength, float xPPullRange)
    {
        playerLevel = level;
        playerHealth = health;
        playerSpeed = speed;
        playercastStrength = castStrength;
        playercastSpeed = castSpeed;
        playerdashStrength = dashStrength;
        playerXPPullRange = xPPullRange;
    }

    public void RecordGeneralResults()
    {
        totalTimePlayed = gameManager.RunTime;
        wavesCompleted = gameManager.wavesCompleted + gameManager.loopsCompleted * 4;
        loopsCompleted = gameManager.loopsCompleted;
    }

    public void UpdateDisplay()
    {
        // Calculate Time
        int minutes = Mathf.FloorToInt(totalTimePlayed / 60f);
        int seconds = Mathf.FloorToInt(totalTimePlayed % 60f);

        // Results/General
        timeSurvivedText.text = "Time Survived: " + $"{minutes:00}:{seconds:00}";
        totalEnemiesKilledText.text = "Monsters Killed: " + totalEnemiesKilled;
        wavesCompletedText.text = "Waves Completed: " + wavesCompleted;
        loopsCompletedText.text = "Loops Completed: " + loopsCompleted;
        soulsEarnedText.text = "Souls Earned: " + soulsEarned;

        // Results/Stats
        playerStatsText.text = "Lvl: " + playerLevel + "\n" + "HP: " + playerHealth + "\n" + "SPD: " + playerSpeed + "\n"
            + "Cast STR: " + playercastStrength + "\n" + "Cast SPD: " + playercastSpeed + "\n" + "Dash STR: " + playerdashStrength + "\n" + "XP Pull: " + playerXPPullRange;
        
        // Results/Spell Damage

    }

    public void IncrementSoulsEarned()
    {
        soulsEarned++;
    }

    public void IncrementEnemiesKilled()
    {
        totalEnemiesKilled++;
    }

    public float GetTotalTimePlayed()
    {
        return totalTimePlayed;
    }

    public float GetWavesCompleted()
    {
        return wavesCompleted;
    }
    public float GetLoopsCompleted()
    {
        return loopsCompleted;
    }

    public int GetTotalSoulsEarned()
    {
        return soulsEarned;
    }

    public int GetTotalEnemiesKilled()
    {
        return totalEnemiesKilled;
    }
}
