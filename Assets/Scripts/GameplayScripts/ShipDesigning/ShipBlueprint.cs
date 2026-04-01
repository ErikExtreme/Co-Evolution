using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipBlueprint : MonoBehaviour
{
    (WeaponGenome genome, WeaponStatsTracker tracker)[,] weaponsArray;
    (ShipGenome genome, ModuleStatsTracker tracker)[] modulesArray;

    public WeaponBoost[,] weaponBoosts;
    public float[,] weaponBoostTier;

    [SerializeField] WaveManager waveManager;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Canvas constructionCanvas;
    [SerializeField] RectTransform gridLayoutGroup;

    [SerializeField] private int moduleListSize = 5;
    [SerializeField] private int maxWeaponGridSize = 10;
    private int currentGridWidth;
    private int currentGridHeight;

    private float totalSpecialDensity;
    private int hashingSeed;
    private int boostTypeAmount = Enum.GetValues(typeof(WeaponBoost)).Length;

    public int ModuleListSize => moduleListSize;
    public int MaxWeaponGridSize => maxWeaponGridSize;
    public int CurrentGridWidth => Math.Clamp(currentGridWidth, 0, maxWeaponGridSize);
    public int CurrentGridHeight => Math.Clamp(currentGridHeight, 0, maxWeaponGridSize);

    private void Start()
    {
        weaponsArray = new (WeaponGenome genome, WeaponStatsTracker tracker)[maxWeaponGridSize, maxWeaponGridSize];
        modulesArray = new (ShipGenome genome, ModuleStatsTracker tracker)[moduleListSize];
        weaponBoosts = new WeaponBoost[maxWeaponGridSize, maxWeaponGridSize];

        hashingSeed = (int)System.DateTime.Now.Ticks;


        gridLayoutGroup.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurrentGridWidth * 75 + 26);//Hard coded, 75 = the width of a cell, 26 = random padding the layoutgroup has
        gridLayoutGroup.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CurrentGridHeight * 75 + 26);//Hard coded, 75 = the height of a cell, 26 = random padding the layoutgroup has
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
        totalSpecialDensity += genome.specialTileDensity;

        float boostTier = (CurrentGridHeight * CurrentGridWidth) >= 12 ? ((CurrentGridHeight * CurrentGridWidth) >= 30 ? 1 : 2) : 3;

        for (int i = 0; i < MaxWeaponGridSize; i++)
            for (int j = 0; j < MaxWeaponGridSize; j++)
            {
                bool shouldBeActive = i < CurrentGridWidth && j < CurrentGridHeight;
                int index = i + j * MaxWeaponGridSize;
                GameObject cell = gridLayoutGroup.GetChild(index).gameObject;
                cell.SetActive(shouldBeActive);

                if (cell.CompareTag("PlacementPoint") && shouldBeActive)
                    ToggleBoost(i, j, cell, boostTier);
            }
        gridLayoutGroup.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurrentGridWidth * 75 + 26);//Hard coded, 75 = the width of a cell, 26 = random padding the layoutgroup has
        gridLayoutGroup.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CurrentGridHeight * 75 + 26);//Hard coded, 75 = the height of a cell, 26 = random padding the layoutgroup has
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
        totalSpecialDensity -= modulesArray[pos].genome.specialTileDensity;

        modulesArray[pos].genome = null;
        modulesArray[pos].tracker = null;

        float boostTier = (CurrentGridHeight * CurrentGridWidth) >= 12 ? ((CurrentGridHeight * CurrentGridWidth) >= 30 ? 1 : 2) : 3;

        for (int i = 0; i < MaxWeaponGridSize; i++)
            for (int j = 0; j < MaxWeaponGridSize; j++)
            {
                bool shouldBeActive = i < CurrentGridWidth && j < CurrentGridHeight;
                int index = i + j * MaxWeaponGridSize;
                GameObject cell = gridLayoutGroup.GetChild(index).gameObject;

                cell.SetActive(shouldBeActive);

                if (cell.CompareTag("PlacementPoint") && shouldBeActive)
                    ToggleBoost(i, j, cell, boostTier);
            }

        gridLayoutGroup.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurrentGridWidth * 75 + 26);//Hard coded, 75 = the width of a cell, 26 = random padding the layoutgroup has
        gridLayoutGroup.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, CurrentGridHeight * 75 + 26);//Hard coded, 75 = the height of a cell, 26 = random padding the layoutgroup has
    }

    public void ConstructShip()
    {
        for (int i = 1; i < transform.childCount; i++)//skip first child
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        transform.GetChild(0).transform.localScale = new Vector2(CurrentGridWidth * 1.06f, CurrentGridHeight * 1.06f);

        PlayerHealth shipHealthScript = gameObject.GetComponent<PlayerHealth>();

        for (int vert = 0; vert < CurrentGridHeight; vert++)
            for (int hori = 0; hori < CurrentGridWidth; hori++)
            {
                if (weaponsArray[hori, vert].genome == null)
                    continue;

                GameObject cellInstance = Instantiate(weaponPrefab, transform, false);
                Weapon weaponScript = cellInstance.GetComponent<Weapon>();
                weaponScript.weapon_Genome = weaponsArray[hori, vert].genome;
                weaponScript.tracker = weaponsArray[hori, vert].tracker;
                weaponScript.shipHealthScript = shipHealthScript;//Separate out energy from health script?
                weaponScript.activeBoost = weaponBoosts[hori, vert];
                weaponScript.boostTier = weaponBoostTier[hori, vert];

                Vector2 originOffset = new Vector2(CurrentGridWidth - 1, CurrentGridHeight - 1) / 2f;
                Vector2 position = (new Vector2(hori, CurrentGridHeight - 1 - vert) - originOffset);
                cellInstance.transform.localPosition = position;
            }


        var shipStats = CalculateShipStats();
        shipHealthScript.SetStats(shipStats.shipCoreStats);
        gameObject.GetComponent<PlayerShip>().SetStats(shipStats.shipMobilityStats);
        gameObject.GetComponent<ShipDroneManager>().SetStats(shipStats.shipDroneStats);


        constructionCanvas.gameObject.SetActive(false);
        waveManager.StartWave();
    }

    public (ShipCoreStats shipCoreStats, ShipMobilityStats shipMobilityStats, ShipDroneStats shipDroneStats) CalculateShipStats()
    {
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

        return (shipCoreStats, shipMobilityStats, shipDroneStats);
    }
    public float CalculatePowerConsumption()
    {
        float consumption = 0;

        foreach (var weapon in weaponsArray)
        {
            if (weapon.genome == null)
                continue;

            //float burstDuration = (weapon.genome.burstSize / weapon.genome.fireRate);

            //consumption += (weapon.genome.powerCost * 1.66f) / (weapon.genome.cooldownTime + burstDuration);
            consumption += weapon.genome.powerCost;
        }

        return consumption;
    }

    private int HashCell(int x, int y, int seed)
    {
        int hash = seed;
        hash ^= x * 53070869;
        hash ^= y * 25314043;

        hash ^= hash >> 13;
        hash *= 98291261;
        hash ^= hash >> 16;
        return hash;

    }
    private void ToggleBoost(int xPos, int yPos, GameObject cell, float powerTier)
    {
        //Gets if cell should have boost
        int hash = HashCell(xPos + 1, yPos + 1, hashingSeed);//+1 cause it stats at 0
        float range = ((float)hash / int.MaxValue) * 5 * 5;//0-25 range, cause density is 0-5 in genome and there are up to 5 modules
        if (range <= totalSpecialDensity)
        {
            float range2 = range / totalSpecialDensity;//turn into 0-1 range with totalSpecialDensity as max
            range2 = Mathf.Clamp(range2, 0, 0.9999f);
            int index = 1 + Mathf.FloorToInt(range2 * (boostTypeAmount - 1));
            weaponBoosts[xPos, yPos] = (WeaponBoost)index;
            weaponBoostTier[xPos, yPos] = powerTier;
        }
        else
            weaponBoosts[xPos, yPos] = WeaponBoost.None;

        ToggleBoostVisuals(xPos, yPos,cell);
    }
    public void ToggleBoostVisuals(int xPos, int yPos, GameObject cell)
    {//Visualy show potential boost
        bool boostActive = weaponBoosts[xPos, yPos] != WeaponBoost.None;

        cell.transform.GetChild(0).gameObject.SetActive(boostActive);
        Color boostColor = cell.transform.GetChild(0).GetComponent<Image>().color;
        switch (weaponBoosts[xPos, yPos])
        {
            case WeaponBoost.None:
                break;
            case WeaponBoost.Damage:
                boostColor = Color.red;
                break;
            case WeaponBoost.BurstRate:
                boostColor = Color.yellow;
                break;
            case WeaponBoost.Accuracy:
                boostColor = Color.green;
                break;
            case WeaponBoost.LowerHeatGeneration:
                boostColor = Color.blue;
                break;
            //case WeaponBoost.MoreDamageMoreHeat:
            //    boostColor = Color.darkRed;
            //    break;
            //case WeaponBoost.BiggerAOELessBurstRate:
            //    boostColor = Color.purple;
            //    break;
            default:
                break;
        }
        boostColor.a = 0.7f;
        cell.transform.GetChild(0).GetComponent<Image>().color = boostColor;
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
    public List<(int count, float speed, int durability, float aggression)> availableDrones;
    public ShipDroneStats()
    {
        availableDrones = new List<(int count, float speed, int durability, float aggression)>();
    }
    public void Add(int droneCount, float droneSpeed, int droneDurability, float droneAggression)
    {
        availableDrones.Add((droneCount, droneSpeed / 10, droneDurability, droneAggression));
    }
}