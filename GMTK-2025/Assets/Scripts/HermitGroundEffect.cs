using UnityEngine;

public class HermitGroundEffect : MonoBehaviour
{
    float damage;
    float duration = 0.5f;

    public void Initialize(float damage)
    {
        this.damage = damage;
    }

    void Start()
    {
        Destroy(this.transform.parent.gameObject, duration);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage to player
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
