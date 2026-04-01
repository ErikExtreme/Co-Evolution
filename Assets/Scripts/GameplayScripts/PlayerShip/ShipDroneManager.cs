//using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipDroneManager : MonoBehaviour
{
    //Variables
    [SerializeField] GameObject dronePrefab;
    List<(int count, float speed, int durability, float aggression, ModuleStatsTracker tracker)> availableDrones;
    List<Drone> activeDrones;

    private void Start()
    {
        availableDrones = new List<(int count, float speed, int durability, float aggression, ModuleStatsTracker tracker)>();
        activeDrones = new List<Drone>();
    }

    public void SpawnDrones()
    {
        activeDrones.ForEach(drone => { if (drone != null) Destroy(drone); });
        activeDrones.Clear();

        foreach (var droneCollection in availableDrones)
        {
            for (int i = 0; i < droneCollection.count; i++)
            {
                Vector2 spawnPosition = transform.position + Random.onUnitSphere * 1.5f;
                Drone newDrone = Instantiate(dronePrefab, spawnPosition, Quaternion.Euler(transform.eulerAngles)).GetComponent<Drone>();
                newDrone.SetStats(droneCollection.speed, droneCollection.durability, droneCollection.aggression, this, droneCollection.tracker);
                activeDrones.Add(newDrone);
            }
        }
    }

    public void SetStats(ShipDroneStats shipDroneStats)
    {
        availableDrones = shipDroneStats.availableDrones;
    }
    public void DestroyDrone(Drone drone)
    {
        activeDrones.Remove(drone);
        Destroy(drone.gameObject);
    }
}
