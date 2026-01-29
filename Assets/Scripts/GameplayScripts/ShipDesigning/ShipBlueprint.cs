using System.Collections.Generic;
using UnityEngine;

public class ShipBlueprint : MonoBehaviour
{
    [SerializeField] int sideLength = 5;
    TempModules[,] shipArray;

    [SerializeField] GameObject tempHullPrefab;
    [SerializeField] GameObject tempWeaponPrefab;
    [SerializeField] GameObject tempThrusterPrefab;

    private void Start()
    {
        shipArray = new TempModules[sideLength, sideLength];
    }

    public void SetModule(TempModules newModule, int horiPos, int vertPos)
    {
        if (shipArray[horiPos, vertPos] == TempModules.none)
            shipArray[horiPos, vertPos] = newModule;
    }
    public void ConstructShip()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int vert = 0; vert < sideLength; vert++)
            for (int hori = 0; hori < sideLength; hori++)
            {
                if (shipArray[hori, vert] == TempModules.none)
                    continue;

                GameObject moduleInstance = null;
                if (shipArray[hori, vert] == TempModules.hull)
                    moduleInstance = Instantiate(tempHullPrefab, transform, false);
                if (shipArray[hori, vert] == TempModules.gun)
                    moduleInstance = Instantiate(tempWeaponPrefab, transform, false);
                if (shipArray[hori, vert] == TempModules.thruster)
                    moduleInstance = Instantiate(tempThrusterPrefab, transform, false);


                float cellSize = 1f;//HARDCODED 1 FIX
                Vector2 originOffset = new Vector2(sideLength - 1, sideLength - 1) / 2f;
                Vector2 position = (new Vector2(hori, sideLength - 1 - vert) - originOffset) * cellSize;
                moduleInstance.transform.localPosition = position;
            }
    }
}
public enum TempModules
{
    none,
    hull,
    gun,
    thruster
}
