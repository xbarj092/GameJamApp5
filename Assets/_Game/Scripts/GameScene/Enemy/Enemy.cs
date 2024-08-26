using System;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private Transform _shootTransform;
    [SerializeField] private Projectile _projectilePrefab;
    private float[] _positions = { -2.5f, 0f, 2.5f };
    private int _currentLine;
    public int CurrentLine => _currentLine;
    private int _nextLine;
    public int NextLine => _nextLine;

    [SerializeField] private EntityInfo _infoTemplate;
    public EntityInfo Info => _infoTemplate;

    private Health _health;
    private SpriteRenderer _renderer;
    private Player _player;
    private EnemySpawner _spawner;

    public event Action<Enemy> OnEnemyKilled;

    private Vector3 _spawnPosition = Vector3.zero;
    private Vector3 _targetPosition = Vector3.zero;
    public Vector3 TargetPosition => _targetPosition;
    private float _moveSpeed;
    private float _shootInterval = 0.5f;
    private float _nextShootTime;
    private bool _isMoving;

    private bool _isDescending = true;
    private float _targetYPosition = 3f;
    private float _descentSpeed = 2f;
    private bool _isDead;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _renderer = GetComponent<SpriteRenderer>();
        _player = FindObjectOfType<Player>();
        _spawner = FindObjectOfType<EnemySpawner>();
    }

    private void OnEnable()
    {
        _health.SetMaxHealth(_infoTemplate.Health);
        _moveSpeed = _infoTemplate.Speed;

        _renderer.material.SetFloat("_DamageProgress", 0);
        _health.OnDamage.AddListener(OnDamage);
        _health.OnDeath.AddListener(Death);

        _nextShootTime = Time.time + _shootInterval;
    }

    private void OnDisable()
    {
        _health.OnDamage.RemoveListener(OnDamage);
        _health.OnDeath.RemoveListener(Death);
    }

    private void Update()
    {
        if (_isDead)
        {
            CancelInvoke();
            return;
        }

        if (_isDescending)
        {
            Descend();
            return;
        }
        
        if (_targetPosition == Vector3.zero)
        {
            SetNextTargetPosition();
        }

        Move();

        if (_isMoving)
        {
            return;
        }

        if (Time.time >= _nextShootTime && _currentLine == _player.CurrentLine)
        {
            Shoot();
            _nextShootTime = Time.time + UnityEngine.Random.Range(0.5f, 1.5f);
        }
    }

    private void Descend()
    {
        if (_spawnPosition == Vector3.zero)
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                if (transform.position.x == _positions[i])
                {
                    _nextLine = i;
                }
            }
        }

        _spawnPosition = new(transform.position.x, _spawner.GetSpawnYPositionForLine(this, _nextLine), transform.position.z);
        _targetPosition = _spawnPosition;
        transform.position = Vector3.Lerp(transform.position, _spawnPosition, _descentSpeed * Time.deltaTime);
        if (Mathf.Abs(transform.position.y - _spawnPosition.y) < 0.01f)
        {
            _isDescending = false;
            _currentLine = _nextLine;
            _nextLine = -1;
            _targetPosition = Vector3.zero;
        }
    }

    private void SetNextTargetPosition()
    {
        do
        {
            _nextLine = UnityEngine.Random.Range(0, _positions.Length);
        }
        while (_spawner.GetEnemiesOnLine(_nextLine) > 2);
        float yPos = 3;
        if (_nextLine == _currentLine)
        {
            yPos = transform.position.y;
        }
        else
        {
            yPos = _spawner.GetSpawnYPositionForLine(this, _nextLine);
        }
        float nextPosition = _positions[_nextLine];
        _targetPosition = new Vector3(nextPosition, yPos, transform.position.z);
        _isMoving = true;
        _currentLine = -1;
    }

    private void Move()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                _isMoving = false;
                _currentLine = _nextLine;
                _nextLine = -1;
                Invoke(nameof(SetNextTargetPosition), UnityEngine.Random.Range(1f, 3f));
            }
        }
    }

    public void Shoot()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _shootTransform.position, Quaternion.identity);
        projectile.Init(transform, _infoTemplate.Damage);
        AudioManager.Instance.Play(SoundType.EnemyShoot);
    }

    public void Damage(float damage)
    {
        _health.DealDamage(damage);
        AudioManager.Instance.Play(SoundType.EnemyHit);
    }

    public void OnDamage(float damage)
    {
        float healthRemaining = Mathf.Clamp(_health.CurrHealth / _health.MaxHealth, 0f, 1f);
        float progress = Mathf.Lerp(0f, 0.4f, 1f - healthRemaining);
        _renderer.material.SetFloat("_Progress", progress);
    }

    public void Death()
    {
        _isDead = true;
        GameManager.Instance.Score += 10;
        OnEnemyKilled?.Invoke(this);
        StartCoroutine(LerpProgress());

        Destroy(gameObject, 1);
    }

    private IEnumerator LerpProgress()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = Mathf.Lerp(0.4f, 1, elapsedTime / duration);
            _renderer.material.SetFloat("_Progress", progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _renderer.material.SetFloat("_Progress", 1);
    }

    public bool IsDead()
    {
        return _isDead;
    }
}
