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
        HullHP = enemyHullHP;
        armor = enemyArmor;
        ShieldCapacity = enemyShieldRegen;
        shieldRegen = enemyShieldRegen;
        PowerCapacity = enemyPowerCapacity;
        powerRegen = enemyPowerRegen;

        base.OnStart();
    }

    protected override void OutOfHealth()
    {
        GetComponentInParent<WaveManager>().DestroyEnemy(gameObject);
    }
}
