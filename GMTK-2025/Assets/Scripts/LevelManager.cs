using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private GameSettings gameSettings; // Reference to the game settings
    [SerializeField] bool isSafeArea; // Indicates next level is a safe area
    [SerializeField] bool isWaveArea; // Indicates next level is a wave area
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isSafeArea)
            {
                SceneManager.LoadScene("Upgrade Hub"); // Load the safe area scene
            }
            else if (isWaveArea)
            {
                SceneManager.LoadScene("OscarSpellScene"); // Load the wave area scene
            }
        }
    }
}
