using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaveManager : MonoBehaviour
{
    List<GameObject> enemiesLeftInWave;

    [SerializeField] GameObject enemyPrefab;

    Camera sceneCamera;

    [SerializeField] int enemiesInWave = 1;
    [SerializeField] int enemyPerWaveIncrease = 1;
    void Start()
    {
        enemiesLeftInWave = new List<GameObject>();
        sceneCamera = Camera.main;
    }

    void Update()
    {
        if (enemiesLeftInWave.Count <= 0)
        {
            StartWave();
        }
    }

    private void StartWave()
    {
        for (int i = 0; i < enemiesInWave; i++)
        {
            Vector2 spawnPosition = RandomPointOutsideScreen(4);
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
            enemiesLeftInWave.Add(enemy);
        }
        enemiesInWave += enemyPerWaveIncrease;
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
    }
}
