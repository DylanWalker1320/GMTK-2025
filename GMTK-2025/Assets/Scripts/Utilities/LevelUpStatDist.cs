using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Enums for StatRarity and LevelUpStatType
public enum StatRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum LevelUpStatType
{
    Speed,
    Health,
    CastSpeed,
    CastStrength,
    SpellLevel
}

public class LevelUpSpellLevelBonus
{
    public Spell.Spells spell;
    public int levelBonus;

    public LevelUpSpellLevelBonus(Spell.Spells spell, int levelBonus)
    {
        this.spell = spell;
        this.levelBonus = levelBonus;
    }

    public override string ToString()
    {
        return $"+{levelBonus} to {spell}";
    }
}

[Serializable]
public class LevelUpStatDist // Needs Renaming. Class for individual stat containing their stat type and corresponding value for an externally defined rarity type
{
    public LevelUpStatType type;
    public float value;
    public LevelUpSpellLevelBonus LevelUpSpellLevelBonus;

    public LevelUpStatDist(LevelUpStatType type, float value)
    {
        this.type = type;
        this.value = value;
        this.LevelUpSpellLevelBonus = null;
    }

    public LevelUpStatDist(LevelUpStatType type, LevelUpSpellLevelBonus spellBonus)
    {
        this.type = type;
        this.value = spellBonus.levelBonus;
        this.LevelUpSpellLevelBonus = spellBonus;
    }

    public override string ToString()
    {
        return type switch
        {
            LevelUpStatType.CastSpeed => $"+{value * 100:F0}% Cast Speed",
            LevelUpStatType.CastStrength => $"+{value * 100:F0}% Cast Strength",
            LevelUpStatType.SpellLevel => LevelUpSpellLevelBonus != null ? LevelUpSpellLevelBonus.ToString() : $"+{value} Spell Level",
            LevelUpStatType.Speed => $"+{value} Speed",
            LevelUpStatType.Health => $"+{value} Health",
            _ => $"{type}: {value}"
        };        
    }
}

[SerializeField]
public class GeneratedStat // class for the generated stat list and the overall rarity
{
    public string statName;
    public StatRarity statRarity;
    public List<LevelUpStatDist> stats;

    public GeneratedStat(string name, StatRarity StatRarity, List<LevelUpStatDist> stats)
    {
        this.statName = name;
        this.statRarity = StatRarity;
        this.stats = stats;
    }
    public override string ToString()
    {
        string output = $"{statName} (Rarity: {statRarity})\nStats:\n";
        foreach (var stat in stats)
        {
            output += $"- {stat}\n";
        }
        return output;
    }
}

public static class LevelUpStatDefinitions
{
    // StatRarity chances (out of 100)
    public static readonly Dictionary<StatRarity, int> StatRarityWeights = new()
    {
        { StatRarity.Common,    40 },
        { StatRarity.Uncommon,  30 },
        { StatRarity.Rare,      15 },
        { StatRarity.Epic,      10 },
        { StatRarity.Legendary, 5  }
    };

    // Number of stat lines per StatRarity
    public static readonly Dictionary<StatRarity, int> StatCountByStatRarity = new()
    {
        { StatRarity.Common,    1 },
        { StatRarity.Uncommon,  1 },
        { StatRarity.Rare,      2 },
        { StatRarity.Epic,      2 },
        { StatRarity.Legendary, 3 }
    };

