using UnityEngine;
using System.Collections;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private float damageCooldown = 1f; // Time in seconds between damage applications
    public static int damage = 10;
    private float lastDamageTime = 0f;
    private bool canDamage => Time.time >= lastDamageTime + damageCooldown; // Returns true if its been damageCooldown seconds since last damage
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (canDamage && other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.TakeDamage(damage);
                lastDamageTime = Time.time;
                animator.SetTrigger("Damage");
                StartCoroutine(WaitCooldown());
            }
        }

        if (canDamage && other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                lastDamageTime = Time.time;
                animator.SetTrigger("Damage");
                StartCoroutine(WaitCooldown());
            }
        }
    }

    IEnumerator WaitCooldown()
    {
        yield return new WaitForSeconds(damageCooldown);
        lastDamageTime = 0f;
    }
}
