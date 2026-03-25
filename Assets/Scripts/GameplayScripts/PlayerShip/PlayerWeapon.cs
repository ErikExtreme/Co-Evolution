using UnityEngine;

public class PlayerWeapon : Weapon
{
    [SerializeField] PlayerShip playerShip;

    protected override void OnStart()
    {
        base.OnStart();

        fireRateAdjusted = weapon_Genome.fireRate * 2;
        projectileSpeedAdjusted = weapon_Genome.projectileSpeed * 0.7f;
        chargeUpTimeAdjusted = weapon_Genome.chargeUpTime * 2;

        chargeUpTimer = chargeUpTimeAdjusted;
        shoot_Timer = 1 / Mathf.Max(fireRateAdjusted, 0.001f);

        playerShip = transform.GetComponentInParent<PlayerShip>();
        playerShip.targetSelection += NewTarget;
    }

    public override Transform FindNewTarget()
    {
        return EnemyManager.Instance.GetClosestEnemy(transform.position);
    }
}
