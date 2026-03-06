using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GlobalSettings;

public class EvolutionManager : MonoBehaviour
{
    public int initalPopulationSize;
    public int burstPopSize;
    public int generations;

    public List<WeaponGenome> weapons;
    public List<ShipGenome> shipModules;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weapons = Seeding.RandomWeaponSeed(initalPopulationSize).ToList();
        shipModules = Seeding.RandomShipSeed(initalPopulationSize).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Evolve()
    {
        // 1. Get Player Tracker
        PlayerBehaviorTracker playerTracker = PlayerBehaviorTracker.Instance;

        // 2. Calculate Individual Fitness for all Weapons and Ship Modules
        foreach (var w in weapons)
        {
            Weapon weapon = WeaponManager.Instance.GetWeapon(w.id);
            if (weapon == null)
                return;
            w.fitness = Fitness.IndividualFitness(w, weapon.tracker, playerTracker);
        }
        foreach (var s in shipModules)
        {
            Module module = ModuleManager.Instance.GetModule(s.id);
            if (module == null)
                return;
            s.fitness = Fitness.IndividualFitness(s, module.tracker, playerTracker);
        }

        // 3. Clone the top X from the global population
        var weaponPop = weapons.OrderByDescending(g => g.fitness).Take(burstPopSize).Select(g => g.Clone()).ToList();
        var shipPop = shipModules.OrderByDescending(g => g.fitness).Take(burstPopSize).Select(g => g.Clone()).ToList();

        // 4. Run the Evolutionary Loop
        for (var gen = 0; gen < generations; gen++)
        {
            var nextGenW = new List<WeaponGenome>();
            var nextGenS = new List<ShipGenome>();

            // 4a. Elitism
            var eliteW = GetElite(weaponPop);
            nextGenW.Add(eliteW);
            var eliteS = GetElite(shipPop);
            nextGenS.Add(eliteS);

            // 4b. Fill rest of Population
            while (nextGenW.Count < burstPopSize)
            {
                var parent = SelectParent(weaponPop);
                var dir = ComputeMutationDirection(parent, weaponPop);
                var child = AxisAlignedMutation(parent, dir);
                child.fitness = Fitness.CooperativeFitness(child, shipPop, playerTracker);
                nextGenW.Add(child);
            }
            while (nextGenS.Count < burstPopSize)
            {
                var parent = SelectParent(shipPop);
                var dir = ComputeMutationDirection(parent, shipPop);
                var child = AxisAlignedMutation(parent, dir);
                child.fitness = Fitness.CooperativeFitness(child, weaponPop, playerTracker);
                nextGenS.Add(child);
            }

            weaponPop = nextGenW;
            shipPop = nextGenS;
        }
    }

    private WeaponGenome SelectParent(List<WeaponGenome> genomes)
    {
        return genomes[0].Clone();
    }

    private ShipGenome SelectParent(List<ShipGenome> genomes)
    {
        return genomes[0].Clone();
    }

    private Vector3 ComputeMutationDirection(WeaponGenome parent, List<WeaponGenome> population)
    {
        // 1. Map parent to axis space
        Vector3 parentAxis = Mapping.MapGenome(parent);

        // 2. Select top 20% of population
        int count = Mathf.Max(1, population.Count / 5);
        var top = population
            .OrderByDescending(g => g.fitness)
            .Take(count)
            .ToList();

        // 3. Compute centroid of top performers
        Vector3 centroid = Vector3.zero;
        foreach (var g in top)
            centroid += Mapping.MapGenome(g);
        centroid /= top.Count;

        // 4. Direction = centroid - parent
        Vector3 direction = centroid - parentAxis;

        // 5. Normalize and scale
        if (direction.sqrMagnitude > 0.0001f)
            direction = direction.normalized * 0.25f; // step size

        return direction;
    }

    private Vector3 ComputeMutationDirection(ShipGenome parent, List<ShipGenome> population)
    {
        // 1. Map parent to axis space
        Vector3 parentAxis = Mapping.MapGenome(parent);

        // 2. Select top 20% of population
        int count = Mathf.Max(1, population.Count / 5);
        var top = population
            .OrderByDescending(g => g.fitness)
            .Take(count)
            .ToList();

        // 3. Compute centroid of top performers
        Vector3 centroid = Vector3.zero;
        foreach (var g in top)
            centroid += Mapping.MapGenome(g);
        centroid /= top.Count;

        // 4. Direction = centroid - parent
        Vector3 direction = centroid - parentAxis;

        // 5. Normalize and scale
        if (direction.sqrMagnitude > 0.0001f)
            direction = direction.normalized * 0.25f; // step size

        return direction;
    }

