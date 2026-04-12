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
        HullHP = Mathf.RoundToInt(enemyHullHP * (increaseMult - 0.4f));
        armor = Mathf.RoundToInt(enemyArmor * (increaseMult / 3) - 0.4f);
        ShieldCapacity = Mathf.RoundToInt(enemyShieldCapacity * (increaseMult / 1.25f - 0.4f));
        shieldRegen = Mathf.RoundToInt(enemyShieldRegen * (increaseMult / 5 - 0.4f));
        PowerCapacity = enemyPowerCapacity;
        powerRegen = Mathf.RoundToInt(enemyPowerRegen * (increaseMult / 5) - 0.4f);
    }
}
