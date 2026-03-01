using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SynergyTestHarness : MonoBehaviour
{
    [Header("Mock Population Sizes")]
    public int weaponCount = 200;
    public int shipCount = 200;

    [Header("Player Mocking")]
    public PlayerBehaviorTracker player; // assign in inspector, or create a dummy

    void Start()
    {
        RunSynergyTest();
    }

    void RunSynergyTest()
    {
        var weapons = MockPopulationGenerator.GenerateMockWeapons(weaponCount);
        var ships = MockPopulationGenerator.GenerateMockShips(shipCount);

        // --- Weapon-centric synergy ---
        List<float> weaponSynergies = new();
        foreach (var w in weapons)
        {
            float f = Fitness.CooperativeFitness(w, ships, player);
            weaponSynergies.Add(f);
        }

        // --- Ship-centric synergy ---
        List<float> shipSynergies = new();
        foreach (var s in ships)
        {
            float f = Fitness.CooperativeFitness(s, weapons, player);
            shipSynergies.Add(f);
        }

        LogStats("Weapon Synergy", weaponSynergies);
        LogStats("Ship Synergy", shipSynergies);

        // Optional: log top/bottom examples
        LogExtremes("Weapon", weapons, weaponSynergies);
        LogExtremes("Ship", ships, shipSynergies);
    }

    void LogStats(string label, List<float> values)
    {
        float min = values.Min();
        float max = values.Max();
        float avg = values.Average();
        float std = Mathf.Sqrt(values.Select(v => (v - avg) * (v - avg)).Average());

        Debug.Log($"{label} -> min: {min:F3}, max: {max:F3}, avg: {avg:F3}, std: {std:F3}");
    }

    void LogExtremes<T>(string label, List<T> genomes, List<float> fitness)
    {
        var indexed = genomes
            .Select((g, i) => new { Genome = g, Fit = fitness[i], Index = i })
            .OrderBy(x => x.Fit)
            .ToList();

        Debug.Log($"Lowest {label} synergies:");
        for (int i = 0; i < 5 && i < indexed.Count; i++)
        {
            Debug.Log($"{label} #{indexed[i].Index} fitness={indexed[i].Fit:F3}");
        }

        Debug.Log($"Highest {label} synergies:");
        for (int i = indexed.Count - 1; i >= 0 && i >= indexed.Count - 5; i--)
        {
            Debug.Log($"{label} #{indexed[i].Index} fitness={indexed[i].Fit:F3}");
        }
    }
}
