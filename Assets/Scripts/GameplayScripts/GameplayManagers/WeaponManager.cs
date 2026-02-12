using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    public List<Weapon> weapons = new List<Weapon>();

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
}
