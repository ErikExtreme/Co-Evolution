using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject UIweaponPrefab;
    [SerializeField] GameObject UImodulePrefab;

    [SerializeField] Text statsTextBox;
  
    public void AddModule(ShipGenome shipGenome, ModuleStatsTracker moduleStatsTracker)
    {
        GameObject instance = Instantiate(UImodulePrefab, transform);
        UIShipModule script = instance.GetComponent<UIShipModule>();
        script.Initialize(shipGenome, moduleStatsTracker, statsTextBox);
    }
    public void AddWeapon(WeaponGenome weaponGenome, WeaponStatsTracker weaponStatsTracker)
    {
        GameObject instance = Instantiate(UIweaponPrefab, transform);
        UIWeapon script = instance.GetComponent<UIWeapon>();
        script.Initialize(weaponGenome, weaponStatsTracker, statsTextBox);
    }
}
