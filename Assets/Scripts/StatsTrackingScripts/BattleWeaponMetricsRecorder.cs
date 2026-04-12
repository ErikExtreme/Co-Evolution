using UnityEngine;

/// <summary>
/// Tracks and records battle metrics for WeaponStatsTracker during gameplay.
/// This system records universal metrics that are fair across all weapon types.
/// 
/// Metrics recorded:
/// - damageEfficiency: Damage per power cost
/// - heatManagementEfficiency: Damage per heat generated
/// - hitRatio: Successful hits / total shots
/// - effectivenessPerCycle: Damage per cooldown cycle
/// - targetingTimeEfficiency: Time spent firing / total battle time
/// - targetUtility: Enemies engaged / total enemies
/// - survivalContribution: Weapon helped keep ship alive
/// - roleFulfillment: Weapon matched its genome mapping
/// </summary>
public class BattleWeaponMetricsRecorder : MonoBehaviour
{
    public static BattleWeaponMetricsRecorder Instance;

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

        // Record outcome for all active weapons
        if (WeaponManager.Instance != null)
        {
            foreach (var (genome, tracker) in WeaponManager.Instance.GetAllWeapons())
            {
                if (tracker != null && genome != null)
                {
                    tracker.RegisterBattleOutcome(battleDuration, playerSurvived);
                }
            }
        }
    }

    /// <summary>
    /// Register damage efficiency for a specific weapon.
    /// Called periodically or at battle end with accumulated stats.
    /// </summary>
    public void RegisterWeaponDamageEfficiency(int weaponId, float totalDamageDealt, float totalPowerCost)
    {
        var (genome, tracker) = WeaponManager.Instance.GetWeapon(weaponId);
        if (tracker == null || genome == null)
            return;

        tracker.RegisterDamageEfficiency(totalDamageDealt, totalPowerCost);
    }

    /// <summary>
    /// Register heat management efficiency for a weapon.
    /// </summary>
    public void RegisterWeaponHeatEfficiency(int weaponId, float totalDamageDealt, float totalHeatGenerated)
    {
        var (genome, tracker) = WeaponManager.Instance.GetWeapon(weaponId);
        if (tracker == null || genome == null)
            return;

        tracker.RegisterHeatManagementEfficiency(totalDamageDealt, totalHeatGenerated);
    }

    /// <summary>
    /// Register hit ratio (accuracy considering burst).
    /// </summary>
    public void RegisterWeaponHitRatio(int weaponId, float successfulHits, float totalShotsFired)
    {
        var (genome, tracker) = WeaponManager.Instance.GetWeapon(weaponId);
        if (tracker == null || genome == null)
            return;

        tracker.RegisterHitRatio(successfulHits, totalShotsFired);
    }

    /// <summary>
    /// Register effectiveness per cooldown cycle.
    /// </summary>
    public void RegisterWeaponEffectivenessPerCycle(int weaponId, float burstDamage, float fireRateCycle, float cooldownTime)
    {
        var (genome, tracker) = WeaponManager.Instance.GetWeapon(weaponId);
        if (tracker == null || genome == null)
            return;

        tracker.RegisterEffectivenessPerCycle(burstDamage, fireRateCycle, cooldownTime);
    }

    /// <summary>
    /// Register targeting time efficiency (how much weapon was used).
    /// </summary>
    public void RegisterWeaponTargetingTimeEfficiency(int weaponId, float fireTime, float totalBattleTime)
    {
        var (genome, tracker) = WeaponManager.Instance.GetWeapon(weaponId);
        if (tracker == null || genome == null)
            return;

        tracker.RegisterTargetingTimeEfficiency(fireTime, totalBattleTime);
    }

    /// <summary>
    /// Register target utility (how many enemies were engaged).
    /// </summary>
    public void RegisterWeaponTargetUtility(int weaponId, float enemiesEngaged, float totalEnemies)
    {
        var (genome, tracker) = WeaponManager.Instance.GetWeapon(weaponId);
        if (tracker == null || genome == null)
            return;

        tracker.RegisterTargetUtility(enemiesEngaged, totalEnemies);
    }

    /// <summary>
    /// Register survival contribution (did weapon help keep ship alive).
    /// </summary>
    public void RegisterWeaponSurvivalContribution(int weaponId, float survivalScore)
    {
        var (genome, tracker) = WeaponManager.Instance.GetWeapon(weaponId);
        if (tracker == null || genome == null)
            return;

        tracker.RegisterSurvivalContribution(survivalScore);
    }

    /// <summary>
    /// Register role fulfillment for a weapon.
    /// Compares the weapon's actual mapping to its genome mapping.
    /// </summary>
    public void RegisterWeaponRoleFulfillment(int weaponId, Vector3 genomeMapping, Vector3 performanceMapping)
    {
        var (genome, tracker) = WeaponManager.Instance.GetWeapon(weaponId);
        if (tracker == null || genome == null)
            return;

        float mappingDistance = Vector3.Distance(genomeMapping, performanceMapping);
        tracker.RegisterRoleFulfillment(mappingDistance);
    }

    /// <summary>
    /// Convenience: Register all metrics for a weapon at once.
    /// Use this if you have all data available at battle end.
    /// </summary>
    public void RegisterWeaponMetricsComplete(
        int weaponId,
        float damageEfficiency,
        float heatEfficiency,
        float hitRatio,
        float effectivenessPerCycle,
        float targetingTimeEfficiency,
        float targetUtility,
        float survivalContribution,
        float roleFulfillmentDistance,
        float battleDuration,
        bool shipSurvived)
    {
        var (genome, tracker) = WeaponManager.Instance.GetWeapon(weaponId);
        if (tracker == null || genome == null)
            return;

        tracker.RegisterDamageEfficiency(damageEfficiency, 1f);
        tracker.RegisterHeatManagementEfficiency(heatEfficiency, 1f);
        tracker.RegisterHitRatio(hitRatio, 1f);
        tracker.RegisterEffectivenessPerCycle(effectivenessPerCycle, 1f, 0f);
        tracker.RegisterTargetingTimeEfficiency(targetingTimeEfficiency, 1f);
        tracker.RegisterTargetUtility(targetUtility, 1f);
        tracker.RegisterSurvivalContribution(survivalContribution);
        tracker.RegisterRoleFulfillment(roleFulfillmentDistance);
        tracker.RegisterBattleOutcome(battleDuration, shipSurvived);
    }
}
