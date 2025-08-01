using UnityEngine;
using System.Collections;

public class ExplosiveShot : Spell
{
    [Header("Explosive Shot Properties")]
    [SerializeField] private float explosionRadius = 5f; // Radius of the explosion
    [SerializeField] private float explosionDamage = 10f; // Damage dealt by the explosion
    [SerializeField] private float fadeDuration = 1f; // Duration for the explosion visual to fade out
    [SerializeField] private GameObject explosionVisual;
    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Spawn the spell at the reticle position
        Destroy(gameObject, destroyTime);; // Destroy the spell after a certain time
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SimpleEnemy enemy = collision.gameObject.GetComponent<SimpleEnemy>();
            if (enemy != null)
            {
                // Hide self and stop
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

                // Raycast to check for enemies in the explosion radius
                Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius / 2f);
                foreach (var hitEnemy in hitEnemies)
                {
                    if (hitEnemy.CompareTag("Enemy"))
                    {
                        SimpleEnemy simpleEnemy = hitEnemy.GetComponent<SimpleEnemy>();
                        if (simpleEnemy != null)
                        {
                            simpleEnemy.TakeDamage(explosionDamage); // Deal damage to each enemy hit by the explosion
                        }
                    }
                }

                explosionVisual.GetComponent<SpriteRenderer>().enabled = true; // Show the explosion visual
                explosionVisual.transform.localScale = new Vector3(2 * explosionRadius, 2 * explosionRadius, 1f); // Scale the explosion visual if needed

                StartCoroutine(Explode());
            }
        }
    }

    private IEnumerator Explode()
    {
        // Lerp the alpha of the explosion visual to create a fade-out effect
        SpriteRenderer explosionRenderer = explosionVisual.GetComponent<SpriteRenderer>();
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            explosionRenderer.color = new Color(explosionRenderer.color.r, explosionRenderer.color.g, explosionRenderer.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
