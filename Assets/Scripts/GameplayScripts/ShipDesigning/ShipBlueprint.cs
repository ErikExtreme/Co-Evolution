using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ShipBlueprint : MonoBehaviour
{
    (WeaponGenome genome, WeaponStatsTracker tracker)[,] weaponsArray;
    (ShipGenome genome, ModuleStatsTracker tracker)[] modulesArray;

    [SerializeField] WaveManager waveManager;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Canvas constructionCanvas;
    [SerializeField] RectTransform gridLayoutGroup;

    [SerializeField] private int moduleListSize = 5;
    [SerializeField] private int maxWeaponGridSize = 20;
    private int currentGridWidth;
    private int currentGridHeight;
    public int ModuleListSize => moduleListSize;
    public int MaxWeaponGridSize => maxWeaponGridSize;
    public int CurrentGridWidth => Math.Clamp(currentGridWidth, 0, maxWeaponGridSize);
    public int CurrentGridHeight => Math.Clamp(currentGridHeight, 0, maxWeaponGridSize);

    private void Start()
    {
        weaponsArray = new (WeaponGenome genome, WeaponStatsTracker tracker)[maxWeaponGridSize, maxWeaponGridSize];
        modulesArray = new (ShipGenome genome, ModuleStatsTracker tracker)[moduleListSize];
    }

    public void SetWeapon(WeaponGenome genome, WeaponStatsTracker tracker, int horiPos, int vertPos)
    {
        if (weaponsArray[horiPos, vertPos].genome == null)
        {
            weaponsArray[horiPos, vertPos] = (genome, tracker);
        }
    }
    public void SetModule(ShipGenome genome, ModuleStatsTracker tracker, int pos)
    {
        if (modulesArray[pos].genome != null)
            return;

        modulesArray[pos] = (genome, tracker);


        currentGridWidth += genome.gridWidth;
        currentGridHeight += genome.gridHeight;

        for (int i = 0; i < MaxWeaponGridSize; i++)
            for (int j = 0; j < MaxWeaponGridSize; j++)
            {
                bool shouldBeActive = i < CurrentGridWidth && j < CurrentGridHeight;
                int index = i + j * MaxWeaponGridSize;
                gridLayoutGroup.GetChild(index).gameObject.SetActive(shouldBeActive);
            }
        gridLayoutGroup.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurrentGridWidth * 75 + 26);//Hard coded, 75 = the width of a cell, 26 = random padding the layoutgroup has
        gridLayoutGroup.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentGridHeight * 75 + 26);//Hard coded, 75 = the height of a cell, 26 = random padding the layoutgroup has
    }
    public void RemoveWeapon(int horiPos, int vertPos)
    {
        weaponsArray[horiPos, vertPos].genome = null;
        weaponsArray[horiPos, vertPos].tracker = null;
    }
    public void RemoveModule(int pos)
    {
        currentGridWidth -= modulesArray[pos].genome.gridWidth;
        currentGridHeight -= modulesArray[pos].genome.gridHeight;

        modulesArray[pos].genome = null;
        modulesArray[pos].tracker = null;

        for (int i = 0; i < MaxWeaponGridSize; i++)
            for (int j = 0; j < MaxWeaponGridSize; j++)
            {
                bool shouldBeActive = i < CurrentGridWidth && j < CurrentGridHeight;
                int index = i + j * MaxWeaponGridSize;
                gridLayoutGroup.GetChild(index).gameObject.SetActive(shouldBeActive);
            }
        gridLayoutGroup.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurrentGridWidth * 75 + 26);//Hard coded, 75 = the width of a cell, 26 = random padding the layoutgroup has
    }

    public void ConstructShip()
    {
        for (int i = 1; i < transform.childCount; i++)//skip first child
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        transform.GetChild(0).transform.localScale = new Vector2(currentGridWidth * 1.06f, currentGridHeight * 1.06f);

        ShipHealth shipHealthScript = gameObject.GetComponent<ShipHealth>();

        for (int vert = 0; vert < CurrentGridHeight; vert++)
            for (int hori = 0; hori < CurrentGridWidth; hori++)
            {
                if (weaponsArray[hori, vert].genome == null)
                    continue;

                GameObject cellInstance = Instantiate(weaponPrefab, transform, false);
                Weapon weaponScript = cellInstance.GetComponent<Weapon>();
                weaponScript.weapon_Genome = weaponsArray[hori, vert].genome;
                weaponScript.shipHealthScript = shipHealthScript;//Separate out energy from health script?
                weaponScript.tracker = weaponsArray[hori, vert].tracker;

                Vector2 originOffset = new Vector2(CurrentGridWidth - 1, CurrentGridHeight - 1) / 2f;
                Vector2 position = (new Vector2(hori, CurrentGridHeight - 1 - vert) - originOffset);
                cellInstance.transform.localPosition = position;
            }



        ShipCoreStats shipCoreStats = new ShipCoreStats();
        ShipMobilityStats shipMobilityStats = new ShipMobilityStats();
        ShipDroneStats shipDroneStats = new ShipDroneStats();

        foreach ((ShipGenome genome, ModuleStatsTracker tracker) module in modulesArray)
        {
            ShipGenome genome = module.genome;
            if (genome == null)
                continue;

            shipCoreStats.Add(genome.hullHP, genome.armor, genome.shieldCapacity, genome.shieldRegen, genome.powerCapacity, genome.powerRegen, genome.evasion);
            shipMobilityStats.Add(genome.speed, genome.turnRate, genome.mass, genome.inertia);
            shipDroneStats.Add(genome.droneCount, genome.droneSpeed, genome.droneDurability, genome.droneAggression);
        }
        shipHealthScript.SetStats(shipCoreStats);
        gameObject.GetComponent<PlayerShip>().SetStats(shipMobilityStats);
        gameObject.GetComponent<ShipDroneManager>().SetStats(shipDroneStats);


        constructionCanvas.gameObject.SetActive(false);
        waveManager.StartWave();
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

    public float evasion;

    public void Add(int hullHP, int armor, int shieldCapacity, int shieldRegen, int powerCapacity, int powerRegen, float evasion)
    {
        this.hullHP += hullHP;
        this.armor += armor;
        this.shieldCapacity += shieldCapacity;
        this.shieldRegen += shieldRegen;
        this.powerCapacity += powerCapacity;
        this.powerRegen += powerRegen;

        this.evasion += evasion;
    }
}
public class ShipMobilityStats
{
    public float speed;
    public float turnRate;
    public float mass;
    public float inertia;

    public void Add(float speed, float turnRate, float mass, float inertia)
    {
        this.speed += speed;
        this.turnRate += turnRate;
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
