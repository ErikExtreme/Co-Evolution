using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
                var parent = SelectParent(nextGenW);
                var child = Mutate(parent);
                child.fitness = Fitness.CooperativeFitness(child, shipPop, playerTracker);
                nextGenW.Add(child);
            }
            while (nextGenS.Count < burstPopSize)
            {
                var parent = SelectParent(nextGenS);
                var child = Mutate(parent);
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

    private WeaponGenome Mutate(WeaponGenome genome)
    {
        return genome.Clone();
    }

    private ShipGenome Mutate(ShipGenome genome)
    {
        return genome.Clone();
    }

    private WeaponGenome GetElite(List<WeaponGenome> genomes)
    {
        return genomes[0];
    }

    private ShipGenome GetElite(List<ShipGenome> genomes)
    {
        return genomes[0];
    }
}
