using System.Collections.Generic;
using UnityEngine;
using WeightedListSpace;

public static class LevelUpStatsGenerator
{
    private static Dictionary<StatRarity, WeightedList<LevelUpStatType>> levelUpStatWeightsByRarity;
    private static WeightedList<StatRarity> statRarityWeights;
    private static bool isInitialized = false;


    private static void EnsureWeightsInitialized()
    {
        if(isInitialized) return;

        // Initialize rarity weights into a weighted dictionary list
        statRarityWeights = new WeightedList<StatRarity>();
        foreach (var kvp in LevelUpStatDefinitions.StatRarityWeights)
        {
            statRarityWeights.Add(kvp.Key, kvp.Value);
        }

        // Initialize dicitonary containing a rarity along with the stat types and their weighted values for each corresponding rarity
        levelUpStatWeightsByRarity = new Dictionary<StatRarity, WeightedList<LevelUpStatType>>();

        // Reads through each Rarity and adds corresponding stats and weights to the rarity key kvp
        foreach (var kvp in LevelUpStatDefinitions.StatWeightsByStatRarity)
        {
            var weightedList = new WeightedList<LevelUpStatType>();
            foreach (var statKvp in kvp.Value)
            {
                weightedList.Add(statKvp.Key, statKvp.Value);
            }
            levelUpStatWeightsByRarity[kvp.Key] = weightedList;
        }

        isInitialized = true;
    }

    public static GeneratedStat GenerateStats(string statName = "LevelUpStat")
    {
        EnsureWeightsInitialized(); // Intialize list of weights for rarity and stats per rarity

        StatRarity rarity = statRarityWeights.Next(); // Generate a rarity by going to the next random index in rarity weight list
        int statCount = LevelUpStatDefinitions.StatCountByStatRarity[rarity]; // rarity stat line count
        List<LevelUpStatDist> stats = GenerateStatsWrapper(rarity, statCount); // Creates random stat upgrade with x lines using rarity weighting

        GeneratedStat generatedStat = new GeneratedStat(statName, rarity, stats); // Creates a stat with a rarity and a list of upgrade stats to go along

        return generatedStat; // returns stat to stat panel
    }

    private static List<LevelUpStatDist> GenerateStatsWrapper(StatRarity rarity, int count)
    {
        List<LevelUpStatDist> stats = new List<LevelUpStatDist>(); // List to contain generated stats
        HashSet<LevelUpStatType> usedStats = new HashSet<LevelUpStatType>(); // List of stats already used to prevent duplicate upgrade stats
        WeightedList<LevelUpStatType> levelUpStatPool = levelUpStatWeightsByRarity[rarity]; // The weighted stat pool affected by the current rarity

        for (int i = 0; i < count; i++) // Generates a stat for [count] lines
        {
            // Ensure no duplicate stats
            LevelUpStatType statType;
            int attempts = 0;
            do
            {
                statType = levelUpStatPool.Next();
                attempts++;
                if (attempts > 50) break; // Prevent infinite loop
            } while (usedStats.Contains(statType)); // Ensures a new stat type to add, possible chance to skip a line if all 50 attempts fail

            if (!usedStats.Contains(statType)) // If this type of stat has not been added to stats variable
            {
                usedStats.Add(statType); // Adds new stat type to pool of used up stats for this level up panel (context: upon level up, players can purchased panels ranging in rarity containing random stats with line amounts corresponding to rarity)

                // Special Handling for spell level stats
                if (statType == LevelUpStatType.SpellLevel) // Common rarity will not have SpellLevel StatType
                {
                    int levelBonus = (int) LevelUpStatDefinitions.GetStatValue(statType, rarity);
                    Spell.Spells randomSpell = LevelUpStatDefinitions.GetRandomSpell();
                    LevelUpSpellLevelBonus spellBonus = new LevelUpSpellLevelBonus(randomSpell, levelBonus);
                    stats.Add(new LevelUpStatDist(LevelUpStatType.SpellLevel, spellBonus));
                }
                else // Assign the stat value associated with this rarity's stat type to it and add it to the list of stat upgrades to be offered
                {
                    float value = LevelUpStatDefinitions.GetStatValue(statType, rarity);
                    stats.Add(new LevelUpStatDist(statType, value));
                }
            }
        }

        return stats;

    }
}