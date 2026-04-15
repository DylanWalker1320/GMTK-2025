using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
abstract public class Enemy : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyStats
    {
        public float health;
        public float damage;
        public float speed;
    }

    protected Rigidbody2D rb;

    [SerializeField] private float applyForceCooldown = 1f; // Cooldown for applying force, to prevent physics issues
    public float applyForceTimer = 0f; // Timer for applying force
    public bool applyForceReady = true; // Flag to check if applying force is ready

    abstract public void TakeDamage(float damage);

    /// <summary>
    /// Applies an outside force to the enemy, used for the black hole. Has a short cooldown to prevent excessive forces being applied in a short time frame, which can cause physics issues.
    /// </summary>
    /// <param name="force"></param>
    public void ApplyForce(Vector2 force)
    {
        if (applyForceReady)
        {
            rb.AddForce(force, ForceMode2D.Impulse);
            applyForceReady = false;
            applyForceTimer = applyForceCooldown; // Reset the cooldown timer
        }
    }
}
