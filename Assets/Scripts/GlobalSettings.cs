using UnityEngine;

public static class GlobalSettings
{
    #region WeaponGenome Settings

    public const int WEAPON_DAMAGE_MAX = 120;
    public const int WEAPON_DAMAGE_MIN = 10;
    public const int WEAPON_BURSTSIZE_MAX = 100;
    public const int WEAPON_BURSTSIZE_MIN = 1;
    public const float WEAPON_FIRERATE_MAX = 100f;
    public const float WEAPON_FIRERATE_MIN = 10f;
    public const float WEAPON_COOLDOWNTIME_MAX = 100f;
    public const float WEAPON_COOLDOWNTIME_MIN = 1f;
    public const float WEAPON_PROJECTILE_SPEED_MAX = 100f;
    public const float WEAPON_PROJECTILE_SPEED_MIN = 1f;
    public const float WEAPON_ACCURACY_MAX = 100f;
    public const float WEAPON_ACCURACY_MIN = 1f;
    public const float WEAPON_SPREAD_ANGLE_MAX = 100f;
    public const float WEAPON_SPREAD_ANGLE_MIN = 100f;
    public const float WEAPON_RANGE_MAX = 100f;
    public const float WEAPON_RANGE_MIN = 1f;

    public const int WEAPON_POWERCOST_MAX = 100;
    public const int WEAPON_POWERCOST_MIN = 1;
    public const int WEAPON_HEATPERSHOT_MAX = 100;
    public const int WEAPON_HEATPERSHOT_MIN = 1;
    public const float WEAPON_HEATDISSIPATION_MAX = 100f;
    public const float WEAPON_HEATDISSIPATION_MIN = 1f;
    public const float WEAPON_CHARGEUPTIME_MAX = 100f;
    public const float WEAPON_CHARGEUPTIME_MIN = 1f;

    public const int WEAPON_TILEFOOTPRINT_X_MAX = 2;
    public const int WEAPON_TILEFOOTPRINT_X_MIN = 1;
    public const int WEAPON_TILEFOOTPRINT_Y_MAX = 2;
    public const int WEAPON_TILEFOOTPRINT_Y_MIN = 1;
    public const int WEAPON_TILEAFFINITY_X_MAX = 2;
    public const int WEAPON_TILEAFFINITY_X_MIN = 1;
    public const int WEAPON_TILEAFFINITY_Y_MAX = 2;
    public const int WEAPON_TILEAFFINITY_Y_MIN = 1;

    #endregion
    #region ShipGenome Settings

    public const int SHIP_HULLHP_MAX = 100;
    public const int SHIP_HULLHP_MIN = 10;
    public const int SHIP_ARMOR_MAX = 100;
    public const int SHIP_ARMOR_MIN = 10;
    public const int SHIP_SHIELD_CAPACITY_MAX = 100;
    public const int SHIP_SHIELD_CAPACITY_MIN = 10;
    public const int SHIP_SHIELD_REGEN_MAX = 100;
    public const int SHIP_SHIELD_REGEN_MIN = 10;
    public const int SHIP_POWER_CAPACITY_MAX = 100;
    public const int SHIP_POWER_CAPACITY_MIN = 10;
    public const int SHIP_POWER_REGEN_MAX = 100;
    public const int SHIP_POWER_REGEN_MIN = 10;

    public const float SHIP_SPEED_MAX = 100f;
    public const float SHIP_SPEED_MIN = 1f;
    public const float SHIP_TURNRATE_MAX = 1f;
    public const float SHIP_TURNRATE_MIN = 0.1f;
    public const float SHIP_EVASION_MAX = 1f;
    public const float SHIP_EVASION_MIN = 0.1f;
    public const float SHIP_MASS_MAX = 100f;
    public const float SHIP_MASS_MIN = 1f;
    public const float SHIP_INERTIA_MAX = 10f;
    public const float SHIP_INERTIA_MIN = 1f;

    public const int SHIP_GRID_WIDTH_MAX = 5;
    public const int SHIP_GRID_WIDTH_MIN = 0;
    public const int SHIP_GRID_HEIGHT_MAX = 5;
    public const int SHIP_GRID_HEIGHT_MIN = 0;
    public const float SHIP_SPECIALTILE_DENSITY_MAX = 10f;
    public const float SHIP_SPECIALTILE_DENSITY_MIN = 0f;

    public const int SHIP_DRONE_COUNT_MAX = 20;
    public const int SHIP_DRONE_COUNT_MIN = 2;
    public const float SHIP_DRONE_SPEED_MAX = 100f;
    public const float SHIP_DRONE_SPEED_MIN = 1f;
    public const int SHIP_DRONE_DURABILITY_MAX = 20;
    public const int SHIP_DRONE_DURABILITY_MIN = 1;
    public const float SHIP_DRONE_AGGRESSION_MAX = 1f;
    public const float SHIP_DRONE_AGGRESSION_MIN = 0f;

    #endregion

    public static WeaponGenome RandomWeaponGenome()
    {
        return new WeaponGenome()
        {
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
                            Random.Range(WEAPON_TILEAFFINITY_Y_MIN, WEAPON_TILEAFFINITY_Y_MAX))
        };
    }

    public static ShipGenome RandomShipGenome()
    {
        return new ShipGenome()
        {
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
}
