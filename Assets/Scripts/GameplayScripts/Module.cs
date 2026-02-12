using UnityEngine;

public class Module : MonoBehaviour
{
    public ShipGenome ship_Genome;
    public ModuleStatsTracker tracker;


    void Start()
    {
        ship_Genome = new ShipGenome();
        //ship_Genome = GlobalSettings.RandomShipGenome();

        //Temporary(?) default values

    }

}
