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

        try
        {
            if (weapons[id] != null)
                return weapons[id];

            Debug.LogError("Could not find weapon on index: " + id);
            return null;
        }
        catch (System.Exception)
        {
            Debug.LogError("Weapon on index: " + id + " does not exist");
            return null;
        }
    }
}
