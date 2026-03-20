using UnityEngine;

public class Pond : MonoBehaviour
{
    [SerializeField] float healAmount = 1f;
    [SerializeField] float healDelay = 0.1f;
    private float healTimer = 0f;

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            FindAnyObjectByType<AudioManager>().Play("HealPond");
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