    private WeaponGenome AxisAlignedMutation(WeaponGenome genome, Vector3 dir)
    {
        WeaponGenome g = genome.Clone();
        const float pct = 0.05f; // 5 percent mutation

        // --- Helper: mutate float gene by percentage ---
        float MutateFloat(float value, float min, float max, float sign)
        {
            float delta = pct * (max - min) * sign;
            float newVal = value + delta;
            return Mathf.Clamp(newVal, min, max);
        }

        // --- Helper: mutate int gene by percentage with stochastic rounding ---
        int MutateInt(int value, int min, int max, float sign)
        {
            float delta = pct * (max - min) * sign;
            float raw = value + delta;

            // Stochastic rounding
            int low = Mathf.FloorToInt(raw);
            float frac = raw - low;
            int rounded = (Random.value < frac) ? low + 1 : low;

            return Mathf.Clamp(rounded, min, max);
        }

        // ---------------------------------------------------------
        // X‑Axis: Burst (+X) vs Sustained (‑X)
        // ---------------------------------------------------------
        if (dir.x > 0f)
        {
            g.baseDamage = MutateInt(g.baseDamage, WEAPON_DAMAGE_MIN, WEAPON_DAMAGE_MAX, +1);
            g.burstSize = MutateInt(g.burstSize, WEAPON_BURSTSIZE_MIN, WEAPON_BURSTSIZE_MAX, +1);
            g.fireRate = MutateFloat(g.fireRate, WEAPON_FIRERATE_MIN, WEAPON_FIRERATE_MAX, -1);
            g.cooldownTime = MutateFloat(g.cooldownTime, WEAPON_COOLDOWNTIME_MIN, WEAPON_COOLDOWNTIME_MAX, +1);
        }
        else if (dir.x < 0f)
        {
            g.baseDamage = MutateInt(g.baseDamage, WEAPON_DAMAGE_MIN, WEAPON_DAMAGE_MAX, -1);
            g.burstSize = MutateInt(g.burstSize, WEAPON_BURSTSIZE_MIN, WEAPON_BURSTSIZE_MAX, -1);
            g.fireRate = MutateFloat(g.fireRate, WEAPON_FIRERATE_MIN, WEAPON_FIRERATE_MAX, +1);
            g.cooldownTime = MutateFloat(g.cooldownTime, WEAPON_COOLDOWNTIME_MIN, WEAPON_COOLDOWNTIME_MAX, -1);
        }

        // ---------------------------------------------------------
        // Y‑Axis: Control (+Y) vs Close‑Quarters (‑Y)
        // ---------------------------------------------------------
        if (dir.y > 0f)
        {
            g.range = MutateFloat(g.range, WEAPON_RANGE_MIN, WEAPON_RANGE_MAX, +1);
            g.accuracy = MutateFloat(g.accuracy, WEAPON_ACCURACY_MIN, WEAPON_ACCURACY_MAX, +1);
            g.statusEffectStrength = MutateFloat(g.statusEffectStrength, WEAPON_STATUS_STRENGTH_MIN, WEAPON_STATUS_STRENGTH_MAX, +1);

            g.projectileSpeed = MutateFloat(g.projectileSpeed, WEAPON_PROJECTILE_SPEED_MIN, WEAPON_PROJECTILE_SPEED_MAX, -1);
            g.fireRate = MutateFloat(g.fireRate, WEAPON_FIRERATE_MIN, WEAPON_FIRERATE_MAX, -1);
            g.spreadAngle = MutateFloat(g.spreadAngle, WEAPON_SPREAD_ANGLE_MIN, WEAPON_SPREAD_ANGLE_MAX, -1);
        }
        else if (dir.y < 0f)
        {
            g.range = MutateFloat(g.range, WEAPON_RANGE_MIN, WEAPON_RANGE_MAX, -1);
            g.accuracy = MutateFloat(g.accuracy, WEAPON_ACCURACY_MIN, WEAPON_ACCURACY_MAX, -1);
            g.statusEffectStrength = MutateFloat(g.statusEffectStrength, WEAPON_STATUS_STRENGTH_MIN, WEAPON_STATUS_STRENGTH_MAX, -1);

            g.projectileSpeed = MutateFloat(g.projectileSpeed, WEAPON_PROJECTILE_SPEED_MIN, WEAPON_PROJECTILE_SPEED_MAX, +1);
            g.fireRate = MutateFloat(g.fireRate, WEAPON_FIRERATE_MIN, WEAPON_FIRERATE_MAX, +1);
            g.spreadAngle = MutateFloat(g.spreadAngle, WEAPON_SPREAD_ANGLE_MIN, WEAPON_SPREAD_ANGLE_MAX, +1);
        }

        // ---------------------------------------------------------
        // Z‑Axis: Efficiency (+Z) vs Volatility (‑Z)
        // ---------------------------------------------------------
        if (dir.z > 0f)
        {
            g.powerCost = MutateInt(g.powerCost, WEAPON_POWERCOST_MIN, WEAPON_POWERCOST_MAX, -1);
            g.heatPerShot = MutateInt(g.heatPerShot, WEAPON_HEATPERSHOT_MIN, WEAPON_HEATPERSHOT_MAX, -1);
            g.heatDissipation = MutateFloat(g.heatDissipation, WEAPON_HEATDISSIPATION_MIN, WEAPON_HEATDISSIPATION_MAX, +1);
            g.chargeUpTime = MutateFloat(g.chargeUpTime, WEAPON_CHARGEUPTIME_MIN, WEAPON_CHARGEUPTIME_MAX, -1);
        }
        else if (dir.z < 0f)
        {
            g.powerCost = MutateInt(g.powerCost, WEAPON_POWERCOST_MIN, WEAPON_POWERCOST_MAX, +1);
            g.heatPerShot = MutateInt(g.heatPerShot, WEAPON_HEATPERSHOT_MIN, WEAPON_HEATPERSHOT_MAX, +1);
            g.heatDissipation = MutateFloat(g.heatDissipation, WEAPON_HEATDISSIPATION_MIN, WEAPON_HEATDISSIPATION_MAX, -1);
            g.chargeUpTime = MutateFloat(g.chargeUpTime, WEAPON_CHARGEUPTIME_MIN, WEAPON_CHARGEUPTIME_MAX, +1);
        }

        return g;
    }

