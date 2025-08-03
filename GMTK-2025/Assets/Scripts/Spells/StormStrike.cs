using UnityEngine;

public class StormStrike : MonoBehaviour
{
    private float damage;
    public void Init(float damage, float destroyTime)
    {
        this.damage = damage;
        Destroy(gameObject, destroyTime); // Destroy the strike after a certain time
    }

    void Start()
    {
        // Flip randomly to add variety
        if (Random.value > 0.5f)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage); // Deal damage to the enemy
            }
        }
    }
}
