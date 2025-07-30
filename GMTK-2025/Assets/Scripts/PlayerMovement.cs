using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TopDownPlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from player
        movement.x = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right
        movement.y = Input.GetAxisRaw("Vertical");   // W/S or Up/Down
        movement = movement.normalized; // Prevent diagonal speed boost
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}
