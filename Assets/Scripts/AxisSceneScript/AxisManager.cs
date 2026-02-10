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
        var result = evoManager.Evolve(50, 20);
        weaponGenomes = result.weaponGenomes;
        mappedWeaponGenomes = new Vector3[weaponGenomes.Length];
        for (int i = 0; i < weaponGenomes.Length; i++)
        {
            mappedWeaponGenomes[i] = Mapping.MapGenome(weaponGenomes[i]);
        }

        shipGenomes = result.shipGenomes;
        mappedShipGenomes = new Vector3[shipGenomes.Length];
        for (int i = 0; i < shipGenomes.Length; i++)
        {
            mappedShipGenomes[i] = Mapping.MapGenome(shipGenomes[i]);
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
