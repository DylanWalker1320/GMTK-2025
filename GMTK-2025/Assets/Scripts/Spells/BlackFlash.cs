using UnityEngine;
using System.Collections;

public class BlackFlash : Spell
{
    [Header("Black Flash Settings")]
    [SerializeField] private float range;
    [SerializeField] public float rotationsPerMinute = 50f;

    [Header("Black Flash Upgrades")]
    [SerializeField] private float rangeUpgrade = 0.5f; // Range increase per upgrade
    [SerializeField] private float damageUpgrade = 2;

    public enum Phase
    {
        Travel,
        Aoe,
        Return
    }

    public Phase currentPhase = Phase.Travel;
    public Vector3 targetPosition;
    public GameObject player;
    public float aoeDuration = 1f; // Duration of the AOE phase
    public float aoeTimer = 0f; // Timer for the AOE phase

    void Start()
    {
        Init();
        OrientSpell();
        AddUpgrade();

        player = GameObject.FindGameObjectWithTag("Player");

        targetPosition = Mathf.Min(Vector3.Distance(transform.position, mousePos) + transform.localScale.x / 2f, range) * direction.normalized + transform.position; // Calculate the target position based on the range
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
                // Handle AOE logic here
                aoeTimer += Time.fixedDeltaTime;
                if (aoeTimer >= aoeDuration)
                {
                    currentPhase = Phase.Return;
                    aoeTimer = 0f;
                    targetPosition = player.transform.position; // Set target position to player's current position for return phase
                }

                break;
            case Phase.Return:
                // Move the spell back to the player            
                Vector3 returnPos = Vector3.MoveTowards(rb.position, player.transform.position, speed * Time.fixedDeltaTime);
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
            currentPhase = Phase.Aoe; // Transition to AOE phase when hitting an enemy

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
        range += rangeUpgrade * spellLevel; // Increase range by the upgrade value
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the target position to visualize the range
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, targetPosition);
    }
}
