using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    public int initalPopulationSize;

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

        // 2. Calculate Fitness for all Weapons and Ship Modules
        foreach (var w in weapons)
        {
            Weapon weapon = WeaponManager.Instance.GetWeapon(w.id);
            if (weapon == null)
                return;
            w.fitness = Fitness.CalculateFitness(w, weapon.tracker, playerTracker);
        }
        foreach (var s in shipModules)
        {
            Module module = ModuleManager.Instance.GetModule(s.id);
            if (module == null)
                return;
            s.fitness = Fitness.CalculateFitness(s, module.tracker, playerTracker);
        }
    }
}
