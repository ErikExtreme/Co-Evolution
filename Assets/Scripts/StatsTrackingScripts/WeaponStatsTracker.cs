using UnityEngine;

public class WeaponStatsTracker
{
    // ===== Efficiency Metrics =====
    /// <summary>
    /// Damage dealt per power cost.
    /// Normalized: damageEfficiency = totalDamageDealt / totalPowerCost
    /// Measures how much damage weapon produces per resource spent.
    /// </summary>
    public float damageEfficiency;

    /// <summary>
    /// Heat management efficiency: total damage dealt per heat generated.
    /// Normalized: heatEfficiency = totalDamageDealt / totalHeatGenerated
    /// Measures how effectively the weapon uses its heat generation.
    /// </summary>
    public float heatManagementEfficiency;

    // ===== Effectiveness Metrics =====
    /// <summary>
    /// Successful hits divided by total shots fired (accuracy + burst consideration).
    /// Normalized to [0, 1]: hitRatio = successfulHits / totalShotsFired
    /// Fair to all weapon types: accounts for accuracy, spread, burst size.
    /// </summary>
    public float hitRatio;

    /// <summary>
    /// Effectiveness per cooldown cycle: burst damage / (burst duration + cooldown).
    /// Normalized: effectivenessPerCycle = totalBurstDamage / (fireRateCycle + cooldownTime)
    /// Balances burst vs sustained naturally.
    /// </summary>
    public float effectivenessPerCycle;

    // ===== Utilization Metrics =====
    /// <summary>
    /// Fraction of battle time this weapon was actively firing.
    /// Normalized to [0, 1]: targetingTimeEfficiency = fireTime / totalBattleTime
    /// Measures how much the weapon stayed relevant during combat.
    /// </summary>
    public float targetingTimeEfficiency;

    /// <summary>
    /// Number of distinct enemies engaged (not just damage dealt).
    /// Normalized: targetUtility = enemiesEngaged / totalEnemiesInBattle
    /// Fair to AoE and single-target weapons equally.
    /// </summary>
    public float targetUtility;

    // ===== Outcome Metrics =====
    /// <summary>
    /// Contribution to ship survival: did this weapon help keep the ship alive?
    /// Normalized to [0, 1]: survivalContribution = 1.0 if ship survived, adjusted by damage prevented.
    /// </summary>
    public float survivalContribution;

    /// <summary>
    /// How well the weapon fulfilled its mapped role in 3D genome space.
    /// 1.0 = perfect alignment with mapping, 0.0 = opposite role.
    /// Distance from mapping normalized: 1.0 - (distance / sqrt(3))
    /// </summary>
    public float roleFulfillment;

    /// <summary>
    /// Total battle duration while this weapon was equipped.
    /// Used for normalizing other metrics across variable battle lengths.
    /// </summary>
    public float battleDuration;

    /// <summary>
    /// Whether the ship survived the entire encounter.
    /// 1.0 = survived with weapon equipped, 0.0 = ship died.
    /// </summary>
    public float survivalSuccess;

    // ===== Legacy metrics (deprecated) =====
    [System.Obsolete("Use damageEfficiency instead")]
    public float timeEquipped;

    [System.Obsolete("Use damageEfficiency and heatManagementEfficiency instead")]
    public float damageDealt;

    [System.Obsolete("Use targetUtility instead")]
    public int kills;

    [System.Obsolete("Use targetingTimeEfficiency instead")]
    public float avgEffectiveRange;

    float smoothing = 0.1f;

    // ===== Registration Methods =====
    /// <summary>
    /// Register damage efficiency for this weapon.
    /// </summary>
    public void RegisterDamageEfficiency(float totalDamageDealt, float totalPowerCost)
    {
        damageEfficiency = totalPowerCost > 0 ? totalDamageDealt / totalPowerCost : 0f;
    }

    /// <summary>
    /// Register heat management efficiency.
    /// </summary>
    public void RegisterHeatManagementEfficiency(float totalDamageDealt, float totalHeatGenerated)
    {
        heatManagementEfficiency = totalHeatGenerated > 0 ? totalDamageDealt / totalHeatGenerated : 0f;
    }

    /// <summary>
    /// Register hit ratio for this weapon.
    /// </summary>
    public void RegisterHitRatio(float successfulHits, float totalShotsFired)
    {
        hitRatio = totalShotsFired > 0 ? Mathf.Clamp01(successfulHits / totalShotsFired) : 0f;
    }

    /// <summary>
    /// Register effectiveness per cooldown cycle.
    /// </summary>
    public void RegisterEffectivenessPerCycle(float burstDamage, float fireRateCycle, float cooldownTime)
    {
        float totalCycleDuration = fireRateCycle + cooldownTime;
        effectivenessPerCycle = totalCycleDuration > 0 ? burstDamage / totalCycleDuration : 0f;
    }

    /// <summary>
    /// Register targeting time efficiency.
    /// </summary>
    public void RegisterTargetingTimeEfficiency(float fireTime, float totalBattleTime)
    {
        targetingTimeEfficiency = totalBattleTime > 0 ? Mathf.Clamp01(fireTime / totalBattleTime) : 0f;
    }

    /// <summary>
    /// Register target utility (distinct enemies engaged).
    /// </summary>
    public void RegisterTargetUtility(float enemiesEngaged, float totalEnemies)
    {
        targetUtility = totalEnemies > 0 ? Mathf.Clamp01(enemiesEngaged / totalEnemies) : 0f;
    }

    /// <summary>
    /// Register survival contribution.
    /// </summary>
    public void RegisterSurvivalContribution(float survivalScore)
    {
        survivalContribution = Mathf.Clamp01(survivalScore);
    }

    /// <summary>
    /// Register how well the weapon fulfilled its mapped role.
    /// </summary>
    public void RegisterRoleFulfillment(float mapping3DDistance)
    {
        // Distance ranges from 0 to sqrt(3) in normalized 3D space
        roleFulfillment = 1f - Mathf.Clamp01(mapping3DDistance / Mathf.Sqrt(3f));
    }

    /// <summary>
    /// Register the outcome of the battle.
    /// </summary>
    public void RegisterBattleOutcome(float duration, bool shipSurvived)
    {
        battleDuration = duration;
        survivalSuccess = shipSurvived ? 1f : 0f;
    }

    // ===== Legacy methods (deprecated) =====
    [System.Obsolete("Use RegisterTargetingTimeEfficiency instead")]
    public void UpdateEquipped(float dt)
    {
        timeEquipped += dt;
    }

    [System.Obsolete("Use RegisterDamageEfficiency instead")]
    public void RegisterDamage(float dmg, float distance)
    {
        damageDealt += dmg;
        avgEffectiveRange = Mathf.Lerp(avgEffectiveRange, distance, smoothing);
    }

    [System.Obsolete("Use RegisterTargetUtility instead")]
    public void RegisterKill()
    {
        kills++;
    }
}
