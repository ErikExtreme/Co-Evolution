using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SessionLog
{
    public string sessionId;
    public string timestamp;

    // One entry per Evolve() call
    public List<EvolveRecording> recordings = new List<EvolveRecording>();
}

[System.Serializable]
public class EvolveRecording
{
    public int evolveIndex;

    public PlayerBehaviorSnapshot playerBehavior;

    // Global population BEFORE evolution
    public List<WeaponGenomeLog> globalWeapons = new List<WeaponGenomeLog>();
    public List<ShipGenomeLog> globalShips = new List<ShipGenomeLog>();

    // All generations of the burst evolution
    public List<GenerationLog> generations = new List<GenerationLog>();
}

[System.Serializable]
public class GenerationLog
{
    public int generationIndex;
    public List<WeaponGenomeLog> weaponGenomes = new List<WeaponGenomeLog>();
    public List<ShipGenomeLog> shipGenomes = new List<ShipGenomeLog>();
}

[System.Serializable]
public class PlayerBehaviorSnapshot
{
    public float avgSpeed;
    public float avgTurnRate;
    public float movementEntropy;
    public float avgEngagementDistance;
    public float avgAngleToEnemy;

    public float normSpeed;
    public float normTurnRate;
    public float normEntropy;
    public float normEngagementDistance;
    public float normAngleToEnemy;
}

[System.Serializable]
public class WeaponGenomeLog
{
    public int id;
    public int parentId;

    public float fitness;
    public float novelty;
    public float finalFitness;
    public float statScore;
    public float trackerScore;
    public float alignmentScore;

    public Vector3 axis;
    public Vector3 mutationDirection;

    public int baseDamage;
    public int burstSize;
    public float fireRate;
    public float cooldownTime;
    public float projectileSpeed;
    public float accuracy;
    public float spreadAngle;
    public float range;
    public int powerCost;
    public int heatPerShot;
    public float heatDissipation;
    public float chargeUpTime;
    public EffectType statusEffectType;
    public float statusEffectStrength;
    public float aoeRadius;

    // New universal tracker metrics
    public float damageEfficiency;
    public float heatManagementEfficiency;
    public float hitRatio;
    public float effectivenessPerCycle;
    public float targetingTimeEfficiency;
    public float targetUtility;
    public float survivalContribution;
    public float roleFulfillment;
    public float battleDuration;
    public float survivalSuccess;

    // Legacy metrics (kept for backward compatibility)
    public float timeEquipped;
    public float damageDealt;
    public int kills;
    public float avgEffectiveRange;
}

[System.Serializable]
public class ShipGenomeLog
{
    public int id;
    public int parentId;

    public float fitness;
    public float novelty;
    public float finalFitness;
    public float statScore;
    public float trackerScore;
    public float alignmentScore;

    public Vector3 axis;
    public Vector3 mutationDirection;

    public int hullHP;
    public int armor;
    public int shieldCapacity;
    public int shieldRegen;
    public int powerCapacity;
    public int powerRegen;
    public float speed;
    public float turnRate;
    public float evasion;
    public float mass;
    public float inertia;
    public int gridWidth;
    public int gridHeight;
    public float specialTileDensity;
    public int droneCount;
    public float droneSpeed;
    public int droneDurability;
    public float droneAggression;

    // New universal tracker metrics
    public float damageEfficiency;
    public float survivalContribution;
    public float offensiveSynergy;
    public float powerEfficiency;
    public float roleFulfillment;
    public float battleDuration;
    public float survivalSuccess;

    // Legacy metrics (kept for backward compatibility)
    public float timeEquipped;
    public float damageAvoided;
    public float powerSaved;
    public float heatReduced;
}

