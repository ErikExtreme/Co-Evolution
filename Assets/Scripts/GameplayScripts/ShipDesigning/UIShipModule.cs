using System;
using UnityEngine;
using UnityEngine.UI;

public class UIShipModule : MonoBehaviour, IGrabbableUI
{
    public string PlacementTag => "ModuleSlot";
    public bool isActive { get; set; }


    public ShipGenome shipGenome;
    public ModuleStatsTracker moduleStatsTracker;

    Text statsTextBox;

    public void Initialize(ShipGenome shipGenome, ModuleStatsTracker moduleStatsTracker, Text textBox)
    {
        this.shipGenome = shipGenome;
        this.moduleStatsTracker = moduleStatsTracker;
        statsTextBox = textBox;
    }
    public void DisplayStats()
    {

        statsTextBox.text =
            "Hull HP: " + shipGenome.hullHP +
            "\nArmor: " + shipGenome.armor / 20 +
            "\nShield cap: " + shipGenome.shieldCapacity / 3 +
            "\nShield regen: " + shipGenome.shieldRegen / 5 +
            "\nPower cap: " + shipGenome.powerCapacity / 5 +
            "\nPower regen: " + shipGenome.powerRegen +

            "\nSpeed: " + FormatFloat(Mathf.Min(shipGenome.speed / 20, 4)) +
            "\nTurn rate: " + FormatFloat(shipGenome.turnRate) +
            "\nEvasion: " + FormatFloat(shipGenome.evasion / 5) +
            "\nMass: " + FormatFloat(shipGenome.mass) +
            "\nInertia: " + FormatFloat(shipGenome.inertia / 20) +

            "\nGrid width: " + shipGenome.gridWidth +
            "\nGrid height: " + shipGenome.gridHeight +
            "\nSpecial Tile Density: " + FormatFloat(shipGenome.specialTileDensity) +

            "\nDrone count: " + shipGenome.droneCount +
            "\nDrone speed: " + FormatFloat(shipGenome.droneSpeed) +
            "\nDrone durability: " + shipGenome.droneDurability +
            "\nDrone aggression: " + FormatFloat(shipGenome.droneAggression);
    }
    private string FormatFloat(float value) => value.ToString("F2");
}
