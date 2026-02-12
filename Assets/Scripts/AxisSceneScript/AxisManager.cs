using UnityEngine;

public class AxisManager : MonoBehaviour
{
    public EvolutionManager evoManager;
    public float axisScale;
    [Range(1, 20)]public int generation;

    public WeaponGenome[] weaponGenomes;
    public ShipGenome[] shipGenomes;

    public Vector3[] mappedWeaponGenomes;
    public Vector3[] mappedShipGenomes;

    public int randomPopSize;

    public bool drawWeaponGenomes;
    public bool drawShipGenomes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Mapping.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (axisScale < 0)
            axisScale = 1;
    }

    public void Generate()
    {
        
    }

    public void GenerateRandom()
    {
        WeaponGenome[] weapons = Seeding.RandomWeaponSeed(randomPopSize);
        ShipGenome[] ships = Seeding.RandomShipSeed(randomPopSize);

        mappedWeaponGenomes = new Vector3[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            mappedWeaponGenomes[i] = Mapping.MapGenome(weapons[i]);
        }

        mappedShipGenomes = new Vector3[ships.Length];
        for (int i = 0; i < ships.Length; i++)
        {
            mappedShipGenomes[i] = Mapping.MapGenome(ships[i]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * axisScale));
        Gizmos.DrawLine(transform.position, transform.position - (Vector3.up * axisScale));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.right * axisScale));
        Gizmos.DrawLine(transform.position, transform.position - (Vector3.right * axisScale));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.forward * axisScale));
        Gizmos.DrawLine(transform.position, transform.position - (Vector3.forward * axisScale));

        Gizmos.color = Color.orange;
        if (mappedWeaponGenomes != null && mappedWeaponGenomes.Length > 0 && drawWeaponGenomes)
        {
            foreach (var g in mappedWeaponGenomes)
            {
                Gizmos.DrawSphere(transform.position + (g * axisScale), 0.01f * axisScale);
            }
        }

        Gizmos.color = Color.blue;
        if (mappedShipGenomes != null && mappedShipGenomes.Length > 0 && drawShipGenomes)
        {
            foreach (var g in mappedShipGenomes)
            {
                Gizmos.DrawSphere(transform.position + (g * axisScale), 0.01f * axisScale);
            }
        }
    }
}
