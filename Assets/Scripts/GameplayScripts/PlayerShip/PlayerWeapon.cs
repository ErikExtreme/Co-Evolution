using UnityEngine;

public class PlayerWeapon : Weapon
{
    [SerializeField] PlayerShip playerShip;

    protected override void OnStart()
    {
        base.OnStart();

        fireRateAdjusted = weapon_Genome.fireRate;
        projectileSpeedAdjusted = weapon_Genome.projectileSpeed * 0.7f;
        chargeUpTimeAdjusted = weapon_Genome.chargeUpTime*2;
        //powerCostAdjusted = weapon_Genome.powerCost * 1.66f;
        damageAdjusted = Mathf.RoundToInt(weapon_Genome.baseDamage / 1.15f);

        chargeUpTimer = chargeUpTimeAdjusted;
        shoot_Timer = 1 / Mathf.Max(fireRateAdjusted, 0.001f);

        playerShip = transform.GetComponentInParent<PlayerShip>();
        playerShip.targetSelection += NewTarget;

        weaponGenomeID = weapon_Genome.id;
    }

    public override Transform FindNewTarget()
    {
        return EnemyManager.Instance.GetClosestEnemy(transform.position);
    }
}
