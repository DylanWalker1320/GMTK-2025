using System.Linq;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

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

    [Header("UI Elements/General")]
    [SerializeField] private GameObject resultsMenu;
    [SerializeField] private float menuTransitionTime;
    [SerializeField] TextMeshProUGUI timeSurvivedText;
    [SerializeField] TextMeshProUGUI totalEnemiesKilledText;
    [SerializeField] TextMeshProUGUI wavesCompletedText;
    [SerializeField] TextMeshProUGUI loopsCompletedText;
    [SerializeField] TextMeshProUGUI soulsEarnedText;
    [SerializeField] TextMeshProUGUI playerStatsText;

    [Header("UI Elements/Spell Damage")]
    [SerializeField] Transform topFiveContainer;
    [SerializeField] Transform generalSpellContainer;
    [SerializeField] private TopDamageEntry topSpellDamageUIPrefab;
    [SerializeField] private GameObject generalSpellDamagePrefab;

    // Spell Damage Section Dictionaries

    [SerializeField] private Dictionary<Spell.Spells, int> spellDamage = new();
    [SerializeField] private Dictionary<Spell.Spells, Sprite> spellSprites = new();
    [SerializeField] private Dictionary<Spell.Spells, Color> spellColors = new();


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

            // Sort
        List<KeyValuePair<Spell.Spells, int>> sortedSpellList = spellDamage.OrderByDescending(x => x.Value).ToList();
        
        int topFiveSpells = Mathf.Min(5, sortedSpellList.Count()); // Ensures there's a cap of 5 we iterate through
        int highestDamage = sortedSpellList.Count > 0 ? sortedSpellList[0].Value : 0;

        for (int i = 0; i < topFiveSpells; i++)
        {
            var currentSpellEntry = sortedSpellList[i];

            float sliderPercentage = (float) currentSpellEntry.Value / highestDamage;

            TopDamageEntry topEntry = Instantiate(topSpellDamageUIPrefab, topFiveContainer);
            topEntry.Setup(spellSprites[currentSpellEntry.Key], currentSpellEntry.Value, sliderPercentage, spellColors[currentSpellEntry.Key], i);


        }


    }

    public void RecordSpellDamage(Spell.Spells spell, float damage, Sprite sprite, Color color)
    {
        if(!spellDamage.ContainsKey(spell))
        {
            spellDamage.Add(spell, 0);

            // If anything, we could definitely add the sprites and colors to the spellDamage dictionary but... its troublesome atm lol
            spellSprites.Add(spell, sprite);
            spellColors.Add(spell, color);
        }

        spellDamage[spell] += (int) damage;
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
