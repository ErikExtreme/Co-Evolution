using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    public List<Weapon> weapons = new List<Weapon>();

    [SerializeField] Inventory inventory;

    void Awake()
    {
        Instance = this;
    }

    public Weapon GetWeapon(int id)
    {
        if (weapons.Count == 0)
            return null;

        foreach (Weapon weapon in weapons)
        {
            if (weapon.weapon_Genome.id == id) return weapon;
        }

        Debug.LogError("Could not find weapon with id: " + id);
        return null;
    }
    public void AddWeapon(WeaponGenome weaponGenome, WeaponStatsTracker weaponStatsTracker)
    {
        PlayerWeapon weapon = new PlayerWeapon();
        weapon.weapon_Genome = weaponGenome;
        weapon.tracker = weaponStatsTracker;
        weapons.Add(weapon);

        inventory.AddWeapon(weaponGenome, weaponStatsTracker);
    }
}
