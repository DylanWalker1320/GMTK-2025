using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Boss : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] protected float attackCooldown = 1f;
    protected float attackCooldownTimer = 0f;
    

    void Start()
    {
        Init();
    }

    new protected void Die()
    {
        isDead = true;
        Instantiate(dropExperienceParticles, transform.position, Quaternion.identity);
        Instantiate(deathParticles, transform.position, Quaternion.identity);
        gameManager.EnemyKilled();
        gameManager.OnBossDied();
        Destroy(gameObject);
    }
    
    public override void TakeDamage(float damage, Color hitParticle = default)
    {
        hitFlashTimer = hitFlashDuration;
        StartCoroutine(HitFlash(Color.red, hitFlashDuration));
        audioManager.Play("EnemyHurt");


        // Spawn damage objects
        SpawnDamageNumber(damage);
        Instantiate(hitParticles, transform.position, Quaternion.identity);

        stats.health -= damage;
        if (stats.health <= 0f && isDead == false)
        {
            Die();
        }

        // Knockback
        if (target != null)
        {
            Vector2 knockbackDirection = (transform.position - target.position).normalized;
            rb.AddForce(knockbackDirection * 5f, ForceMode2D.Impulse);
        }
    } 


    private IEnumerator HitFlash(Color hitColor, float duration)
    {
        float elapsed = 0f;
        Color originalColor = Color.white;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float lerpT = Mathf.Clamp01(elapsed / duration);
            Color flashColor = hitColor;
            flashColor.a = 1f - lerpT;

            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(ColorProperty, flashColor);
            spriteRenderer.SetPropertyBlock(propertyBlock);

            yield return null;
        }

        // Ensure the color is reset at the end
        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(ColorProperty, new Color(1, 1, 1, 0));
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }
}
