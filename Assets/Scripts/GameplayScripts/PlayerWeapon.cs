using UnityEngine;

public class PlayerWeapon : Weapon
{
    [SerializeField] PlayerShip playerShip;

    protected override void OnStart()
    {
        base.OnStart();

        playerShip = transform.GetComponentInParent<PlayerShip>();
        playerShip.targetSelection += NewTarget;
    }

    public override Transform FindNewTarget()
    {
        return EnemyManager.Instance.GetClosestEnemy(transform.position);
    }
}
