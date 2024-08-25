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
    private int _nextLine;

    [SerializeField] private EntityInfo _infoTemplate;
    public EntityInfo Info => _infoTemplate;

    private float _damage;

    private Health _health;
    private SpriteRenderer _renderer;
    private Player _player;

    private Vector3 _targetPosition = Vector3.zero;
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

    private void Awake()
    {
        _health = GetComponent<Health>();
        _renderer = GetComponent<SpriteRenderer>();
        _player = FindObjectOfType<Player>();
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
            else
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
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = new(currentPosition.x, _targetYPosition, currentPosition.z);
        transform.position = Vector3.Lerp(currentPosition, targetPosition, _descentSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.y - _targetYPosition) < 0.01f)
        {
            _isDescending = false;
        }
    }

    private void SetNextTargetPosition()
    {
        _nextLine = Random.Range(0, _positions.Length);
        _targetPosition = new Vector3(_positions[_nextLine], transform.position.y, transform.position.z);
        _isMoving = true;
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
        while (_visitedLines.Count < _positions.Length)
        {
            do
            {
                _nextLine = Random.Range(0, _positions.Length);
            } while (_visitedLines.Contains(_nextLine));

            _visitedLines.Add(_nextLine);
            _targetPosition = new Vector3(_positions[_nextLine], transform.position.y, transform.position.z);
            _isMoving = true;

            while (_isMoving)
            {
                yield return null;
            }

            ShootSpecial();
            yield return new WaitForSeconds(0.5f);
        }

        _isSpecialAttackActive = false;
        _specialAttackCoroutine = null;
        _nextShootTime = Time.time + 2f;
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
        _renderer.material.SetFloat("_DamageProgress", damage / _health.MaxHealth);
    }

    public void Death()
    {
        GameManager.Instance.Score += 100;
        GameEvents.OnBossEnemyKilledInvoke(this);
        Destroy(gameObject);
    }
}
