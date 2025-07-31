using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownPlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveForce = 50f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private bool canCastMagic = true;
    [SerializeField] private Transform reticle; // Reference to the reticle script for aiming

    private Inventory inventory; // Reference to the inventory script
    private SpriteRenderer playerSprite; // Reference to the player's sprite renderer for flipping
    private Rigidbody2D rb;
    private Vector2 movement;

    void Awake()
    {
        playerSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        reticle = FindFirstObjectByType<Reticle>().GetComponent<Transform>();
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

    }

    void FixedUpdate()
    {
        // Only add force if under max speed
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(movement * moveForce);
        }

        // Flip the player to face the movement direction
        if (movement.x > 0)
        {
            playerSprite.flipX = true;
        }
        else if (movement.x < 0)
        {
            playerSprite.flipX = false;
        }
    }

    public void CastMagic(GameObject magicBallPrefab)
    {
        Instantiate(magicBallPrefab, reticle.GetChild(0).position, Quaternion.Euler(0f, 180f, 0f));

    }

}
