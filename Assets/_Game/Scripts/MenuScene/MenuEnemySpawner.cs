using System.Collections;
using UnityEngine;

public class MenuEnemySpawner : MonoBehaviour
{
    public MenuEnemy enemyPrefab;
    public float spawnInterval = 2f;

    private Camera _mainCamera;

    void Start()
    {
        _mainCamera = Camera.main;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();
        Vector2 destination = GetDiagonalOppositePosition(spawnPosition);

        MenuEnemy enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemy.SetDestination(destination);
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 screenMin = _mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 screenMax = _mainCamera.ViewportToWorldPoint(new Vector2(1, 1));

        int edge = Random.Range(0, 4);

        Vector2 spawnPos = Vector2.zero;

        switch (edge)
        {
            case 0:
                spawnPos = new Vector2(screenMin.x - 1, Random.Range(screenMin.y, screenMax.y));
                break;
            case 1:
                spawnPos = new Vector2(screenMax.x + 1, Random.Range(screenMin.y, screenMax.y));
                break;
            case 2:
                spawnPos = new Vector2(Random.Range(screenMin.x, screenMax.x), screenMax.y + 1);
                break;
            case 3:
                spawnPos = new Vector2(Random.Range(screenMin.x, screenMax.x), screenMin.y - 1);
                break;
        }

        return spawnPos;
    }

    private Vector2 GetDiagonalOppositePosition(Vector2 spawnPosition)
    {
        Vector2 screenMin = _mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 screenMax = _mainCamera.ViewportToWorldPoint(new Vector2(1, 1));

        Vector2 destination = Vector2.zero;

        destination.x = (spawnPosition.x < screenMin.x) ? screenMax.x + 1 : screenMin.x - 1;
        destination.y = (spawnPosition.y < screenMin.y) ? screenMax.y + 1 : screenMin.y - 1;

        return destination;
    }
}
