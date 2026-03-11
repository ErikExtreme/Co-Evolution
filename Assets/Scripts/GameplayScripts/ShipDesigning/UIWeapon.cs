using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : MonoBehaviour, IGrabbableUI
{
    public string PlacementTag => "PlacementPoint";
    public bool isActive { get; set; }


    public WeaponGenome weaponGenome;
    public WeaponStatsTracker weaponStatsTracker;

    Text statsTextBox;

    public void Initialize(WeaponGenome weaponGenome,WeaponStatsTracker weaponStatsTracker, Text textBox)
    {
        this.weaponGenome = weaponGenome;
        this.weaponStatsTracker = weaponStatsTracker;
        statsTextBox = textBox;
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
