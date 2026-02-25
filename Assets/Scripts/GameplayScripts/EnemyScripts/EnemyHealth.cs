using UnityEngine;

public class EnemyHealth : ShipHealth
{
    [SerializeField] int enemyHullHP;
    [SerializeField] int enemyArmor;
    [SerializeField] int enemyShieldCapacity;
    [SerializeField] int enemyShieldRegen;
    [SerializeField] int enemyPowerCapacity;
    [SerializeField] int enemyPowerRegen;

    [SerializeField] float enemyEvasion;

    void Start()
    {
        hullHP = enemyHullHP;
        armor = enemyArmor;
        shieldCapacity = enemyShieldRegen;
        shieldRegen = enemyShieldRegen;
        powerCapacity = enemyPowerCapacity;
        powerRegen = enemyPowerRegen;
    }

    protected override void OutOfHealth()
    {
        Destroy(gameObject);
    }
}
