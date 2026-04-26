using System.Collections;
using UnityEngine;

public class HermitCrab : Enemy
{
    public bool isShooting = false;
    public bool isWalking = false;
    public Vector2 walkTarget;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject targetMarker;
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private float minShootDistance = 2f;
    [SerializeField] private float maxShootDistance = 10f;
    public float distanceToTarget;
    public static EnemySpawner enemySpawner;

    void Start()
    {
        Init();
        
        if (enemySpawner == null)
            enemySpawner = FindFirstObjectByType<EnemySpawner>();

        walkTarget = GetWalkTarget();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        distanceToTarget = Vector2.Distance(transform.position, target.position);
        bool inShootingRange = distanceToTarget < maxShootDistance && distanceToTarget > minShootDistance;

        if (inShootingRange && !isShooting)
        {
            // Interrupt walk and start shooting
            StopAllCoroutines();
            isWalking = false;
            isShooting = true;
            spriteRenderer.flipX = (target.position - transform.position).x > 0;
            StartCoroutine(Shoot());
        }
        else if (!inShootingRange && !isShooting && !isWalking)
        {
            // Find a new valid walk target and move
            walkTarget = GetWalkTarget();
            StartCoroutine(Walk());
        }
        else if (isShooting)
        {
            spriteRenderer.flipX = (target.position - transform.position).x > 0;

            if (!inShootingRange)
            {
                isShooting = false; // Shoot() coroutine will exit on next iteration
            }
        }
    }

    private Vector2 GetWalkTarget()
    {
        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        Vector2 directionToTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;
        
        // If too far, walk towards player; if too close, walk away
        float desiredDistance = distanceToTarget > maxShootDistance ? maxShootDistance - 0.5f : minShootDistance + 0.5f;
        Vector3 candidate = (Vector2)target.position - directionToTarget * desiredDistance;

        if (UnityEngine.AI.NavMesh.SamplePosition(candidate, out UnityEngine.AI.NavMeshHit hit, 1f, UnityEngine.AI.NavMesh.AllAreas))
        {
            agent.CalculatePath(hit.position, path);
            if (path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
                return hit.position;
        }

        Debug.LogWarning("HermitCrab: Failed to find valid walk target, defaulting to current position");
        return transform.position;
    }

    private IEnumerator Shoot()
    {
        while (isShooting)
        {
            // Spawn projectile
            // GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            // Vector2 shootDirection = (target.position - transform.position).normalized;
            // projectile.GetComponent<Projectile>().Initialize(shootDirection, stats.damage);

            // audioManager.Play("HermitCrabShoot");
            Debug.Log("HermitCrab shoots!");

            yield return new WaitForSeconds(shootCooldown);
        }
    }

    private IEnumerator Walk()
    {
        isWalking = true;
        agent.SetDestination(walkTarget);
        while (Vector2.Distance(transform.position, walkTarget) > 0.1f) {
            yield return null;
        }
        isWalking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minShootDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxShootDistance);
    }
}
