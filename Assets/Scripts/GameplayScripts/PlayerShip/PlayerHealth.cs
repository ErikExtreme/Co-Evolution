using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerHealth : ShipHealth
{
    [SerializeField] Canvas gameoverCanvas;

    List<(ShipGenome genome, ModuleStatsTracker tracker)> genomeTrackers;

    protected override void OnStart()
    {
        base.OnStart();

        genomeTrackers = new List<(ShipGenome, ModuleStatsTracker)>();
    }

    public void SetStats(ShipCoreStats shipCoreStats, (ShipGenome genome, ModuleStatsTracker tracker)[] genomeTrackers)
    {
        //Stats
        HullHP = shipCoreStats.hullHP;
        armor = shipCoreStats.armor / 20;
        ShieldCapacity = shipCoreStats.shieldCapacity / 3;
        shieldRegen = shipCoreStats.shieldRegen / 5;
        PowerCapacity = shipCoreStats.powerCapacity / 5;
        powerRegen = shipCoreStats.powerRegen;

        evasion = shipCoreStats.evasion / 5;

        //Current
        Health = HullHP;
        Shield = ShieldCapacity;
        Power = PowerCapacity;

        //Tracker
        this.genomeTrackers = genomeTrackers.ToList();
    }
    public override bool TakeDamage(float damage)
    {
        foreach (var trackerPair in genomeTrackers)
        {
            if (trackerPair.tracker != null && trackerPair.genome != null)
                trackerPair.tracker.RegisterDamageTaken(trackerPair.genome.evasion / evasion * damage);
        }
        //Evasion
        if (Random.value < evasion)
            return false;



        //Shield damage
        if (Shield >= damage)
        {
            Shield -= damage;

            foreach (var trackerPair in genomeTrackers)
            {
                if (trackerPair.tracker != null && trackerPair.genome != null)
                    trackerPair.tracker.RegisterDamageTaken(trackerPair.genome.shieldCapacity / ShieldCapacity * damage);
            }

            return false;
        }
        else
        {
            damage -= Shield;

            foreach (var trackerPair in genomeTrackers)
            {
                if (trackerPair.tracker != null && trackerPair.genome != null)
                    trackerPair.tracker.RegisterDamageTaken(trackerPair.genome.shieldCapacity / ShieldCapacity * Shield);
            }

            Shield = 0;
        }


        //Armor
        int damageReduction = Mathf.RoundToInt(Mathf.Max(armor * armorReduction, 0));

        foreach (var trackerPair in genomeTrackers)
        {
            if (trackerPair.tracker != null && trackerPair.genome != null && armor != 0)
                trackerPair.tracker.RegisterDamageTaken(trackerPair.genome.armor / armor * damageReduction);
        }

        damage -= damageReduction;


        //Hull Damage
        foreach (var trackerPair in genomeTrackers)
        {
            if (trackerPair.tracker != null && trackerPair.genome != null)
                trackerPair.tracker.RegisterDamageTaken(trackerPair.genome.hullHP / HullHP * damage);
        }
        Health -= Mathf.Max(damage, 1);
        if (Health <= 0)
        {
            OutOfHealth();
            return true;
        }
        return false;
    }
    protected override void OutOfHealth()
    {
        gameoverCanvas.gameObject.SetActive(true);
    }
}
