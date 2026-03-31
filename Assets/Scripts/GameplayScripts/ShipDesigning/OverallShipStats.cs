using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OverallShipStats : MonoBehaviour
{
    [SerializeField] ShipBlueprint shipBlueprint;

    [SerializeField] Text statsTextBox;

    float netEnergyProduction;
    float speed;
    public void DisplayStats()
    {
        var shipStats = shipBlueprint.CalculateShipStats();
        ShipCoreStats shipCoreStats = shipStats.shipCoreStats;
        ShipMobilityStats shipMobilityStats = shipStats.shipMobilityStats;
        ShipDroneStats shipDroneStats = shipStats.shipDroneStats;

        netEnergyProduction = shipCoreStats.powerRegen - shipBlueprint.CalculatePowerConsumption();

        statsTextBox.text =
            "Health: " + shipCoreStats.hullHP +
            "\nArmor: " + FormatFloat((float)shipCoreStats.armor / 20) +
            "\nShield Cap: " + FormatFloat((float)shipCoreStats.shieldCapacity / 3) +
            "\nShield Regen: " + FormatFloat((float)shipCoreStats.shieldRegen / 5) +
            "\nPower cap: " + FormatFloat((float)shipCoreStats.powerCapacity / 5) +
            "\nNet power: " + FormatFloat(netEnergyProduction) +

            "\nSpeed: " + FormatFloat(Mathf.Min(shipMobilityStats.speed / 20, 4)) +
            "\nTurn rate: " + FormatFloat(shipMobilityStats.turnRate) +
            "\nEvasion: " + FormatFloat(shipCoreStats.evasion / 5) +

            "\nDrone count: " + shipDroneStats.availableDrones.Sum(drones => (drones.count));

    }
    private string FormatFloat(float value) => value.ToString("F2");
}
