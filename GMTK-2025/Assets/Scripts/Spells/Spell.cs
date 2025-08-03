using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Spell : MonoBehaviour
{
    public enum SpellType
    {
        Fire,
        Water,
        Lightning,
        Dark,
        None,
        General
    }

    public enum Spells
    {
        BlackFlash,
        BlackHole,
        ChainLightning,
        Dark,
        ExplosiveShot,
        Fireball,
        FissureFlare,
        Ghostflame,
        Lightning,
        PoisonPuddle,
        SteamVent,
        Storm,
        Waterball,
        Wave
    }

    protected static Dictionary<Spells, int> spellLevels = new Dictionary<Spells, int>
    {
        { Spells.BlackFlash, 0 },
        { Spells.BlackHole, 0 },
        { Spells.ChainLightning, 0 },
        { Spells.Dark, 0 },
        { Spells.ExplosiveShot, 0 },
        { Spells.Fireball, 0 },
        { Spells.FissureFlare, 0 },
        { Spells.Ghostflame, 0 },
        { Spells.Lightning, 0 },
        { Spells.PoisonPuddle, 0 },
        { Spells.SteamVent, 0 },
        { Spells.Storm, 0 },
        { Spells.Waterball, 0 },
        { Spells.Wave, 0 }
    };
    protected static Dictionary<SpellType, float> spellModifiers = new Dictionary<SpellType, float>
    {
        { SpellType.Fire, 1f },
        { SpellType.Water, 1f },
        { SpellType.Lightning, 1f },
        { SpellType.Dark, 1f },
        { SpellType.General, 1f }
    };

    [Header("Spell Properties")]
    [SerializeField] protected float destroyTime = 5f;
    [SerializeField] protected float damage = 2f; // Damage dealt by the spell
    [SerializeField] protected float speed;
    public SpellType spellType1;
    public SpellType spellType2;
    [SerializeField] public Sprite spellSprite; // Sprite for the spell
    protected Rigidbody2D rb;
    protected Vector3 mousePos;
    protected Camera mainCam;

    protected void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    public float GetDamage()
    {
        return damage;
    }

    protected void OrientSpell()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        Vector3 rotation = transform.position - mousePos;
        rb.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(direction.x, direction.y).normalized * speed; //normalized so that ball stays at a constant speed no matter how far mouse is from player
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg; //make a degree float
        transform.rotation = Quaternion.Euler(0, 0, rot - 180);
    }

    public static void UpgradeSpell(Spells spell)
    {
        if (spellLevels.ContainsKey(spell))
        {
            spellLevels[spell]++;
        }
    }

    public static void UpgradeModifier(SpellType type, float modifier)
    {
        if (spellModifiers.ContainsKey(type))
        {
            spellModifiers[type] += modifier;
        }
    }

    public static int GetSpellLevel(Spells spell)
    {
        if (spellLevels.ContainsKey(spell))
        {
            return spellLevels[spell];
        }
        return 0; // Return 0 if the spell is not found
    }

    public static float GetModifier(SpellType type)
    {
        if (spellModifiers.ContainsKey(type))
        {
            return spellModifiers[type];
        }
        return 0; // Return 0 if the type is not found
    }

    protected float CalculateDamage(float baseDamage, SpellType type1, SpellType type2)
    {
        // Prevent double counting of damage modifiers if both types are the same, or the second type is None
        float damage = baseDamage * spellModifiers[type1] * spellModifiers[SpellType.General];
        if (type1 != type2 && type2 != SpellType.None)
        {
            damage *= spellModifiers[type2]; // Also multiply by the second type's modifier if it's different from the first
        }
        return damage;
    }
}