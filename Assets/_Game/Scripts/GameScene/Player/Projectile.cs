using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private SpriteRenderer _renderer;

    private Transform _holder;
    [SerializeField] private bool _enemy; 
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
            _rb.velocity = _speed * 2.5f * GameManager.Instance.MovementSpeed() * (_enemy ? -_holder.up : _holder.up);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_holder != null && _holder.TryGetComponent(out IDamageable holder) && !holder.IsDead() && !_holder.CompareTag(collision.transform.tag) && collision.TryGetComponent(out IDamageable damageable) && !damageable.IsDead())
        {
            damageable.Damage(_damage);
            _rb.velocity = Vector2.zero;
            _rb.simulated = false;
            _renderer.enabled = false;
            Destroy(gameObject);
        }
    }
}
