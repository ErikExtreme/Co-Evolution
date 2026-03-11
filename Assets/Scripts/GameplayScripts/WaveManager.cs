using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WaveManager : MonoBehaviour
{
    [SerializeField] Canvas upgradeSelectionCanvas;
    [SerializeField] Canvas shipConstructionCanvas;
    [SerializeField] Text waveDisplayText;
    [SerializeField] Text enemiesLeftText;

    List<GameObject> enemiesLeftInWave;

    [SerializeField] GameObject enemyPrefab;

    Camera sceneCamera;

    [SerializeField] int initialEnemies = 1;
    [SerializeField] int enemyAmountIncrease = 1;
    private int currentWave = 1;
    private int enemiesInWave { get { return initialEnemies + enemyAmountIncrease * (currentWave - 1); } }
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
        wavesPaused = true;
        currentWave++;

        upgradeSelectionCanvas.gameObject.SetActive(true);
        upgradeSelectionCanvas.GetComponent<UpgradeSelection>().GetUpgrades();


        //shipConstructionCanvas.gameObject.SetActive(true);
    }
    public void StartWave()
    {
        for (int i = 0; i < enemiesInWave; i++)
        {
            Vector2 spawnPosition = RandomPointOutsideScreen(4);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
            enemiesLeftInWave.Add(enemy);
        }

        waveDisplayText.text = "Wave: " + currentWave;
        enemiesLeftText.text = enemiesLeftInWave.Count + "/" + enemiesInWave;

        wavesPaused = false;
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
