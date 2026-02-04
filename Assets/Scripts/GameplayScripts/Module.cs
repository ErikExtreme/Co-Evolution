using UnityEngine;

public class Module : MonoBehaviour
{
    ShipGenome ship_Genome;


    void Start()
    {
        ship_Genome = new ShipGenome();
        //ship_Genome = GlobalSettings.RandomShipGenome();

        //Temporary(?) default values

    }

}
