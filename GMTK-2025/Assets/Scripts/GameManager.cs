
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerMovement player; // Reference to the player movement script
    private EnemySpawner enemySpawner; // Reference to the enemy spawner script
    private UIManager uIManager;
    public bool betaMode;
    public int betaSpellCounter;
    public bool bossAlive = false; // Flag to check if a boss is alive
    public bool isInSafeArea = false; // Flag to check if the player is in a safe area
    public bool levelComplete = false;
    public bool loopComplete = false;
    public int loopsCompleted;
    public int wavesCompleted;
    public TextMeshProUGUI loopsAmountCompleted;
    public TextMeshProUGUI enemiesRemaining;
    public GameObject bossPrefab; // Reference to the boss prefab

    [Header("Spell Allocation")]
    // Reference for Upgrade Screen & Slot Allocation
    public Spell.SpellType allocateSpell; // Original Reference for Upgrade Screen
    public Sprite spellImage; // Original Reference for Upgrade Screen
    [SerializeField] private Sprite fireSprite;
    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite lightSprite;
    [SerializeField] private Sprite darkSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        uIManager = FindFirstObjectByType<UIManager>();
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
        if(betaMode)
        {
            SetEnemyPause(true);
            FindAnyObjectByType<InteractableLoopBar>();
            uIManager.SetBetaGameplay();
        }
    }

    // Update is called once per frame
    void Update()
    {
        loopsAmountCompleted.text = loopsCompleted.ToString() + " Loops";
        if (levelComplete)
        {
            levelComplete = false; // Reset level complete flag
            // SetExperience(player.experience); // Save player's experience when level is complete
        }
        else if (enemySpawner != null)
        {
            enemiesRemaining.text = enemySpawner.currentEnemies.ToString();
            if (enemySpawner.maxWavePopulation <= 0 && enemySpawner.currentEnemies <= 0 && !isInSafeArea && !bossAlive)
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
        if (player == null)
        {
            ResetGame();
        }
    }

    public void TrySpawnBoss()
    {
        if (loopsCompleted * 4 + 3 == wavesCompleted)
        {
            // Spawn a boss enemy
            bossAlive = true;   
            Instantiate(bossPrefab, Vector3.zero, Quaternion.identity);
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

    void ResetGame()
    {
        SceneManager.LoadScene("MainScene");
    }


}
