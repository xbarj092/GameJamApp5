using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _spawnInterval = 5f;

    private List<Enemy> _activeEnemies = new();

    [SerializeField] private SerializedDictionary<int, int> _maxEnemies = new();

    private void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    private void OnDisable()
    {
        foreach (Enemy enemy in _activeEnemies)
        {
            enemy.OnEnemyKilled -= HandleEnemyKilled;
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval);
            foreach (KeyValuePair<int, int> maxEnemies in _maxEnemies)
            {
                if (maxEnemies.Key <= GameManager.Instance.SecondsPassed)
                {
                    if (_activeEnemies.Count < maxEnemies.Value)
                    {
                        SpawnEnemy();
                        break;
                    }
                }
            }
        }
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        Enemy newEnemy = Instantiate(_enemyPrefab, spawnPoint.position, new(0, 0, 180, 0));
        _activeEnemies.Add(newEnemy);
        newEnemy.OnEnemyKilled += HandleEnemyKilled;
    }

    private void HandleEnemyKilled(Enemy enemy)
    {
        enemy.OnEnemyKilled -= HandleEnemyKilled;
        _activeEnemies.Remove(enemy);
    }
}
