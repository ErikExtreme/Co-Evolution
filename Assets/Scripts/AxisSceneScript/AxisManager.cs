using System.IO;
using UnityEngine;

public class AxisManager : MonoBehaviour
{
    public EvolutionManager evoManager;
    public float axisScale;

    public string jsonFilePath;
    public SessionLog loadedLog;

    [Range(0, 50)]
    public int evolveIndex;
    [Range(0, 50)]
    public int generationIndex;

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

    public void LoadRecording()
    {
        if (loadedLog == null)
        {
            Debug.LogError("No log loaded.");
            return;
        }

        if (evolveIndex < 0 || evolveIndex >= loadedLog.recordings.Count)
        {
            Debug.LogError("Invalid evolve index.");
            return;
        }

        var rec = loadedLog.recordings[evolveIndex];

        // If generationIndex == -1, show global population
        if (generationIndex < 0)
        {
            LoadGlobalPopulation(rec);
        }
        else
        {
            LoadGeneration(rec);
        }
    }

    private void LoadGlobalPopulation(EvolveRecording rec)
    {
        mappedWeaponGenomes = new Vector3[rec.globalWeapons.Count];
        for (int i = 0; i < rec.globalWeapons.Count; i++)
            mappedWeaponGenomes[i] = rec.globalWeapons[i].axis;

        mappedShipGenomes = new Vector3[rec.globalShips.Count];
        for (int i = 0; i < rec.globalShips.Count; i++)
            mappedShipGenomes[i] = rec.globalShips[i].axis;
    }

    private void LoadGeneration(EvolveRecording rec)
    {
        if (generationIndex < 0 || generationIndex >= rec.generations.Count)
        {
            Debug.LogError("Invalid generation index.");
            return;
        }

        var gen = rec.generations[generationIndex];

        mappedWeaponGenomes = new Vector3[gen.weaponGenomes.Count];
        for (int i = 0; i < gen.weaponGenomes.Count; i++)
            mappedWeaponGenomes[i] = gen.weaponGenomes[i].axis;

        mappedShipGenomes = new Vector3[gen.shipGenomes.Count];
        for (int i = 0; i < gen.shipGenomes.Count; i++)
            mappedShipGenomes[i] = gen.shipGenomes[i].axis;
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

    public void LoadLog()
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("JSON file not found: " + jsonFilePath);
            return;
        }

        string json = File.ReadAllText(jsonFilePath);
        loadedLog = JsonUtility.FromJson<SessionLog>(json);

        Debug.Log("Loaded log with " + loadedLog.recordings.Count + " recordings.");
    }

    private int lastEvolveIndex = -999;
    private int lastGenerationIndex = -999;

    void OnValidate()
    {
        if (loadedLog == null)
            return;

        if (evolveIndex != lastEvolveIndex || generationIndex != lastGenerationIndex)
        {
            lastEvolveIndex = evolveIndex;
            lastGenerationIndex = generationIndex;
            LoadRecording();
        }
    }

}
