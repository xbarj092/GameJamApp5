using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private Transform _shootTransform;
    [SerializeField] private Projectile _projectilePrefab;
    private float[] _positions = { -2.5f, 0f, 2.5f };
    [SerializeField] private EntityInfo _infoTemplate;
    public EntityInfo Info => _infoTemplate;

    private Health _health;
    private SpriteRenderer _renderer;

    public event Action<Enemy> OnEnemyKilled;

    private Vector3 _targetPosition;
    private float _moveSpeed;
    private float _shootInterval = 0.5f;
    private float _nextShootTime;
    private bool _isMoving;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _health.SetMaxHealth(_infoTemplate.Health);
        _moveSpeed = _infoTemplate.Speed;

        _renderer.material.SetFloat("_DamageProgress", 0);
        _health.OnDamage.AddListener(OnDamage);
        _health.OnDeath.AddListener(Death);

        SetNextTargetPosition();
        _nextShootTime = Time.time + _shootInterval;
    }

    private void OnDisable()
    {
        _health.OnDamage.RemoveListener(OnDamage);
        _health.OnDeath.RemoveListener(Death);
    }

    private void Update()
    {
        Move();

        if (_isMoving)
        {
            return;
        }

        if (Time.time >= _nextShootTime)
        {
            Shoot();
            _nextShootTime = Time.time + UnityEngine.Random.Range(0.5f, 1.5f);
        }
    }

    private void SetNextTargetPosition()
    {
        float nextPosition = _positions[UnityEngine.Random.Range(0, _positions.Length)];
        _targetPosition = new Vector3(nextPosition, transform.position.y, transform.position.z);
        _isMoving = true;
    }

    private void Move()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
            {
                _isMoving = false;
                Invoke(nameof(SetNextTargetPosition), UnityEngine.Random.Range(1f, 3f));
            }
        }
    }

    public void Shoot()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _shootTransform.position, Quaternion.identity);
        projectile.Init(transform, _infoTemplate.Damage);
    }

    public void Damage(float damage)
    {
        _health.DealDamage(damage);
    }

    public void OnDamage(float damage)
    {
        _renderer.material.SetFloat("_DamageProgress", damage / _health.MaxHealth);
    }

    public void Death()
    {
        OnEnemyKilled?.Invoke(this);
        Destroy(gameObject);
    }
}
