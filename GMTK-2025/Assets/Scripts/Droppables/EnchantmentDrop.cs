using UnityEngine;
using System.Collections.Generic;

public class EnchantmentDrop : DroppableObject
{
    [SerializeField] public Sprite[] enchantmentSprites;
    private EnchantmentType enchantmentType;
    private Sprite enchantmentSprite; // Chosen sprite for the enchantment drop

    public enum EnchantmentType
    {
        None, // Keep as the first option so that the default value is None when not set
        Flame,
        Poison,
        Stun,
        Slow,
        LifeSteal,
        Blind,
        Mark,
    }

    public static Dictionary<EnchantmentType, string> enchantmentDescriptions = new Dictionary<EnchantmentType, string>
    {
        { EnchantmentType.None, "None"},
        { EnchantmentType.Flame, "Applies fire damage over time." },
        { EnchantmentType.Poison, "Applies poison damage over time." },
        { EnchantmentType.Stun, "Chance to stun enemies on hit." },
        { EnchantmentType.Slow, "Slows enemies on hit." },
        { EnchantmentType.LifeSteal, "Restores health based on damage dealt." },
        { EnchantmentType.Blind, "Chance to blind enemies on hit." },
        { EnchantmentType.Mark, "Marks enemies for increased damage from all sources." }
    };

    public static Dictionary<EnchantmentType, string> enchantmentMarkupColours = new Dictionary<EnchantmentType, string>
    {
        { EnchantmentType.None, "#FFF"},
        { EnchantmentType.Flame, "#FF4500" },
        { EnchantmentType.Poison, "#137813" }, 
        { EnchantmentType.Stun, "#FFFF00" },
        { EnchantmentType.Slow, "#376da2" }, 
        { EnchantmentType.LifeSteal, "#24ff14" }, 
        { EnchantmentType.Blind, "#dd2ddd" },
        { EnchantmentType.Mark, "#e22b2b" } 
    };

    void Start()
    {
        enchantmentType = (EnchantmentType)Random.Range(1, System.Enum.GetValues(typeof(EnchantmentType)).Length); // Range starts from 1 to avoid None
        enchantmentSprite = GetEnchantmentSprite(enchantmentType);

        // Set the sprite of the enchantment drop to the specified enchantment sprite
        GetComponent<SpriteRenderer>().sprite = enchantmentSprite;
    }

    private Sprite GetEnchantmentSprite(EnchantmentType type)
    {
        switch (type)
        {
            case EnchantmentType.Flame:
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

