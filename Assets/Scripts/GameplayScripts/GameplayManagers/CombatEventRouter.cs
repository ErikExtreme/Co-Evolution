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

    public void ReportDamageAvoided(int id, float amount)
    {
        ModuleManager.Instance.modules[id].tracker.RegisterDamageAvoided(amount);
    }

    public void ReportEnemyFocus(string enemyType)
    {
        PlayerBehaviorTracker.Instance.RegisterEnemyFocus(enemyType);
    }
}
