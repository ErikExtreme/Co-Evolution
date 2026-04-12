using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GlobalSettings;

public static class Fitness
{
    // Weapon -> Ship compatibility matrix (3x3)
    private static readonly float[,] WeaponToShipMatrixOG = new float[,]
    {
    //   Xs (Dur/Mob)   Ys (Pow/Plat)   Zs (Sta/Agg)
    {   0.2f,          0.7f,           0.3f },   // Xw (Burst/Sustained)
    {  -0.4f,          0.8f,          -0.2f },   // Yw (Control/CQ)
    {   0.8f,          0.3f,           0.7f }    // Zw (Eff/Vol)
    };

    // Ship -> Weapon compatibility matrix (3x3)
    private static readonly float[,] ShipToWeaponMatrixOG = new float[,]
    {
    //   Xw (Burst/Sus)  Yw (Ctrl/CQ)   Zw (Eff/Vol)
    {   0.6f,          -0.5f,          0.7f },   // Xs (Dur/Mob)
    {   0.7f,           0.8f,          0.3f },   // Ys (Pow/Plat)
    {   0.3f,          -0.2f,          0.8f }    // Zs (Sta/Agg)
    };

    // In Fitness.cs, make these internal so test harness can touch them if needed
    internal static float[,] WeaponToShipMatrix = new float[,]
    {
    { 2f, 7f, 3f },
    { -4f, 8f, -2f },
    { 8f, 3f, 7f }
    };

    internal static float[,] ShipToWeaponMatrix = new float[,]
    {
    { 6f, -5f, 7f },
    { 7f,  8f, 3f },
    { 3f, -2f, 8f }
    };

    private static Vector3 Multiply(float[,] M, Vector3 v)
    {
        return new Vector3(
            M[0, 0] * v.x + M[0, 1] * v.y + M[0, 2] * v.z,
            M[1, 0] * v.x + M[1, 1] * v.y + M[1, 2] * v.z,
            M[2, 0] * v.x + M[2, 1] * v.y + M[2, 2] * v.z
        );
    }

    private static float Match(float a, float b)
    {
        return 1f / (1f + Mathf.Abs(a - b));
    }

    public static float IndividualFitness(WeaponGenome g, WeaponStatsTracker t, PlayerBehaviorTracker p)
    {
        // 1. Stat quality
        Vector3 axis = Mapping.MapGenome(g);
        float statScore = axis.magnitude / Mathf.Sqrt(3f);

        // 2. Tracker performance using new universal metrics
        // Normalize each metric to [0, 1] range with reasonable divisors
        
        // damageEfficiency: damage per power cost (higher is better)
        // Typical range: 10-100 damage per power cost
        float efficiencyNorm = Mathf.Clamp01(t.damageEfficiency / 50f);
        
        // heatManagementEfficiency: damage per heat (higher is better)
        // Typical range: 5-50 damage per heat
        float heatNorm = Mathf.Clamp01(t.heatManagementEfficiency / 25f);
        
        // hitRatio: already [0, 1], direct use
        float accuracyNorm = Mathf.Clamp01(t.hitRatio);
        
        // effectivenessPerCycle: damage per cycle time
        // Typical range: 10-100 damage per second
        float cycleNorm = Mathf.Clamp01(t.effectivenessPerCycle / 50f);
        
        // targetingTimeEfficiency: already [0, 1], direct use
        float utilizationNorm = Mathf.Clamp01(t.targetingTimeEfficiency);
        
        // targetUtility: already [0, 1], direct use
        float targetNorm = Mathf.Clamp01(t.targetUtility);
        
        // roleFulfillment: already [0, 1], direct use
        float roleNorm = Mathf.Clamp01(t.roleFulfillment);
        
        // survivalContribution: already [0, 1], direct use
        float survivalNorm = Mathf.Clamp01(t.survivalContribution);

        // Weighted tracker score: balance efficiency, accuracy, and outcome
        float trackerScore = 
            (efficiencyNorm * 0.2f) +        // Power efficiency
            (heatNorm * 0.15f) +             // Heat efficiency
            (accuracyNorm * 0.15f) +         // Hit ratio
            (cycleNorm * 0.15f) +            // Damage per cycle
            (utilizationNorm * 0.1f) +       // Time spent attacking
            (targetNorm * 0.1f) +            // Targets engaged
            (roleNorm * 0.05f) +             // Role fulfillment
            (survivalNorm * 0.1f);           // Survival contribution

        // 3. Player alignment
        Vector3 playerAxis = Mapping.PlayerPreferenceWeaponMapping(p);
        float alignment = Vector3.Dot(playerAxis, axis);
        float alignmentScore = (alignment + 1f) * 0.5f;

        // Final score
        float fitness =
            (statScore * 0.4f) +
            (trackerScore * 0.4f) +
            (alignmentScore * 0.2f);

        // Store components inside genome
        g.statScore = statScore;
        g.trackerScore = trackerScore;
        g.alignmentScore = alignmentScore;

        return Mathf.Clamp01(fitness);
    }

