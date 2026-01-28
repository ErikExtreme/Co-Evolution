using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public (WeaponGenome[] weaponGenomes, ShipGenome[] shipGenomes) Evolve(int populationSize, int totalGenerations)
    {
        WeaponGenome[] weapons = Seeding.RandomWeaponSeed(populationSize);
        ShipGenome[] ships = Seeding.RandomShipSeed(populationSize);



        return (weapons, ships);
    }
}
