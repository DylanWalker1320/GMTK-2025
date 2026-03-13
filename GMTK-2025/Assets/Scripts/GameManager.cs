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
    private bool bossHasDied = false; // Tracks boss death without opening portal yet
    public int loopsCompleted;
    public int wavesCompleted;
    public TextMeshProUGUI loopsAmountCompleted;
    public TextMeshProUGUI enemiesRemaining;
    public GameObject bossPrefab;
    public GameObject portalObject;

    public Spell.SpellType allocateSpell;
    public Sprite spellImage;

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

            bool waveCleared = enemySpawner.maxWavePopulation <= 0 && enemySpawner.currentEnemies <= 0;

            if (waveCleared && !isInSafeArea && !bossAlive)
            {
                // If the boss already died before the last enemy, open the portal now
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

        // Only open the portal if all regular enemies are also gone
        bool waveCleared = enemySpawner != null &&
                           enemySpawner.maxWavePopulation <= 0 &&
                           enemySpawner.currentEnemies <= 0;

        if (waveCleared)
            OpenPortal();
        // Otherwise Update() will catch it when the last enemy dies
    }

    private void OpenPortal()
    {
        waitingForPortalReturn = true;
        bossHasDied = false; // Reset for next loop 
        isInSafeArea = true;

        if (portalObject != null)
            portalObject.SetActive(true);

        if (enemySpawner != null)
            enemySpawner.SetSpawningPaused(true);
    }

    public void OnPlayerReturnedFromPortal()
    {
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