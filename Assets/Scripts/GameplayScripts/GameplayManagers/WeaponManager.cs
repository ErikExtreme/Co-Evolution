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
}
