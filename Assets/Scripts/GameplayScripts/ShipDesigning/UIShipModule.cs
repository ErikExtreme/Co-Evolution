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

    private void Start()
    {
        shipGenome = new ShipGenome();
        //shipGenome = GlobalSettings.RandomShipGenome();

        //Temporary(?) default values
        shipGenome.hullHP = 1;
        shipGenome.armor = 1;
        shipGenome.shieldCapacity = 1;
        shipGenome.shieldRegen = 1;
        shipGenome.powerCapacity = 1;
        shipGenome.powerRegen = 1;

        shipGenome.speed = 4;
        shipGenome.turnRate = 1.2f;
        //shipGenome.evasion = 0;
        //shipGenome.mass = 1;
        //shipGenome.inertia = 1;

        shipGenome.gridWidth = 3;
        shipGenome.gridHeight = 3;
        //shipGenome.specialTileDensity

        shipGenome.droneCount = 1;
        shipGenome.droneSpeed = 2;
        shipGenome.droneDurability = 1;
        //float droneAggression;

    }
    public void Initialize(ShipGenome shipGenome,ModuleStatsTracker moduleStatsTracker, Text textBox)
    {
        this.shipGenome = shipGenome;
        this.moduleStatsTracker = moduleStatsTracker;
        statsTextBox = textBox;
    }
    public void DisplayStats()
    {
        statsTextBox.text =
            "Hull HP: " + shipGenome.hullHP +
            "\nArmor: " + shipGenome.armor +
            "\nShield cap: " + shipGenome.shieldCapacity +
            "\nShiled regen: " + shipGenome.shieldRegen +
            "\nPower cap: " + shipGenome.powerCapacity +
            "\nPower regen: " + shipGenome.powerRegen +

            "\nSpeed: " + shipGenome.speed +
            "\nTurn rate: " + shipGenome.turnRate +
            "\nEvasion: " + shipGenome.evasion +
            "\nMass: " + shipGenome.mass +
            "\nInertia: " + shipGenome.inertia +

            "\nGrid width: " + shipGenome.gridWidth +
            "\nGrid height: " + shipGenome.gridHeight +
            "\nSpecial Tile Density: " + shipGenome.specialTileDensity +

            "\nDrone count: " + shipGenome.droneCount +
            "\nDrone speed: " + shipGenome.droneSpeed +
            "\nDrone durability: " + shipGenome.droneDurability +
            "\nDrone aggression: " + shipGenome.droneAggression;
    }

}
