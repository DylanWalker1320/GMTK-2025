using System.Collections;
using UnityEngine;

public class MagicBall : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private MagicBallType magicBallType;
    [SerializeField] private int speed;
    [SerializeField] private int destroyTime = 5;
    private Rigidbody2D magicRB;
    private TopDownPlayerMovement player;
    private Vector3 mousePos;
    private Camera mainCam;


    void Start()
    {
        magicRB = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<TopDownPlayerMovement>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;
        magicRB.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(direction.x, direction.y).normalized * speed; //normalized so that ball stays at a constant speed no matter how far mouse is from player
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg; //make a degree float
        transform.rotation = Quaternion.Euler(0, 0, rot - 180);
        StartCoroutine(DestructionTime(destroyTime));
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collisionObject = other.gameObject;
        if (collisionObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestructionTime(int waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        Destroy(gameObject);
    }
}

public enum MagicBallType
{
Fire,
Water
}