using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy _enemyPrefab;
    [SerializeField] private BossEnemy _bossPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _spawnInterval = 5f;

    private List<Enemy> _activeEnemies = new();
    private BossEnemy _activeBoss = null;

    [SerializeField] private SerializedDictionary<int, int> _maxEnemies = new();
    [SerializeField] private float _bossStatMultiplier;

    private bool _bossFightActive = false;

    private int _lastBossKillTime = 0;
    private int _bossesFought = 0;

    private void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(CheckBossSpawn());
    }

    private void OnDisable()
    {
        foreach (Enemy enemy in _activeEnemies)
        {
            enemy.OnEnemyKilled -= HandleEnemyKilled;
        }

        if (_activeBoss != null)
        {
            GameEvents.OnBossEnemyKilled -= HandleBossKilled;
        }
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            if (!_bossFightActive)
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
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator CheckBossSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (_lastBossKillTime + 120 <= GameManager.Instance.SecondsPassed && !_bossFightActive && _activeBoss == null)
            {
                _bossFightActive = true;
                SpawnBossEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        Enemy newEnemy = Instantiate(_enemyPrefab, spawnPoint.position, Quaternion.Euler(0, 0, 0));
        _activeEnemies.Add(newEnemy);
        newEnemy.OnEnemyKilled += HandleEnemyKilled;
    }

    private void SpawnBossEnemy()
    {
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        _activeBoss = Instantiate(_bossPrefab, spawnPoint.position, Quaternion.Euler(0, 0, 0));
        if (_bossesFought > 0)
        {
            float damage = _activeBoss.Info.Damage + _activeBoss.Info.Damage * (_bossesFought * _bossStatMultiplier);
            float health = _activeBoss.Info.Health + _activeBoss.Info.Health * (_bossesFought * _bossStatMultiplier);
            _activeBoss.SetStats(damage, health);
        }

        GameEvents.OnBossEnemySpawnedInvoke(_activeBoss);
        GameEvents.OnBossEnemyKilled += HandleBossKilled;
    }

    private void HandleEnemyKilled(Enemy enemy)
    {
        enemy.OnEnemyKilled -= HandleEnemyKilled;
        _activeEnemies.Remove(enemy);
    }

    private void HandleBossKilled(BossEnemy boss)
    {
        _bossesFought++;
        _lastBossKillTime = GameManager.Instance.SecondsPassed;
        GameEvents.OnBossEnemyKilled -= HandleBossKilled;
        _activeBoss = null;
        _bossFightActive = false;
    }

    public float GetSpawnYPositionForLine(IDamageable entity, int lineIndex, float defaultYPosition = 3f, float spacing = 1f)
    {
        GameObject entityGameObject = ((MonoBehaviour)entity)?.gameObject;
        List<float> enemiesOnLine = _activeEnemies.Where(enemy => enemy.gameObject != entityGameObject && enemy.CurrentLine == lineIndex).Select(enemy => enemy.transform.position.y).ToList();
        enemiesOnLine.AddRange(_activeEnemies.Where(enemy => enemy.gameObject != entityGameObject && enemy.NextLine == lineIndex).Select(enemy => enemy.TargetPosition.y));

        if (_activeBoss != null && _activeBoss.gameObject != entityGameObject)
        {
            if (_activeBoss.CurrentLine == lineIndex)
            {
                enemiesOnLine.Add(_activeBoss.transform.position.y);
            }
            else if (_activeBoss.NextLine == lineIndex)
            {
                enemiesOnLine.Add(_activeBoss.TargetPosition.y);
            }
        }

        enemiesOnLine.Sort();
        float spawnY = defaultYPosition;

        for (int i = 0; i < enemiesOnLine.Count; i++)
        {
            if (Mathf.Approximately(spawnY, enemiesOnLine[i]))
            {
                spawnY += spacing;
            }
            else
            {
                break;
            }
        }

        return spawnY;
    }

    public int GetEnemiesOnLine(int lineIndex)
    {
        int enemiesOnLine = _activeEnemies.Where(enemy => enemy.CurrentLine == lineIndex || enemy.NextLine == lineIndex).Count();
        if (_activeBoss != null && (_activeBoss.CurrentLine == lineIndex || _activeBoss.NextLine == lineIndex))
        {
            enemiesOnLine++;
        }

        return enemiesOnLine;
    }
}
