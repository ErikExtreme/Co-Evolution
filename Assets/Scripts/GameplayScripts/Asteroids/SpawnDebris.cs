using UnityEngine;

public class SpawnDebris : MonoBehaviour
{
    [SerializeField] float spawnIntervalSeconds = 1;
    float timePassed;

    GameObject debrisParent;
    [SerializeField] GameObject debrisPrefab;

    [SerializeField] float debrisSpeed;

    float debrisWidth;

    Camera sceneCamera;

    void Start()
    {
        debrisWidth = debrisPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        debrisParent = new GameObject("Debris");

        sceneCamera = Camera.main;

        //Spawns debris on start based on calculated average debris on screen
        float averageScreenSize = sceneCamera.orthographicSize * (1 + sceneCamera.aspect);
        float averageDebrisTimeOnScreen = averageScreenSize / debrisSpeed;
        int averageDebrisOnScreen = Mathf.RoundToInt(averageDebrisTimeOnScreen / spawnIntervalSeconds);
        for (int i = 0; i < averageDebrisOnScreen - 3; i++)
        {
            SpawnDebrisOnScreen();
        }
    }

    void Update()
    {
        if (timePassed >= spawnIntervalSeconds)
        {
            float scale = Random.Range(0.7f, 1.3f);

            (Vector2 position, int edge) edgePosition = RandomPointOutsideScreen(debrisWidth * scale);

            Vector2 destination = RandomPointOutsideScreen(debrisWidth * scale, edgePosition.edge).position;
            Vector2 direction = (destination - edgePosition.position).normalized;

            SpawnDebrisAtPosition(edgePosition.position, scale, direction);

            timePassed -= spawnIntervalSeconds;
        }
        timePassed += Time.deltaTime;
    }
    private void SpawnDebrisAtPosition(Vector2 position, float scale, Vector2 direction)
    {
        GameObject debris = Instantiate(debrisPrefab, position, Quaternion.identity, debrisParent.transform);
        debris.transform.localScale *= scale;
        debris.GetComponent<Rigidbody2D>().linearVelocity = direction * debrisSpeed;

        float randomAngularVelocity = Random.Range(20f, 30f) * Mathf.Sign(Random.value - 0.5f);
        debris.GetComponent<Rigidbody2D>().angularVelocity = randomAngularVelocity;
    }
    private void SpawnDebrisOnScreen()
    {
        Vector2 randomPositionOnScreen = sceneCamera.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
        float scale = Random.Range(0.7f, 1.3f);
        Vector2 direction = Random.insideUnitCircle.normalized;
        SpawnDebrisAtPosition(randomPositionOnScreen, scale, direction);
    }
    private (Vector2 position, int edge) RandomPointOutsideScreen(float objectWidth, int skippedEdge)
    {
        float edgePosition;
        do
        {
            edgePosition = Random.Range(0f, 4f);
        } while (edgePosition >= skippedEdge - 0.1f && edgePosition < skippedEdge + 1.1f);
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
        return (worldPosition, edge);
    }
    private (Vector2 position, int edge) RandomPointOutsideScreen(float objectWidth)
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
        return (worldPosition, edge);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Asteroid"))
            Destroy(collision.gameObject);
    }
}
