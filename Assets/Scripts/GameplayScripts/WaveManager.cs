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
    private int enemiesInWave { get { return Mathf.CeilToInt(initialEnemies * Mathf.Pow((currentWave),enemyAmountExponent)); } }
    bool wavesPaused = true;
    void Start()
    {
        enemiesLeftInWave = new List<GameObject>();
        sceneCamera = Camera.main;
    }

    void Update()
    {
        if (enemiesLeftInWave.Count <= 0 && wavesPaused == false)
        {
            WaveCompleted();
        }
    }
    private void WaveCompleted()
    {
        if (currentWave < 5 && currentWave >= 0)
            moduleGrid.GetChild(currentWave).gameObject.SetActive(true);


        wavesPaused = true;
        currentWave++;

        gameplayCanvas.gameObject.SetActive(false);
        upgradeSelectionCanvas.gameObject.SetActive(true);
        upgradeSelectionCanvas.GetComponent<UpgradeSelectionManager>().GetUpgrades();
    }
    public void StartWave()
    {
        gameplayCanvas.gameObject.SetActive(true);

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
