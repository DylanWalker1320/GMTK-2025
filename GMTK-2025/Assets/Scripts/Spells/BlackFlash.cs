using UnityEngine;
using System.Collections;

public class BlackFlash : Spell
{
    [Header("Black Flash Settings")]
    [SerializeField] private float throwRange;
    [SerializeField] public float rotationsPerMinute;
    [SerializeField] private GameObject aoeEffectPrefab;
    [SerializeField] private float aoeRadius;

    [Header("Black Flash Upgrades")]
    [SerializeField] private float throwRangeUpgrade;
    [SerializeField] private float radiusUpgrade;
    [SerializeField] private float damageUpgrade;

    public enum Phase
    {
        Travel,
        Aoe,
        Return
    }

    private Phase currentPhase = Phase.Travel;
    private Vector3 targetPosition;
    private GameObject player;

    void Start()
    {
        Init();
        OrientSpell();
        AddUpgrade();

        player = GameObject.FindGameObjectWithTag("Player");

        targetPosition = Mathf.Min(Vector3.Distance(transform.position, mousePos) + transform.localScale.x / 2f, throwRange) * direction.normalized + transform.position; // Calculate the target position based on the throwRange
        targetPosition.z = 0; // Ensure the target position is on the same plane as the spell
    }

    void FixedUpdate()
    {
        transform.Rotate(0, 0, 6f * rotationsPerMinute * Time.deltaTime);

        switch (currentPhase)
        {
            case Phase.Travel:
                // Move the spell towards the target position
                Vector3 newPos = Vector3.MoveTowards(rb.position, targetPosition, speed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);

                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.angularVelocity = 0f;
                    currentPhase = Phase.Aoe;
                }

                break;
            case Phase.Aoe:
        
                Transform aoeEffect = Instantiate(aoeEffectPrefab, transform.position, Quaternion.identity).transform;
                aoeEffect.localScale = new Vector3(aoeRadius * 2, aoeRadius * 2, 1f); // Set the scale based on the aoeRadius

                Destroy(aoeEffect.gameObject, 0.5f); // This will be handled by animation events, but for now destroy it after a short delay

                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        Enemy enemy = collider.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.TakeDamage(CalculateDamage(damage, spellType1, spellType2) * 1.25f, damageColor); // Apply 25% more damage for AOE effect
                        }
                    }
                }

                targetPosition = player.transform.position; // Set target position to player's current position for return phase
                currentPhase = Phase.Return;

                break;
            case Phase.Return:
                // Move the spell back to the player            
                Vector3 returnPos = Vector3.MoveTowards(rb.position, player.transform.position, speed * Time.fixedDeltaTime * 2); // Move faster on return
                rb.MovePosition(returnPos); 

                if (Vector3.Distance(transform.position, player.transform.position) < 0.1f)
                {
                    Destroy(gameObject);
                }

                break;
        }
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
        int spellLevel = GetSpellLevel(Spells.BlackFlash);
        damage += damageUpgrade * spellLevel; // Increase damage by the upgrade value
        throwRange += throwRangeUpgrade * spellLevel; // Increase throwRange by the upgrade value
        aoeRadius += radiusUpgrade * spellLevel; // Increase aoeRadius by the upgrade value
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the target position to visualize the throwRange
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}
