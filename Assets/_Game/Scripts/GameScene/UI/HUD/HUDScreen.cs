using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDScreen : GameScreen
{
    [SerializeField] private TMP_Text _bulletAmount;
    [SerializeField] private TMP_Text _healthAmount;
    [SerializeField] private Image _shieldEnabled;
    [SerializeField] private TMP_Text _scoreText;

    private Player _player;

    private void Awake()
    {
        _scoreText.text = 0.ToString();
        _player = FindObjectOfType<Player>();
        ChangeHealth(_player.Info.Health);
        ChangeBullets(_player.Ammo);
        ChangeShield(false);
    }

    private void OnEnable()
    {
        GameEvents.OnScoreChanged += OnScoreChanged;
        _player.OnHealthChanged += ChangeHealth;
        _player.OnShieldStateChanged += ChangeShield;
        _player.OnBulletsChanged += ChangeBullets;
    }

    private void OnDisable()
    {
        GameEvents.OnScoreChanged -= OnScoreChanged;
        _player.OnHealthChanged -= ChangeHealth;
        _player.OnShieldStateChanged -= ChangeShield;
        _player.OnBulletsChanged -= ChangeBullets;
    }

    private void OnScoreChanged()
    {
        _scoreText.text = GameManager.Instance.Score.ToString();
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
