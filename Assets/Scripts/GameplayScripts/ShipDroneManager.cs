using UnityEngine;

public class ShipDroneManager : MonoBehaviour
{
    //Stats
    int droneCount = 1;
    float droneSpeed = 2;
    int droneDurability = 1;
    float droneAggression;

    //Variables
    [SerializeField] GameObject dronePrefab;
    Drone[] drones;


    void Start()
    {
        drones = new Drone[droneCount];

        for (int i = 0; i < droneCount; i++)
        {
            drones[i] = Instantiate(dronePrefab, transform.position, Quaternion.Euler(transform.eulerAngles)).GetComponent<Drone>();
            drones[i].SetStats(droneSpeed,droneDurability,droneAggression);
        }
    }
}
