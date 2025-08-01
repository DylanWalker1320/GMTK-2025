using UnityEngine;

public class Waterball : Spell
{
    void Start()
    {
        Init(); // Initialize the spell properties
        OrientSpell(); // Orient the spell towards the mouse position and set its velocity
        DestroyAfter(destroyTime);
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
