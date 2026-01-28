using UnityEngine;

public static class Seeding
{
    public static WeaponGenome[] RandomWeaponSeed(int populationSize)
    {
        WeaponGenome[] weapons = new WeaponGenome[populationSize];

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i] = GlobalSettings.RandomWeaponGenome();
        }

        return weapons;
    }

    public static ShipGenome[] RandomShipSeed(int populationSize)
    {
        ShipGenome[] shipGenomes = new ShipGenome[populationSize];

        for(int i = 0; i < shipGenomes.Length; i++)
        {
            shipGenomes[i] = GlobalSettings.RandomShipGenome();
        }

        return shipGenomes;
    }
}
