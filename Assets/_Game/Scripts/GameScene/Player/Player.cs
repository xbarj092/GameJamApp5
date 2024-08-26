using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{
    [SerializeField] private TMP_Text _outOfAmmoText;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _shieldVisual;

    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _shootTransform;
    [SerializeField] private EntityInfo _infoTemplate;
    public EntityInfo Info => _infoTemplate;

    private Health _health;

    private bool _shield = false;
    public bool Shield
    {
        get => _shield;
        set
        {
            if (value != _shield)
            {
                _shield = value;
                OnShieldStateChanged?.Invoke(value);
            }
        }
    }

    private int _ammo = 10;
    public int Ammo => _ammo;

    private float[] _positions = { -2.5f, 0f, 2.5f };
    public int CurrentLine = 1;
    public float _moveSpeed;

    private float _keyPressDelay = 0.05f;
    private float _timeSinceLastKeyPress = 0f;
    private bool _isWaitingForInput = false;
    private KeyCode _lastKeyPressed;
    private bool _invincible;

    public float BonusDamage;

    private bool _isMoving = false;

    public PickupableItem CurrentPickupable;

    public event Action<float> OnHealthChanged;
    public event Action<bool> OnShieldStateChanged;
    public event Action<int> OnBulletsChanged;

    private void Awake()
    {
        _health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        _health.SetMaxHealth(_infoTemplate.Health);
        _moveSpeed = _infoTemplate.Speed;

        _health.OnDamage.AddListener(OnDamage);
        _health.OnHeal.AddListener(OnHeal);
        _health.OnDeath.AddListener(Death);
    }

    private void OnDisable()
    {
        _health.OnDamage.RemoveListener(OnDamage);
        _health.OnHeal.RemoveListener(OnHeal);
        _health.OnDeath.RemoveListener(Death);
    }

    public float CurrentHealth()
    {
        return _health.CurrHealth;
    }

    private void OnHeal(float damage)
    {
        OnHealthChanged?.Invoke(_health.MaxHealth - damage);
    }

    private void OnDamage(float damage)
    {
        if (_shield)
        {
            Shield = false;
        }
        else
        {
            OnHealthChanged?.Invoke(_health.MaxHealth - damage);
        }

        float healthRemaining = Mathf.Clamp(_health.CurrHealth / _health.MaxHealth, 0f, 1f);
        float progress = Mathf.Lerp(0f, 0.4f, 1f - healthRemaining);
        _renderer.material.SetFloat("_Progress", progress);
    }

    public void Death()
    {
        if (_shield)
        {
            Shield = false;
            _shieldVisual.SetActive(false);
        }

        Time.timeScale = 0;
        StartCoroutine(LerpProgress());

        Destroy(gameObject, 1.5f);
    }

    private IEnumerator LerpProgress()
    {
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float progress = Mathf.Lerp(0.4f, 1, elapsedTime / duration);
            _renderer.material.SetFloat("_Progress", progress);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        _renderer.material.SetFloat("_Progress", 1);
        ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.GameOver);
    }

    private void Update()
    {
        if (_isWaitingForInput)
        {
            _timeSinceLastKeyPress += Time.deltaTime;

            if (_timeSinceLastKeyPress >= _keyPressDelay)
            {
                ExecuteMove();
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            HandleKeyPress(KeyCode.A);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            HandleKeyPress(KeyCode.D);
        }
    }

    private void HandleKeyPress(KeyCode key)
    {
        if (_isWaitingForInput)
        {
            if ((key == KeyCode.A && _lastKeyPressed == KeyCode.D) || (key == KeyCode.D && _lastKeyPressed == KeyCode.A))
            {
                _isWaitingForInput = false;
                _timeSinceLastKeyPress = 0f;

                if (!_isMoving && _ammo > 0)
                {
                    Shoot();
                    _ammo--;
                    OnBulletsChanged?.Invoke(_ammo);
                }
                else if (_ammo <= 0)
                {
                    StartCoroutine(ShowText("Out of ammo!"));
                }

                return;
            }
        }

        _isWaitingForInput = true;
        _timeSinceLastKeyPress = 0f;
        _lastKeyPressed = key;
    }

    private IEnumerator ShowText(string message)
    {
        _outOfAmmoText.gameObject.SetActive(true);
        _outOfAmmoText.color = new Color(_outOfAmmoText.color.r, _outOfAmmoText.color.g, _outOfAmmoText.color.b, 1);
        _outOfAmmoText.text = message;
        yield return new WaitForSeconds(1f);
        while (_outOfAmmoText.color.a > 0)
        {
            _outOfAmmoText.color = new Color(_outOfAmmoText.color.r, _outOfAmmoText.color.g, _outOfAmmoText.color.b, _outOfAmmoText.color.a - 0.005f);
            yield return null;
        }
        _outOfAmmoText.gameObject.SetActive(false);
    }

    private void Shoot()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _shootTransform.position, Quaternion.identity);
        projectile.Init(transform, _infoTemplate.Damage + BonusDamage);
        AudioManager.Instance.Play(SoundType.PlayerShoot);
    }

    private void ExecuteMove()
    {
        _isWaitingForInput = false;

        if (_lastKeyPressed == KeyCode.A)
        {
            MoveLeft();
        }
        else if (_lastKeyPressed == KeyCode.D)
        {
            MoveRight();
        }
    }

    private void MoveLeft()
    {
        if (CurrentLine > 0)
        {
            MoveToLine(CurrentLine - 1);
        }
    }

    private void MoveRight()
    {
        if (CurrentLine < _positions.Length - 1)
        {
            MoveToLine(CurrentLine + 1);
        }
    }

    public void MoveToLine(int lineIndex)
    {
        if (lineIndex >= 0 && lineIndex < _positions.Length)
        {
            CurrentLine = lineIndex;
            _isMoving = true;
        }
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = new(_positions[CurrentLine], transform.position.y, transform.position.z);
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);
        }
        else
        {
            _isMoving = false;
        }
    }

    public void Damage(float damage)
    {
        if (_invincible)
        {
            return;
        }

        StartCoroutine(Invincibility());

        if (_shield)
        {
            Shield = false;
            _shieldVisual.SetActive(false);
            _renderer.color = new(_renderer.color.r, _renderer.color.g, _renderer.color.b, 255f / 255f);
            AudioManager.Instance.Play(SoundType.ShieldBreak);
            if (CurrentPickupable != null && CurrentPickupable.PickupableStrategy is ShieldPickup)
            {
                CurrentPickupable.Collect(this);
            }
        }
        else
        {
            AudioManager.Instance.Play(SoundType.PlayerHitBullet);
            _health.DealDamage(damage);

            if (CurrentPickupable != null && CurrentPickupable.PickupableStrategy is HpPickup)
            {
                CurrentPickupable.Collect(this);
            }
        }
    }

    private IEnumerator Invincibility()
    {
        _invincible = true;
        Color baseColor = _renderer.color;
        Color disabledColor = new(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0);
        for (int i = 0; i < 5; i++)
        {
            _renderer.color = disabledColor;
            yield return new WaitForSeconds(0.1f);
            _renderer.color = baseColor;
            yield return new WaitForSeconds(0.1f);
        }

        _invincible = false;
    }

    public void IncreaseShield()
    {
        Shield = true;
        _shieldVisual.SetActive(true);
        _renderer.color = new(_renderer.color.r, _renderer.color.g, _renderer.color.b, 230f / 255f);
    }

    public void RestoreHealth(int amount)
    {
        _health.Heal(amount);
    }

    public void AddAmmo(int amount)
    {
        _ammo += amount;
        OnBulletsChanged?.Invoke(_ammo);
    }

    public bool IsDead()
    {
        return false;
    }
}
