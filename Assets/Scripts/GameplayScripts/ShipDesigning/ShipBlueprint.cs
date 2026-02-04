using System.Collections.Generic;
using UnityEngine;

public class ShipBlueprint : MonoBehaviour
{
    [SerializeField] int sideLength = 5;
    TempModules[,] shipArray;

    [SerializeField] GameObject weaponPrefab;
    [SerializeField] GameObject modulePrefab;

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

                GameObject cellInstance = null;
                if (shipArray[hori, vert] == TempModules.weapon)
                    cellInstance = Instantiate(weaponPrefab, transform, false);
                if (shipArray[hori, vert] == TempModules.module)
                {
                    cellInstance = Instantiate(modulePrefab, transform, false);


                }


                float cellSize = 1f;//HARDCODED 1 FIX
                Vector2 originOffset = new Vector2(sideLength - 1, sideLength - 1) / 2f;
                Vector2 position = (new Vector2(hori, sideLength - 1 - vert) - originOffset) * cellSize;
                cellInstance.transform.localPosition = position;
            }
    }
}
public enum TempModules
{
    none,
    weapon,
    module
}
