using UnityEngine;

public class DropItem : DroppableObject
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Collect(collision.GetComponent<PlayerMovement>());   
        }
    }

    public void Collect(PlayerMovement player)
    {
        player.PickUpDrop(dropType, powerValue);
        Destroy(gameObject);
    }
}