    private ShipGenome AxisAlignedMutation(ShipGenome genome, Vector3 dir)
    {
        ShipGenome g = genome.Clone();
        const float pct = 0.05f; // 5 percent mutation per step

        // --- Helper: mutate float gene by percentage ---
        float MutateFloat(float value, float min, float max, float sign)
        {
            float delta = pct * (max - min) * sign;
            float newVal = value + delta;
            return Mathf.Clamp(newVal, min, max);
        }

        // --- Helper: mutate int gene by percentage with stochastic rounding ---
        int MutateInt(int value, int min, int max, float sign)
        {
            float delta = pct * (max - min) * sign;
            float raw = value + delta;

            int low = Mathf.FloorToInt(raw);
            float frac = raw - low;
            int rounded = (Random.value < frac) ? low + 1 : low;

            return Mathf.Clamp(rounded, min, max);
        }

        // ---------------------------------------------------------
        // X‑Axis: Durability (+X) vs Mobility (‑X)
        // ---------------------------------------------------------
        if (dir.x > 0f)
        {
            g.hullHP = MutateInt(g.hullHP, SHIP_HULLHP_MIN, SHIP_HULLHP_MAX, +1);
            g.armor = MutateInt(g.armor, SHIP_ARMOR_MIN, SHIP_ARMOR_MAX, +1);
            g.shieldCapacity = MutateInt(g.shieldCapacity, SHIP_SHIELD_CAPACITY_MIN, SHIP_SHIELD_CAPACITY_MAX, +1);
            g.shieldRegen = MutateInt(g.shieldRegen, SHIP_SHIELD_REGEN_MIN, SHIP_SHIELD_REGEN_MAX, +1);

            g.speed = MutateFloat(g.speed, SHIP_SPEED_MIN, SHIP_SPEED_MAX, -1);
            g.turnRate = MutateFloat(g.turnRate, SHIP_TURNRATE_MIN, SHIP_TURNRATE_MAX, -1);
            g.evasion = MutateFloat(g.evasion, SHIP_EVASION_MIN, SHIP_EVASION_MAX, -1);
        }
        else if (dir.x < 0f)
        {
            g.hullHP = MutateInt(g.hullHP, SHIP_HULLHP_MIN, SHIP_HULLHP_MAX, -1);
            g.armor = MutateInt(g.armor, SHIP_ARMOR_MIN, SHIP_ARMOR_MAX, -1);
            g.shieldCapacity = MutateInt(g.shieldCapacity, SHIP_SHIELD_CAPACITY_MIN, SHIP_SHIELD_CAPACITY_MAX, -1);
            g.shieldRegen = MutateInt(g.shieldRegen, SHIP_SHIELD_REGEN_MIN, SHIP_SHIELD_REGEN_MAX, -1);

            g.speed = MutateFloat(g.speed, SHIP_SPEED_MIN, SHIP_SPEED_MAX, +1);
            g.turnRate = MutateFloat(g.turnRate, SHIP_TURNRATE_MIN, SHIP_TURNRATE_MAX, +1);
            g.evasion = MutateFloat(g.evasion, SHIP_EVASION_MIN, SHIP_EVASION_MAX, +1);
        }

        // ---------------------------------------------------------
        // Y‑Axis: Power Economy (+Y) vs Weapon Platform (‑Y)
        // ---------------------------------------------------------
        if (dir.y > 0f)
        {
            g.powerCapacity = MutateInt(g.powerCapacity, SHIP_POWER_CAPACITY_MIN, SHIP_POWER_CAPACITY_MAX, +1);
            g.powerRegen = MutateInt(g.powerRegen, SHIP_POWER_REGEN_MIN, SHIP_POWER_REGEN_MAX, +1);
            g.specialTileDensity = MutateFloat(g.specialTileDensity, SHIP_SPECIALTILE_DENSITY_MIN, SHIP_SPECIALTILE_DENSITY_MAX, +1);

            g.gridWidth = MutateInt(g.gridWidth, SHIP_GRID_WIDTH_MIN, SHIP_GRID_WIDTH_MAX, -1);
            g.gridHeight = MutateInt(g.gridHeight, SHIP_GRID_HEIGHT_MIN, SHIP_GRID_HEIGHT_MAX, -1);
            g.droneCount = MutateInt(g.droneCount, SHIP_DRONE_COUNT_MIN, SHIP_DRONE_COUNT_MAX, -1);
        }
        else if (dir.y < 0f)
        {
            g.powerCapacity = MutateInt(g.powerCapacity, SHIP_POWER_CAPACITY_MIN, SHIP_POWER_CAPACITY_MAX, -1);
            g.powerRegen = MutateInt(g.powerRegen, SHIP_POWER_REGEN_MIN, SHIP_POWER_REGEN_MAX, -1);
            g.specialTileDensity = MutateFloat(g.specialTileDensity, SHIP_SPECIALTILE_DENSITY_MIN, SHIP_SPECIALTILE_DENSITY_MAX, -1);

            g.gridWidth = MutateInt(g.gridWidth, SHIP_GRID_WIDTH_MIN, SHIP_GRID_WIDTH_MAX, +1);
            g.gridHeight = MutateInt(g.gridHeight, SHIP_GRID_HEIGHT_MIN, SHIP_GRID_HEIGHT_MAX, +1);
            g.droneCount = MutateInt(g.droneCount, SHIP_DRONE_COUNT_MIN, SHIP_DRONE_COUNT_MAX, +1);
        }

        // ---------------------------------------------------------
        // Z‑Axis: Stability (+Z) vs Aggression (‑Z)
        // ---------------------------------------------------------
        if (dir.z > 0f)
        {
            g.mass = MutateFloat(g.mass, SHIP_MASS_MIN, SHIP_MASS_MAX, +1);
            g.inertia = MutateFloat(g.inertia, SHIP_INERTIA_MIN, SHIP_INERTIA_MAX, +1);
            g.droneDurability = MutateInt(g.droneDurability, SHIP_DRONE_DURABILITY_MIN, SHIP_DRONE_DURABILITY_MAX, +1);

            g.droneSpeed = MutateFloat(g.droneSpeed, SHIP_DRONE_SPEED_MIN, SHIP_DRONE_SPEED_MAX, -1);
            g.droneAggression = MutateFloat(g.droneAggression, SHIP_DRONE_AGGRESSION_MIN, SHIP_DRONE_AGGRESSION_MAX, -1);
        }
        else if (dir.z < 0f)
        {
            g.mass = MutateFloat(g.mass, SHIP_MASS_MIN, SHIP_MASS_MAX, -1);
            g.inertia = MutateFloat(g.inertia, SHIP_INERTIA_MIN, SHIP_INERTIA_MAX, -1);
            g.droneDurability = MutateInt(g.droneDurability, SHIP_DRONE_DURABILITY_MIN, SHIP_DRONE_DURABILITY_MAX, -1);

            g.droneSpeed = MutateFloat(g.droneSpeed, SHIP_DRONE_SPEED_MIN, SHIP_DRONE_SPEED_MAX, +1);
            g.droneAggression = MutateFloat(g.droneAggression, SHIP_DRONE_AGGRESSION_MIN, SHIP_DRONE_AGGRESSION_MAX, +1);
        }

        return g;
    }

    private WeaponGenome GetElite(List<WeaponGenome> genomes)
    {
        return genomes[0].Clone();
    }

    private ShipGenome GetElite(List<ShipGenome> genomes)
    {
        return genomes[0].Clone();
    }
}
