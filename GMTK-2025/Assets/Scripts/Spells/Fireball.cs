using UnityEngine;

public class Fireball : Spell
{
    void Awake()
    {
        spellType.fire = true; // Set the spell type to water
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init(); // Initialize the spell properties
        DestroyAfter(destroyTime);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collisionObject = other.gameObject;
        if (collisionObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }
}
