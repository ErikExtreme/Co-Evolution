using UnityEngine;

public class CombatEventRouter : MonoBehaviour
{
    public static CombatEventRouter Instance;

    void Awake()
    {
        Instance = this;
    }

    public void ReportWeaponDamage(int id, float dmg, float distance)
    {
        WeaponManager.Instance.weapons[id].tracker.RegisterDamage(dmg, distance);
    }

    public void ReportWeaponKill(int id)
    {
        WeaponManager.Instance.weapons[id].tracker.RegisterKill();
    }

    /// <summary>
    /// Report damage that was avoided/mitigated by a module's defensive stats.
    /// This updates the new damageEfficiency metric.
    /// </summary>
    public void ReportModuleDamageMitigated(int moduleId, float damageAmount)
    {
        if (BattleMetricsRecorder.Instance != null)
        {
            BattleMetricsRecorder.Instance.RegisterModuleDamageMitigation(moduleId, damageAmount);
        }

        // Legacy support
        var (genome, tracker) = ModuleManager.Instance.GetModule(moduleId);
        if (tracker != null)
        {
            tracker.damageAvoided += damageAmount;
        }
    }

    /// <summary>
    /// Report power saved by a module (legacy metric, kept for compatibility).
    /// </summary>
    public void ReportDamageAvoided(int id, float amount)
    {
        ReportModuleDamageMitigated(id, amount);
    }

    public void ReportEnemyFocus(string enemyType)
    {
        //PlayerBehaviorTracker.Instance.RegisterEnemyFocus(enemyType);
    }
}
