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
        base.OnStart();
    }

    protected override void OutOfHealth()
    {
        GetComponentInParent<WaveManager>().DestroyEnemy(gameObject);
    }

    public void SetStats(float increaseMult)
    {
        HullHP = Mathf.RoundToInt(enemyHullHP * (1 + increaseMult));
        armor = Mathf.RoundToInt(enemyArmor * (1 + increaseMult / 3));
        ShieldCapacity = Mathf.RoundToInt(enemyShieldCapacity * (1 + increaseMult / 1.25f));
        shieldRegen = Mathf.RoundToInt(enemyShieldRegen * (1 + increaseMult/5));
        PowerCapacity = enemyPowerCapacity;
        powerRegen = Mathf.RoundToInt(enemyPowerRegen * (1 + increaseMult/5));
    }
}
