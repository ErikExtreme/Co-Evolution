using UnityEngine;

public class WeaponGenome
{
    public int id;
    public int parentId;

    public float fitness;
    public Vector3 mapping = Vector3.zero;

    public float statScore;
    public float trackerScore;
    public float alignmentScore;

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

    public static int nextWeaponId = 0;
    public static int GetNextWeaponId() => nextWeaponId++;

    public WeaponGenome CloneForEvo()
    {
        WeaponGenome g = new WeaponGenome(); 
        g.id = GetNextWeaponId(); 
        g.parentId = this.id;

        g.baseDamage = baseDamage;
        g.burstSize = burstSize;
        g.fireRate = fireRate;
        g.cooldownTime = cooldownTime;
        g.projectileSpeed = projectileSpeed;
        g.accuracy = accuracy;
        g.spreadAngle = spreadAngle;
        g.range = range;
        g.powerCost = powerCost;
        g.heatPerShot = heatPerShot;
        g.heatDissipation = heatDissipation;
        g.chargeUpTime = chargeUpTime;
        g.tileFootprint = tileFootprint;
        g.tileAffinity = tileAffinity;
        g.statusEffectStrength = statusEffectStrength;
        g.aoeRadius = aoeRadius;

        return g;
    }

    public WeaponGenome CloneExact()
    {
        WeaponGenome g = new WeaponGenome(); 
        g.id = this.id; 
        g.parentId = this.parentId; 
        g.fitness = this.fitness; 
        g.mapping = this.mapping;
        g.statScore = this.statScore;
        g.trackerScore = this.trackerScore;
        g.alignmentScore = this.alignmentScore;

        g.baseDamage = baseDamage;
        g.burstSize = burstSize;
        g.fireRate = fireRate;
        g.cooldownTime = cooldownTime;
        g.projectileSpeed = projectileSpeed;
        g.accuracy = accuracy;
        g.spreadAngle = spreadAngle;
        g.range = range;
        g.powerCost = powerCost;
        g.heatPerShot = heatPerShot;
        g.heatDissipation = heatDissipation;
        g.chargeUpTime = chargeUpTime;
        g.tileFootprint = tileFootprint;
        g.tileAffinity = tileAffinity;
        g.statusEffectStrength = statusEffectStrength;
        g.aoeRadius = aoeRadius;

        return g;
    }
}

public class ShipGenome
{
    public int id;
    public int parentId;

    public float fitness;
    public Vector3 mapping = Vector3.zero;

    public float statScore;
    public float trackerScore;
    public float alignmentScore;

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

    public static int nextShipId = 0;
    public static int GetNextShipId() => nextShipId++;

    public ShipGenome CloneForEvo()
    {
        ShipGenome s = new ShipGenome();
        s.id = GetNextShipId();
        s.parentId = this.id;

        s.hullHP = hullHP;
        s.armor = armor;
        s.shieldCapacity = shieldCapacity;
        s.shieldRegen = shieldRegen;
        s.powerCapacity = powerCapacity;
        s.powerRegen = powerRegen;
        s.speed = speed;
        s.turnRate = turnRate;
        s.evasion = evasion;
        s.mass = mass;
        s.inertia = inertia;
        s.gridWidth = gridWidth;
        s.gridHeight = gridHeight;
        s.specialTileDensity = specialTileDensity;
        s.droneCount = droneCount;
        s.droneSpeed = droneSpeed;
        s.droneDurability = droneDurability;
        s.droneAggression = droneAggression;

        return s;
    }

    public ShipGenome CloneExact()
    {
        ShipGenome s = new ShipGenome();
        s.id = this.id;
        s.parentId = this.parentId;
        s.fitness = this.fitness;
        s.mapping = this.mapping;
        s.statScore = this.statScore;
        s.trackerScore = this.trackerScore;
        s.alignmentScore = this.alignmentScore;

        s.hullHP = hullHP;
        s.armor = armor;
        s.shieldCapacity = shieldCapacity;
        s.shieldRegen = shieldRegen;
        s.powerCapacity = powerCapacity;
        s.powerRegen = powerRegen;
        s.speed = speed;
        s.turnRate = turnRate;
        s.evasion = evasion;
        s.mass = mass;
        s.inertia = inertia;
        s.gridWidth = gridWidth;
        s.gridHeight = gridHeight;
        s.specialTileDensity = specialTileDensity;
        s.droneCount = droneCount;
        s.droneSpeed = droneSpeed;
        s.droneDurability = droneDurability;
        s.droneAggression = droneAggression;

        return s;
    }
}
