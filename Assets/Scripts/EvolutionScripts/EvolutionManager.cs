using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

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

        WeaponGenome[] selectedWeapons = SelectionElitist<WeaponGenome>(weapons, 50);//Hard coded 50
        WeaponGenome[] offspringWeapons = CrossoverSinglePoint<WeaponGenome>(selectedWeapons);

        ShipGenome[] selectedModules = SelectionElitist<ShipGenome>(shipModules, 50);//Hard coded 50
        ShipGenome[] offspringModules = CrossoverSinglePoint<ShipGenome>(selectedModules);
    }

    private T[] SelectionElitist<T>(List<T> initialPopulation, int amountToSelect) where T : IGenome
    {
        List<T> sortedPopulation = initialPopulation.OrderByDescending(x => x.fitness).ToList();

        T[] selectedIndividuals = sortedPopulation.Take(amountToSelect).ToArray();

        //selectedIndividuals = selectedIndividuals.OrderBy(x => Random.value).ToArray();//Shuffles, could be replaced with better shuffeling function

        return selectedIndividuals;
    }

    private T[] CrossoverSinglePoint<T>(T[] selectedPopulation) where T : IGenome, new()
    {
        var variablesInGenome = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        T[] newGenomes = new T[selectedPopulation.Length];

        for (int i = 0; i < selectedPopulation.Length; i++)
        {
            newGenomes[i] = (T)selectedPopulation[i].Clone();

            int crossoverPoint = Random.Range(0, variablesInGenome.Length);//if 0 or length = full copy, not inherently bad but could potentialy cause problems

            int secondIndex;
            do
            {
                secondIndex = Random.Range(0, selectedPopulation.Length);
            } while (secondIndex == i);
            T secondParent = selectedPopulation[secondIndex];


            for (int j = crossoverPoint; j < variablesInGenome.Length; j++)
                variablesInGenome[j].SetValue(newGenomes[i], variablesInGenome[j].GetValue(secondParent));
        }
        return newGenomes;
    }

}
