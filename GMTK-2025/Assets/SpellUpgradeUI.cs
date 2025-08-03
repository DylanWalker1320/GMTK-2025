using UnityEngine;

public class SpellUpgradeUI : MonoBehaviour
{

    private GameManager gameManager; // Reference to the GameManager script
    private PlayerMovement player; // Reference to the PlayerMovement script
    private UIManager uiManager;

    void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        player = FindFirstObjectByType<PlayerMovement>();
        uiManager = FindFirstObjectByType<UIManager>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
