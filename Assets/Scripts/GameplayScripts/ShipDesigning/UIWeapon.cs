using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : MonoBehaviour, IGrabbableUI
{
    public string PlacementTag => "PlacementPoint";
    public bool isActive { get; set; }


    public WeaponGenome weaponGenome;
    public WeaponStatsTracker weaponStatsTracker;

    Text statsTextBox;

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
        weaponGenome.heatPerShot = 10;
        weaponGenome.heatDissipation = 50;
        weaponGenome.chargeUpTime = 2;

        //weaponGenome.tileFootprint
        //weaponGenome.tileAffinity

        weaponGenome.statusEffectType = EffectType.None;
        weaponGenome.statusEffectStrength = 1f;
        weaponGenome.aoeRadius = 1f;
    }
    public void Initialize(WeaponGenome weaponGenome, Text textBox)
    {
        this.weaponGenome = weaponGenome;
        statsTextBox = textBox;

        weaponStatsTracker = new WeaponStatsTracker();
    }
    public void DisplayStats()
    {
        statsTextBox.text =
            "Damage: " + weaponGenome.baseDamage +
            "\nBurst size: " + weaponGenome.burstSize +
            "\nFire rate: " + weaponGenome.fireRate +
            "\nShot cooldown: " + weaponGenome.cooldownTime +
            "\nProjectile speed: " + weaponGenome.projectileSpeed +
            "\nAccuracy: " + weaponGenome.accuracy +
            "\nSpread: " + weaponGenome.spreadAngle +
            "\nRange: " + weaponGenome.range +

            "\nPower cost: " + weaponGenome.powerCost +
            "\nHeat per shot: " + weaponGenome.heatPerShot +
            "\nHeat dissipation: " + weaponGenome.heatDissipation +
            "\nCharge up time: " + weaponGenome.chargeUpTime +

            "\nTile footprint: " + weaponGenome.tileFootprint +
            "\nTile affinity: " + weaponGenome.tileAffinity +

            "\nStatus effect: " + weaponGenome.statusEffectType.ToString() +
            "\nStatus effect strength: " + weaponGenome.statusEffectStrength +
            "\nAoe radius: " + weaponGenome.aoeRadius;
    }
}
