using UnityEngine;

public class PlayerWeapon : Weapon
{
    [SerializeField] PlayerShip playerShip;

    protected override void OnStart()
    {
        base.OnStart();

        playerShip = transform.GetComponentInParent<PlayerShip>();
        playerShip.targetSelection += NewTarget;

        chargeUpTimer = weapon_Genome.chargeUpTime * 3;
        projectileSpeed = weapon_Genome.projectileSpeed * 0.7f;
    }

    public override Transform FindNewTarget()
    {
        return EnemyManager.Instance.GetClosestEnemy(transform.position);
    }
}
