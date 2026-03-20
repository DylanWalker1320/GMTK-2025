
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
            FindAnyObjectByType<InteractableLoopBar>();
            uIManager.SetBetaGameplay();
        }
        if (portalObject != null)
            portalObject.SetActive(false);
    }

    void Update()
    {
        loopsAmountCompleted.text = loopsCompleted.ToString() + " Loops";
        if (debugMode) Debug.Log($"Waves completed: {wavesCompleted} | waveCleared: {enemySpawner != null && enemySpawner.maxWavePopulation <= 0 && enemySpawner.currentEnemies <= 0} | isInSafeArea: {isInSafeArea} | bossAlive: {bossAlive} | bossHasDied: {bossHasDied}");

        if (levelComplete)
        {
            levelComplete = false;
        }
        else if (enemySpawner != null && !waitingForPortalReturn)
        {
            enemiesRemaining.text = enemySpawner.currentEnemies.ToString();

            bool waveCleared = enemySpawner.maxWavePopulation <= 0 && enemySpawner.currentEnemies <= 0;

            if (waveCleared && !isInSafeArea && !bossAlive)
            {
                if (bossHasDied)
                {
                    OpenPortal();
                }
                else
                {
                    levelComplete = true;
                    isInSafeArea = true;
                    wavesCompleted++;

                    if (wavesCompleted % 4 == 0)
                    {
                        loopComplete = true;
                        loopsCompleted++;
                    }
                }
            }
        }

        if (player == null)
            ResetGame();
    }

    public void TrySpawnBoss()
    {
        if (loopsCompleted * 4 + 3 == wavesCompleted)
        {
            bossAlive = true;
            Instantiate(bossPrefab, enemySpawner.GetValidSpawnPosition(), Quaternion.identity);
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

    private void OpenPortal()
    {
        // Count the wave and loop here for the boss wave
        wavesCompleted++;
        if (wavesCompleted % 4 == 0)
        {
            loopComplete = true;
            loopsCompleted++;
        }

        levelComplete = true;
        isInSafeArea = true;       // Safe area is true while waiting for portal return
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

    public void OnPlayerReturnedFromPortal()
    {
        //mapGenerator.Generate();

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