using System.Collections.Generic;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    public static ModuleManager Instance;

    public List<(ShipGenome genome, ModuleStatsTracker tracker)> modules = new List<(ShipGenome genome, ModuleStatsTracker tracker)>();

    [SerializeField] Inventory inventory;

    void Awake()
    {
        Instance = this;
    }

    public (ShipGenome genome, ModuleStatsTracker tracker) GetModule(int id)
    {
        if (modules.Count == 0)
            return (null,null);

        foreach (var module in modules)
        {
            if (module.genome.id == id) return module;
        }

        Debug.LogError("Could not find module with id: " + id);
        return (null, null);
    }
    public void AddModule(ShipGenome shipGenome, ModuleStatsTracker moduleStatsTracker)
    {
        modules.Add((shipGenome, moduleStatsTracker));

        inventory.AddModule(shipGenome, moduleStatsTracker);
    }
}
