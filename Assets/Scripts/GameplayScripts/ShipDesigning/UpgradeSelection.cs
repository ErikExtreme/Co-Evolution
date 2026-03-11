using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSelection : MonoBehaviour
{
    [SerializeField] EvolutionManager evolutionManager;
    [SerializeField] Transform weaponSelectionParent;
    [SerializeField] Transform moduleSelectionParent;
    [SerializeField] Text statsTextBox;

    [SerializeField] GameObject uiWeaponPrefab;
    [SerializeField] GameObject uiModulePrefab;

    [SerializeField] int upgradeAmount = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GetUpgrades()
    {
        foreach (Transform child in weaponSelectionParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in moduleSelectionParent)
        {
            Destroy(child.gameObject);
        }


        (List<WeaponGenome> weapons, List<ShipGenome> modules) upgrades = evolutionManager.Evolve();

        for (int i = 0; i < upgradeAmount; i++)
        {
            GameObject weaponInstance = Instantiate(uiWeaponPrefab, weaponSelectionParent);
            upgrades.weapons[i].id = WeaponGenome.GetNextWeaponId();
            weaponInstance.GetComponent<UIWeapon>().weaponGenome = upgrades.weapons[i];
            WeaponStatsTracker weaponStatsTracker = new WeaponStatsTracker();
            UIWeapon weaponScript = weaponInstance.GetComponent<UIWeapon>();
            weaponScript.Initialize(upgrades.weapons[i], weaponStatsTracker, statsTextBox);

            GameObject moduleInstance = Instantiate(uiModulePrefab, moduleSelectionParent);
            upgrades.modules[i].id = ShipGenome.GetNextShipId();
            ModuleStatsTracker moduleStatsTracker = new ModuleStatsTracker();
            UIShipModule moduleScript = moduleInstance.GetComponent<UIShipModule>();
            moduleScript.Initialize(upgrades.modules[i], moduleStatsTracker, statsTextBox);
        }
    }
}
