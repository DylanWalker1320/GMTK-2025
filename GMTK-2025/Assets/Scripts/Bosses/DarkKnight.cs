using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DarkKnight : Boss
{
    [SerializeField] private Phase currentPhase = Phase.Phase1;
    [SerializeField] private Attack currentAttack = Attack.Walk;

    private Vector2 movement;
    private GameObject target;
    private Rigidbody2D rb;
    private float delayTime = 0;

    private enum Phase
    {
        Phase1,
        Phase2
    }

    private enum Attack
    {
        BigSwipers,
        BigSlammer,
        Walk,
        FastSwipers,
        GigaSlammer,
        Run
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player");

        //Play intro animation
        //todo
    }

    Attack ChooseAttack()
    {
        Attack atk = Attack.Walk;
        switch (currentPhase)
        {
            case Phase.Phase1:
                switch (Random.Range(0, 3)) // Randomly choose between 0, 1, or 2
                {
                    case 0:
                        atk = Attack.BigSwipers;
                        break;
                    case 1:
                        atk = Attack.BigSlammer;
                        break;
                    case 2:
                        atk = Attack.Walk;
                        break;
                }
                break;

            case Phase.Phase2:
                switch (Random.Range(0, 3))
                {
                    case 0:
                        atk = Attack.FastSwipers;
                        break;
                    case 1:
                        atk = Attack.GigaSlammer;
                        break;
                    case 2:
                        atk = Attack.Run;
                        break;
                }
                break;

            default:
                return Attack.Walk; // Fallback
        }

        if (atk == currentAttack)
        {
            return ChooseAttack(); // Ensure the attack is not the same as the last one. Awful recursion but it works for now
        }

        return atk;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        UpdatePhase();
        DoAttacks();

        // Temporary movement logic for testing
        Vector2 direction = (target.transform.position - transform.position).normalized;

        if (rb.linearVelocity.magnitude < maxMoveSpeed)
        {
            rb.AddForce(direction * moveSpeed);
        }

    }

    void DoAttacks()
    {

        // For now, just wait for a time
        if (delayTime < 2f)
        {
            delayTime += Time.fixedDeltaTime;
            return; // Wait for the next attack
        }
        else
        {
            delayTime = 0; // Reset delay time
            currentAttack = ChooseAttack();
        }

        switch (currentAttack)
        {
            case Attack.BigSwipers:
                BigSwipers();
                break;

            case Attack.BigSlammer:
                BigSlammer();
                break;

            case Attack.Walk:
                Walk();
                break;

            case Attack.FastSwipers:
                FastSwipers();
                break;

            case Attack.GigaSlammer:
                GigaSlammer();
                break;

            case Attack.Run:
                Run();
                break;
        }
    }

    void BigSwipers()
    {
        // Implement BigSwipers attack behavior
    }

    void BigSlammer()
    {
        // Implement BigSlammer attack behavior
    }

    void Walk()
    {
        // Implement Walk attack behavior
    }

    void FastSwipers()
    {
        // Implement FastSwipers attack behavior
    }

    void GigaSlammer()
    {
        // Implement GigaSlammer attack behavior
    }

    void Run()
    {
        // Implement Run attack behavior
    }

    void UpdatePhase()
    {
        if (health <= phaseChangeHealth && currentPhase == Phase.Phase1)
        {
            currentPhase = Phase.Phase2;
            Debug.Log("Phase changed to Phase 2");
            // Play phase change animation or effects
        }
    }
}
