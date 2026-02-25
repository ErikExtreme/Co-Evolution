using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class OverrideWeaponStats : MonoBehaviour
{
    [SerializeField] Weapon weaponScript;

    // Combat Profile
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
    //public EffectType statusEffectType;
    [SerializeField] float statusEffectStrength;
    [SerializeField] float aoeRadius;
    //public float piercingDepth;


    private void Start()
    {
        weaponScript.weapon_Genome = new WeaponGenome();

        weaponScript.weapon_Genome.baseDamage = baseDamage;
        weaponScript.weapon_Genome.burstSize = burstSize;
        weaponScript.weapon_Genome.fireRate = fireRate;
        weaponScript.weapon_Genome.cooldownTime = cooldownTime;
        weaponScript.weapon_Genome.projectileSpeed  = projectileSpeed;
        weaponScript.weapon_Genome.accuracy = accuracy;
        weaponScript.weapon_Genome.spreadAngle = spreadAngle;
        weaponScript.weapon_Genome.range = range;

        weaponScript.weapon_Genome.powerCost = powerCost;
        weaponScript.weapon_Genome.heatPerShot = heatPerShot;
        weaponScript.weapon_Genome.heatDissipation = heatDissipation;
        weaponScript.weapon_Genome.chargeUpTime = chargeUpTime;

        weaponScript.weapon_Genome.statusEffectStrength = statusEffectStrength;
        weaponScript.weapon_Genome.aoeRadius = aoeRadius;
    }
}
