using UnityEngine;

public class EnemyWeapon : Weapon
{
    [SerializeField] int baseDamage;
    [SerializeField] int burstSize;
    [SerializeField] float fireRate;
    [SerializeField] float cooldownTime;
    [SerializeField] float projectileSpeed;
    [SerializeField] float accuracy;
    [SerializeField] float spreadAngle;
    [SerializeField] float range;

    // Resource & Economy
    [SerializeField] int powerCost;
    [SerializeField] int heatPerShot;
    [SerializeField] float heatDissipation;
    [SerializeField] float chargeUpTime;

    //// Spatial & Synergy
    //public Vector2 tileFootprint;
    //public Vector2 tileAffinity;
    //// public SynergyTag synergyTag;

    // Special Effects
    [SerializeField] EffectType statusEffectType;
    [SerializeField] float statusEffectStrength;
    [SerializeField] float aoeRadius;
    //public float piercingDepth;

    protected override void OnStart()
    {
        weapon_Genome = new WeaponGenome();

        weapon_Genome.baseDamage = baseDamage;
        weapon_Genome.burstSize = burstSize;
        weapon_Genome.fireRate = fireRate;
        weapon_Genome.cooldownTime = cooldownTime;
        weapon_Genome.projectileSpeed = projectileSpeed;
        weapon_Genome.accuracy = accuracy;
        weapon_Genome.spreadAngle = spreadAngle;
        weapon_Genome.range = range;

        weapon_Genome.powerCost = powerCost;
        weapon_Genome.heatPerShot = heatPerShot;
        weapon_Genome.heatDissipation = heatDissipation;
        weapon_Genome.chargeUpTime = chargeUpTime;

        weapon_Genome.statusEffectType = statusEffectType;
        weapon_Genome.statusEffectStrength = statusEffectStrength;
        weapon_Genome.aoeRadius = aoeRadius;

        base.OnStart();
    }

    public override Transform FindNewTarget()
    {
        return EnemyManager.Instance.GetClosestEnemy(transform.position);//not used
    }
}
