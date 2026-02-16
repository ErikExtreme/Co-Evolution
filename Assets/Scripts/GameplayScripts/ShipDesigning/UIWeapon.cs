using UnityEngine;

public class UIWeapon : MonoBehaviour, IGrabbableUI
{
    public string PlacementTag => "PlacementPoint";
    public bool isActive { get; set; }

    public WeaponGenome weaponGenome;

    void Start()
    {
        weaponGenome = new WeaponGenome();
        //weaponGenome = GlobalSettings.RandomWeaponGenome();

        //Temporary(?) default values
        weaponGenome.baseDamage = 1;
        weaponGenome.burstSize = 3;
        weaponGenome.fireRate = 1f;
        weaponGenome.cooldownTime = 0.3f;
        weaponGenome.projectileSpeed = 2;
        //weapon_Genome.accuracy   unimplemented
        weaponGenome.spreadAngle = 5;
        weaponGenome.range = 5;

        weaponGenome.powerCost = 1;
        //weaponGenome.heatPerShot
        //weaponGenome.heatDissipation
        //weaponGenome.chargeUpTime

        //weaponGenome.tileFootprint
        //weaponGenome.tileAffinity

        //weaponGenome.statusEffectStrength
        weaponGenome.aoeRadius = 1f;
    }
}
