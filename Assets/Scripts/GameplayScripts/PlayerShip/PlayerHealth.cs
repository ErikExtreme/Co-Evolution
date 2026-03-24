using UnityEngine;

public class PlayerHealth : ShipHealth
{
    [SerializeField] Canvas gameoverCanvas;
    public void SetStats(ShipCoreStats shipCoreStats)
    {
        //Stats
        HullHP = shipCoreStats.hullHP;
        armor = shipCoreStats.armor / 4;
        ShieldCapacity = shipCoreStats.shieldCapacity / 3;
        shieldRegen = shipCoreStats.shieldRegen / 5;
        PowerCapacity = shipCoreStats.powerCapacity;
        powerRegen = shipCoreStats.powerRegen*2;

        evasion = shipCoreStats.evasion / 5;

        //Current, shouldn't necessarly be set here, depends on design
        Health = HullHP;
        Shield = ShieldCapacity;
        Power = PowerCapacity;
    }
    protected override void OutOfHealth()
    {
        gameoverCanvas.gameObject.SetActive(true);
    }
}
