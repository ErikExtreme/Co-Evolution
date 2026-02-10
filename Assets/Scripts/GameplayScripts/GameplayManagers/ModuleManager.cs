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

        try
        {
            if (modules[id] != null)
                return modules[id];

            Debug.LogError("Could not find modules on index: " + id);
            return null;
        }
        catch (System.Exception)
        {
            Debug.LogError("Module on index: " + id + " does not exist");
            return null;
        }
    }
}
