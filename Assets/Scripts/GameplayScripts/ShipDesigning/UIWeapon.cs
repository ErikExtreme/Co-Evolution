using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : MonoBehaviour, IGrabbableUI
{
    public string PlacementTag => "PlacementPoint";
    public bool isActive { get; set; }


    public WeaponGenome weaponGenome;
    public WeaponStatsTracker weaponStatsTracker;

    Text statsTextBox;

    public void Initialize(WeaponGenome weaponGenome, WeaponStatsTracker weaponStatsTracker, Text textBox)
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
            "\nFire rate: " + FormatFloat(weaponGenome.fireRate) +
            "\nShot cooldown: " + FormatFloat(weaponGenome.cooldownTime) +
            "\nProjectile speed: " + FormatFloat(weaponGenome.projectileSpeed * 0.7f) +
            "\nAccuracy: " + FormatFloat(weaponGenome.accuracy) +
            "\nSpread: " + FormatFloat(weaponGenome.spreadAngle) +
            "\nRange: " + FormatFloat(weaponGenome.range) +

            "\nPower cost: " + weaponGenome.powerCost +
            "\nHeat per shot: " + weaponGenome.heatPerShot +
            "\nHeat dissipation: " + FormatFloat(weaponGenome.heatDissipation) +
            "\nCharge up time: " + FormatFloat(weaponGenome.chargeUpTime * 3) +

            "\nStatus effect: " + weaponGenome.statusEffectType.ToString() +
            "\nStatus effect strength: " + FormatFloat(weaponGenome.statusEffectStrength) +
            "\nAoe radius: " + FormatFloat(weaponGenome.aoeRadius);
    }
    private string FormatFloat(float value) => value.ToString("F2");
}
