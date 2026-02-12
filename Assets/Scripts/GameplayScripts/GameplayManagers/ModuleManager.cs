using System.Collections.Generic;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    public static ModuleManager Instance;

    public List<Module> modules = new List<Module>();

    void Awake()
    {
        Instance = this;
    }

    public Module GetModule(int id)
    {
        if (modules.Count == 0)
            return null;

        foreach (Module module in modules)
        {
            if (module.ship_Genome.id == id) return module;
        }

        Debug.LogError("Could not find weapon with id: " + id);
        return null;
    }
}
