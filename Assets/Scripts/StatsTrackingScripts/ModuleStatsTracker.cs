using UnityEngine;

public class ModuleStatsTracker
{
    // ===== Survival Metrics =====
    /// <summary>
    /// Damage mitigated per unit of defensive stat investment (HP + armor + shield cap).
    /// Normalized: damageEfficiency = damageAbsorbed / statInvestment
    /// </summary>
    public float damageEfficiency;

    /// <summary>
    /// Fraction of battle where this module contributed to survival.
    /// Normalized to [0, 1]: survivalContribution = timeSurvived / totalBattleTime
    /// </summary>
    public float survivalContribution;

    // ===== Offensive Support Metrics =====
    /// <summary>
    /// How much did this module enable weapons/drones to perform?
    /// Normalized to [0, 1] based on grid expansion + drone support.
    /// </summary>
    public float offensiveSynergy;

    /// <summary>
    /// Power resource efficiency: available power / consumed power.
    /// Measures how well the module's power stats supported active weapons.
    /// </summary>
    public float powerEfficiency;

    // ===== Role Fulfillment Metrics =====
    /// <summary>
    /// How well the module fulfilled its mapped role in 3D genome space.
    /// 1.0 = perfect alignment with mapping, 0.0 = opposite role.
    /// Distance from mapping normalized: 1.0 - (distance / sqrt(3))
    /// </summary>
    public float roleFulfillment;

    // ===== Outcome Metrics =====
    /// <summary>
    /// Total battle duration while this module was equipped.
    /// Used for normalizing other metrics across variable battle lengths.
    /// </summary>
    public float battleDuration;

    /// <summary>
    /// Whether the ship survived the entire encounter.
    /// 1.0 = survived with module equipped, 0.0 = ship died.
    /// </summary>
    public float survivalSuccess;

    // ===== Legacy metrics (deprecated) =====
    [System.Obsolete("Use damageEfficiency instead")]
    public float damageAvoided;

    [System.Obsolete("Use powerEfficiency instead")]
    public float powerSaved;

    [System.Obsolete("Use survivalContribution instead")]
    public float heatReduced;

    [System.Obsolete("Use survivalContribution instead")]
    public float timeEquipped;

    // ===== Registration Methods =====
    /// <summary>
    /// Register damage mitigation efficiency for this module.
    /// </summary>
    public void RegisterDamageEfficiency(float damageAbsorbed, float statInvestment)
    {
        damageEfficiency = statInvestment > 0 ? damageAbsorbed / statInvestment : 0f;
    }

    /// <summary>
    /// Register what fraction of battle this module contributed to survival.
    /// </summary>
    public void RegisterSurvivalContribution(float timeSurvived, float totalBattleTime)
    {
        survivalContribution = totalBattleTime > 0 ? timeSurvived / totalBattleTime : 0f;
    }

    /// <summary>
    /// Register how much this module enabled offensive capability.
    /// </summary>
    public void RegisterOffensiveSynergy(float synergyScore)
    {
        offensiveSynergy = Mathf.Clamp01(synergyScore);
    }

    /// <summary>
    /// Register power resource efficiency.
    /// </summary>
    public void RegisterPowerEfficiency(float powerAvailable, float powerConsumed)
    {
        powerEfficiency = powerConsumed > 0 ? powerAvailable / powerConsumed : 1f;
    }

    /// <summary>
    /// Register how well the module fulfilled its mapped role.
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
    [System.Obsolete("Use RegisterSurvivalContribution instead")]
    public void UpdateEquipped(float dt)
    {
        timeEquipped += dt;
    }

    [System.Obsolete("Use RegisterDamageEfficiency instead")]
    public void RegisterDamageAvoided(float amount)
    {
        damageTaken += amount;
    }

    [System.Obsolete("Use RegisterPowerEfficiency instead")]
    public void RegisterPowerSaved(float amount)
    {
        droneDamageDealt += amount;
    }

    [System.Obsolete("Use RegisterSurvivalContribution instead")]
    public void RegisterHeatReduced(float amount)
    {
        distanceMoved += amount;
    }
}
