using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GlobalSettings
{
    #region WeaponGenome Settings

    public const int WEAPON_DAMAGE_MAX = 60;
    public const int WEAPON_DAMAGE_MIN = 5;
    public const int WEAPON_BURSTSIZE_MAX = 8;
    public const int WEAPON_BURSTSIZE_MIN = 1;
    public const float WEAPON_FIRERATE_MAX = 10f;
    public const float WEAPON_FIRERATE_MIN = 0.5f;
    public const float WEAPON_COOLDOWNTIME_MAX = 5f;
    public const float WEAPON_COOLDOWNTIME_MIN = 0.1f;
    public const float WEAPON_PROJECTILE_SPEED_MAX = 60f;
    public const float WEAPON_PROJECTILE_SPEED_MIN = 5f;
    public const float WEAPON_ACCURACY_MAX = 1f;
    public const float WEAPON_ACCURACY_MIN = 0.3f;
    public const float WEAPON_SPREAD_ANGLE_MAX = 25f; //degrees
    public const float WEAPON_SPREAD_ANGLE_MIN = 0f;
    public const float WEAPON_RANGE_MAX = 40f;
    public const float WEAPON_RANGE_MIN = 5f;

    public const int WEAPON_POWERCOST_MAX = 15;
    public const int WEAPON_POWERCOST_MIN = 1;
    public const int WEAPON_HEATPERSHOT_MAX = 15;
    public const int WEAPON_HEATPERSHOT_MIN = 0;
    public const float WEAPON_HEATDISSIPATION_MAX = 15f;
    public const float WEAPON_HEATDISSIPATION_MIN = 0f;
    public const float WEAPON_CHARGEUPTIME_MAX = 3f;
    public const float WEAPON_CHARGEUPTIME_MIN = 0f;

    public const int WEAPON_TILEFOOTPRINT_X_MAX = 2;
    public const int WEAPON_TILEFOOTPRINT_X_MIN = 1;
    public const int WEAPON_TILEFOOTPRINT_Y_MAX = 2;
    public const int WEAPON_TILEFOOTPRINT_Y_MIN = 1;
    public const int WEAPON_TILEAFFINITY_X_MAX = 2;
    public const int WEAPON_TILEAFFINITY_X_MIN = 1;
    public const int WEAPON_TILEAFFINITY_Y_MAX = 2;
    public const int WEAPON_TILEAFFINITY_Y_MIN = 1;

    public const float WEAPON_STATUS_STRENGTH_MAX = 1f; // 0-1 scalar
    public const float WEAPON_STATUS_STRENGTH_MIN = 0f; // no effect
    public const float WEAPON_AOE_RADIUS_MAX = 6f;
    public const float WEAPON_AOE_RADIUS_MIN = 0f; // no AoE

    // Control Weights
    public const float WEAPON_RANGE_WEIGHT = 1.5f;
    public const float WEAPON_ACCURACY_WEIGHT = 1.7f;
    public const float WEAPON_STATUS_STRENGTH_WEIGHT_Y = 1.1f;

    // Close-Quarters Weights
    public const float WEAPON_PROJECTILE_SPEED_WEIGHT = 0.85f;
    public const float WEAPON_FIRERATE_WEIGHT = 0.75f;
    public const float WEAPON_SPREAD_ANGLE_WEIGHT = 0.55f;

    // Efficiency Weights
    public const float WEAPON_DPS_WEIGHT = 0.25f;
    public const float WEAPON_POWERCOST_WEIGHT = 0.4f;
    public const float WEAPON_HEATPERSHOT_WEIGHT = 0.35f;
    public const float WEAPON_CHARGEUP_EFF_WEIGHT = 0.35f;

    // Volatility Weights
    public const float WEAPON_AOE_WEIGHT = 2.2f;
    public const float WEAPON_STATUS_STRENGTH_WEIGHT_Z = 2.1f;
    public const float WEAPON_HEATDISSIPATION_WEIGHT = 2.6f;
    public const float WEAPON_CHARGEUP_VOL_WEIGHT = 2.4f;

    public const float WEAPON_SUSTAINED_WEIGHT = 2f;

    #endregion
    #region ShipGenome Settings

    public const int SHIP_HULLHP_MAX = 300;
    public const int SHIP_HULLHP_MIN = 50;
    public const int SHIP_ARMOR_MAX = 200;
    public const int SHIP_ARMOR_MIN = 0;
    public const int SHIP_SHIELD_CAPACITY_MAX = 250;
    public const int SHIP_SHIELD_CAPACITY_MIN = 50;
    public const int SHIP_SHIELD_REGEN_MAX = 40;
    public const int SHIP_SHIELD_REGEN_MIN = 5;

    public const int SHIP_POWER_CAPACITY_MAX = 200;
    public const int SHIP_POWER_CAPACITY_MIN = 50;
    public const int SHIP_POWER_REGEN_MAX = 30;
    public const int SHIP_POWER_REGEN_MIN = 5;

    public const float SHIP_SPEED_MAX = 80f;
    public const float SHIP_SPEED_MIN = 20f;
    public const float SHIP_TURNRATE_MAX = 1.2f;
    public const float SHIP_TURNRATE_MIN = 0.2f;
    public const float SHIP_EVASION_MAX = 0.5f;
    public const float SHIP_EVASION_MIN = 0f;
    public const float SHIP_MASS_MAX = 200f;
    public const float SHIP_MASS_MIN = 20f;
    public const float SHIP_INERTIA_MAX = 8f;
    public const float SHIP_INERTIA_MIN = 1f;

    public const int SHIP_GRID_WIDTH_MAX = 8;
    public const int SHIP_GRID_WIDTH_MIN = 2;
    public const int SHIP_GRID_HEIGHT_MAX = 8;
    public const int SHIP_GRID_HEIGHT_MIN = 2;
    public const float SHIP_SPECIALTILE_DENSITY_MAX = 5f;
    public const float SHIP_SPECIALTILE_DENSITY_MIN = 0f;

    public const int SHIP_DRONE_COUNT_MAX = 12;
    public const int SHIP_DRONE_COUNT_MIN = 0;
    public const float SHIP_DRONE_SPEED_MAX = 40f;
    public const float SHIP_DRONE_SPEED_MIN = 10f;
    public const int SHIP_DRONE_DURABILITY_MAX = 40;
    public const int SHIP_DRONE_DURABILITY_MIN = 5;
    public const float SHIP_DRONE_AGGRESSION_MAX = 1f;
    public const float SHIP_DRONE_AGGRESSION_MIN = 0f;

    // Durability Weights
    public const float SHIP_HULLHP_WEIGHT = 1.2f;
    public const float SHIP_ARMOR_WEIGHT = 1.0f;
    public const float SHIP_SHIELD_CAPACITY_WEIGHT = 1.0f;
    public const float SHIP_SHIELD_REGEN_WEIGHT = 0.8f;

    // Mobility Weights
    public const float SHIP_SPEED_WEIGHT = 1.2f;
    public const float SHIP_TURNRATE_WEIGHT = 1.0f;
    public const float SHIP_EVASION_WEIGHT = 0.8f;

    // Power Economy Weights
    public const float SHIP_POWER_CAPACITY_WEIGHT = 1.0f;
    public const float SHIP_POWER_REGEN_WEIGHT = 1.2f;
    public const float SHIP_SPECIALTILE_DENSITY_WEIGHT = 0.8f;

    // Weapon Platform Weights
    public const float SHIP_GRID_WIDTH_WEIGHT = 1.0f;
    public const float SHIP_GRID_HEIGHT_WEIGHT = 1.0f;
    public const float SHIP_DRONE_COUNT_WEIGHT = 1.2f;

    // Stability Weights
    public const float SHIP_MASS_WEIGHT = 1.0f;
    public const float SHIP_INERTIA_WEIGHT = 1.0f;
    public const float SHIP_DRONE_DURABILITY_WEIGHT = 1.2f;

    // Aggression Weights
    public const float SHIP_DRONE_SPEED_WEIGHT = 1.2f;
    public const float SHIP_DRONE_AGGRESSION_WEIGHT = 1.0f;

    #endregion

    #region Genome Methods

    public static WeaponGenome RandomWeaponGenome()
    {
        return new WeaponGenome()
        {
            id = WeaponGenome.GetNextWeaponId(),
            baseDamage = Random.Range(WEAPON_DAMAGE_MIN, WEAPON_DAMAGE_MAX),
            burstSize = Random.Range(WEAPON_BURSTSIZE_MIN, WEAPON_BURSTSIZE_MAX),
            fireRate = Random.Range(WEAPON_FIRERATE_MIN, WEAPON_FIRERATE_MAX),
            cooldownTime = Random.Range(WEAPON_COOLDOWNTIME_MIN, WEAPON_COOLDOWNTIME_MAX),
            projectileSpeed = Random.Range(WEAPON_PROJECTILE_SPEED_MIN, WEAPON_PROJECTILE_SPEED_MAX),
            accuracy = Random.Range(WEAPON_ACCURACY_MIN, WEAPON_ACCURACY_MAX),
            spreadAngle = Random.Range(WEAPON_SPREAD_ANGLE_MIN, WEAPON_SPREAD_ANGLE_MAX),
            range = Random.Range(WEAPON_RANGE_MIN, WEAPON_RANGE_MAX),
            powerCost = Random.Range(WEAPON_POWERCOST_MIN, WEAPON_POWERCOST_MAX),
            heatPerShot = Random.Range(WEAPON_HEATPERSHOT_MIN, WEAPON_HEATPERSHOT_MAX),
            heatDissipation = Random.Range(WEAPON_HEATDISSIPATION_MIN, WEAPON_HEATDISSIPATION_MAX),
            chargeUpTime = Random.Range(WEAPON_CHARGEUPTIME_MIN, WEAPON_CHARGEUPTIME_MAX),
            tileFootprint = new Vector2(Random.Range(WEAPON_TILEFOOTPRINT_X_MIN, WEAPON_TILEFOOTPRINT_X_MAX),
                            Random.Range(WEAPON_TILEFOOTPRINT_Y_MIN, WEAPON_TILEFOOTPRINT_Y_MAX)),
            tileAffinity = new Vector2(Random.Range(WEAPON_TILEAFFINITY_X_MIN, WEAPON_TILEAFFINITY_X_MAX),
                            Random.Range(WEAPON_TILEAFFINITY_Y_MIN, WEAPON_TILEAFFINITY_Y_MAX)),
            statusEffectType = (EffectType)Random.Range(0, (int)Enum.GetValues(typeof(EffectType)).Cast<EffectType>().Max()),
            statusEffectStrength = Random.Range(WEAPON_STATUS_STRENGTH_MIN, WEAPON_STATUS_STRENGTH_MAX),
            aoeRadius = Random.Range(WEAPON_AOE_RADIUS_MIN, WEAPON_AOE_RADIUS_MAX)
        };
    }

    public static ShipGenome RandomShipGenome()
    {
        return new ShipGenome()
        {
            id = ShipGenome.GetNextShipId(),
            hullHP = Random.Range(SHIP_HULLHP_MIN, SHIP_HULLHP_MAX),
            armor = Random.Range(SHIP_ARMOR_MIN, SHIP_ARMOR_MAX),
            shieldCapacity = Random.Range(SHIP_SHIELD_CAPACITY_MIN, SHIP_SHIELD_CAPACITY_MAX),
            shieldRegen = Random.Range(SHIP_SHIELD_REGEN_MIN, SHIP_SHIELD_REGEN_MAX),
            powerCapacity = Random.Range(SHIP_POWER_CAPACITY_MIN, SHIP_POWER_CAPACITY_MAX),
            powerRegen = Random.Range(SHIP_POWER_REGEN_MIN, SHIP_POWER_REGEN_MAX),
            speed = Random.Range(SHIP_SPEED_MIN, SHIP_SPEED_MAX),
            turnRate = Random.Range(SHIP_TURNRATE_MIN, SHIP_TURNRATE_MAX),
            evasion = Random.Range(SHIP_EVASION_MIN, SHIP_EVASION_MAX),
            mass = Random.Range(SHIP_MASS_MIN, SHIP_MASS_MAX),
            inertia = Random.Range(SHIP_INERTIA_MIN, SHIP_INERTIA_MAX),
            gridWidth = Random.Range(SHIP_GRID_WIDTH_MIN, SHIP_GRID_WIDTH_MAX),
            gridHeight = Random.Range(SHIP_GRID_HEIGHT_MIN, SHIP_GRID_HEIGHT_MAX),
            specialTileDensity = Random.Range(SHIP_SPECIALTILE_DENSITY_MIN, SHIP_SPECIALTILE_DENSITY_MAX),
            droneCount = Random.Range(SHIP_DRONE_COUNT_MIN, SHIP_DRONE_COUNT_MAX),
            droneSpeed = Random.Range(SHIP_DRONE_SPEED_MIN, SHIP_DRONE_SPEED_MAX),
            droneDurability = Random.Range(SHIP_DRONE_DURABILITY_MIN, SHIP_DRONE_DURABILITY_MAX),
            droneAggression = Random.Range(SHIP_DRONE_AGGRESSION_MIN, SHIP_DRONE_AGGRESSION_MAX)
        };
    }

    #endregion
}
