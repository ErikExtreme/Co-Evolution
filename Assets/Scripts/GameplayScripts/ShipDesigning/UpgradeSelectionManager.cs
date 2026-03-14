using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSelectionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] EvolutionManager evolutionManager;
    [SerializeField] Canvas constructionCanvas;
    [SerializeField] Transform weaponSelectionParent;
    [SerializeField] Transform moduleSelectionParent;
    [SerializeField] Text statsTextBox;

    [Header("Prefabs")]
    [SerializeField] GameObject uiWeaponPrefab;
    [SerializeField] GameObject uiModulePrefab;

    [Header("Variables")]
    [SerializeField] int availableWeapons = 5;
    [SerializeField] int availableModules = 5;
    [SerializeField] int weaponSelectionAmount = 1;
    [SerializeField] int moduleSelectionAmount = 1;

    void Update()
    {
        if(moduleSelectionAmount<=0 && moduleSelectionAmount <= 0)
        {
            constructionCanvas.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }    
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

        for (int i = 0; i < availableWeapons; i++)
        {
            GameObject weaponInstance = Instantiate(uiWeaponPrefab, weaponSelectionParent);
            upgrades.weapons[i].id = WeaponGenome.GetNextWeaponId();

            WeaponStatsTracker weaponStatsTracker = new WeaponStatsTracker();
            UIWeapon weaponScript = weaponInstance.GetComponent<UIWeapon>();
            weaponScript.Initialize(upgrades.weapons[i], weaponStatsTracker, statsTextBox);
        }
        for (int i = 0; i < availableModules; i++)
        {
            GameObject moduleInstance = Instantiate(uiModulePrefab, moduleSelectionParent);
            upgrades.modules[i].id = ShipGenome.GetNextShipId();

            ModuleStatsTracker moduleStatsTracker = new ModuleStatsTracker();
            UIShipModule moduleScript = moduleInstance.GetComponent<UIShipModule>();
            moduleScript.Initialize(upgrades.modules[i], moduleStatsTracker, statsTextBox);
        }
    }
}
