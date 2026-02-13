using UnityEngine;

public class WeaponGenome : IGenome
{
    public int id { get; set; }
    public float fitness { get; set; }

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
    //public EffectType statusEffectType;
    public float statusEffectStrength;
    public float aoeRadius;
    //public float piercingDepth;
}

public class ShipGenome : IGenome
{
    public int id { get; set; }
    public float fitness { get; set; }

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
}

public interface IGenome
{
    int id { get; set; }
    float fitness { get; set; }
}
