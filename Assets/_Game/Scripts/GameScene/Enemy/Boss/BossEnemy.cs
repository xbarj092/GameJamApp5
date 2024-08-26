using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private Transform _shootTransform;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Projectile _specialProjectilePrefab;
    private float[] _positions = { -2.5f, 0f, 2.5f };
    private int _currentLine;
    public int CurrentLine => _currentLine;
    private int _nextLine;
    public int NextLine => _nextLine;

    [SerializeField] private EntityInfo _infoTemplate;
    public EntityInfo Info => _infoTemplate;

    private float _damage;

    private Health _health;
    private SpriteRenderer _renderer;
    private Player _player;
    private EnemySpawner _spawner;

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

    private bool _isSpecialAttackActive = false;
    private HashSet<int> _visitedLines = new();
    private Coroutine _specialAttackCoroutine;

    [SerializeField] private float _specialAttackCooldown = 10f;
    private float _lastSpecialAttackTime;
    private bool _isDead;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _renderer = GetComponent<SpriteRenderer>();
        _player = FindObjectOfType<Player>();
        _spawner = FindObjectOfType<EnemySpawner>();
    }

    private void Start()
    {
        for (int i = 0; i < _positions.Length; i++)
        {
            if (transform.position.x == _positions[i])
            {
                _nextLine = i;
            }
        }
    }

    private void OnEnable()
    {
        _health.SetMaxHealth(_infoTemplate.Health);
        _moveSpeed = _infoTemplate.Speed;
        _damage = _infoTemplate.Damage;

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
            if (_specialAttackCoroutine != null)
            {
                StopCoroutine(_specialAttackCoroutine);
                _specialAttackCoroutine = null;
            }
            return;
        }

        if (_isDescending)
        {
            Descend();
            return;
        }

        if (_isSpecialAttackActive && _specialAttackCoroutine == null)
        {
            _specialAttackCoroutine = StartCoroutine(ExecuteSpecialAttack());
            return;
        }

        if (Time.time >= _nextShootTime)
        {
            if (ShouldStartSpecialAttack())
            {
                StartSpecialAttack();
            }
            else if (!_isMoving && !_isSpecialAttackActive && _currentLine == _player.CurrentLine)
            {
                Shoot();
                _nextShootTime = Time.time + Random.Range(0.5f, 1.5f);
            }
        }

        if (_targetPosition == Vector3.zero)
        {
            SetNextTargetPosition();
        }

        Move();
    }

    public void SetStats(float damage, float hp)
    {
        _damage = damage;
        _health.SetMaxHealth(hp);
    }

    private bool ShouldStartSpecialAttack()
    {
        return Time.time - _lastSpecialAttackTime >= _specialAttackCooldown && Random.Range(0, 3) == 0;
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
            _nextLine = Random.Range(0, _positions.Length);
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
                Invoke(nameof(SetNextTargetPosition), Random.Range(1f, 3f));
            }
        }
    }

    private void Shoot()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _shootTransform.position, Quaternion.identity);
        projectile.Init(transform, _infoTemplate.Damage);
        AudioManager.Instance.Play(SoundType.EnemyShoot);
    }

    public void StartSpecialAttack()
    {
        _isSpecialAttackActive = true;
        _visitedLines.Clear();
        _lastSpecialAttackTime = Time.time;
        _specialAttackCoroutine = StartCoroutine(ExecuteSpecialAttack());
    }

    private IEnumerator ExecuteSpecialAttack()
    {
        CancelInvoke();
        while (_visitedLines.Count < _positions.Length)
        {
            do
            {
                _nextLine = Random.Range(0, _positions.Length);
            } while (_visitedLines.Contains(_nextLine));

            _visitedLines.Add(_nextLine);
            float yPos = 3;
            if (_nextLine == _currentLine)
            {
                yPos = transform.position.y;
            }
            else
            {
                yPos = _spawner.GetSpawnYPositionForLine(this, _nextLine);
            }
            _targetPosition = new Vector3(_positions[_nextLine], yPos, transform.position.z);
            _isMoving = true;
            _currentLine = -1;

            while (_isMoving)
            {
                if (_isDead)
                {
                    CancelInvoke();
                    yield break;
                }

                yield return null;
            }

            CancelInvoke();
            ShootSpecial();
            yield return new WaitForSeconds(0.5f);
        }

        _isSpecialAttackActive = false;
        _specialAttackCoroutine = null;
        _nextShootTime = Time.time + 2f;
        Invoke(nameof(SetNextTargetPosition), Random.Range(1f, 3f));
    }

    private void ShootSpecial()
    {
        Projectile specialProjectile = Instantiate(_specialProjectilePrefab, _shootTransform.position, Quaternion.identity);
        specialProjectile.Init(transform, _infoTemplate.Damage * 2);
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
        GameManager.Instance.Score += 100;
        GameEvents.OnBossEnemyKilledInvoke(this);
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
