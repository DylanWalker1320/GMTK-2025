using UnityEngine;

public class DropItem : DroppableObject
{
    [SerializeField] private Color dropColor;
    [SerializeField] private string textToDisplay;

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
        player.SpawnBuffEffect(textToDisplay, dropColor);
        Destroy(gameObject);
    }
}
