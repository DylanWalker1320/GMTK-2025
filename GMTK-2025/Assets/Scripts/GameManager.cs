using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private PlayerMovement player;
    private EnemySpawner enemySpawner;
    public bool bossAlive = false;
    public bool isInSafeArea = false;
    public bool levelComplete = false;
    public bool loopComplete = false;
    public bool waitingForPortalReturn = false; // True after boss dies, waiting for player to use portal
    public int loopsCompleted;
    public int wavesCompleted;
    public TextMeshProUGUI loopsAmountCompleted;
    public TextMeshProUGUI enemiesRemaining;
    public GameObject bossPrefab;
    public GameObject portalObject; // Assign the portal GameObject in Inspector — disabled by default

    public Spell.SpellType allocateSpell;
    public Sprite spellImage;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        if (player == null)
            Debug.LogError("PlayerMovement not found in the scene.");

        if (!isInSafeArea)
            enemySpawner = FindFirstObjectByType<EnemySpawner>();

        // Ensure portal starts disabled
        if (portalObject != null)
            portalObject.SetActive(false);
    }

    void Start() { }

    void Update()
    {
        loopsAmountCompleted.text = loopsCompleted.ToString() + " Loops";

        if (levelComplete)
        {
            levelComplete = false;
        }
        else if (enemySpawner != null && !waitingForPortalReturn)
        {
            enemiesRemaining.text = enemySpawner.currentEnemies.ToString();

            if (enemySpawner.maxWavePopulation <= 0 && enemySpawner.currentEnemies <= 0 && !isInSafeArea && !bossAlive)
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

        if (player == null)
            ResetGame();
    }

    public void TrySpawnBoss()
    {
        if (loopsCompleted * 4 + 3 == wavesCompleted)
        {
            bossAlive = true;
            Instantiate(bossPrefab, Vector3.zero, Quaternion.identity);
        }
    }


    // Called from the Boss script when the boss dies.
    public void OnBossDied()
    {
        bossAlive = false;
        waitingForPortalReturn = true;

        if (portalObject != null)
            portalObject.SetActive(true);

        // Stop any remaining enemy spawning while player is in refresh room
        if (enemySpawner != null)
            enemySpawner.SetSpawningPaused(true);
    }

    /// <summary>
    /// Called by the Portal when the player travels through it back to the main area.
    /// </summary>
    public void OnPlayerReturnedFromPortal()
    {
        waitingForPortalReturn = false;

        if (portalObject != null)
            portalObject.SetActive(false);

        // Resume spawning for the next loop
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
        SceneManager.LoadScene("MainScene");
    }
}