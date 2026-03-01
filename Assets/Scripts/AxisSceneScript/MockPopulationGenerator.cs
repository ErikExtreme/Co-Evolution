using System.Collections.Generic;

public static class MockPopulationGenerator
{
    public static List<WeaponGenome> GenerateMockWeapons(int count)
    {
        var list = new List<WeaponGenome>(count);
        for (int i = 0; i < count; i++)
        {
            var g = GlobalSettings.RandomWeaponGenome();
            g.mapping = Mapping.MapGenome(g);
            list.Add(g);
        }
        return list;
    }

    public static List<ShipGenome> GenerateMockShips(int count)
    {
        var list = new List<ShipGenome>(count);
        for (int i = 0; i < count; i++)
        {
            var g = GlobalSettings.RandomShipGenome();
            g.mapping = Mapping.MapGenome(g);
            list.Add(g);
        }
        return list;
    }
}
