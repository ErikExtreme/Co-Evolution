using UnityEngine;
using static GlobalSettings;

public static class Fitness
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

    public static float CalculateFitness(WeaponGenome genome)
    {
        float fitness = 0;

        return fitness;
    }

    public static float CalculateFitness(ShipGenome genome)
    {
        float fitness = 0;

        return fitness;
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

    public static Vector3 MapGenome(ShipGenome genome)
    {
        Vector3 map = Vector3.zero;

        return map;
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
