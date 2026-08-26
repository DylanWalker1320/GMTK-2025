
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerMovement player; // Reference to the player movement script
    private EnemySpawner enemySpawner; // Reference to the enemy spawner script
    private UIManager uIManager;
    public bool betaMode;
    public bool debugMode;
    public int betaSpellCounter;
    public bool bossAlive = false; // Flag to check if a boss is alive
    public bool isInSafeArea = false; // Flag to check if the player is in a safe area
    public bool levelComplete = false;
    public bool loopComplete = false;
    public bool waitingForPortalReturn = false;
    private bool bossHasDied = false;
    public int loopsCompleted;
    public int wavesCompleted;
    public TextMeshProUGUI loopsAmountCompleted;
    public TextMeshProUGUI enemiesRemaining;
    public GameObject bossPrefab;
    public GameObject portalObject;
    public MapGenerator mapGenerator;

    [Header("Spell Allocation")]
    // Reference for Upgrade Screen & Slot Allocation
    public Spell.SpellType allocateSpell; // Original Reference for Upgrade Screen
    public Sprite spellImage; // Original Reference for Upgrade Screen
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite lightSprite;
    [SerializeField] private Sprite darkSprite;

    public bool playerInSafeArea = false;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        uIManager = FindFirstObjectByType<UIManager>();
        if (player == null)
            if (debugMode) Debug.LogError("PlayerMovement not found in the scene.");

        if (!isInSafeArea)
            enemySpawner = FindFirstObjectByType<EnemySpawner>();

    }
    void Start()
    {
        if(betaMode)
        {
            SetEnemyPause(true);
            uIManager.SetBetaGameplay();
        }
        if (portalObject != null)
            portalObject.SetActive(false);

        enemiesRemaining.text = $"{enemySpawner.maxWavePopulation - enemySpawner.currentEnemies} / {enemySpawner.maxWavePopulation}";
    }

    void Update()
    {
        loopsAmountCompleted.text = loopsCompleted.ToString() + " Loops";
        if (debugMode) Debug.Log($"Waves completed: {wavesCompleted} | isInSafeArea: {isInSafeArea} | bossAlive: {bossAlive} | bossHasDied: {bossHasDied}");

        if (levelComplete)
        {
            levelComplete = false;
        }
        else if (enemySpawner != null && !waitingForPortalReturn && !bossAlive)
        {
            bool waveCleared = enemySpawner.maxWavePopulation <= 0 && enemySpawner.currentEnemies <= 0;

            if (waveCleared && !isInSafeArea)
            {
                if (bossHasDied)
                {
                    OpenPortal();
                }
                else
                {
                    wavesCompleted++;

                    if (wavesCompleted >= 4)
                    {
                        // 4 normal waves done this loop -> boss wave, no safe area in between
                        TrySpawnBoss();
                    }
                    else
                    {
                        levelComplete = true;
                        isInSafeArea = true;
                    }
                }
            }
        }

        if (player == null)
            ResetGame();
    }

    // Called by the main canvas' UIManager's "On Shop Finish" event
    public void OnShopFinished()
    {
        if (wavesCompleted >= 4 && !bossHasDied)
        {
            TrySpawnBoss();
        }
        else
        {
            enemySpawner.Restart();
        }
    }

    public void TrySpawnBoss()
    {
        if (bossAlive || waitingForPortalReturn) return;
        if (wavesCompleted < 4) return; // not time for the boss yet
        if (enemySpawner != null && (enemySpawner.maxWavePopulation > 0 || enemySpawner.currentEnemies > 0))
            return; // regular enemies still active

        bossAlive = true;
        SetEnemyPause(true);
        enemySpawner.maxWavePopulation = 0; // boss wave = boss only
        Instantiate(bossPrefab, enemySpawner.GetValidSpawnPosition(), Quaternion.identity);
    }

    private void OpenPortal()
    {
        loopComplete = true;
        loopsCompleted++;
        wavesCompleted = 0; // reset counter for the next loop

        levelComplete = true;
        isInSafeArea = true;
        waitingForPortalReturn = true;
        bossHasDied = false;

        if (portalObject != null)
            portalObject.SetActive(true);

        if (enemySpawner != null)
        {
            enemySpawner.SetSpawningPaused(true);
            if (debugMode) Debug.Log("Enemy spawning paused.");
        }

        if (debugMode) Debug.Log("Portal opened! Waiting for player to return...");
    }
    public void UpdateEnemiesRemaining()
    {
        enemiesRemaining.text = $"{enemySpawner.mobsKilled}\n /\n {enemySpawner.lastMaxWavePopulation}";
    }

    public void EnemyKilled()
    {
        if (enemySpawner != null)
        {
            enemySpawner.mobsKilled++;
            enemySpawner.currentEnemies--;
            UpdateEnemiesRemaining();
        }
    }

    public void ChangeSpellAllocation()
    {
        int rand = Random.Range(1, 5);

        switch(rand)
        {
            case 1:
                allocateSpell = Spell.SpellType.Fire;
                spellImage = fireSprite;
                break;
            case 2:
                allocateSpell = Spell.SpellType.Water;
                spellImage = waterSprite;
                break;
            case 3:
                allocateSpell = Spell.SpellType.Lightning;
                spellImage = lightSprite;
                break;
            case 4:
                allocateSpell = Spell.SpellType.Dark;
                spellImage = darkSprite;
                break;
        }
    }

    public void SetEnemyPause(bool value) // used for other scripts to access enemyspawner if they're only allowed to have gamemanager access
    {
        enemySpawner.SetSpawningPaused(value);
    }
    public void OnBossDied()
    {
        bossAlive = false;
        bossHasDied = true;
    }

    public void OnPlayerReturnedFromPortal()
    {
        mapGenerator.Generate();

        playerInSafeArea = false;

        waitingForPortalReturn = false;
        isInSafeArea = false;
        loopComplete = false;
        levelComplete = false;

        if (portalObject != null)
            portalObject.SetActive(false);

        if (enemySpawner != null)
        {
            enemySpawner.SetSpawningPaused(false);
            enemySpawner.Restart();
        }
        Statue.TogglePurchaseAvailability(false);
    }

    public void ToggleSafeArea(bool isInSafeArea)
    {
        this.isInSafeArea = isInSafeArea;
    }

    void ResetGame()
    {
        betaMode = true;
        SceneManager.LoadScene("MainScene");
    }
}