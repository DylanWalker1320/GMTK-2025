using UnityEngine;

public class EnchantmentDrop : DroppableObject
{
    [SerializeField] public Sprite[] enchantmentSprites;
    private EnchantmentType enchantmentType;
    private Sprite enchantmentSprite; // Chosen sprite for the enchantment drop

    public enum EnchantmentType
    {
        Fire,
        Poison,
        Stun,
        Slow,
        LifeSteal,
        Blind,
        Mark,
        Speed
    }

    void Start()
    {
        enchantmentType = (EnchantmentType)Random.Range(0, System.Enum.GetValues(typeof(EnchantmentType)).Length);
        enchantmentSprite = GetEnchantmentSprite(enchantmentType);

        // Set the sprite of the enchantment drop to the specified enchantment sprite
        GetComponent<SpriteRenderer>().sprite = enchantmentSprite;
    }

    private Sprite GetEnchantmentSprite(EnchantmentType type)
    {
        switch (type)
        {
            case EnchantmentType.Fire:
                return enchantmentSprites[0];
            case EnchantmentType.Poison:
                return enchantmentSprites[1];
            case EnchantmentType.Stun:
                return enchantmentSprites[2];
            case EnchantmentType.Slow:
                return enchantmentSprites[3];
            case EnchantmentType.LifeSteal:
                return enchantmentSprites[4];
            case EnchantmentType.Blind:
                return enchantmentSprites[5];
            case EnchantmentType.Mark:
                return enchantmentSprites[6];
            case EnchantmentType.Speed:
                return enchantmentSprites[7];
            default:
                return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Collect(collision.GetComponent<PlayerMovement>());   
        }
    }

    public void Collect(PlayerMovement player)
    {
        player.PickUpEnchantment(enchantmentType, enchantmentSprite);
        Destroy(gameObject);
    }
}

