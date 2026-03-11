using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SynergySensitivityTest : MonoBehaviour
{
    public int weaponCount = 200;
    public int shipCount = 200;
    public PlayerBehaviorTracker player;

    void Start()
    {
        RunSensitivity();
    }

    void RunSensitivity()
    {
        var weapons = MockPopulationGenerator.GenerateMockWeapons(weaponCount);
        var ships = MockPopulationGenerator.GenerateMockShips(shipCount);

        // Baseline
        var baseline = ComputeWeaponSynergies(weapons, ships);

        // Perturb each matrix entry by ±20%
        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 3; c++)
            {
                float original = Fitness.WeaponToShipMatrix[r, c];

                // +20%
                Fitness.WeaponToShipMatrix[r, c] = original * 1.2f;
                var plus = ComputeWeaponSynergies(weapons, ships);
                float corrPlus = RankCorrelation(baseline, plus);
                Debug.Log($"WeaponToShip[{r},{c}] +20% -> rank corr={corrPlus:F3}");

                // -20%
                Fitness.WeaponToShipMatrix[r, c] = original * 0.8f;
                var minus = ComputeWeaponSynergies(weapons, ships);
                float corrMinus = RankCorrelation(baseline, minus);
                Debug.Log($"WeaponToShip[{r},{c}] -20% -> rank corr={corrMinus:F3}");

                // restore
                Fitness.WeaponToShipMatrix[r, c] = original;
            }
        }
    }

    List<float> ComputeWeaponSynergies(List<WeaponGenome> weapons, List<ShipGenome> ships)
    {
        var list = new List<float>(weapons.Count);
        foreach (var w in weapons)
            list.Add(Fitness.CooperativeFitness(w, ships, player));
        return list;
    }

    float RankCorrelation(List<float> a, List<float> b)
    {
        // Very rough Spearman-like: correlation of ranks
        int n = a.Count;
        var ra = a.Select((v, i) => new { v, i }).OrderBy(x => x.v).Select((x, rank) => (x.i, rank)).ToDictionary(x => x.i, x => x.rank);
        var rb = b.Select((v, i) => new { v, i }).OrderBy(x => x.v).Select((x, rank) => (x.i, rank)).ToDictionary(x => x.i, x => x.rank);

        float mean = (n - 1) / 2f;
        float num = 0f, denA = 0f, denB = 0f;
        for (int i = 0; i < n; i++)
        {
            float da = ra[i] - mean;
            float db = rb[i] - mean;
            num += da * db;
            denA += da * da;
            denB += db * db;
        }
        return num / Mathf.Sqrt(denA * denB + 1e-6f);
    }
}
