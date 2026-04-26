using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Boss : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] protected float attackCooldown = 1f;
    [SerializeField] protected float phaseChangeHealth;
    protected float attackCooldownTimer = 0f;
    

    void Start()
    {
        phaseChangeHealth = stats.health / 2f;
        Init();
    }
}
