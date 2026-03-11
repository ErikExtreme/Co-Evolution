using UnityEngine;
using static GlobalSettings;

public class Mapping
{
    private static float weaponDeltaXMin;
    private static float weaponDeltaXMax;
    private static float weaponDeltaYMin;
    private static float weaponDeltaYMax;
    private static float weaponDeltaZMin;
    private static float weaponDeltaZMax;

    public static void Init()
    {
        weaponDeltaXMin = float.MaxValue;
        weaponDeltaXMax = float.MinValue;
        weaponDeltaYMin = float.MaxValue;
        weaponDeltaYMax = float.MinValue;
        weaponDeltaZMin = float.MaxValue;
        weaponDeltaZMax = float.MinValue;
    }

    /*
    public static Vector3 OldMapGenome(WeaponGenome genome)
    {
        int B = genome.baseDamage * genome.burstSize;
        float S = (genome.baseDamage * genome.fireRate) / (1 + genome.cooldownTime);

        float deltaX = B - S;
        float X = (deltaX - WeaponDeltaXMin) / (WeaponDeltaXMax - WeaponDeltaXMin) * 2 - 1;

        float wr = WEAPON_RANGE_WEIGHT;
        float wa = WEAPON_ACCURACY_WEIGHT;
        float ws = WEAPON_STATUS_STRENGTH_WEIGHT;
        float wp = WEAPON_PROJECTILE_SPEED_WEIGHT;
        float wf = WEAPON_FIRERATE_WEIGHT;
        float wh = WEAPON_SPREAD_ANGLE_WEIGHT;

        float C = wr * genome.range + wa * genome.accuracy + ws * genome.statusEffectStrength;
        float Q = wp * genome.projectileSpeed + wf * genome.fireRate + wh * genome.spreadAngle;
        float deltaY = C - Q;
        float Y = (deltaY - WeaponDeltaYMin) / (WeaponDeltaYMax - WeaponDeltaYMin) * 2 - 1;

        float DPS = (genome.baseDamage * genome.burstSize) / ((genome.burstSize / genome.fireRate) + genome.cooldownTime);
        float E = DPS / genome.powerCost + genome.accuracy + genome.heatDissipation;
        float V = genome.aoeRadius + genome.statusEffectStrength + genome.heatPerShot;
        float deltaZ = E - V;
        float Z = (deltaZ - WeaponDeltaZMin) / (WeaponDeltaZMax - WeaponDeltaZMin) * 2 - 1;

        Vector3 map = new Vector3(X, Y, Z);
        return map;
    }
    */
    public static Vector3 MapGenome(WeaponGenome g)
    {
        float Norm(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        // ---------------------------------------------------------
        // X-Axis: Burst vs Sustained
        // ---------------------------------------------------------
        float burstNorm = Norm(
            g.baseDamage * g.burstSize,
            WEAPON_DAMAGE_MIN * WEAPON_BURSTSIZE_MIN,
            WEAPON_DAMAGE_MAX * WEAPON_BURSTSIZE_MAX
        );

        float sustainedNorm = Norm(
            (g.baseDamage * g.fireRate) / (1f + g.cooldownTime),
            (WEAPON_DAMAGE_MIN * WEAPON_FIRERATE_MIN) / (1f + WEAPON_COOLDOWNTIME_MAX),
            (WEAPON_DAMAGE_MAX * WEAPON_FIRERATE_MAX) / (1f + WEAPON_COOLDOWNTIME_MIN)
        );

        float wBurst = 1.0f; // Does not have any scaling yet, if you want to add scaling, create a new variable in GlobalSettings
        float wSustained = WEAPON_SUSTAINED_WEIGHT;

        // Weighted, normalized difference (guaranteed in [-1,1])
        float X = (wBurst * burstNorm - wSustained * sustainedNorm) / (wBurst + wSustained);
        X = Mathf.Clamp(X * 1.6f, -1f, 1f);


        // ---------------------------------------------------------
        // Y-Axis: Control vs Close-Quarters
        // ---------------------------------------------------------
        float wr = WEAPON_RANGE_WEIGHT;
        float wa = WEAPON_ACCURACY_WEIGHT;
        float ws = WEAPON_STATUS_STRENGTH_WEIGHT_Y;
        float wp = WEAPON_PROJECTILE_SPEED_WEIGHT;
        float wf = WEAPON_FIRERATE_WEIGHT;
        float wh = WEAPON_SPREAD_ANGLE_WEIGHT;

        float controlWeighted =
            wr * Norm(g.range, WEAPON_RANGE_MIN, WEAPON_RANGE_MAX) +
            wa * Norm(g.accuracy, WEAPON_ACCURACY_MIN, WEAPON_ACCURACY_MAX) +
            ws * Norm(g.statusEffectStrength, WEAPON_STATUS_STRENGTH_MIN, WEAPON_STATUS_STRENGTH_MAX);

        float closeWeighted =
            wp * Norm(g.projectileSpeed, WEAPON_PROJECTILE_SPEED_MIN, WEAPON_PROJECTILE_SPEED_MAX) +
            wf * Norm(g.fireRate, WEAPON_FIRERATE_MIN, WEAPON_FIRERATE_MAX) +
            wh * Norm(g.spreadAngle, WEAPON_SPREAD_ANGLE_MIN, WEAPON_SPREAD_ANGLE_MAX);

        float controlMax = wr + wa + ws;
        float closeMax = wp + wf + wh;

        float controlNorm = controlWeighted / controlMax; // 0..1
        float closeNorm = closeWeighted / closeMax;     // 0..1

        float Y = controlNorm - closeNorm; // in [-1,1]


        // ---------------------------------------------------------
        // Z-Axis: Efficiency vs Volatility
        // ---------------------------------------------------------
        float DPS = (g.baseDamage * g.burstSize) /
                    ((g.burstSize / g.fireRate) + g.cooldownTime);

        float DPSNorm = Norm(DPS, WeaponDPSMin, WeaponDPSMax);
        float powCostNorm = Norm(g.powerCost, WEAPON_POWERCOST_MIN, WEAPON_POWERCOST_MAX);
        float heatDissNorm = Norm(g.heatDissipation, WEAPON_HEATDISSIPATION_MIN, WEAPON_HEATDISSIPATION_MAX);

        float aoeNorm = Norm(g.aoeRadius, WEAPON_AOE_RADIUS_MIN, WEAPON_AOE_RADIUS_MAX);
        float statusNorm = Norm(g.statusEffectStrength, WEAPON_STATUS_STRENGTH_MIN, WEAPON_STATUS_STRENGTH_MAX);
        float heatPerNorm = Norm(g.heatPerShot, WEAPON_HEATPERSHOT_MIN, WEAPON_HEATPERSHOT_MAX);

        float chargeNorm = Norm(g.chargeUpTime, WEAPON_CHARGEUPTIME_MIN, WEAPON_CHARGEUPTIME_MAX);

        float wDPS = WEAPON_DPS_WEIGHT;
        float wPowCost = WEAPON_POWERCOST_WEIGHT;
        float wHeatPer = WEAPON_HEATPERSHOT_WEIGHT;
        float wHeatD = WEAPON_HEATDISSIPATION_WEIGHT;
        float wAoE = WEAPON_AOE_WEIGHT;
        float wStatus = WEAPON_STATUS_STRENGTH_WEIGHT_Z;
        float wChargeE = WEAPON_CHARGEUP_EFF_WEIGHT;
        float wChargeV = WEAPON_CHARGEUP_VOL_WEIGHT;

        float effRaw = wDPS * DPSNorm + wPowCost * (1f - powCostNorm) + wHeatPer * (1f - heatPerNorm) + wChargeE * (1f - chargeNorm);
        float volRaw = wAoE * aoeNorm + wStatus * statusNorm + wHeatD * (1f - heatDissNorm) + wChargeV * chargeNorm;

        float effNorm = effRaw / (wDPS + wPowCost + wHeatPer + wChargeE);
        float volNorm = volRaw / (wAoE + wStatus + wHeatD + wChargeV);

        float Z = effNorm - volNorm; // guaranteed in [-1,1]

        return new Vector3(X, Y, Z);
    }

    public static Vector3 MapGenome(ShipGenome g)
    {
        float Norm(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }

        // ---------------------------------------------------------
        // X-Axis: Durability vs Mobility
        // ---------------------------------------------------------
        float hpNorm = Norm(g.hullHP, SHIP_HULLHP_MIN, SHIP_HULLHP_MAX);
        float armorNorm = Norm(g.armor, SHIP_ARMOR_MIN, SHIP_ARMOR_MAX);
        float shCapNorm = Norm(g.shieldCapacity, SHIP_SHIELD_CAPACITY_MIN, SHIP_SHIELD_CAPACITY_MAX);
        float shRegNorm = Norm(g.shieldRegen, SHIP_SHIELD_REGEN_MIN, SHIP_SHIELD_REGEN_MAX);

        float speedNorm = Norm(g.speed, SHIP_SPEED_MIN, SHIP_SPEED_MAX);
        float turnNorm = Norm(g.turnRate, SHIP_TURNRATE_MIN, SHIP_TURNRATE_MAX);
        float evaNorm = Norm(g.evasion, SHIP_EVASION_MIN, SHIP_EVASION_MAX);

        float wHp = SHIP_HULLHP_WEIGHT;
        float wArmor = SHIP_ARMOR_WEIGHT;
        float wShCap = SHIP_SHIELD_CAPACITY_WEIGHT;
        float wShReg = SHIP_SHIELD_REGEN_WEIGHT;
        float wSpeed = SHIP_SPEED_WEIGHT;
        float wTurn = SHIP_TURNRATE_WEIGHT;
        float wEva = SHIP_EVASION_WEIGHT;

        float durRaw = wHp * hpNorm + wArmor * armorNorm + wShCap * shCapNorm + wShReg * shRegNorm;
        float mobRaw = wSpeed * speedNorm + wTurn * turnNorm + wEva * evaNorm;

        float durNorm = durRaw / (wHp + wArmor + wShCap + wShReg);
        float mobNorm = mobRaw / (wSpeed + wTurn + wEva);

        float X = durNorm - mobNorm;


        // ---------------------------------------------------------
        // Y-Axis: Power economy vs Weapon platform
        // ---------------------------------------------------------
        float powCapNorm = Norm(g.powerCapacity, SHIP_POWER_CAPACITY_MIN, SHIP_POWER_CAPACITY_MAX);
        float powRegNorm = Norm(g.powerRegen, SHIP_POWER_REGEN_MIN, SHIP_POWER_REGEN_MAX);
        float spTiDenNorm = Norm(g.specialTileDensity, SHIP_SPECIALTILE_DENSITY_MIN, SHIP_SPECIALTILE_DENSITY_MAX);

        float gridWNorm = Norm(g.gridWidth, SHIP_GRID_WIDTH_MIN, SHIP_GRID_WIDTH_MAX);
        float gridHNorm = Norm(g.gridHeight, SHIP_GRID_HEIGHT_MIN, SHIP_GRID_HEIGHT_MAX);
        float droneNorm = Norm(g.droneCount, SHIP_DRONE_COUNT_MIN, SHIP_DRONE_COUNT_MAX);

        float wPowCap = SHIP_POWER_CAPACITY_WEIGHT;
        float wPowReg = SHIP_POWER_REGEN_WEIGHT;
        float wSpTiDen = SHIP_SPECIALTILE_DENSITY_WEIGHT;
        float wGridW = SHIP_GRID_WIDTH_WEIGHT;
        float wGridH = SHIP_GRID_HEIGHT_WEIGHT;
        float wDrone = SHIP_DRONE_COUNT_WEIGHT;

        float powRaw = wPowCap * powCapNorm + wPowReg * powRegNorm + wSpTiDen * spTiDenNorm;
        float wePRaw = wGridW * gridWNorm + wGridH * gridHNorm + wDrone * droneNorm;

        float powNorm = powRaw / (wPowCap + wPowReg + wSpTiDen);
        float wePNorm = wePRaw / (wGridW + wGridH + wDrone);

        float Y = powNorm - wePNorm;


        // ---------------------------------------------------------
        // Z-Axis: Stability vs Aggression
        // ---------------------------------------------------------
        float massNorm = Norm(g.mass, SHIP_MASS_MIN, SHIP_MASS_MAX);
        float inerNorm = Norm(g.inertia, SHIP_INERTIA_MIN, SHIP_INERTIA_MAX);
        float droDurNorm = Norm(g.droneDurability, SHIP_DRONE_DURABILITY_MIN, SHIP_DRONE_DURABILITY_MAX);

        float droSpeedNorm = Norm(g.droneSpeed, SHIP_DRONE_SPEED_MIN, SHIP_DRONE_SPEED_MAX);
        float droAgreNorm = Norm(g.droneAggression, SHIP_DRONE_AGGRESSION_MIN, SHIP_DRONE_AGGRESSION_MAX);

        float wMass = SHIP_MASS_WEIGHT;
        float wIner = SHIP_INERTIA_WEIGHT;
        float wDroDur = SHIP_DRONE_DURABILITY_WEIGHT;
        float wDroSpeed = SHIP_DRONE_SPEED_WEIGHT;
        float wDroAgre = SHIP_DRONE_AGGRESSION_WEIGHT;

        float staRaw = wMass * massNorm + wIner * inerNorm + wDroDur * droDurNorm;
        float aggRaw = wDroSpeed * droSpeedNorm + wDroAgre * droAgreNorm;

        float staNorm = staRaw / (wMass + wIner + wDroDur);
        float aggNorm = aggRaw / (wDroSpeed + wDroAgre);

        float Z = staNorm - aggNorm;

        return new Vector3(X, Y, Z);
    }

    public static Vector3 PlayerPreferenceWeaponMapping(PlayerBehaviorTracker p)
    {
        // X‑axis: Burst ↔ Sustained
        // High entropy and high turn rate → bursty, chaotic play
        // Long engagement distance → sustained, steady play
        float burst = Mathf.Clamp01((p.normEntropy + p.normTurnRate) * 0.5f);
        float sustained = Mathf.Clamp01(p.normEngagementDistance);
        float x = Mathf.Clamp(burst - sustained, -1f, 1f);

        // Y‑axis: Control ↔ Close‑Quarters
        // Long distance + low angle deviation → control
        // High turn rate + high entropy → CQ
        float control = Mathf.Clamp01((p.normEngagementDistance - p.normAngleToEnemy) * 0.5f);
        float cq = Mathf.Clamp01((p.normTurnRate + p.normEntropy) * 0.5f);
        float y = Mathf.Clamp(control - cq, -1f, 1f);

        // Z‑axis: Efficiency ↔ Volatility
        // Low entropy → efficient, precise
        // High entropy → volatile, chaotic
        float efficiency = Mathf.Clamp01(-p.normEntropy);
        float volatility = Mathf.Clamp01(p.normEntropy);
        float z = Mathf.Clamp(efficiency - volatility, -1f, 1f);

        return new Vector3(x, y, z);
    }

    public static Vector3 PlayerPreferenceShipMapping(PlayerBehaviorTracker p)
    {
        // X‑axis: Durability ↔ Mobility
        // High speed + high turn rate → mobility
        // Low entropy + low speed → durability
        float mobility = Mathf.Clamp01((p.normSpeed + p.normTurnRate) * 0.5f);
        float durability = Mathf.Clamp01(-mobility);
        float x = Mathf.Clamp(durability - mobility, -1f, 1f);

        // Y‑axis: Power Economy ↔ Weapon Platform
        // Long engagement distance → prefers range/accuracy → weapon platform
        // Close distance → prefers tanking/regen → power economy
        float weaponPlatform = Mathf.Clamp01(p.normEngagementDistance);
        float powerEconomy = Mathf.Clamp01(-p.normEngagementDistance);
        float y = Mathf.Clamp(powerEconomy - weaponPlatform, -1f, 1f);

        // Z‑axis: Stability ↔ Aggression
        // High entropy → aggressive
        // Low entropy → stable
        float aggression = Mathf.Clamp01(p.normEntropy);
        float stability = Mathf.Clamp01(-p.normEntropy);
        float z = Mathf.Clamp(stability - aggression, -1f, 1f);

        return new Vector3(x, y, z);
    }


    #region Weapon Delta Properties
    /*
    public static float WeaponDeltaXMin 
    { 
        get 
        {
            if (weaponDeltaXMin > 0) 
            { 
                int Bmin = WEAPON_DAMAGE_MIN * WEAPON_BURSTSIZE_MIN;
                float Smax = (WEAPON_DAMAGE_MAX * WEAPON_FIRERATE_MAX) / (1 + WEAPON_COOLDOWNTIME_MIN);
                weaponDeltaXMin = Bmin - Smax;
            }
            return weaponDeltaXMin;
        } }

    public static float WeaponDeltaXMax
    {
        get
        {
            if (weaponDeltaXMax < 0)
            {
                int Bmax = WEAPON_DAMAGE_MAX * WEAPON_BURSTSIZE_MAX;
                float Smin = (WEAPON_DAMAGE_MIN * WEAPON_FIRERATE_MIN) / (1 + WEAPON_COOLDOWNTIME_MAX);
                weaponDeltaXMax = Bmax - Smin;
            }
            return weaponDeltaXMax;
        }
    }

    public static float WeaponDeltaYMin
    {
        get
        {
            if (weaponDeltaYMin > 0)
            {
                float Cmin = WEAPON_RANGE_WEIGHT * WEAPON_RANGE_MIN +
                    WEAPON_ACCURACY_WEIGHT * WEAPON_ACCURACY_MIN +
                    WEAPON_STATUS_STRENGTH_WEIGHT * WEAPON_STATUS_STRENGTH_MIN;
                float Qmax = WEAPON_PROJECTILE_SPEED_WEIGHT * WEAPON_PROJECTILE_SPEED_MAX +
                    WEAPON_FIRERATE_WEIGHT * WEAPON_FIRERATE_MAX +
                    WEAPON_SPREAD_ANGLE_WEIGHT * WEAPON_SPREAD_ANGLE_MAX;
                weaponDeltaYMin = Cmin - Qmax;
            }
            return weaponDeltaYMin;
        }
    }

    public static float WeaponDeltaYMax
    {
        get
        {
            if (weaponDeltaYMax < 0)
            {
                float Cmax = WEAPON_RANGE_WEIGHT * WEAPON_RANGE_MAX +
                    WEAPON_ACCURACY_WEIGHT * WEAPON_ACCURACY_MAX +
                    WEAPON_STATUS_STRENGTH_WEIGHT * WEAPON_STATUS_STRENGTH_MAX;
                float Qmin = WEAPON_PROJECTILE_SPEED_WEIGHT * WEAPON_PROJECTILE_SPEED_MIN +
                    WEAPON_FIRERATE_WEIGHT * WEAPON_FIRERATE_MIN +
                    WEAPON_SPREAD_ANGLE_WEIGHT * WEAPON_SPREAD_ANGLE_MIN;
                weaponDeltaYMax = Cmax - Qmin;
            }
            return weaponDeltaYMax;
        }
    }

    public static float WeaponDeltaZMin
    {
        get
        {
            if (weaponDeltaZMin > 0)
            {
                float Emin = (WeaponDPSMin / WEAPON_POWERCOST_MAX) +
                    WEAPON_ACCURACY_MIN + WEAPON_HEATDISSIPATION_MIN;
                float Vmax = WEAPON_AOE_RADIUS_MAX + WEAPON_STATUS_STRENGTH_MAX + WEAPON_HEATPERSHOT_MAX;
                weaponDeltaZMin = Emin - Vmax;
            }
            return weaponDeltaZMin;
        }
    }

    public static float WeaponDeltaZMax
    {
        get
        {
            if (weaponDeltaZMax < 0)
            {
                float Emax = (WeaponDPSMax / WEAPON_POWERCOST_MIN) +
                    WEAPON_ACCURACY_MAX + WEAPON_HEATDISSIPATION_MAX;
                float Vmin = WEAPON_AOE_RADIUS_MIN + WEAPON_STATUS_STRENGTH_MIN + WEAPON_HEATPERSHOT_MIN;
                weaponDeltaZMax = Emax - Vmin;
            }
            return weaponDeltaZMax;
        }
    }
    */
    public static float WeaponDPSMin
    {
        get
        {
            float Nmin = WEAPON_DAMAGE_MIN * WEAPON_BURSTSIZE_MIN;
            float Dmax = (WEAPON_BURSTSIZE_MAX / WEAPON_FIRERATE_MIN) + WEAPON_COOLDOWNTIME_MAX;
            float DPS = Nmin / Dmax;
            return DPS;
        }
    }

    public static float WeaponDPSMax
    {
        get
        {
            float Nmax = WEAPON_DAMAGE_MAX * WEAPON_BURSTSIZE_MAX;
            float Dmin = (WEAPON_BURSTSIZE_MIN / WEAPON_FIRERATE_MAX) + WEAPON_COOLDOWNTIME_MIN;
            float DPS = Nmax / Dmin;
            return DPS;
        }
    }

    #endregion
}
