using System;
using UnityEngine;

public class UIShipModule : MonoBehaviour, IGrabbableUI
{
    public string PlacementTag => "ModuleSlot";
    public bool isActive { get; set; }

    public ShipGenome shipGenome;

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
}
