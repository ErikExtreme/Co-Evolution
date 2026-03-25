//using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipDroneManager : MonoBehaviour
{
    //Stats
    int droneCount;
    float droneSpeed;
    int droneDurability;
    float droneAggression;

    //Variables
    [SerializeField] GameObject dronePrefab;
    List<Drone> drones;

    private void Start()
    {
        drones = new List<Drone>();
    }

    public void SpawnDrones()
    {
        int dronesToSpawn = droneCount - drones.Count;
        for (int i = 0; i < dronesToSpawn; i++)
        {
            Vector2 spawnPosition = transform.position + Random.onUnitSphere * 1.5f;
            Drone newDrone = Instantiate(dronePrefab, spawnPosition, Quaternion.Euler(transform.eulerAngles)).GetComponent<Drone>();
            newDrone.SetStats(droneSpeed, droneDurability, droneAggression);
            drones.Add(newDrone);
        }
    }

    public void SetStats(ShipDroneStats ShipDroneStats)
    {
        //Stats
        droneCount = ShipDroneStats.droneCount;
        droneSpeed = ShipDroneStats.droneSpeed / 10;
        droneDurability = ShipDroneStats.droneDurability;
        droneAggression = ShipDroneStats.droneAggression;
    }
}
