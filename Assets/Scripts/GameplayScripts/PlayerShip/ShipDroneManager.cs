//using System;
using System.Collections.Generic;
using UnityEngine;

public class ShipDroneManager : MonoBehaviour
{
    //Variables
    [SerializeField] GameObject dronePrefab;
    List<(int count, float speed, int durability, float aggression)> availableDrones;
    List<Drone> activeDrones;

    private void Start()
    {
        availableDrones = new List<(int count, float speed, int durability, float aggression)> ();
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
                newDrone.SetStats(droneCollection.speed, droneCollection.durability, droneCollection.aggression, this);
                activeDrones.Add(newDrone);
            }
        }
    }

    public void SetStats(ShipDroneStats ShipDroneStats)
    {
        availableDrones = ShipDroneStats.availableDrones;
    }
    public void DestroyDrone(Drone drone)
    {
        activeDrones.Remove(drone);
        Destroy(drone.gameObject);
    }
}
