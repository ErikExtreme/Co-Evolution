using System.Collections.Generic;
using System.IO;
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

    public SessionLog sessionLog;
    private int evolveIndex = -1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sessionLog = EvolutionLogger.CreateNewSession();
        weapons = Seeding.RandomWeaponSeed(initalPopulationSize).ToList();
        foreach (var weaponGenome in weapons)
        {
            WeaponManager.Instance.AddWeapon(weaponGenome, new WeaponStatsTracker());
            weaponGenome.mapping = Mapping.MapGenome(weaponGenome);
        }

        shipModules = Seeding.RandomShipSeed(initalPopulationSize).ToList();
        foreach (var moduleGenome in shipModules)
        {
            ModuleManager.Instance.AddModule(moduleGenome, new ModuleStatsTracker());
            moduleGenome.mapping = Mapping.MapGenome(moduleGenome);
        }
    }

    private void OnApplicationQuit()
    {
        if (sessionLog != null) 
        {
            // Called once per session
            string logFolder = Path.Combine(Application.persistentDataPath, "EvolutionLogs");
            Directory.CreateDirectory(logFolder);

            string logFilePath = Path.Combine(logFolder, "evo_" + sessionLog.sessionId + ".json");

            // Called whenever you want to save
            EvolutionLogger.SaveLog(sessionLog, logFilePath);
        }
    }

    public (List<WeaponGenome> weapons, List<ShipGenome> modules) Evolve()
    {
        // 1. Get Player Tracker
        PlayerBehaviorTracker playerTracker = PlayerBehaviorTracker.Instance;

        var rec = EvolutionLogger.BeginRecording(sessionLog, evolveIndex++, playerTracker);

        // 2. Calculate Individual Fitness for all Weapons and Ship Modules
        foreach (var w in weapons)
        {
            (WeaponGenome genome, WeaponStatsTracker tracker) weapon = WeaponManager.Instance.GetWeapon(w.id);
            if (weapon.genome == null || weapon.tracker == null)
            {
                w.fitness = 0;
                Debug.LogWarning("Could not find weapon for id: " + w.id);
                continue;
            }
            w.fitness = Fitness.IndividualFitness(w, weapon.tracker, playerTracker);
            w.finalFitness = w.fitness;
        }
        foreach (var s in shipModules)
        {
            (ShipGenome genome, ModuleStatsTracker tracker) module = ModuleManager.Instance.GetModule(s.id);
            if (module.genome == null || module.tracker == null)
            {
                s.fitness = 0;
                Debug.LogWarning("Could not find module for id: " + s.id);
                continue;
            }
            s.fitness = Fitness.IndividualFitness(s, module.tracker, playerTracker);
            s.finalFitness = s.fitness;
        }

        EvolutionLogger.RecordGlobalPopulation(rec, weapons, shipModules);

        // 3. Clone the top X from the global population
        var weaponPop = weapons.OrderByDescending(g => g.fitness).Take(burstPopSize).Select(g => g.CloneExact()).ToList();
        var shipPop = shipModules.OrderByDescending(g => g.fitness).Take(burstPopSize).Select(g => g.CloneExact()).ToList();

        // 4. Run the Evolutionary Loop
        for (var gen = 0; gen < generations; gen++)
        {
            var nextGenW = new List<WeaponGenome>();
            var nextGenS = new List<ShipGenome>();

            var mutationDirsW = new List<Vector3>();
            var mutationDirsS = new List<Vector3>();

            // 4a. Elitism
            var eliteW = GetElite(weaponPop);
            nextGenW.Add(eliteW);
            mutationDirsW.Add(Vector3.zero);

            var eliteS = GetElite(shipPop);
            nextGenS.Add(eliteS);
            mutationDirsS.Add(Vector3.zero);

            // 4b. Fill rest of Population
            while (nextGenW.Count < burstPopSize)
            {
                var parent = SelectParent(weaponPop);
                var dir = ComputeMutationDirection_NSLC(parent, weaponPop);
                var child = AxisAlignedMutation(parent, dir);

                child.fitness = Fitness.CooperativeFitness(child, shipPop, playerTracker);

                nextGenW.Add(child);
                mutationDirsW.Add(dir);
            }
            while (nextGenS.Count < burstPopSize)
            {
                var parent = SelectParent(shipPop);
                var dir = ComputeMutationDirection_NSLC(parent, shipPop);
                var child = AxisAlignedMutation(parent, dir);

                child.fitness = Fitness.CooperativeFitness(child, weaponPop, playerTracker);

                nextGenS.Add(child);
                mutationDirsS.Add(dir);
            }

            foreach (var g in nextGenW)
            {
                g.novelty = Fitness.ComputeNovelty(g, nextGenW);
                g.finalFitness = g.fitness * 0.5f + g.novelty * 0.5f;
            }
            foreach (var g in nextGenS)
            {
                g.novelty = Fitness.ComputeNovelty(g, nextGenS);
                g.finalFitness = g.fitness * 0.5f + g.novelty * 0.5f;
            }

            EvolutionLogger.RecordGeneration(rec, gen, nextGenW, mutationDirsW, nextGenS, mutationDirsS);

            weaponPop = nextGenW;
            shipPop = nextGenS;
        }

        //weaponPop = weaponPop.OrderByDescending(g => g.finalFitness).ToList();
        //shipPop = shipPop.OrderByDescending(g => g.finalFitness).ToList();

        weaponPop = Fitness.SelectDiverseTopX(weaponPop, 20);
        shipPop = Fitness.SelectDiverseTopX(shipPop, 20);
        return (weaponPop, shipPop);
    }

    private WeaponGenome SelectParent(List<WeaponGenome> genomes)
    {
        int k = 3;
        WeaponGenome best = null;

        for (int i = 0; i < k; i++)
        {
            var candidate = genomes[Random.Range(0, genomes.Count)];
            if (best == null || candidate.finalFitness > best.finalFitness)
                best = candidate;
        }

        return best.CloneExact();
    }

    private ShipGenome SelectParent(List<ShipGenome> genomes)
    {
        int k = 3;
        ShipGenome best = null;

        for (int i = 0; i < k; i++)
        {
            var candidate = genomes[Random.Range(0, genomes.Count)];
            if (best == null || candidate.finalFitness > best.finalFitness)
                best = candidate;
        }

        return best.CloneExact();
    }

    private Vector3 ComputeMutationDirection(WeaponGenome parent, List<WeaponGenome> population)
    {
        // 1. Map parent to axis space
        Vector3 parentAxis = Mapping.MapGenome(parent);

        // 2. Select top 20% of population
        int count = Mathf.Max(1, population.Count / 5);
        var top = population
            .OrderByDescending(g => g.finalFitness)
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

        Vector3 noise = new Vector3(
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f));
        
        direction += noise;

        return direction;
    }

    private Vector3 ComputeMutationDirection(ShipGenome parent, List<ShipGenome> population)
    {
        // 1. Map parent to axis space
        Vector3 parentAxis = Mapping.MapGenome(parent);

        // 2. Select top 20% of population
        int count = Mathf.Max(1, population.Count / 5);
        var top = population
            .OrderByDescending(g => g.finalFitness)
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

        Vector3 noise = new Vector3(
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f));

        direction += noise;

        return direction;
    }

    private Vector3 ComputeMutationDirection_NSLC(WeaponGenome parent, List<WeaponGenome> population)
    {
        Vector3 parentAxis = Mapping.MapGenome(parent);

        // --- 1. Find nearest neighbor (for novelty repulsion) ---
        WeaponGenome nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var g in population)
        {
            if (g == parent) continue;

            Vector3 axis = Mapping.MapGenome(g);
            float d = Vector3.Distance(parentAxis, axis);

            if (d < nearestDist)
            {
                nearestDist = d;
                nearest = g;
            }
        }

        Vector3 noveltyDir = Vector3.zero;
        if (nearest != null)
        {
            Vector3 nearestAxis = Mapping.MapGenome(nearest);
            noveltyDir = (parentAxis - nearestAxis).normalized; // push away
        }

        // --- 2. Find nearest higher-fitness neighbor (local competition) ---
        WeaponGenome bestLocal = null;
        float bestLocalDist = float.MaxValue;

        foreach (var g in population)
        {
            if (g == parent) continue;
            if (g.finalFitness <= parent.finalFitness) continue; // must be better

            Vector3 axis = Mapping.MapGenome(g);
            float d = Vector3.Distance(parentAxis, axis);

            if (d < bestLocalDist)
            {
                bestLocalDist = d;
                bestLocal = g;
            }
        }

        Vector3 localCompDir = Vector3.zero;
        if (bestLocal != null)
        {
            Vector3 bestAxis = Mapping.MapGenome(bestLocal);
            localCompDir = (bestAxis - parentAxis).normalized; // climb toward better neighbor
        }

        // --- 3. Combine novelty + local competition ---
        // NSLC typically uses a balance between exploration and exploitation.
        Vector3 direction = noveltyDir * 0.6f + localCompDir * 0.4f;

        // --- 4. Add exploration noise ---
        Vector3 noise = new Vector3(
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f)
        );

        direction += noise;

        // --- 5. Normalize and scale ---
        if (direction.sqrMagnitude > 0.0001f)
            direction = direction.normalized * 0.25f;

        return direction;
    }

    private Vector3 ComputeMutationDirection_NSLC(ShipGenome parent, List<ShipGenome> population)
    {
        Vector3 parentAxis = Mapping.MapGenome(parent);

        // --- 1. Nearest neighbor (novelty repulsion) ---
        ShipGenome nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var g in population)
        {
            if (g == parent) continue;

            Vector3 axis = Mapping.MapGenome(g);
            float d = Vector3.Distance(parentAxis, axis);

            if (d < nearestDist)
            {
                nearestDist = d;
                nearest = g;
            }
        }

        Vector3 noveltyDir = Vector3.zero;
        if (nearest != null)
        {
            Vector3 nearestAxis = Mapping.MapGenome(nearest);
            noveltyDir = (parentAxis - nearestAxis).normalized;
        }

        // --- 2. Nearest higher-fitness neighbor (local competition) ---
        ShipGenome bestLocal = null;
        float bestLocalDist = float.MaxValue;

        foreach (var g in population)
        {
            if (g == parent) continue;
            if (g.finalFitness <= parent.finalFitness) continue;

            Vector3 axis = Mapping.MapGenome(g);
            float d = Vector3.Distance(parentAxis, axis);

            if (d < bestLocalDist)
            {
                bestLocalDist = d;
                bestLocal = g;
            }
        }

        Vector3 localCompDir = Vector3.zero;
        if (bestLocal != null)
        {
            Vector3 bestAxis = Mapping.MapGenome(bestLocal);
            localCompDir = (bestAxis - parentAxis).normalized;
        }

        // --- 3. Combine novelty + local competition ---
        Vector3 direction = noveltyDir * 0.6f + localCompDir * 0.4f;

        // --- 4. Add exploration noise ---
        Vector3 noise = new Vector3(
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f),
            Random.Range(-0.1f, 0.1f)
        );

        direction += noise;

        // --- 5. Normalize and scale ---
        if (direction.sqrMagnitude > 0.0001f)
            direction = direction.normalized * 0.25f;

        return direction;
    }

    private WeaponGenome AxisAlignedMutation(WeaponGenome genome, Vector3 dir)
    {
        WeaponGenome g = genome.CloneForEvo();

        // Scale factor for how strongly direction affects mutation
        const float mutationScale = 0.05f; // 5% of axis direction magnitude

        // Convert axis direction into per-axis mutation strength
        float stepX = dir.x * mutationScale;
        float stepY = dir.y * mutationScale;
        float stepZ = dir.z * mutationScale;

        // Helper functions
        float MutateFloat(float value, float min, float max, float amount)
        {
            float range = max - min;
            float delta = amount * range;
            return Mathf.Clamp(value + delta, min, max);
        }

        int MutateInt(int value, int min, int max, float amount)
        {
            float range = max - min;
            float raw = value + amount * range;

            int low = Mathf.FloorToInt(raw);
            float frac = raw - low;
            int rounded = (Random.value < frac) ? low + 1 : low;

            return Mathf.Clamp(rounded, min, max);
        }

        // -------------------------
        // X‑Axis: Burst vs Sustained
        // -------------------------
        g.baseDamage = MutateInt(g.baseDamage, WEAPON_DAMAGE_MIN, WEAPON_DAMAGE_MAX, stepX);
        g.burstSize = MutateInt(g.burstSize, WEAPON_BURSTSIZE_MIN, WEAPON_BURSTSIZE_MAX, stepX);
        g.fireRate = MutateFloat(g.fireRate, WEAPON_FIRERATE_MIN, WEAPON_FIRERATE_MAX, -stepX);
        g.cooldownTime = MutateFloat(g.cooldownTime, WEAPON_COOLDOWNTIME_MIN, WEAPON_COOLDOWNTIME_MAX, stepX);

        // -------------------------
        // Y‑Axis: Control vs CQ
        // -------------------------
        g.range = MutateFloat(g.range, WEAPON_RANGE_MIN, WEAPON_RANGE_MAX, stepY);
        g.accuracy = MutateFloat(g.accuracy, WEAPON_ACCURACY_MIN, WEAPON_ACCURACY_MAX, stepY);
        g.statusEffectStrength = MutateFloat(g.statusEffectStrength, WEAPON_STATUS_STRENGTH_MIN, WEAPON_STATUS_STRENGTH_MAX, stepY);

        g.projectileSpeed = MutateFloat(g.projectileSpeed, WEAPON_PROJECTILE_SPEED_MIN, WEAPON_PROJECTILE_SPEED_MAX, -stepY);
        g.fireRate = MutateFloat(g.fireRate, WEAPON_FIRERATE_MIN, WEAPON_FIRERATE_MAX, -stepY);
        g.spreadAngle = MutateFloat(g.spreadAngle, WEAPON_SPREAD_ANGLE_MIN, WEAPON_SPREAD_ANGLE_MAX, -stepY);

        // -------------------------
        // Z‑Axis: Efficiency vs Volatility
        // -------------------------
        g.powerCost = MutateInt(g.powerCost, WEAPON_POWERCOST_MIN, WEAPON_POWERCOST_MAX, -stepZ);
        g.heatPerShot = MutateInt(g.heatPerShot, WEAPON_HEATPERSHOT_MIN, WEAPON_HEATPERSHOT_MAX, -stepZ);
        g.heatDissipation = MutateFloat(g.heatDissipation, WEAPON_HEATDISSIPATION_MIN, WEAPON_HEATDISSIPATION_MAX, stepZ);
        g.chargeUpTime = MutateFloat(g.chargeUpTime, WEAPON_CHARGEUPTIME_MIN, WEAPON_CHARGEUPTIME_MAX, -stepZ);

        // Categorical mutation
        const float statusMutationChance = 0.02f;
        if (Random.value < statusMutationChance)
        {
            var values = (EffectType[])System.Enum.GetValues(typeof(EffectType));
            EffectType newType;
            do { newType = values[Random.Range(0, values.Length)]; }
            while (newType == g.statusEffectType);
            g.statusEffectType = newType;
        }

        return g;
    }

    private ShipGenome AxisAlignedMutation(ShipGenome genome, Vector3 dir)
    {
        ShipGenome g = genome.CloneForEvo();

        const float mutationScale = 0.05f;

        float stepX = dir.x * mutationScale;
        float stepY = dir.y * mutationScale;
        float stepZ = dir.z * mutationScale;

        float MutateFloat(float value, float min, float max, float amount)
        {
            float range = max - min;
            float delta = amount * range;
            return Mathf.Clamp(value + delta, min, max);
        }

        int MutateInt(int value, int min, int max, float amount)
        {
            float range = max - min;
            float raw = value + amount * range;

            int low = Mathf.FloorToInt(raw);
            float frac = raw - low;
            int rounded = (Random.value < frac) ? low + 1 : low;

            return Mathf.Clamp(rounded, min, max);
        }

        // -------------------------
        // X‑Axis: Durability vs Mobility
        // -------------------------
        g.hullHP = MutateInt(g.hullHP, SHIP_HULLHP_MIN, SHIP_HULLHP_MAX, stepX);
        g.armor = MutateInt(g.armor, SHIP_ARMOR_MIN, SHIP_ARMOR_MAX, stepX);
        g.shieldCapacity = MutateInt(g.shieldCapacity, SHIP_SHIELD_CAPACITY_MIN, SHIP_SHIELD_CAPACITY_MAX, stepX);
        g.shieldRegen = MutateInt(g.shieldRegen, SHIP_SHIELD_REGEN_MIN, SHIP_SHIELD_REGEN_MAX, stepX);

        g.speed = MutateFloat(g.speed, SHIP_SPEED_MIN, SHIP_SPEED_MAX, -stepX);
        g.turnRate = MutateFloat(g.turnRate, SHIP_TURNRATE_MIN, SHIP_TURNRATE_MAX, -stepX);
        g.evasion = MutateFloat(g.evasion, SHIP_EVASION_MIN, SHIP_EVASION_MAX, -stepX);

        // -------------------------
        // Y‑Axis: Power Economy vs Weapon Platform
        // -------------------------
        g.powerCapacity = MutateInt(g.powerCapacity, SHIP_POWER_CAPACITY_MIN, SHIP_POWER_CAPACITY_MAX, stepY);
        g.powerRegen = MutateInt(g.powerRegen, SHIP_POWER_REGEN_MIN, SHIP_POWER_REGEN_MAX, stepY);
        g.specialTileDensity = MutateFloat(g.specialTileDensity, SHIP_SPECIALTILE_DENSITY_MIN, SHIP_SPECIALTILE_DENSITY_MAX, stepY);

        g.gridWidth = MutateInt(g.gridWidth, SHIP_GRID_WIDTH_MIN, SHIP_GRID_WIDTH_MAX, -stepY);
        g.gridHeight = MutateInt(g.gridHeight, SHIP_GRID_HEIGHT_MIN, SHIP_GRID_HEIGHT_MAX, -stepY);
        g.droneCount = MutateInt(g.droneCount, SHIP_DRONE_COUNT_MIN, SHIP_DRONE_COUNT_MAX, -stepY);

        // -------------------------
        // Z‑Axis: Stability vs Aggression
        // -------------------------
        g.mass = MutateFloat(g.mass, SHIP_MASS_MIN, SHIP_MASS_MAX, stepZ);
        g.inertia = MutateFloat(g.inertia, SHIP_INERTIA_MIN, SHIP_INERTIA_MAX, stepZ);
        g.droneDurability = MutateInt(g.droneDurability, SHIP_DRONE_DURABILITY_MIN, SHIP_DRONE_DURABILITY_MAX, stepZ);

        g.droneSpeed = MutateFloat(g.droneSpeed, SHIP_DRONE_SPEED_MIN, SHIP_DRONE_SPEED_MAX, -stepZ);
        g.droneAggression = MutateFloat(g.droneAggression, SHIP_DRONE_AGGRESSION_MIN, SHIP_DRONE_AGGRESSION_MAX, -stepZ);

        return g;
    }

    private WeaponGenome GetElite(List<WeaponGenome> genomes)
    {
        WeaponGenome best = genomes[0];

        for (int i = 1; i < genomes.Count; i++)
        {
            if (genomes[i].finalFitness > best.finalFitness)
                best = genomes[i];
        }

        return best.CloneExact();
    }

    private ShipGenome GetElite(List<ShipGenome> genomes)
    {
        ShipGenome best = genomes[0];

        for (int i = 1; i < genomes.Count; i++)
        {
            if (genomes[i].finalFitness > best.finalFitness)
                best = genomes[i];
        }

        return best.CloneExact();
    }
}
