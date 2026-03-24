using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    public List<(WeaponGenome genome, WeaponStatsTracker tracker)> weapons = new List<(WeaponGenome genome, WeaponStatsTracker tracker)>();

    [SerializeField] Inventory inventory;

    void Awake()
    {
        Instance = this;
    }

    public (WeaponGenome genome, WeaponStatsTracker tracker) GetWeapon(int id)
    {
        if (weapons.Count == 0)
            return (null, null);

        foreach (var weapon in weapons)
        {
            if (weapon.genome.id == id)
            {
                return weapon;
            }
        }

        Debug.LogError("Could not find weapon with id: " + id);
        return (null, null);
    }
    public void AddWeapon(WeaponGenome weaponGenome, WeaponStatsTracker weaponStatsTracker)
    {
        weapons.Add((weaponGenome, weaponStatsTracker));

        inventory.AddWeapon(weaponGenome, weaponStatsTracker);
    }
}
