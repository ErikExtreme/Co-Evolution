using UnityEngine;

/// <summary>
/// Tracks and records battle metrics for ModuleStatsTracker during gameplay.
/// This system records universal metrics that are fair across all module types.
/// 
/// Metrics recorded:
/// - damageEfficiency: Damage prevented per defensive stat invested
/// - survivalContribution: Fraction of battle that ship survived with this module
/// - offensiveSynergy: How well the module enabled weapon/drone performance
/// - powerEfficiency: Power resource management (available vs consumed)
/// - roleFulfillment: How well the module matched its genome mapping
/// - battleOutcome: Whether ship survived
/// </summary>
public class BattleMetricsRecorder : MonoBehaviour
{
    public static BattleMetricsRecorder Instance;

    private float battleStartTime;
    private float battleEndTime;
    private bool battleActive;

    void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Call at the start of a battle.
    /// </summary>
    public void BeginBattle()
    {
        battleStartTime = Time.time;
        battleActive = true;
    }

    /// <summary>
    /// Call at the end of a battle.
    /// </summary>
    public void EndBattle(bool playerSurvived)
    {
        battleEndTime = Time.time;
        battleActive = false;

        float battleDuration = battleEndTime - battleStartTime;

        // Record outcome for all active modules
        if (ModuleManager.Instance != null)
        {
            foreach (var (genome, tracker) in ModuleManager.Instance.modules)
            {
                if (tracker != null && genome != null)
                {
                    tracker.RegisterBattleOutcome(battleDuration, playerSurvived);
                }
            }
        }
    }

    /// <summary>
    /// Register damage mitigation for a specific module.
    /// Called when damage is reduced/prevented by a module's defensive stats.
    /// </summary>
    public void RegisterModuleDamageMitigation(int moduleId, float damagePreventedOrMitigated)
    {
        var (genome, tracker) = ModuleManager.Instance.GetModule(moduleId);
        if (tracker == null || genome == null)
            return;

        // Calculate stat investment: sum of all defensive contributions
        float statInvestment = genome.hullHP + (genome.armor * 20f) + (genome.shieldCapacity * 3f);

        tracker.RegisterDamageEfficiency(damagePreventedOrMitigated, statInvestment);
    }

    /// <summary>
    /// Register time the ship remained alive while this module was active.
    /// Should be called periodically or at battle end.
    /// </summary>
    public void RegisterModuleSurvivalTime(int moduleId, float timeSurvived, float totalBattleTime)
    {
        var (genome, tracker) = ModuleManager.Instance.GetModule(moduleId);
        if (tracker == null)
            return;

        tracker.RegisterSurvivalContribution(timeSurvived, totalBattleTime);
    }

    /// <summary>
    /// Register offensive synergy: how much this module enabled weapons to perform.
    /// synergScore should be normalized to [0, 1].
    /// 
    /// Calculation example:
    /// - gridSize modules: (gridWidth * gridHeight) / (maxGridWidth * maxGridHeight)
    /// - drone modules: (droneCount * droneStats) / (maxDroneCount * maxStats)
    /// - power modules: (powerAvailable - powerCost) / maxPowerCapacity
    /// </summary>
    public void RegisterModuleOffensiveSynergy(int moduleId, float synergyScore)
    {
        var (genome, tracker) = ModuleManager.Instance.GetModule(moduleId);
        if (tracker == null)
            return;

        tracker.RegisterOffensiveSynergy(synergyScore);
    }

    /// <summary>
    /// Register power efficiency for a module.
    /// Call this once per battle or periodically with accumulated stats.
    /// </summary>
    public void RegisterModulePowerEfficiency(int moduleId, float totalPowerAvailable, float totalPowerConsumed)
    {
        var (genome, tracker) = ModuleManager.Instance.GetModule(moduleId);
        if (tracker == null)
            return;

        tracker.RegisterPowerEfficiency(totalPowerAvailable, totalPowerConsumed);
    }

    /// <summary>
    /// Register role fulfillment for a module.
    /// Compares the module's actual mapping to its genome mapping.
    /// 
    /// Call once at battle start with the genome's intended mapping,
    /// then calculate distance between intended and actual performance.
    /// </summary>
    public void RegisterModuleRoleFulfillment(int moduleId, Vector3 genomeMapping, Vector3 performanceMapping)
    {
        var (genome, tracker) = ModuleManager.Instance.GetModule(moduleId);
        if (tracker == null)
            return;

        float mappingDistance = Vector3.Distance(genomeMapping, performanceMapping);
        tracker.RegisterRoleFulfillment(mappingDistance);
    }

    /// <summary>
    /// Convenience: Register all metrics for a module at once.
    /// Use this if you have all data available.
    /// </summary>
    public void RegisterModuleMetricsComplete(
        int moduleId,
        float damagePreventedPerStat,
        float survivalFraction,
        float offensiveSynergyScore,
        float powerEfficiencyRatio,
        float roleFulfillmentDistance,
        float battleDuration,
        bool shipSurvived)
    {
        var (genome, tracker) = ModuleManager.Instance.GetModule(moduleId);
        if (tracker == null)
            return;

        tracker.RegisterDamageEfficiency(damagePreventedPerStat, 1f); // Already normalized
        tracker.RegisterSurvivalContribution(survivalFraction, 1f); // Already normalized
        tracker.RegisterOffensiveSynergy(offensiveSynergyScore);
        tracker.RegisterPowerEfficiency(powerEfficiencyRatio, 1f); // Already normalized
        tracker.RegisterRoleFulfillment(roleFulfillmentDistance);
        tracker.RegisterBattleOutcome(battleDuration, shipSurvived);
    }
}
