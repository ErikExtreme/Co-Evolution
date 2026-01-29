using UnityEngine;

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

    public static Vector3 MapGenome(WeaponGenome genome)
    {
        int B = genome.baseDamage * genome.burstSize;
        float S = (genome.baseDamage * genome.fireRate) / (1 + genome.cooldownTime);

        float deltaX = B - S;
        float X = (deltaX - WeaponDeltaXMin) / (WeaponDeltaXMax - WeaponDeltaXMin) * 2 - 1;

        float wr = GlobalSettings.WEAPON_RANGE_WEIGHT;
        float wa = GlobalSettings.WEAPON_ACCURACY_WEIGHT;
        float ws = GlobalSettings.WEAPON_STATUS_STRENGTH_WEIGHT;
        float wp = GlobalSettings.WEAPON_PROJECTILE_SPEED_WEIGHT;
        float wf = GlobalSettings.WEAPON_FIRERATE_WEIGHT;
        float wh = GlobalSettings.WEAPON_SPREAD_ANGLE_WEIGHT;

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

    public static Vector3 MapGenome(ShipGenome genome)
    {
        Vector3 map = Vector3.zero;

        return map;
    }

    #region Weapon Delta Properties

    public static float WeaponDeltaXMin 
    { 
        get 
        {
            if (weaponDeltaXMin > 0) 
            { 
                int Bmin = GlobalSettings.WEAPON_DAMAGE_MIN * GlobalSettings.WEAPON_BURSTSIZE_MIN;
                float Smax = (GlobalSettings.WEAPON_DAMAGE_MAX * GlobalSettings.WEAPON_FIRERATE_MAX) / (1 + GlobalSettings.WEAPON_COOLDOWNTIME_MIN);
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
                int Bmax = GlobalSettings.WEAPON_DAMAGE_MAX * GlobalSettings.WEAPON_BURSTSIZE_MAX;
                float Smin = (GlobalSettings.WEAPON_DAMAGE_MIN * GlobalSettings.WEAPON_FIRERATE_MIN) / (1 + GlobalSettings.WEAPON_COOLDOWNTIME_MAX);
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
                float Cmin = GlobalSettings.WEAPON_RANGE_WEIGHT * GlobalSettings.WEAPON_RANGE_MIN +
                    GlobalSettings.WEAPON_ACCURACY_WEIGHT * GlobalSettings.WEAPON_ACCURACY_MIN +
                    GlobalSettings.WEAPON_STATUS_STRENGTH_WEIGHT * GlobalSettings.WEAPON_STATUS_STRENGTH_MIN;
                float Qmax = GlobalSettings.WEAPON_PROJECTILE_SPEED_WEIGHT * GlobalSettings.WEAPON_PROJECTILE_SPEED_MAX +
                    GlobalSettings.WEAPON_FIRERATE_WEIGHT * GlobalSettings.WEAPON_FIRERATE_MAX +
                    GlobalSettings.WEAPON_SPREAD_ANGLE_WEIGHT * GlobalSettings.WEAPON_SPREAD_ANGLE_MAX;
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
                float Cmax = GlobalSettings.WEAPON_RANGE_WEIGHT * GlobalSettings.WEAPON_RANGE_MAX +
                    GlobalSettings.WEAPON_ACCURACY_WEIGHT * GlobalSettings.WEAPON_ACCURACY_MAX +
                    GlobalSettings.WEAPON_STATUS_STRENGTH_WEIGHT * GlobalSettings.WEAPON_STATUS_STRENGTH_MAX;
                float Qmin = GlobalSettings.WEAPON_PROJECTILE_SPEED_WEIGHT * GlobalSettings.WEAPON_PROJECTILE_SPEED_MIN +
                    GlobalSettings.WEAPON_FIRERATE_WEIGHT * GlobalSettings.WEAPON_FIRERATE_MIN +
                    GlobalSettings.WEAPON_SPREAD_ANGLE_WEIGHT * GlobalSettings.WEAPON_SPREAD_ANGLE_MIN;
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
                float Emin = (WeaponDPSMin / GlobalSettings.WEAPON_POWERCOST_MAX) +
                    GlobalSettings.WEAPON_ACCURACY_MIN + GlobalSettings.WEAPON_HEATDISSIPATION_MIN;
                float Vmax = GlobalSettings.WEAPON_AOE_RADIUS_MAX + GlobalSettings.WEAPON_STATUS_STRENGTH_MAX + GlobalSettings.WEAPON_HEATPERSHOT_MAX;
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
                float Emax = (WeaponDPSMax / GlobalSettings.WEAPON_POWERCOST_MIN) +
                    GlobalSettings.WEAPON_ACCURACY_MAX + GlobalSettings.WEAPON_HEATDISSIPATION_MAX;
                float Vmin = GlobalSettings.WEAPON_AOE_RADIUS_MIN + GlobalSettings.WEAPON_STATUS_STRENGTH_MIN + GlobalSettings.WEAPON_HEATPERSHOT_MIN;
                weaponDeltaZMax = Emax - Vmin;
            }
            return weaponDeltaZMax;
        }
    }

    public static float WeaponDPSMin
    {
        get 
        {
            float Nmin = GlobalSettings.WEAPON_DAMAGE_MIN * GlobalSettings.WEAPON_BURSTSIZE_MIN;
            float Dmax = (GlobalSettings.WEAPON_BURSTSIZE_MAX / GlobalSettings.WEAPON_FIRERATE_MIN) + GlobalSettings.WEAPON_COOLDOWNTIME_MAX;
            float DPS = Nmin / Dmax;
            return DPS;
        }
    }

    public static float WeaponDPSMax
    {
        get
        {
            float Nmax = GlobalSettings.WEAPON_DAMAGE_MAX * GlobalSettings.WEAPON_BURSTSIZE_MAX;
            float Dmin = (GlobalSettings.WEAPON_BURSTSIZE_MIN / GlobalSettings.WEAPON_FIRERATE_MAX) + GlobalSettings.WEAPON_COOLDOWNTIME_MIN;
            float DPS = Nmax / Dmin;
            return DPS;
        }
    }

    #endregion
}
