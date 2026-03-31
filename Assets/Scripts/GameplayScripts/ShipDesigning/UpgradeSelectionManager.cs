using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSelectionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] EvolutionManager evolutionManager;
    [SerializeField] Transform weaponSelectionParent;
    [SerializeField] Transform moduleSelectionParent;
    [SerializeField] Text statsTextBox;

    [SerializeField] Canvas constructionCanvas;
    [SerializeField] Inventory inventoryScript;

    [SerializeField] MappingDisplayBar MappingDisplayBarX;
    [SerializeField] MappingDisplayBar MappingDisplayBarY;
    [SerializeField] MappingDisplayBar MappingDisplayBarZ;
    [SerializeField] GameObject WeaponTexts;
    [SerializeField] GameObject ModuleTexts;

    [Header("Prefabs")]
    [SerializeField] GameObject uiWeaponPrefab;
    [SerializeField] GameObject uiModulePrefab;

    [Header("Variables")]
    [SerializeField] int availableWeapons = 5;
    [SerializeField] int availableModules = 5;
    [SerializeField] public int weaponSelectionAmount = 1;
    [SerializeField] public int moduleSelectionAmount = 1;

    private int selectedWeapons = 0;
    private int selectedModules = 0;

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
            //upgrades.weapons[i].id = WeaponGenome.GetNextWeaponId();

            WeaponStatsTracker weaponStatsTracker = new WeaponStatsTracker();
            UIWeapon weaponScript = weaponInstance.GetComponent<UIWeapon>();
            weaponScript.Initialize(upgrades.weapons[i], weaponStatsTracker, statsTextBox);
            weaponScript.MappingDisplayBarX = MappingDisplayBarX;
            weaponScript.MappingDisplayBarY = MappingDisplayBarY;
            weaponScript.MappingDisplayBarZ = MappingDisplayBarZ;
            weaponScript.WeaponTexts = WeaponTexts;
            weaponScript.ModuleTexts = ModuleTexts;

            weaponInstance.GetComponent<SelectUpgrade>().upgradeSelectionManager = this;
        }
        for (int i = 0; i < availableModules; i++)
        {
            GameObject moduleInstance = Instantiate(uiModulePrefab, moduleSelectionParent);
            //upgrades.modules[i].id = ShipGenome.GetNextShipId();

            ModuleStatsTracker moduleStatsTracker = new ModuleStatsTracker();
            UIShipModule moduleScript = moduleInstance.GetComponent<UIShipModule>();
            moduleScript.Initialize(upgrades.modules[i], moduleStatsTracker, statsTextBox);
            moduleScript.MappingDisplayBarX = MappingDisplayBarX;
            moduleScript.MappingDisplayBarY = MappingDisplayBarY;
            moduleScript.MappingDisplayBarZ = MappingDisplayBarZ;
            moduleScript.WeaponTexts = WeaponTexts;
            moduleScript.ModuleTexts = ModuleTexts;

            moduleInstance.GetComponent<SelectUpgrade>().upgradeSelectionManager = this;
        }
    }

    public void AddWeapon(UIWeapon weapon)
    {
        if (selectedWeapons >= weaponSelectionAmount)
            return;
        selectedWeapons++;

        inventoryScript.AddWeapon(weapon.weaponGenome, weapon.weaponStatsTracker);
        Destroy(weapon.gameObject);

        IsSelectionDone();
    }
    public void AddModule(UIShipModule module)
    {
        if (selectedModules >= moduleSelectionAmount)
            return;
        selectedModules++;

        inventoryScript.AddModule(module.shipGenome, module.moduleStatsTracker);
        Destroy(module.gameObject);

        IsSelectionDone();
    }
    private void IsSelectionDone()
    {
        if (selectedWeapons < weaponSelectionAmount)
            return;
        if (selectedModules < moduleSelectionAmount)
            return;

        selectedWeapons = 0;
        selectedModules = 0;

        constructionCanvas.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
