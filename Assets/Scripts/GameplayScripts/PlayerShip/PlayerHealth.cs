using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : ShipHealth
{
    [SerializeField] Canvas gameoverCanvas;

    List<(ModuleStatsTracker tracker, ShipCoreStats stats)> genomeTrackers;

    protected override void OnStart()
    {
        base.OnStart();

        genomeTrackers = new List<(ModuleStatsTracker, ShipCoreStats)>();
    }

    public void SetStats(ShipCoreStats shipCoreStats)
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
    }
    void Update()
    {
        if (Shield < ShieldCapacity)
            Shield += shieldRegen * Time.deltaTime;

        if (Power < PowerCapacity)
        {
            Power += powerRegen * Time.deltaTime;

            foreach (var trackerPair in genomeTrackers)
            {
                trackerPair.tracker.RegisterPowerGenerated(trackerPair.stats.powerRegen * Time.deltaTime);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        foreach (var trackerPair in genomeTrackers)
        {
            trackerPair.tracker.RegisterDamageTaken(trackerPair.stats.evasion / evasion * damage);
        }
        //Evasion
        if (Random.value < evasion)
            return;


       
        //Shield damage
        if (Shield >= damage)
        {
            Shield -= damage;

            foreach (var trackerPair in genomeTrackers)
            {
                trackerPair.tracker.RegisterDamageTaken(trackerPair.stats.shieldCapacity / ShieldCapacity * damage);
            }

            return;
        }
        else
        {
            damage -= Shield; 
            
            foreach (var trackerPair in genomeTrackers)
            {
                trackerPair.tracker.RegisterDamageTaken(trackerPair.stats.shieldCapacity / ShieldCapacity * Shield);
            }

            Shield = 0;
        }


        //Armor
        int damageReduction = Mathf.RoundToInt(Mathf.Max(armor * armorReduction, 0));

        foreach (var trackerPair in genomeTrackers)
        {
            trackerPair.tracker.RegisterDamageTaken(trackerPair.stats.armor / armor * damageReduction);
        }

        damage -= damageReduction;


        //Hull Damage
        foreach (var trackerPair in genomeTrackers)
        {
            trackerPair.tracker.RegisterDamageTaken(trackerPair.stats.hullHP / HullHP * damage);
        }
        Health -= Mathf.Max(damage, 1);
        if (Health <= 0)
            OutOfHealth();
    }
    protected override void OutOfHealth()
    {
        gameoverCanvas.gameObject.SetActive(true);
    }
}
