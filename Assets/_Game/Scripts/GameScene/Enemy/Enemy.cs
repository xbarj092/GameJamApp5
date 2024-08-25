using System;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private Transform _shootTransform;
    [SerializeField] private Projectile _projectilePrefab;
    private float[] _positions = { -2.5f, 0f, 2.5f };
    private int _currentLine;
    private int _nextLine;

    [SerializeField] private EntityInfo _infoTemplate;
    public EntityInfo Info => _infoTemplate;

    private Health _health;
    private SpriteRenderer _renderer;
    private Player _player;

    public event Action<Enemy> OnEnemyKilled;

    private Vector3 _targetPosition = Vector3.zero;
    private float _moveSpeed;
    private float _shootInterval = 0.5f;
    private float _nextShootTime;
    private bool _isMoving;

    private bool _isDescending = true;
    private float _targetYPosition = 3f;
    private float _descentSpeed = 2f;

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
        _nextLine = UnityEngine.Random.Range(0, _positions.Length);
        float nextPosition = _positions[_nextLine];
        _targetPosition = new Vector3(nextPosition, transform.position.y, transform.position.z);
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
        _renderer.material.SetFloat("_DamageProgress", damage / _health.MaxHealth);
    }

    public void Death()
    {
        GameManager.Instance.Score += 10;
        OnEnemyKilled?.Invoke(this);
        Destroy(gameObject);
    }
}
