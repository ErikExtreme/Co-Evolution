using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject UIweaponPrefab;
    [SerializeField] GameObject UImodulePrefab;

    [SerializeField] Text statsTextBox; 
    private void Start()
    {
        //For testing
        for (int i = 0; i < 5; i++)
        {
            AddModule(new ShipGenome());
        }
        for (int i = 0; i < 10; i++)
        {
            AddWeapon(new WeaponGenome());
        }
    }
    public void AddModule(ShipGenome shipGenome)
    {
        GameObject instance = Instantiate(UImodulePrefab, transform);
        instance.GetComponent<UIShipModule>().Initialize(shipGenome, statsTextBox);
    }
    public void AddWeapon(WeaponGenome weaponGenome)
    {
        GameObject instance = Instantiate(UIweaponPrefab, transform);
        instance.GetComponent<UIWeapon>().Initialize(weaponGenome, statsTextBox);
    }
}
