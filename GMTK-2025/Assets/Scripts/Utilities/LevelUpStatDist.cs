using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Enums for Rarity and StatType
public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum StatType
{
    Speed,
    Health,
    CastSpeed,
    CastStrength,
    SpellLevel
}

public class SpellLevelBonus
{
    public Spell.Spells spell;
    public int levelBonus;

    public SpellLevelBonus(Spell.Spells spell, int levelBonus)
    {
        this.spell = spell;
        this.levelBonus = levelBonus;
    }

    public override string ToString()
    {
        return $"+{levelBonus} to {spell}";
    }
}


public class LevelUpStatDist : MonoBehaviour
{
    public StatType type;
    public float value;
    public SpellLevelBonus spellLevelBonus;

    public void CreateStatWithoutSpellBonus(StatType type, float value)
    {
        this.type = type;
        this.value = value;
        this.spellLevelBonus = null;
    }

    public void CreateStatWithSpellBonus(StatType type, SpellLevelBonus spellBonus)
    {
        this.type = type;
        this.value = spellBonus.levelBonus;
        this.spellLevelBonus = spellBonus;
    }

    public override string ToString()
    {
        return type switch
        {
            StatType.CastSpeed => $"+{value * 100:F0}% Cast Speed",
            StatType.CastStrength => $"+{value * 100:F0}% Cast Strength",
            StatType.SpellLevel => spellLevelBonus != null ? spellLevelBonus.ToString() : $"+{value} Spell Level",
            StatType.Speed => $"+{value} Speed",
            StatType.Health => $"+{value} Health",
            _ => $"{type}: {value}"
        };        
    }
}

[SerializeField]
public class GeneratedStat
{
    public string statName;
    public Rarity rarity;
    public List<LevelUpStatDist> stats;

    public GeneratedStat(string name, Rarity rarity, List<LevelUpStatDist> stats)
    {
        this.statName = name;
        this.rarity = rarity;
        this.stats = stats;
    }
}

public static class LevelUpStatDefinitions
{
    // Rarity chances (out of 100)
    public static readonly Dictionary<Rarity, int> RarityWeights = new()
    {
        { Rarity.Common,    40 },
        { Rarity.Uncommon,  30 },
        { Rarity.Rare,      15 },
        { Rarity.Epic,      10 },
        { Rarity.Legendary, 5  }
    };

    // Number of stat lines per rarity
    public static readonly Dictionary<Rarity, int> StatCountByRarity = new()
    {
        { Rarity.Common,    1 },
        { Rarity.Uncommon,  1 },
        { Rarity.Rare,      2 },
        { Rarity.Epic,      2 },
        { Rarity.Legendary, 3 }
    };

    // Stat weights by rarity (Weights are in n% form because WeightedList uses int weights)
    public static readonly Dictionary<Rarity, Dictionary<StatType, int>> StatWeightsByRarity = new()
    {
        { 
            Rarity.Common, new Dictionary<StatType, int>
            {
                { StatType.Speed,        30 },
                { StatType.Health,       30 },
                { StatType.CastSpeed,    20 },
                { StatType.CastStrength, 20 }
            }
        },
        { 
            Rarity.Uncommon, new Dictionary<StatType, int>
            {
                { StatType.Speed,        25 },
                { StatType.Health,       25 },
                { StatType.CastSpeed,    20 },
                { StatType.CastStrength, 25 },
                { StatType.SpellLevel,   5  }
            }
        },
        { 
            Rarity.Rare, new Dictionary<StatType, int>
            {
                { StatType.Speed,        20 },
                { StatType.Health,       20 },
                { StatType.CastSpeed,    25 },
                { StatType.CastStrength, 25 },
                { StatType.SpellLevel,   10 }
            }
        },
        { 
            Rarity.Epic, new Dictionary<StatType, int>
            {
                { StatType.Speed,        20 },
                { StatType.Health,       20 },
                { StatType.CastSpeed,    20 },
                { StatType.CastStrength, 20 },
                { StatType.SpellLevel,   20 }
            }
        },
        { 
            Rarity.Legendary, new Dictionary<StatType, int>
            {
                { StatType.Speed,        10 },
                { StatType.Health,       10 },
                { StatType.CastSpeed,    20 },
                { StatType.CastStrength, 20 },
                { StatType.SpellLevel,   40 }
            }
        }
    };

    // Stat ranges by rarity and stat type
    public static float GetStatValue(StatType statType, Rarity rarity)
    {
        return statType switch
        {
            StatType.Speed => rarity switch
            {
                Rarity.Common =>    2f,
                Rarity.Uncommon =>  4f,
                Rarity.Rare =>      6f,
                Rarity.Epic =>      8f,
                Rarity.Legendary => 10f,
                _ => 2f
            },
            StatType.Health => rarity switch
            {
                Rarity.Common =>    Random.Range(5, 15),
                Rarity.Uncommon =>  Random.Range(5, 25),
                Rarity.Rare =>      Random.Range(10, 30),
                Rarity.Epic =>      Random.Range(20, 40),
                Rarity.Legendary => Random.Range(30, 50),
                _ => 5
            },
            StatType.CastSpeed => rarity switch
            {
                Rarity.Common =>    Random.Range(0.01f, 0.05f),
                Rarity.Uncommon =>  Random.Range(0.06f, 0.10f),
                Rarity.Rare =>      Random.Range(0.11f, 0.15f),
                Rarity.Epic =>      Random.Range(0.16f, 0.20f),
                Rarity.Legendary => Random.Range(0.21f, 0.25f),
                _ => 0.05f
            },
            StatType.CastStrength => rarity switch
            {
                Rarity.Common =>    Random.Range(0.01f, 0.05f),
                Rarity.Uncommon =>  Random.Range(0.06f, 0.10f),
                Rarity.Rare =>      Random.Range(0.11f, 0.20f),
                Rarity.Epic =>      Random.Range(0.21f, 0.30f),
                Rarity.Legendary => Random.Range(0.31f, 0.40f),
                _ => 0.05f
            },
            StatType.SpellLevel => rarity switch
            {
                Rarity.Uncommon =>  1f,
                Rarity.Rare =>      Random.Range(1f, 2f) >= 1.5f ? 2f : 1f, // 50% chance for +2
                Rarity.Epic =>      Random.Range(1f, 2f) >= 1.5f ? 2f : 1f, // 50% chance for +2
                Rarity.Legendary => Random.Range(1, 4),                     // +1 to +3
                _ => 0f
            },
            _ => 0f
        };
    }

    // Get a random spell for spell level bonuses
    public static Spell.Spells GetRandomSpell()
    {
        var allSpells = System.Enum.GetValues(typeof(Spell.Spells));
        return (Spell.Spells)allSpells.GetValue(Random.Range(0, allSpells.Length));
    }
}
