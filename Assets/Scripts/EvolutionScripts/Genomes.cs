using UnityEngine;

public class WeaponGenome
{
    public int id;
    public float fitness;
    public Vector3 mapping = Vector3.zero;

    // Combat Profile
    public int baseDamage;
    public int burstSize;
    public float fireRate;
    public float cooldownTime;
    public float projectileSpeed;
    public float accuracy;
    public float spreadAngle;
    public float range;

    // Resource & Economy
    public int powerCost;
    public int heatPerShot;
    public float heatDissipation;
    public float chargeUpTime;

    // Spatial & Synergy
    public Vector2 tileFootprint;
    public Vector2 tileAffinity;
    // public SynergyTag synergyTag;

    // Special Effects
    public EffectType statusEffectType;
    public float statusEffectStrength;
    public float aoeRadius;
    //public float piercingDepth;

    public WeaponGenome Clone() => new WeaponGenome(this);

    public static int nextWeaponId = 0;
    public static int GetNextWeaponId() => nextWeaponId++;

    public WeaponGenome(WeaponGenome other)
    {
        id = 0;
        fitness = other.fitness;
        mapping = other.mapping;
        baseDamage = other.baseDamage;
        burstSize = other.burstSize;
        fireRate = other.fireRate;
        cooldownTime = other.cooldownTime;
        projectileSpeed = other.projectileSpeed;
        accuracy = other.accuracy;
        spreadAngle = other.spreadAngle;
        range = other.range;
        powerCost = other.powerCost;
        heatPerShot = other.heatPerShot;
        heatDissipation = other.heatDissipation;
        chargeUpTime = other.chargeUpTime;
        tileFootprint = other.tileFootprint;
        tileAffinity = other.tileAffinity;
        statusEffectStrength = other.statusEffectStrength;
        aoeRadius = other.aoeRadius;
    }

    public WeaponGenome() { }
}

public class ShipGenome
{
    public int id;
    public float fitness;
    public Vector3 mapping = Vector3.zero;

    // Core Systems
    public int hullHP;
    public int armor;
    public int shieldCapacity;
    public int shieldRegen;
    public int powerCapacity;
    public int powerRegen;

    // Mobility
    public float speed;
    public float turnRate;
    public float evasion;
    public float mass;
    public float inertia;

    // Layout
    public int gridWidth;
    public int gridHeight;
    public float specialTileDensity;

    // Drones
    // public DroneType droneType;
    public int droneCount;
    public float droneSpeed;
    public int droneDurability;
    public float droneAggression;

    // Specialization
    /*
    public WeaponTag weaponBonusTag;
    public DefenceTag defenceBonusTag;
    public UtilityTag utilityBonusTag;
     */

    public ShipGenome Clone() => new ShipGenome(this);

    public static int nextShipId = 0;
    public static int GetNextShipId() => nextShipId++;

    public ShipGenome(ShipGenome other)
    {
        id = 0;
        fitness = other.fitness;
        mapping = other.mapping;
        hullHP = other.hullHP;
        armor = other.armor;
        shieldCapacity = other.shieldCapacity;
        shieldRegen = other.shieldRegen;
        powerCapacity = other.powerCapacity;
        powerRegen = other.powerRegen;
        speed = other.speed;
        turnRate = other.turnRate;
        evasion = other.evasion;
        mass = other.mass;
        inertia = other.inertia;
        gridWidth = other.gridWidth;
        gridHeight = other.gridHeight;
        specialTileDensity = other.specialTileDensity;
        droneCount = other.droneCount;
        droneSpeed = other.droneSpeed;
        droneDurability = other.droneDurability;
        droneAggression = other.droneAggression;
    }

    public ShipGenome() { }
}
