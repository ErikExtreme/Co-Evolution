using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OverallShipStats : MonoBehaviour
{
    [SerializeField]ShipBlueprint shipBlueprint;

    [SerializeField]Text statsTextBox;

    float netEnergyProduction;
    float speed;
    public void DisplayStats()
    {
        var shipStats = shipBlueprint.CalculateShipStats();
        ShipCoreStats shipCoreStats = shipStats.shipCoreStats;
        ShipMobilityStats shipMobilityStats = shipStats.shipMobilityStats;
        ShipDroneStats shipDroneStats = shipStats.shipDroneStats;

        netEnergyProduction = shipCoreStats.powerRegen*1.25f - shipBlueprint.CalculatePowerConsumption();

        statsTextBox.text =
            "Health: " + shipCoreStats.hullHP +
            "\nArmor: " + shipCoreStats.armor/4 +
            "\nShield Cap: " + shipCoreStats.shieldCapacity/3 +
            "\nShield Regen: " + shipCoreStats.shieldRegen/5 +
            "\nPower cap: " + shipCoreStats.powerCapacity +
            "\nNet power: " + FormatFloat(netEnergyProduction) +

            "\nSpeed: " + FormatFloat(shipMobilityStats.speed/20) +
            "\nTurn rate: " + FormatFloat(shipMobilityStats.turnRate) +
            "\nEvasion: " + FormatFloat(shipCoreStats.evasion/5) +
            "\nMass: " + FormatFloat(shipMobilityStats.mass) +
            "\nInertia: " + FormatFloat(shipMobilityStats.inertia/20) +

            "\nDrone count: " + shipDroneStats.availableDrones.Sum(drones =>(drones.count));

    }
    private string FormatFloat(float value) => value.ToString("F2");
}
