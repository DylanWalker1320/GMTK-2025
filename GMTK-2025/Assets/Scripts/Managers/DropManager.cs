
using UnityEngine;

public class DropManager : MonoBehaviour
{
    [SerializeField] private bool debugMode;
    public static DropManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ProcessDrops(EnemyDropTable table, Vector3 position)
    {
        foreach(DropEntry entry in table.drops)
        {
            float chance = entry.probability;

            // If we have a luck multiplier, multiply chance here

            if(Random.value <= chance)
            {
                SpawnDrop(entry.dropObject, position);
                break;
            }
        }   
    }

    public void SpawnDrop(DroppableObject drop, Vector3 position)
    {

        if(drop == null)
        {
            return;
        }
        
        if(debugMode)
        {
            Debug.Log("Dropping: " + drop);   
        }
        
        Instantiate(drop, position, Quaternion.identity);
    }
}
