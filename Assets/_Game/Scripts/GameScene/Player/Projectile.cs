using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private SpriteRenderer _renderer;

    private Transform _holder;
    private float _damage;

    public void Init(Transform holder, float damage)
    {
        Destroy(gameObject, 10);

        _holder = holder;
        _damage = damage;
        _rb.simulated = true;
        _renderer.enabled = true;
    }

    private void FixedUpdate()
    {
        if (_holder != null)
        {
            _rb.velocity = 2.5f * GameManager.Instance.MovementSpeed() * _holder.up;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_holder != null && collision.transform != _holder && collision.TryGetComponent(out IDamageable damageable))
        {
            damageable.Damage(_damage);
            _rb.velocity = Vector2.zero;
            _rb.simulated = false;
            _renderer.enabled = false;
            Destroy(gameObject);
        }
    }
}