    // Stat weights by StatRarity (Weights are in n% form because WeightedList uses int weights)
    public static readonly Dictionary<StatRarity, Dictionary<LevelUpStatType, int>> StatWeightsByStatRarity = new()
    {
        { 
            StatRarity.Common, new Dictionary<LevelUpStatType, int>
            {
                { LevelUpStatType.Speed,        30 },
                { LevelUpStatType.Health,       30 },
                { LevelUpStatType.CastSpeed,    20 },
                { LevelUpStatType.CastStrength, 20 }
            }
        },
        { 
            StatRarity.Uncommon, new Dictionary<LevelUpStatType, int>
            {
                { LevelUpStatType.Speed,        25 },
                { LevelUpStatType.Health,       25 },
                { LevelUpStatType.CastSpeed,    20 },
                { LevelUpStatType.CastStrength, 25 },
                { LevelUpStatType.SpellLevel,   5  }
            }
        },
        { 
            StatRarity.Rare, new Dictionary<LevelUpStatType, int>
            {
                { LevelUpStatType.Speed,        20 },
                { LevelUpStatType.Health,       20 },
                { LevelUpStatType.CastSpeed,    25 },
                { LevelUpStatType.CastStrength, 25 },
                { LevelUpStatType.SpellLevel,   10 }
            }
        },
        { 
            StatRarity.Epic, new Dictionary<LevelUpStatType, int>
            {
                { LevelUpStatType.Speed,        20 },
                { LevelUpStatType.Health,       20 },
                { LevelUpStatType.CastSpeed,    20 },
                { LevelUpStatType.CastStrength, 20 },
                { LevelUpStatType.SpellLevel,   20 }
            }
        },
        { 
            StatRarity.Legendary, new Dictionary<LevelUpStatType, int>
            {
                { LevelUpStatType.Speed,        10 },
                { LevelUpStatType.Health,       10 },
                { LevelUpStatType.CastSpeed,    20 },
                { LevelUpStatType.CastStrength, 20 },
                { LevelUpStatType.SpellLevel,   40 }
            }
        }
    };

    // Stat ranges by StatRarity and stat type
    public static float GetStatValue(LevelUpStatType LevelUpStatType, StatRarity StatRarity)
    {
        return LevelUpStatType switch
        {
            LevelUpStatType.Speed => StatRarity switch
            {
                StatRarity.Common =>    2f,
                StatRarity.Uncommon =>  4f,
                StatRarity.Rare =>      6f,
                StatRarity.Epic =>      8f,
                StatRarity.Legendary => 10f,
                _ => 2f
            },
            LevelUpStatType.Health => StatRarity switch
            {
                StatRarity.Common =>    Random.Range(5, 15),
                StatRarity.Uncommon =>  Random.Range(5, 25),
                StatRarity.Rare =>      Random.Range(10, 30),
                StatRarity.Epic =>      Random.Range(20, 40),
                StatRarity.Legendary => Random.Range(30, 50),
                _ => 5
            },
            LevelUpStatType.CastSpeed => StatRarity switch
            {
                StatRarity.Common =>    Random.Range(0.01f, 0.05f),
                StatRarity.Uncommon =>  Random.Range(0.06f, 0.10f),
                StatRarity.Rare =>      Random.Range(0.11f, 0.15f),
                StatRarity.Epic =>      Random.Range(0.16f, 0.20f),
                StatRarity.Legendary => Random.Range(0.21f, 0.25f),
                _ => 0.05f
            },
            LevelUpStatType.CastStrength => StatRarity switch
            {
                StatRarity.Common =>    Random.Range(0.01f, 0.05f),
                StatRarity.Uncommon =>  Random.Range(0.06f, 0.10f),
                StatRarity.Rare =>      Random.Range(0.11f, 0.20f),
                StatRarity.Epic =>      Random.Range(0.21f, 0.30f),
                StatRarity.Legendary => Random.Range(0.31f, 0.40f),
                _ => 0.05f
            },
            LevelUpStatType.SpellLevel => StatRarity switch
            {
                StatRarity.Uncommon =>  1f,
                StatRarity.Rare =>      Random.Range(1f, 2f) >= 1.5f ? 2f : 1f, // 50% chance for +2
                StatRarity.Epic =>      Random.Range(1f, 2f) >= 1.5f ? 2f : 1f, // 50% chance for +2
                StatRarity.Legendary => Random.Range(1, 4),                     // +1 to +3
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
