using System.Collections.Generic;
using UnityEngine;

public class ShipBlueprint : MonoBehaviour
{
    [SerializeField] int sideLength = 5;
    WeaponGenome[,] weaponsArray;
    ShipGenome[] modulesArray;

    [SerializeField] GameObject weaponPrefab;
    [SerializeField] Canvas canvas;

    private void Start()
    {
        weaponsArray = new WeaponGenome[sideLength, sideLength];
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
        if (modulesArray[pos] == null)
            modulesArray[pos] = newModule;
    }
    public void ConstructShip()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int vert = 0; vert < sideLength; vert++)
            for (int hori = 0; hori < sideLength; hori++)
            {
                if (weaponsArray[hori, vert] == null)
                    continue;

                GameObject cellInstance = Instantiate(weaponPrefab, transform, false);
                Weapon weaponScript = cellInstance.GetComponent<Weapon>();
                weaponScript.weapon_Genome = weaponsArray[hori, vert];
                weaponScript.shipHealthScript = gameObject.GetComponent<ShipHealth>();//Separate out energy from health script?

                Vector2 originOffset = new Vector2(sideLength - 1, sideLength - 1) / 2f;
                Vector2 position = (new Vector2(hori, sideLength - 1 - vert) - originOffset);
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
        gameObject.GetComponent<ShipHealth>().SetStats(shipCoreStats);
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
