using UnityEngine;

public class EnemyMarker : MonoBehaviour
{
    [SerializeField] private float reticleDistance;
    public Transform target;
    private static Transform player;

    void Awake()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        if (target == null || player == null)
        {
            Debug.LogError("EnemyMarker: Target or Player is null. Destroying marker.");
            Destroy(gameObject);
            return;
        }

        Vector3 aimDirection = target.position - player.position;
        aimDirection.z = 0f;
        aimDirection = aimDirection.normalized;

        // reticle positioning
        transform.position = player.position + aimDirection * reticleDistance;

        // reticle rotation
        float rotZ = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg; // gives angle in radians using aim
        transform.rotation = Quaternion.Euler(0, 0, rotZ);   
    }
}