public static class EvolutionLogger
{
    // Create a new session log
    public static SessionLog CreateNewSession()
    {
        return new SessionLog
        {
            sessionId = Guid.NewGuid().ToString(),
            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public static PlayerBehaviorSnapshot SnapshotPlayerBehavior(PlayerBehaviorTracker p)
    {
        return new PlayerBehaviorSnapshot
        {
            avgSpeed = p.avgSpeed,
            avgTurnRate = p.avgTurnRate,
            movementEntropy = p.movementEntropy,
            avgEngagementDistance = p.avgEngagementDistance,
            avgAngleToEnemy = p.avgAngleToEnemy,

            normSpeed = p.normSpeed,
            normTurnRate = p.normTurnRate,
            normEntropy = p.normEntropy,
            normEngagementDistance = p.normEngagementDistance,
            normAngleToEnemy = p.normAngleToEnemy
        };
    }

    // Create a new recording for one Evolve() call
    public static EvolveRecording BeginRecording(SessionLog session, int evolveIndex, PlayerBehaviorTracker p)
    {
        var rec = new EvolveRecording
        {
            evolveIndex = evolveIndex,
            playerBehavior = SnapshotPlayerBehavior(p)
        };

        session.recordings.Add(rec);
        return rec;
    }

    // Record the global population BEFORE evolution
    public static void RecordGlobalPopulation(
        EvolveRecording rec,
        List<WeaponGenome> weapons,
        List<ShipGenome> ships)
    {
        foreach (var w in weapons)
            rec.globalWeapons.Add(CreateWeaponLogEntry(w, Vector3.zero, includeTracker: true));

        foreach (var s in ships)
            rec.globalShips.Add(CreateShipLogEntry(s, Vector3.zero, includeTracker: true));
    }

    // Record one generation of the burst evolution
    public static void RecordGeneration(
        EvolveRecording rec,
        int generationIndex,
        List<WeaponGenome> weapons,
        List<Vector3> mutationDirsW,
        List<ShipGenome> ships,
        List<Vector3> mutationDirsS)
    {
        GenerationLog gen = new GenerationLog();
        gen.generationIndex = generationIndex;

        for (int i = 0; i < weapons.Count; i++)
            gen.weaponGenomes.Add(CreateWeaponLogEntry(weapons[i], mutationDirsW[i], includeTracker: false));

        for (int i = 0; i < ships.Count; i++)
            gen.shipGenomes.Add(CreateShipLogEntry(ships[i], mutationDirsS[i], includeTracker: false));

        rec.generations.Add(gen);
    }

    private static WeaponGenomeLog CreateWeaponLogEntry(WeaponGenome g, Vector3 dir, bool includeTracker)
    {
        WeaponStatsTracker t = null;
        if (includeTracker)
        {
            var weapon = WeaponManager.Instance.GetWeapon(g.id);
            t = weapon.tracker;
        }

        return new WeaponGenomeLog
        {
            id = g.id,
            parentId = g.parentId,

            fitness = g.fitness,
            novelty = g.novelty,
            finalFitness = g.finalFitness,
            statScore = g.statScore,
            trackerScore = g.trackerScore,
            alignmentScore = g.alignmentScore,

            axis = g.mapping != Vector3.zero ? g.mapping : Mapping.MapGenome(g),
            mutationDirection = dir,

            baseDamage = g.baseDamage,
            burstSize = g.burstSize,
            fireRate = g.fireRate,
            cooldownTime = g.cooldownTime,
            projectileSpeed = g.projectileSpeed,
            accuracy = g.accuracy,
            spreadAngle = g.spreadAngle,
            range = g.range,
            powerCost = g.powerCost,
            heatPerShot = g.heatPerShot,
            heatDissipation = g.heatDissipation,
            chargeUpTime = g.chargeUpTime,
            statusEffectType = g.statusEffectType,
            statusEffectStrength = g.statusEffectStrength,
            aoeRadius = g.aoeRadius,

            // New universal metrics
            damageEfficiency = includeTracker ? t?.damageEfficiency ?? 0f : 0f,
            heatManagementEfficiency = includeTracker ? t?.heatManagementEfficiency ?? 0f : 0f,
            hitRatio = includeTracker ? t?.hitRatio ?? 0f : 0f,
            effectivenessPerCycle = includeTracker ? t?.effectivenessPerCycle ?? 0f : 0f,
            targetingTimeEfficiency = includeTracker ? t?.targetingTimeEfficiency ?? 0f : 0f,
            targetUtility = includeTracker ? t?.targetUtility ?? 0f : 0f,
            survivalContribution = includeTracker ? t?.survivalContribution ?? 0f : 0f,
            roleFulfillment = includeTracker ? t?.roleFulfillment ?? 0f : 0f,
            battleDuration = includeTracker ? t?.battleDuration ?? 0f : 0f,
            survivalSuccess = includeTracker ? t?.survivalSuccess ?? 0f : 0f,

            // Legacy metrics
            timeEquipped = includeTracker ? t?.timeEquipped ?? 0f : 0f,
            damageDealt = includeTracker ? t?.damageDealt ?? 0f : 0f,
            kills = includeTracker ? t?.kills ?? 0 : 0,
            avgEffectiveRange = includeTracker ? t?.avgEffectiveRange ?? 0f : 0f
        };
    }

    private static ShipGenomeLog CreateShipLogEntry(ShipGenome g, Vector3 dir, bool includeTracker)
    {
        ModuleStatsTracker t = null;
        if (includeTracker)
        {
            var module = ModuleManager.Instance.GetModule(g.id);
            t = module.tracker;
        }

        return new ShipGenomeLog
        {
            id = g.id,
            parentId = g.parentId,

            fitness = g.fitness,
            novelty = g.novelty,
            finalFitness = g.finalFitness,
            statScore = g.statScore,
            trackerScore = g.trackerScore,
            alignmentScore = g.alignmentScore,

            axis = g.mapping != Vector3.zero ? g.mapping : Mapping.MapGenome(g),
            mutationDirection = dir,

            hullHP = g.hullHP,
            armor = g.armor,
            shieldCapacity = g.shieldCapacity,
            shieldRegen = g.shieldRegen,
            powerCapacity = g.powerCapacity,
            powerRegen = g.powerRegen,
            speed = g.speed,
            turnRate = g.turnRate,
            evasion = g.evasion,
            mass = g.mass,
            inertia = g.inertia,
            gridWidth = g.gridWidth,
            gridHeight = g.gridHeight,
            specialTileDensity = g.specialTileDensity,
            droneCount = g.droneCount,
            droneSpeed = g.droneSpeed,
            droneDurability = g.droneDurability,
            droneAggression = g.droneAggression,

            // New universal metrics
            damageEfficiency = includeTracker ? t?.damageEfficiency ?? 0f : 0f,
            survivalContribution = includeTracker ? t?.survivalContribution ?? 0f : 0f,
            offensiveSynergy = includeTracker ? t?.offensiveSynergy ?? 0f : 0f,
            powerEfficiency = includeTracker ? t?.powerEfficiency ?? 0f : 0f,
            roleFulfillment = includeTracker ? t?.roleFulfillment ?? 0f : 0f,
            battleDuration = includeTracker ? t?.battleDuration ?? 0f : 0f,
            survivalSuccess = includeTracker ? t?.survivalSuccess ?? 0f : 0f,

            // Legacy metrics
            timeEquipped = includeTracker ? t?.timeEquipped ?? 0f : 0f,
            damageAvoided = includeTracker ? t?.damageAvoided ?? 0f : 0f,
            powerSaved = includeTracker ? t?.powerSaved ?? 0f : 0f,
            heatReduced = includeTracker ? t?.heatReduced ?? 0f : 0f
        };
    }

    public static void SaveLog(SessionLog log, string filePath)
    {
        string json = JsonUtility.ToJson(log, true);
        File.WriteAllText(filePath, json);
    }

    public static SessionLog LoadLog(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<SessionLog>(json);
    }
}
