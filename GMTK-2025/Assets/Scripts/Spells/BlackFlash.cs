using UnityEngine;

public class BlackFlash : Spell
{
    [Header("Black Flash Settings")]
    [SerializeField] private float range;

    void Start()
    {
        Init();
        OrientSpell();

        // Scale the spell visually
        transform.localScale = new Vector3(range / 5f, range / 10f, 1f); // 5 is the base size, adjust as needed

        // Move it forward by half its length in world units
        float halfLength = GetComponent<SpriteRenderer>().bounds.extents.x;
        transform.position += transform.right * halfLength;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            SimpleEnemy enemy = other.GetComponent<SimpleEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
