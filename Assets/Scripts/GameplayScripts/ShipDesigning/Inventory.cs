using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] GameObject UIweaponPrefab;
    [SerializeField] GameObject UImodulePrefab;
    private void Start()
    {
        AddModule(new ShipGenome());
        AddWeapon(new WeaponGenome());
    }
    public void AddModule(ShipGenome shipGenome)
    {
        GameObject instance = Instantiate(UImodulePrefab, transform);
        instance.GetComponent<UIShipModule>().shipGenome = shipGenome;
    }
    public void AddWeapon(WeaponGenome weaponGenome)
    {
        GameObject instance = Instantiate(UIweaponPrefab, transform);
        instance.GetComponent<UIWeapon>().weaponGenome = weaponGenome;
    }
}
