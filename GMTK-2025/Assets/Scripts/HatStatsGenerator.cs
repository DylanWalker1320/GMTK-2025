using System.Collections.Generic;
using UnityEngine;
using WeightedListSpace;

public static class HatStatsGenerator
{
    // Cached weighted lists for each rarity (initialized once)
    private static Dictionary<Rarity, WeightedList<StatType>> statWeightsByRarity;
    private static WeightedList<Rarity> rarityWeights;
    private static bool isInitialized = false;

    private static void EnsureInitialized()
    {
        if (isInitialized) return;

        // Initialize rarity weights
        rarityWeights = new WeightedList<Rarity>();
        foreach (var kvp in HatStatDefinitions.RarityWeights)
        {
            rarityWeights.Add(kvp.Key, kvp.Value);
        }

        // Initialize stat weights for each rarity
        statWeightsByRarity = new Dictionary<Rarity, WeightedList<StatType>>();
        foreach (var rarityKvp in HatStatDefinitions.StatWeightsByRarity)
        {
            var wl = new WeightedList<StatType>();
            foreach (var statKvp in rarityKvp.Value)
            {
                wl.Add(statKvp.Key, statKvp.Value);
            }
            statWeightsByRarity[rarityKvp.Key] = wl;
        }

        isInitialized = true;
    }

    public static GeneratedHat GenerateHatStats(string hatName = "Hat")
    {
        EnsureInitialized();
        
        Rarity rarity = rarityWeights.Next();
        int statCount = HatStatDefinitions.StatCountByRarity[rarity];
        List<HatStat> stats = GenerateStats(rarity, statCount);
        
        return new GeneratedHat(hatName, rarity, stats);
    }

    private static List<HatStat> GenerateStats(Rarity rarity, int count)
    {
        List<HatStat> stats = new List<HatStat>();
        HashSet<StatType> usedStats = new HashSet<StatType>();
        WeightedList<StatType> statPool = statWeightsByRarity[rarity];

        for (int i = 0; i < count; i++)
        {
            // Ensure no duplicate stats
            StatType statType;
            int attempts = 0;
            do
            {
                statType = statPool.Next();
                attempts++;
                if (attempts > 50) break; // Prevent infinite loop
            } while (usedStats.Contains(statType));

            if (!usedStats.Contains(statType))
            {
                usedStats.Add(statType);
                
                // Special handling for spell level stats
                if (statType == StatType.SpellLevel)
                {
                    int levelBonus = (int)HatStatDefinitions.GetStatValue(statType, rarity);
                    Spell.Spells randomSpell = HatStatDefinitions.GetRandomSpell();
                    SpellLevelBonus spellBonus = new SpellLevelBonus(randomSpell, levelBonus);
                    stats.Add(new HatStat(StatType.SpellLevel, spellBonus));
                }
                else
                {
                    float value = HatStatDefinitions.GetStatValue(statType, rarity);
                    stats.Add(new HatStat(statType, value));
                }
            }
        }

        return stats;
    }
}