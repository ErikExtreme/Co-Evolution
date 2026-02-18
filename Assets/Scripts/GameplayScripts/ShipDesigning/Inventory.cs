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
        UIShipModule script = instance.GetComponent<UIShipModule>();
        script.Initialize(shipGenome, statsTextBox);
        ModuleManager.Instance.AddModule(shipGenome, script.moduleStatsTracker);
    }
    public void AddWeapon(WeaponGenome weaponGenome)
    {
        GameObject instance = Instantiate(UIweaponPrefab, transform);
        UIWeapon script = instance.GetComponent<UIWeapon>();
        script.Initialize(weaponGenome, statsTextBox);
        WeaponManager.Instance.AddWeapon(weaponGenome,script.weaponStatsTracker);
    }
}