    public static float IndividualFitness(ShipGenome g, ModuleStatsTracker t, PlayerBehaviorTracker p)
    {
        // 1. Stat quality
        Vector3 axis = Mapping.MapGenome(g);
        float statScore = axis.magnitude / Mathf.Sqrt(3f);

        // 2. Tracker performance using new universal metrics
        // damageEfficiency: how much damage prevented per stat point invested
        float efficiencyNorm = Mathf.Clamp01(t.damageEfficiency / 50f);
        
        // survivalContribution: what fraction of battle kept ship alive
        float survivalNorm = Mathf.Clamp01(t.survivalContribution);
        
        // offensiveSynergy: how well this module enabled weapons
        float synergyNorm = Mathf.Clamp01(t.offensiveSynergy);
        
        // roleFulfillment: how well module matched its intended role
        float roleFulfillmentNorm = Mathf.Clamp01(t.roleFulfillment);

        // Weighted tracker score: emphasis on survival & efficiency
        float trackerScore = 
            (efficiencyNorm * 0.35f) +      // Damage prevention per stat
            (survivalNorm * 0.40f) +        // Overall survival contribution
            (synergyNorm * 0.15f) +         // Support for weapons
            (roleFulfillmentNorm * 0.10f);  // Role alignment

        // 3. Player alignment
        Vector3 playerAxis = Mapping.PlayerPreferenceShipMapping(p);
        float alignment = Vector3.Dot(playerAxis, axis);
        float alignmentScore = (alignment + 1f) * 0.5f;

        // Final score
        float fitness =
            (statScore * 0.4f) +
            (trackerScore * 0.4f) +
            (alignmentScore * 0.2f);

        // Store components inside genome
        g.statScore = statScore;
        g.trackerScore = trackerScore;
        g.alignmentScore = alignmentScore;

        return Mathf.Clamp01(fitness);
    }

    public static float CooperativeFitness(WeaponGenome offspring, List<ShipGenome> shipPopulation, PlayerBehaviorTracker player)
    {
        // Ensure mapping is cached
        if (offspring.mapping == Vector3.zero)
            offspring.mapping = Mapping.MapGenome(offspring);

        Vector3 w = offspring.mapping;

        // ---------------------------------------------------------
        // 1. Synergy (stronger influence)
        // ---------------------------------------------------------
        Vector3 idealShip = Multiply(WeaponToShipMatrix, w);

        float totalDist = 0f;
        foreach (var ship in shipPopulation)
        {
            if (ship.mapping == Vector3.zero)
                ship.mapping = Mapping.MapGenome(ship);

            totalDist += Vector3.Distance(idealShip, ship.mapping);
        }

        float avgDist = totalDist / shipPopulation.Count;

        // Stronger synergy shaping
        float synergy = Mathf.Exp(-0.6f * avgDist);
        // (was 0.3f, doubling the slope makes synergy differences matter more)

        // ---------------------------------------------------------
        // 2. Player preference alignment (unchanged)
        // ---------------------------------------------------------
        Vector3 playerPref = Mapping.PlayerPreferenceWeaponMapping(player);
        float playerAlign = 1f - Vector3.Distance(w, playerPref);
        playerAlign = Mathf.Clamp01(playerAlign);

        // ---------------------------------------------------------
        // 3. Centroid penalty (prevents collapse)
        // ---------------------------------------------------------
        Vector3 centroid = Vector3.zero;
        foreach (var ship in shipPopulation)
            centroid += ship.mapping;
        centroid /= shipPopulation.Count;

        float distToCentroid = Vector3.Distance(w, centroid);

        // Normalize: max possible distance in normalized axis space is sqrt(3)
        float centroidPenalty = 1f - (distToCentroid / Mathf.Sqrt(3f));
        centroidPenalty = Mathf.Clamp01(centroidPenalty);
        // High penalty near centroid -> low fitness

        // ---------------------------------------------------------
        // 4. Diversity pressure (reward being different)
        // ---------------------------------------------------------
        float diversitySum = 0f;
        foreach (var ship in shipPopulation)
            diversitySum += Vector3.Distance(w, ship.mapping);

        float avgDiversity = diversitySum / shipPopulation.Count;

        // Normalize diversity to [0,1]
        float diversityScore = Mathf.Clamp01(avgDiversity / 1.5f);

        // ---------------------------------------------------------
        // 5. Combine components
        // ---------------------------------------------------------
        const float A = 0.65f;  // synergy (increased)
        const float B = 0.20f;  // player alignment
        const float C = 0.10f;  // diversity pressure
        const float D = 0.05f;  // centroid penalty

        float fitness =
            (A * synergy) +
            (B * playerAlign) +
            (C * diversityScore) +
            (D * (1f - centroidPenalty));
        // (1 - penalty) means: far from centroid = good

        return Mathf.Clamp01(fitness);
    }

