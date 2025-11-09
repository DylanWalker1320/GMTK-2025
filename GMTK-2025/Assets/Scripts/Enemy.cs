using UnityEngine;

abstract public class Enemy : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyStats
    {
        public float health;
        public float damage;
        public float speed;
    }
    abstract public void TakeDamage(float damage);
}
