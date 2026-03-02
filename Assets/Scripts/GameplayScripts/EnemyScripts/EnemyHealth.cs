using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemyHealth : ShipHealth
{
    [SerializeField] int enemyHullHP;
    [SerializeField] int enemyArmor;
    [SerializeField] int enemyShieldCapacity;
    [SerializeField] int enemyShieldRegen;
    [SerializeField] int enemyPowerCapacity;
    [SerializeField] int enemyPowerRegen;

    [SerializeField] float enemyEvasion;

    protected override void OnStart()
    {
        hullHP = enemyHullHP;
        armor = enemyArmor;
        shieldCapacity = enemyShieldRegen;
        shieldRegen = enemyShieldRegen;
        powerCapacity = enemyPowerCapacity;
        powerRegen = enemyPowerRegen;

        base.OnStart();
    }

    protected override void OutOfHealth()
    {
        Destroy(gameObject);
    }
}
