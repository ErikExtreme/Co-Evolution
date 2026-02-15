using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipBlueprint : MonoBehaviour
{
    WeaponGenome[,] weaponsArray;
    ShipGenome[] modulesArray;

    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform gridLayoutGroup;

    [SerializeField] private int maxWeaponGridSize = 20;
    public int MaxWeaponGridSize => maxWeaponGridSize;
    public int CurrentGridWidth { get; private set; }
    public int CurrentGridHeight { get; private set; }

    private void Start()
    {
        weaponsArray = new WeaponGenome[maxWeaponGridSize, maxWeaponGridSize];
        modulesArray = new ShipGenome[5];//Hard coded fix
    }

    public void SetWeapon(WeaponGenome newModule, int horiPos, int vertPos)
    {
        if (weaponsArray[horiPos, vertPos] == null)
        {
            weaponsArray[horiPos, vertPos] = newModule;
        }
    }
    public void SetModule(ShipGenome newModule, int pos)
    {
        if (modulesArray[pos] != null)
            return;

        modulesArray[pos] = newModule;


        CurrentGridWidth += newModule.gridWidth;
        CurrentGridWidth = Math.Clamp(CurrentGridWidth, 0, maxWeaponGridSize);
        CurrentGridHeight += newModule.gridHeight;
        CurrentGridHeight = Math.Clamp(CurrentGridHeight, 0, maxWeaponGridSize);

        for (int i = 0; i < CurrentGridWidth * CurrentGridHeight; i++)
        {
            gridLayoutGroup.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = CurrentGridWidth * CurrentGridHeight; i < MaxWeaponGridSize * MaxWeaponGridSize; i++)
        {
            gridLayoutGroup.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ConstructShip()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }


        ShipHealth shipHealthScript = gameObject.GetComponent<ShipHealth>();

        for (int vert = 0; vert < CurrentGridHeight; vert++)
            for (int hori = 0; hori < CurrentGridWidth; hori++)
            {
                if (weaponsArray[hori, vert] == null)
                    continue;

                GameObject cellInstance = Instantiate(weaponPrefab, transform, false);
                Weapon weaponScript = cellInstance.GetComponent<Weapon>();
                weaponScript.weapon_Genome = weaponsArray[hori, vert];
                weaponScript.shipHealthScript = shipHealthScript;//Separate out energy from health script?

                Vector2 originOffset = new Vector2(CurrentGridWidth - 1, CurrentGridHeight - 1) / 2f;
                Vector2 position = (new Vector2(hori, CurrentGridHeight - 1 - vert) - originOffset);
                cellInstance.transform.localPosition = position;
            }



        ShipCoreStats shipCoreStats = new ShipCoreStats();
        ShipMobilityStats shipMobilityStats = new ShipMobilityStats();
        ShipDroneStats shipDroneStats = new ShipDroneStats();

        foreach (ShipGenome module in modulesArray)
        {
            if (module == null)
                continue;

            shipCoreStats.Add(module.hullHP, module.armor, module.shieldCapacity, module.shieldRegen, module.powerCapacity, module.powerRegen);
            shipMobilityStats.Add(module.speed, module.turnRate, module.evasion, module.mass, module.inertia);
            shipDroneStats.Add(module.droneCount, module.droneSpeed, module.droneDurability, module.droneAggression);
        }
        shipHealthScript.SetStats(shipCoreStats);
        gameObject.GetComponent<PlayerShip>().SetStats(shipMobilityStats);
        gameObject.GetComponent<ShipDroneManager>().SetStats(shipDroneStats);


        canvas.gameObject.SetActive(false);
    }
}
public class ShipCoreStats
{
    public int hullHP;
    public int armor;
    public int shieldCapacity;
    public int shieldRegen;
    public int powerCapacity;
    public int powerRegen;

    public void Add(int hullHP, int armor, int shieldCapacity, int shieldRegen, int powerCapacity, int powerRegen)
    {
        this.hullHP += hullHP;
        this.armor += armor;
        this.shieldCapacity += shieldCapacity;
        this.shieldRegen += shieldRegen;
        this.powerCapacity += powerCapacity;
        this.powerRegen += powerRegen;
    }
}
public class ShipMobilityStats
{
    public float speed;
    public float turnRate;
    public float evasion;
    public float mass;
    public float inertia;

    public void Add(float speed, float turnRate, float evasion, float mass, float inertia)
    {
        this.speed += speed;
        this.turnRate += turnRate;
        this.evasion += evasion;
        this.mass += mass;
        this.inertia += inertia;
    }
}
//public class ShipLayoutStats
//{
//    public int gridWidth;
//    public int gridHeight;
//    public float specialTileDensity;
//}
public class ShipDroneStats
{
    public int droneCount;
    public float droneSpeed;
    public int droneDurability;
    public float droneAggression;

    public void Add(int droneCount, float droneSpeed, int droneDurability, float droneAggression)
    {
        this.droneCount += droneCount;
        this.droneSpeed += droneSpeed;
        this.droneDurability += droneDurability;
        this.droneAggression += droneAggression;
    }
}
