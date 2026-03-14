using UnityEngine;

public class PlayerHealth : ShipHealth
{
    [SerializeField] Canvas gameoverCanvas;
    public void SetStats(ShipCoreStats shipCoreStats)
    {
        //Stats
        HullHP = shipCoreStats.hullHP;
        armor = shipCoreStats.armor;
        ShieldCapacity = shipCoreStats.shieldCapacity;
        shieldRegen = shipCoreStats.shieldRegen;
        PowerCapacity = shipCoreStats.powerCapacity;
        powerRegen = shipCoreStats.powerRegen;

        evasion = shipCoreStats.evasion;

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
