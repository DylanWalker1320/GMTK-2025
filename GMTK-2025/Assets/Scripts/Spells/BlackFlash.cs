using UnityEngine;

public class BlackFlash : Spell
{
    [Header("Black Flash Settings")]
    [SerializeField] private float range;

    void Start()
    {
        Init();
        OrientSpell();

        // Move it forward by half its length in world units
        float halfLength = GetComponent<SpriteRenderer>().bounds.extents.x;
        transform.position += transform.right * halfLength;

        Destroy(gameObject, destroyTime);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
