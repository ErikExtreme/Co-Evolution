using UnityEngine;

public class OverrideShipStats : MonoBehaviour
{
    [SerializeField] ShipHealth shipHealthScript;
    [SerializeField] PlayerShip playerShipScript;

    // Core Systems
    [SerializeField] int hullHP;
    [SerializeField] int armor;
    [SerializeField] int shieldCapacity;
    [SerializeField] int shieldRegen;
    [SerializeField] int powerCapacity;
    [SerializeField] int powerRegen;

    // Mobility
    [SerializeField] float speed;
    [SerializeField] float turnRate;
    [SerializeField] float evasion;
    [SerializeField] float mass;
    [SerializeField] float inertia;

    //// Layout
    //public int gridWidth;
    //public int gridHeight;
    //public float specialTileDensity;

    //// Drones
    //// public DroneType droneType;
    //public int droneCount;
    //public float droneSpeed;
    //public int droneDurability;
    //public float droneAggression;

    void Start()
    {
        ShipCoreStats shipCoreStats = new ShipCoreStats();
        shipCoreStats.hullHP = hullHP;
        shipCoreStats.armor = armor;
        shipCoreStats.shieldCapacity = shieldCapacity;
        shipCoreStats.shieldRegen= shieldRegen;
        shipCoreStats.powerCapacity = powerCapacity;
        shipCoreStats.powerRegen= powerRegen;
        shipCoreStats.evasion = evasion;
        shipHealthScript.SetStats(shipCoreStats);

        ShipMobilityStats mobilityStats = new ShipMobilityStats();
        mobilityStats.speed = speed;
        mobilityStats.turnRate = turnRate;
        mobilityStats.mass = mass;
        mobilityStats.inertia = inertia;
        playerShipScript.SetStats(mobilityStats);
    }
}