    public static float CooperativeFitness(ShipGenome offspring, List<WeaponGenome> weaponPopulation, PlayerBehaviorTracker player)
    {
        if (offspring.mapping == Vector3.zero)
            offspring.mapping = Mapping.MapGenome(offspring);

        Vector3 s = offspring.mapping;

        // ---------------------------------------------------------
        // 1. Synergy (stronger influence)
        // ---------------------------------------------------------
        Vector3 idealWeapon = Multiply(ShipToWeaponMatrix, s);

        float totalDist = 0f;
        foreach (var weapon in weaponPopulation)
        {
            if (weapon.mapping == Vector3.zero)
                weapon.mapping = Mapping.MapGenome(weapon);

            totalDist += Vector3.Distance(idealWeapon, weapon.mapping);
        }

        float avgDist = totalDist / weaponPopulation.Count;
        float synergy = Mathf.Exp(-0.6f * avgDist);

        // ---------------------------------------------------------
        // 2. Player preference alignment
        // ---------------------------------------------------------
        Vector3 playerPref = Mapping.PlayerPreferenceShipMapping(player);
        float playerAlign = 1f - Vector3.Distance(s, playerPref);
        playerAlign = Mathf.Clamp01(playerAlign);

        // ---------------------------------------------------------
        // 3. Centroid penalty
        // ---------------------------------------------------------
        Vector3 centroid = Vector3.zero;
        foreach (var weapon in weaponPopulation)
            centroid += weapon.mapping;
        centroid /= weaponPopulation.Count;

        float distToCentroid = Vector3.Distance(s, centroid);
        float centroidPenalty = 1f - (distToCentroid / Mathf.Sqrt(3f));
        centroidPenalty = Mathf.Clamp01(centroidPenalty);

        // ---------------------------------------------------------
        // 4. Diversity pressure
        // ---------------------------------------------------------
        float diversitySum = 0f;
        foreach (var weapon in weaponPopulation)
            diversitySum += Vector3.Distance(s, weapon.mapping);

        float avgDiversity = diversitySum / weaponPopulation.Count;
        float diversityScore = Mathf.Clamp01(avgDiversity / 1.5f);

        // ---------------------------------------------------------
        // 5. Combine
        // ---------------------------------------------------------
        const float A = 0.65f;
        const float B = 0.20f;
        const float C = 0.10f;
        const float D = 0.05f;

        float fitness =
            (A * synergy) +
            (B * playerAlign) +
            (C * diversityScore) +
            (D * (1f - centroidPenalty));

        return Mathf.Clamp01(fitness);
    }

