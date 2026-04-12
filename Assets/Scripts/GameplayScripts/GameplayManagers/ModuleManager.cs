using System.Collections.Generic;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    public static ModuleManager Instance;

    public List<(ShipGenome genome, ModuleStatsTracker tracker)> modules = new List<(ShipGenome genome, ModuleStatsTracker tracker)>();

    [SerializeField] Inventory inventory;

    void Awake()
    {
        Instance = this;
    }

    public (ShipGenome genome, ModuleStatsTracker tracker) GetModule(int id)
    {
        if (modules.Count == 0)
            return (null, null);

        foreach (var module in modules)
        {
            if (module.genome.id == id) return module;
        }

        Debug.LogError("Could not find module with id: " + id);
        return (null, null);
    }

    public void AddModule(ShipGenome shipGenome, ModuleStatsTracker moduleStatsTracker)
    {
        modules.Add((shipGenome, moduleStatsTracker));

        inventory.AddModule(shipGenome, moduleStatsTracker);
    }

    /// <summary>
    /// Get all modules currently managed by this system.
    /// </summary>
    public List<(ShipGenome genome, ModuleStatsTracker tracker)> GetAllModules()
    {
        return new List<(ShipGenome, ModuleStatsTracker)>(modules);
    }

    /// <summary>
    /// Register metrics for all active modules at the end of a battle.
    /// Should be called by the battle/wave manager when combat ends.
    /// </summary>
    public void RegisterBattleMetricsForAll(float battleDuration, bool shipSurvived)
    {
        foreach (var (genome, tracker) in modules)
        {
            if (tracker != null && genome != null)
            {
                tracker.RegisterBattleOutcome(battleDuration, shipSurvived);
            }
        }
    }

    /// <summary>
    /// Clear all metrics for all modules (for starting a new battle).
    /// </summary>
    public void ResetMetricsForAll()
    {
        foreach (var (genome, tracker) in modules)
        {
            if (tracker != null)
            {
                // Keep genome/historical data, but reset battle-specific metrics
                tracker.damageEfficiency = 0f;
                tracker.survivalContribution = 0f;
                tracker.offensiveSynergy = 0f;
                tracker.powerEfficiency = 0f;
                tracker.roleFulfillment = 0f;
                tracker.battleDuration = 0f;
                tracker.survivalSuccess = 0f;
            }
        }
    }
}
