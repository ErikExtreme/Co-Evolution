using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class EvolutionLog
{
    public string sessionId;
    public string timestamp;
    public PlayerBehaviorSnapshot playerBehavior;
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
    public long id;
    public long parentId;

    public float fitness;
    public float statScore;
    public float trackerScore;
    public float alignmentScore;

    public Vector3 axis;
    public Vector3 mutationDirection;

    public float timeEquipped;
    public float damageDealt;
    public int kills;
    public float avgEffectiveRange;

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
    public float statusEffectStrength;
    public float aoeRadius;
}

[System.Serializable]
public class ShipGenomeLog
{
    public long id;
    public long parentId;

    public float fitness;
    public float statScore;
    public float trackerScore;
    public float alignmentScore;

    public Vector3 axis;
    public Vector3 mutationDirection;

    public float timeEquipped;
    public float damageAvoided;
    public float powerSaved;
    public float heatReduced;

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
}

public static class EvolutionLogger
{
    public static EvolutionLog CreateNewSession(PlayerBehaviorTracker p)
    {
        EvolutionLog log = new EvolutionLog();
        log.sessionId = Guid.NewGuid().ToString();
        log.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        log.playerBehavior = SnapshotPlayerBehavior(p);
        return log;
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

    public static void RecordGeneration(
        EvolutionLog log,
        int generationIndex,
        List<WeaponGenome> weapons,
        List<Vector3> mutationDirsW,
        List<ShipGenome> ships,
        List<Vector3> mutationDirsS)
    {
        GenerationLog gen = new GenerationLog();
        gen.generationIndex = generationIndex;

        for (int i = 0; i < weapons.Count; i++)
            gen.weaponGenomes.Add(CreateWeaponLogEntry(weapons[i], mutationDirsW[i]));

        for (int i = 0; i < ships.Count; i++)
            gen.shipGenomes.Add(CreateShipLogEntry(ships[i], mutationDirsS[i]));

        log.generations.Add(gen);
    }

    private static WeaponGenomeLog CreateWeaponLogEntry(WeaponGenome g, Vector3 dir)
    {
        WeaponStatsTracker t = null; 
        Weapon weapon = WeaponManager.Instance.GetWeapon(g.id); 
        if (weapon != null) 
            t = weapon.tracker;

        return new WeaponGenomeLog
        {
            id = g.id,
            parentId = g.parentId,

            fitness = g.fitness,
            statScore = g.statScore,
            trackerScore = g.trackerScore,
            alignmentScore = g.alignmentScore,

            axis = Mapping.MapGenome(g),
            mutationDirection = dir,

            timeEquipped = t.timeEquipped,
            damageDealt = t.damageDealt,
            kills = t.kills,
            avgEffectiveRange = t.avgEffectiveRange,

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
            statusEffectStrength = g.statusEffectStrength,
            aoeRadius = g.aoeRadius
        };
    }

    private static ShipGenomeLog CreateShipLogEntry(ShipGenome g, Vector3 dir)
    {
        ModuleStatsTracker t = null; 
        Module module = ModuleManager.Instance.GetModule(g.id); 
        if (module != null) 
            t = module.tracker;

        return new ShipGenomeLog
        {
            id = g.id,
            parentId = g.parentId,

            fitness = g.fitness,
            statScore = g.statScore,
            trackerScore = g.trackerScore,
            alignmentScore = g.alignmentScore,

            axis = Mapping.MapGenome(g),
            mutationDirection = dir,

            timeEquipped = t.timeEquipped,
            damageAvoided = t.damageAvoided,
            powerSaved = t.powerSaved,
            heatReduced = t.heatReduced,

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
            droneAggression = g.droneAggression
        };
    }

    public static void SaveLog(EvolutionLog log, string filePath)
    {
        string json = JsonUtility.ToJson(log, true);
        File.WriteAllText(filePath, json);
    }

    public static EvolutionLog LoadLog(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<EvolutionLog>(json);
    }
}
