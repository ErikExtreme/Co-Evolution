using UnityEngine;

public class AxisManager : MonoBehaviour
{
    public EvolutionManager evoManager;
    public float axisScale;
    [Range(1, 20)]public int generation;

    public WeaponGenome[] weaponGenomes;

    public Vector3[] mappedWeaponGenomes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Fitness.Init();
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
            mappedWeaponGenomes[i] = Fitness.MapGenome(weaponGenomes[i]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * axisScale));
        Gizmos.DrawLine(transform.position, transform.position - (Vector3.up * axisScale));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.right * axisScale));
        Gizmos.DrawLine(transform.position, transform.position - (Vector3.right * axisScale));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.forward * axisScale));
        Gizmos.DrawLine(transform.position, transform.position - (Vector3.forward * axisScale));

        if (mappedWeaponGenomes.Length > 0)
        {
            foreach (var g in mappedWeaponGenomes)
            {
                Gizmos.DrawSphere(transform.position + (g * axisScale), 0.01f * axisScale);
            }
        }
    }
}
