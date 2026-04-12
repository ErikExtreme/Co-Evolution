using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WaveManager : MonoBehaviour
{
    [SerializeField] Canvas upgradeSelectionCanvas;
    [SerializeField] Canvas shipConstructionCanvas;
    [SerializeField] Canvas gameplayCanvas;
    [SerializeField] RectTransform moduleGrid;

    [SerializeField] Text waveDisplayText;
    [SerializeField] Text enemiesLeftText;

    [SerializeField] ShipDroneManager shipDroneManager;

    public List<GameObject> enemiesLeftInWave;

    [SerializeField] List<GameObject> enemyPrefabs;

    Camera sceneCamera;

    [SerializeField] int initialEnemies = 3;
    [SerializeField] float enemyAmountExponent = 0.6f;
    [SerializeField] float enemyStatsExponent = 0.5f;
    private int currentWave = 1;
    private int enemiesInWave { get { return Mathf.CeilToInt(initialEnemies * Mathf.Pow((currentWave), enemyAmountExponent)); } }
    bool wavesPaused = true;

    List<ModuleStatsTracker> moduleStatsTrackers;
    void Start()
    {
        enemiesLeftInWave = new List<GameObject>();
        sceneCamera = Camera.main;

        moduleStatsTrackers = new List<ModuleStatsTracker>();
    }

    private float waveStartTime;

    void Update()
    {
        if (enemiesLeftInWave.Count <= 0 && !wavesPaused)
        {
            WaveCompleted();
        }
        if (!wavesPaused)
        {
            foreach (var tracker in moduleStatsTrackers)
            {
                tracker.UpdateEquipped(Time.deltaTime);
            }
        }
    }

    private void WaveCompleted()
    {
        if (currentWave < 5 && currentWave >= 0)
            moduleGrid.GetChild(currentWave).gameObject.SetActive(true);

        // Record battle metrics before wave ends
        float waveDuration = Time.time - waveStartTime;
        bool playerSurvived = GetPlayerHealth()?.Health > 0;
        
        if (BattleMetricsRecorder.Instance != null && ModuleManager.Instance != null)
        {
            ModuleManager.Instance.RegisterBattleMetricsForAll(waveDuration, playerSurvived);
            BattleMetricsRecorder.Instance.EndBattle(playerSurvived);
        }

        if (BattleWeaponMetricsRecorder.Instance != null && WeaponManager.Instance != null)
        {
            WeaponManager.Instance.RegisterBattleMetricsForAll(waveDuration, playerSurvived);
            BattleWeaponMetricsRecorder.Instance.EndBattle(playerSurvived);
        }

        // Register individual weapon metrics
        RegisterWeaponMetrics(waveDuration, playerSurvived);

        wavesPaused = true;
        currentWave++;

        gameplayCanvas.gameObject.SetActive(false);
        upgradeSelectionCanvas.gameObject.SetActive(true);
        upgradeSelectionCanvas.GetComponent<UpgradeSelectionManager>().GetUpgrades();
    }

    public void StartWave()
    {
        gameplayCanvas.gameObject.SetActive(true);

        // Start metrics recording
        waveStartTime = Time.time;
        if (BattleMetricsRecorder.Instance != null)
        {
            BattleMetricsRecorder.Instance.BeginBattle();
        }

        if (BattleWeaponMetricsRecorder.Instance != null)
        {
            BattleWeaponMetricsRecorder.Instance.BeginBattle();
        }

        // Reset metrics for all modules before starting
        if (ModuleManager.Instance != null)
        {
            ModuleManager.Instance.ResetMetricsForAll();
        }

        // Reset metrics for all weapons before starting
        if (WeaponManager.Instance != null)
        {
            WeaponManager.Instance.ResetMetricsForAll();
        }

        for (int i = 0; i < enemiesInWave; i++)
        {
            Vector2 spawnPosition = RandomPointOutsideScreen(4);
            int enemyType = Random.Range(0, enemyPrefabs.Count);

            GameObject enemy = Instantiate(enemyPrefabs[enemyType], spawnPosition, Quaternion.identity, transform);
            enemy.GetComponent<EnemyHealth>().SetStats(Mathf.Pow((currentWave), enemyStatsExponent));
            enemiesLeftInWave.Add(enemy);
        }

        waveDisplayText.text = "Wave: " + currentWave;
        enemiesLeftText.text = enemiesLeftInWave.Count + "/" + enemiesInWave;

        wavesPaused = false;

        shipDroneManager.SpawnDrones();

        foreach (RectTransform module in moduleGrid)
        {
            if (module.GetComponent<UIShipModule>()?.moduleStatsTracker != null)
                moduleStatsTrackers.Add(module.GetComponent<UIShipModule>().moduleStatsTracker);
        }
    }

    private PlayerHealth GetPlayerHealth()
    {
        return GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
    }

    /// <summary>
    /// Register all weapon metrics at the end of a battle.
    /// </summary>
    private void RegisterWeaponMetrics(float waveDuration, bool playerSurvived)
    {
        if (WeaponManager.Instance == null)
            return;

        foreach (var (genome, tracker) in WeaponManager.Instance.GetAllWeapons())
        {
            if (tracker != null && genome != null)
            {
                // Find the weapon script to get accumulated metrics
                Weapon weaponScript = FindWeaponScript(genome.id);
                if (weaponScript != null)
                {
                    // Register the battle metrics
                    weaponScript.RegisterBattleMetrics(waveDuration);
                    
                    // Register survival outcome
                    tracker.RegisterBattleOutcome(waveDuration, playerSurvived);
                    
                    // Register survival contribution (binary for now)
                    tracker.RegisterSurvivalContribution(playerSurvived ? 1f : 0f);
                }
            }
        }
    }

    /// <summary>
    /// Find the Weapon script instance by genome ID.
    /// </summary>
    private Weapon FindWeaponScript(int genomeId)
    {
        Weapon[] allWeapons = FindObjectsOfType<Weapon>();
        foreach (var weapon in allWeapons)
        {
            if (weapon.weapon_Genome != null && weapon.weapon_Genome.id == genomeId)
            {
                return weapon;
            }
        }
        return null;
    }

    private Vector2 RandomPointOutsideScreen(float objectWidth)
    {
        float edgePosition = edgePosition = Random.Range(0f, 4f);
        int edge = Mathf.FloorToInt(edgePosition);
        float posAlongEdge = edgePosition - edge;

        Vector2 worldPosition;
        switch (edge)
        {
            case 0:
                worldPosition = sceneCamera.ViewportToWorldPoint(new Vector2(0, posAlongEdge)) + new Vector3(-objectWidth / 2, 0);
                break;
            case 1:
                worldPosition = sceneCamera.ViewportToWorldPoint(new Vector2(1, posAlongEdge)) + new Vector3(objectWidth / 2, 0);
                break;
            case 2:
                worldPosition = sceneCamera.ViewportToWorldPoint(new Vector2(posAlongEdge, 0)) + new Vector3(0, -objectWidth / 2);
                break;
            case 3:
                worldPosition = sceneCamera.ViewportToWorldPoint(new Vector2(posAlongEdge, 1)) + new Vector3(0, objectWidth / 2);
                break;
            default:
                worldPosition = new Vector2(0, 0);
                break;
        }
        return worldPosition;
    }

    public void DestroyEnemy(GameObject enemy)
    {
        enemiesLeftInWave.Remove(enemy);
        Destroy(enemy);

        enemiesLeftText.text = enemiesLeftInWave.Count + "/" + enemiesInWave;
    }
}
