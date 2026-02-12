using UnityEngine;
using static GlobalSettings;

public static class Fitness
{
    private static float Match(float a, float b)
    {
        return 1f / (1f + Mathf.Abs(a - b));
    }

    public static float CalculateFitness(WeaponGenome genome, WeaponStatsTracker tracker, PlayerBehaviorTracker playerTracker)
    {
        float fitness = 0;

        return fitness;
    }

    public static float CalculateFitness(ShipGenome genome, ModuleStatsTracker tracker, PlayerBehaviorTracker playerTracker)
    {
        float fitness = 0;

        return fitness;
    }
    
}
