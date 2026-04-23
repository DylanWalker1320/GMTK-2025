using UnityEngine;

public class NavEnemy : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent agent;
    Transform player;
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }
    void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }
}
