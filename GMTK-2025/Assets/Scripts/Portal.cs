using UnityEngine;
using Unity.Cinemachine;

public class Portal : MonoBehaviour
{
    public Transform location = null;
    [SerializeField] private bool isReturnPortal = false; // Enable on the portal that brings the player to the main area

    private static CinemachineFollow camera;
    private GameManager gameManager;

    void Start()
    {
        if (camera == null)
            camera = GameObject.FindGameObjectWithTag("MainCamera").transform.parent.gameObject.GetComponent<CinemachineFollow>();

        gameManager = FindFirstObjectByType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        camera.OnTargetObjectWarped(other.transform, location.position - other.transform.position);
        other.transform.position = location.position;

        // If this is the return portal, tell GameManager the player is back
        if (isReturnPortal && gameManager != null)
            gameManager.OnPlayerReturnedFromPortal();
        else if (gameManager != null)
            gameManager.playerInSafeArea = true;
    }
}