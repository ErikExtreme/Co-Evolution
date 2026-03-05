using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipDroneManager : MonoBehaviour
{
    //Stats
    int droneCount;
    float droneSpeed;
    int droneDurability ;
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
        for (int i = 0; i < droneCount-drones.Count; i++)
        {
            Drone newDrone = Instantiate(dronePrefab, transform.position, Quaternion.Euler(transform.eulerAngles)).GetComponent<Drone>();
            newDrone.SetStats(droneSpeed, droneDurability, droneAggression);
            drones.Add(newDrone);
        }
    }

    public void SetStats(ShipDroneStats ShipDroneStats)
    {
        //Stats
        droneCount = ShipDroneStats.droneCount;
        droneSpeed = ShipDroneStats.droneSpeed;
        droneDurability = ShipDroneStats.droneDurability;
        droneAggression = ShipDroneStats.droneAggression;
    }
}
