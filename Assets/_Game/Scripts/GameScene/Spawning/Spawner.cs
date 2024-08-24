using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float[] _positions = { -2.5f, 0f, 2.5f };
    [SerializeField] private float _spawnInterval = 1f;

    [Header("Obstacles")]
    [SerializeField] private GameObject _obstaclePrefab;
    [SerializeField][Range(0f, 1f)] private float _spawnChance = 1f;

    [Header("Pickupables")]
    [SerializeField] private PickupableItem _shieldPickupPrefab;
    [SerializeField] private PickupableItem _hpPickupPrefab;
    [SerializeField] private PickupableItem _bulletPickupPrefab;

    [SerializeField][Range(0f, 1f)] private float _pickupSpawnChance = 0.6f;

    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval);
            if (Random.value <= _spawnChance)
            {
                if (Random.value <= _pickupSpawnChance)
                {
                    SpawnRandomPickup();
                }
                else
                {
                    SpawnObstacle();
                }
            }
        }
    }

    private void SpawnObstacle()
    {
        float spawnPositionX = _positions[Random.Range(0, _positions.Length)];
        Vector3 spawnPosition = new(spawnPositionX, transform.position.y, transform.position.z);
        GameObject obstacle = Instantiate(_obstaclePrefab, spawnPosition, Quaternion.identity);
        obstacle.transform.localScale = new(2, Mathf.CeilToInt(Random.Range(1, 5)), 1);
    }

    private void SpawnRandomPickup()
    {
        float hpWeight = 1f;
        float ammoWeight = 1f;
        float shieldWeight = 1f;

        if (_player.CurrentHealth() <= 50)
        {
            hpWeight = 2f;
        }
        if (_player.Ammo <= 5)
        {
            ammoWeight = 4f;
        }
        if (!_player.Shield)
        {
            shieldWeight = 1.5f;
        }

        float totalWeight = hpWeight + ammoWeight + shieldWeight;
        float randomValue = Random.Range(0f, totalWeight);

        if (randomValue < hpWeight)
        {
            SpawnPickup(_hpPickupPrefab, new HpPickup(10));
        }
        else if (randomValue < hpWeight + ammoWeight)
        {
            SpawnPickup(_bulletPickupPrefab, new BulletPickup(5));
        }
        else
        {
            SpawnPickup(_shieldPickupPrefab, new ShieldPickup());
        }
    }

    private void SpawnPickup(PickupableItem prefab, IPickupable pickupStrategy)
    {
        float spawnPositionX = _positions[Random.Range(0, _positions.Length)];
        Vector3 spawnPosition = new(spawnPositionX, transform.position.y, transform.position.z);
        PickupableItem pickup = Instantiate(prefab, spawnPosition, Quaternion.identity);
        pickup.SetPickupable(pickupStrategy);
    }
}
