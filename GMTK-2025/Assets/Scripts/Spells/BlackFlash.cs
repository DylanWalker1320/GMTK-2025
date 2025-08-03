using UnityEngine;
using System.Collections;

public class BlackFlash : Spell
{
    [Header("Black Flash Settings")]
    [SerializeField] private float range;
    [Header("Black Flash Upgrades")]
    [SerializeField] private float rangeUpgrade = 0.5f; // Range increase per upgrade
    [SerializeField] private float damageUpgrade = 2;

    void Start()
    {
        Init();
        OrientSpell();
        AddUpgrade();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(CalculateDamage(damage, spellType1, spellType2));
            }
        }
    }

    public void AddUpgrade()
    {
        damage += damageUpgrade; // Increase damage by the upgrade value
        range += rangeUpgrade; // Increase range by the upgrade value
        transform.localScale = new Vector3(range, range, 1f); // Scale the spell based on the new range
    }

    public void DestroySpell()
    {
        Destroy(gameObject);
    }
}
