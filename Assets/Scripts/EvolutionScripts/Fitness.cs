using System.Collections.Generic;
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

        // 2. Tracker performance
        float dmgNorm = Mathf.Clamp01(t.damageDealt / 5000f);
        float killNorm = Mathf.Clamp01(t.kills / 50f);
        float rangeNorm = Mathf.Clamp01(t.avgEffectiveRange / 40f);

        float trackerScore = (dmgNorm * 0.5f) + (killNorm * 0.3f) + (rangeNorm * 0.2f);

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

        // 2. Tracker performance
        float avoidNorm = Mathf.Clamp01(t.damageAvoided / 3000f);
        float powerNorm = Mathf.Clamp01(t.powerSaved / 2000f);
        float heatNorm = Mathf.Clamp01(t.heatReduced / 2000f);

        float trackerScore = (avoidNorm * 0.5f) + (powerNorm * 0.3f) + (heatNorm * 0.2f);

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

        // 1. Compute ideal ship vector from weapon vector
        Vector3 idealShip = Multiply(WeaponToShipMatrix, w);

        // 2. Compare ideal ship to actual ship population
        float totalDist = 0f;
        foreach (var ship in shipPopulation)
        {
            if (ship.mapping == Vector3.zero)
                ship.mapping = Mapping.MapGenome(ship);

            totalDist += Vector3.Distance(idealShip, ship.mapping);
        }

        float avgDist = totalDist / shipPopulation.Count;
        //float synergy = 1f / (1f + avgDist); // old
        float synergy = Mathf.Exp(-0.3f * avgDist); // new

        // 3. Player preference alignment
        Vector3 playerPref = Mapping.PlayerPreferenceWeaponMapping(player);
        float playerAlign = 1f - Vector3.Distance(w, playerPref);
        playerAlign = Mathf.Clamp01(playerAlign);

        // 4. Combine
        const float alpha = 0.7f;
        const float beta = 0.3f;

        return alpha * synergy + beta * playerAlign;
        //return synergy; // for the synergy test
    }

    public static float CooperativeFitness(ShipGenome offspring, List<WeaponGenome> weaponPopulation, PlayerBehaviorTracker player)
    {
        // Ensure mapping is cached
        if (offspring.mapping == Vector3.zero)
            offspring.mapping = Mapping.MapGenome(offspring);

        Vector3 s = offspring.mapping;

        // 1. Compute ideal weapon vector from ship vector
        Vector3 idealWeapon = Multiply(ShipToWeaponMatrix, s);

        // 2. Compare ideal weapon to actual weapon population
        float totalDist = 0f;
        foreach (var weapon in weaponPopulation)
        {
            if (weapon.mapping == Vector3.zero)
                weapon.mapping = Mapping.MapGenome(weapon);

            totalDist += Vector3.Distance(idealWeapon, weapon.mapping);
        }

        float avgDist = totalDist / weaponPopulation.Count;
        //float synergy = 1f / (1f + avgDist); // old
        float synergy = Mathf.Exp(-0.3f * avgDist); // new

        // 3. Player preference alignment
        Vector3 playerPref = Mapping.PlayerPreferenceShipMapping(player);
        float playerAlign = 1f - Vector3.Distance(s, playerPref);
        playerAlign = Mathf.Clamp01(playerAlign);

        // 4. Combine
        const float alpha = 0.7f;
        const float beta = 0.3f;

        return alpha * synergy + beta * playerAlign;
        //return synergy; // for the synergy test
    }
}
