using UnityEngine;

public class Pond : MonoBehaviour
{
    [SerializeField] float healAmount = 1f;
    [SerializeField] float healDelay = 0.1f;
    private float healTimer = 0f;


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FindAnyObjectByType<AudioManager>().Play("HealPond");   
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                healTimer += Time.deltaTime;
                if (healTimer >= healDelay)
                {
                    playerMovement.Heal(healAmount);
                    healTimer = 0f;
                }
            }
        }
    }
}
