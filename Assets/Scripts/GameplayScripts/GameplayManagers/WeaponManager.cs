using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    public List<(WeaponGenome genome, WeaponStatsTracker tracker)> weapons = new List<(WeaponGenome genome, WeaponStatsTracker tracker)>();

    [SerializeField] Inventory inventory;

    void Awake()
    {
        Instance = this;
    }

    public (WeaponGenome genome, WeaponStatsTracker tracker) GetWeapon(int id)
    {
        if (weapons.Count == 0)
            return (null, null);

        foreach (var weapon in weapons)
        {
            if (weapon.genome.id == id)
            {
                return weapon;
            }
        }

        Debug.LogError("Could not find weapon with id: " + id);
        return (null, null);
    }

    public void AddWeapon(WeaponGenome weaponGenome, WeaponStatsTracker weaponStatsTracker)
    {
        weapons.Add((weaponGenome, weaponStatsTracker));

        inventory.AddWeapon(weaponGenome, weaponStatsTracker);
    }

    /// <summary>
    /// Get all weapons currently managed by this system.
    /// </summary>
    public List<(WeaponGenome genome, WeaponStatsTracker tracker)> GetAllWeapons()
    {
        return new List<(WeaponGenome, WeaponStatsTracker)>(weapons);
    }

    /// <summary>
    /// Register battle outcome metrics for all active weapons.
    /// Should be called by the battle/wave manager when combat ends.
    /// </summary>
    public void RegisterBattleMetricsForAll(float battleDuration, bool shipSurvived)
    {
        foreach (var (genome, tracker) in weapons)
        {
            if (tracker != null && genome != null)
            {
                tracker.RegisterBattleOutcome(battleDuration, shipSurvived);
            }
        }
    }

    /// <summary>
    /// Clear all metrics for all weapons (for starting a new battle).
    /// </summary>
    public void ResetMetricsForAll()
    {
        foreach (var (genome, tracker) in weapons)
        {
            if (tracker != null)
            {
                // Keep genome/historical data, but reset battle-specific metrics
                tracker.damageEfficiency = 0f;
                tracker.heatManagementEfficiency = 0f;
                tracker.hitRatio = 0f;
                tracker.effectivenessPerCycle = 0f;
                tracker.targetingTimeEfficiency = 0f;
                tracker.targetUtility = 0f;
                tracker.survivalContribution = 0f;
                tracker.roleFulfillment = 0f;
                tracker.battleDuration = 0f;
                tracker.survivalSuccess = 0f;
            }
        }
    }
}
