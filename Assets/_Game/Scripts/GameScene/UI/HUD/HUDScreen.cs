using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDScreen : GameScreen
{
    [SerializeField] private TMP_Text _bulletAmount;
    [SerializeField] private TMP_Text _healthAmount;
    [SerializeField] private Image _shieldEnabled;

    private Player _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        ChangeHealth(_player.Info.Health);
        ChangeBullets(_player.Ammo);
        ChangeShield(false);
    }

    private void OnEnable()
    {
        _player.OnHealthChanged += ChangeHealth;
        _player.OnShieldStateChanged += ChangeShield;
        _player.OnBulletsChanged += ChangeBullets;
    }

    private void OnDisable()
    {
        _player.OnHealthChanged -= ChangeHealth;
        _player.OnShieldStateChanged -= ChangeShield;
        _player.OnBulletsChanged -= ChangeBullets;
    }

    private void ChangeHealth(float health)
    {
        _healthAmount.text = health.ToString();
    }

    private void ChangeShield(bool enabled)
    {
        _shieldEnabled.enabled = enabled;
    }

    private void ChangeBullets(int bullets)
    {
        _bulletAmount.text = bullets.ToString();
    }
}
