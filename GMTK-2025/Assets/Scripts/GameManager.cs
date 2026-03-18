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
    public bool waitingForPortalReturn = false;
    private bool bossHasDied = false;
    public int loopsCompleted;
    public int wavesCompleted;
    public TextMeshProUGUI loopsAmountCompleted;
    public TextMeshProUGUI enemiesRemaining;
    public GameObject bossPrefab;
    public GameObject portalObject;
    public MapGenerator mapGenerator;

    public Spell.SpellType allocateSpell;
    public Sprite spellImage;

    public bool playerInSafeArea = false;

    void Awake()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        if (player == null)
            Debug.LogError("PlayerMovement not found in the scene.");

        if (!isInSafeArea)
            enemySpawner = FindFirstObjectByType<EnemySpawner>();

        if (portalObject != null)
            portalObject.SetActive(false);
    }

    void Update()
    {
        loopsAmountCompleted.text = loopsCompleted.ToString() + " Loops";
        Debug.Log($"Waves completed: {wavesCompleted} | waveCleared: {enemySpawner != null && enemySpawner.maxWavePopulation <= 0 && enemySpawner.currentEnemies <= 0} | isInSafeArea: {isInSafeArea} | bossAlive: {bossAlive} | bossHasDied: {bossHasDied}");

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
            Instantiate(bossPrefab, Vector3.zero, Quaternion.identity);
        }
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
            Debug.Log("Enemy spawning paused.");
        }

        Debug.Log("Portal opened! Waiting for player to return...");
    }

    public void OnPlayerReturnedFromPortal()
    {
        //mapGenerator.Generate();

        playerInSafeArea = false;

        waitingForPortalReturn = false;
        isInSafeArea = false;

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
        SceneManager.LoadScene("MainScene");
    }
}