    public static float ComputeNovelty(WeaponGenome genome, List<WeaponGenome> population, int k = 5)
    {
        // Ensure mapping is computed
        Vector3 gMap = genome.mapping;
        if (gMap == Vector3.zero)
            gMap = genome.mapping = Mapping.MapGenome(genome);

        List<float> distances = new List<float>();

        foreach (var other in population)
        {
            if (ReferenceEquals(other, genome))
                continue;

            Vector3 oMap = other.mapping;
            if (oMap == Vector3.zero)
                oMap = other.mapping = Mapping.MapGenome(other);

            float d = Vector3.Distance(gMap, oMap);
            distances.Add(d);
        }

        if (distances.Count == 0)
            return 0f;

        distances.Sort();

        int take = Mathf.Min(k, distances.Count);
        float sum = 0f;

        for (int i = 0; i < take; i++)
            sum += distances[i];

        float avg = sum / take;

        // Normalize novelty to [0,1]
        float novelty = Mathf.Clamp01(avg / Mathf.Sqrt(3f));
        return novelty;
    }

    public static float ComputeNovelty(ShipGenome genome, List<ShipGenome> population, int k = 5)
    {
        // Ensure mapping is computed
        Vector3 gMap = genome.mapping;
        if (gMap == Vector3.zero)
            gMap = genome.mapping = Mapping.MapGenome(genome);

        List<float> distances = new List<float>();

        foreach (var other in population)
        {
            if (ReferenceEquals(other, genome))
                continue;

            Vector3 oMap = other.mapping;
            if (oMap == Vector3.zero)
                oMap = other.mapping = Mapping.MapGenome(other);

            float d = Vector3.Distance(gMap, oMap);
            distances.Add(d);
        }

        if (distances.Count == 0)
            return 0f;

        distances.Sort();

        int take = Mathf.Min(k, distances.Count);
        float sum = 0f;

        for (int i = 0; i < take; i++)
            sum += distances[i];

        float avg = sum / take;

        // Normalize novelty to [0,1]
        float novelty = Mathf.Clamp01(avg / Mathf.Sqrt(3f));
        return novelty;
    }

    public static List<WeaponGenome> SelectDiverseTopX(List<WeaponGenome> population, int topX)
    {
        if (population == null || population.Count == 0)
            return new List<WeaponGenome>();

        // 1. Sort by finalFitness descending
        var sorted = population
            .OrderByDescending(g => g.finalFitness)
            .ToList();

        // 2. Start with the best genome
        List<WeaponGenome> selected = new List<WeaponGenome>();
        selected.Add(sorted[0]);
        sorted.RemoveAt(0);

        // 3. Pick the most diverse next
        while (selected.Count < topX && sorted.Count > 0)
        {
            WeaponGenome bestCandidate = null;
            float bestDistance = -1f;

            foreach (var g in sorted)
            {
                float minDist = float.MaxValue;

                foreach (var s in selected)
                {
                    float d = Vector3.Distance(g.mapping, s.mapping);
                    if (d < minDist)
                        minDist = d;
                }

                if (minDist > bestDistance)
                {
                    bestDistance = minDist;
                    bestCandidate = g;
                }
            }

            selected.Add(bestCandidate);
            sorted.Remove(bestCandidate);
        }

        return selected;
    }

    public static List<ShipGenome> SelectDiverseTopX(List<ShipGenome> population, int topX)
    {
        if (population == null || population.Count == 0)
            return new List<ShipGenome>();

        // 1. Sort by finalFitness descending
        var sorted = population
            .OrderByDescending(g => g.finalFitness)
            .ToList();

        // 2. Start with the best genome
        List<ShipGenome> selected = new List<ShipGenome>();
        selected.Add(sorted[0]);
        sorted.RemoveAt(0);

        // 3. Pick the most diverse next
        while (selected.Count < topX && sorted.Count > 0)
        {
            ShipGenome bestCandidate = null;
            float bestDistance = -1f;

            foreach (var g in sorted)
            {
                float minDist = float.MaxValue;

                foreach (var s in selected)
                {
                    float d = Vector3.Distance(g.mapping, s.mapping);
                    if (d < minDist)
                        minDist = d;
                }

                if (minDist > bestDistance)
                {
                    bestDistance = minDist;
                    bestCandidate = g;
                }
            }

            selected.Add(bestCandidate);
            sorted.Remove(bestCandidate);
        }

        return selected;
    }
}
