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
}
