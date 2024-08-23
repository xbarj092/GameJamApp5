using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private SpriteRenderer _renderer;

    private float _damage;

    public void Init(float damage)
    {
        Destroy(gameObject, 10);

        _rb.simulated = true;
        _renderer.enabled = true;
    }

    private void FixedUpdate()
    {
        _rb.velocity = Vector2.up * _speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision.TryGetComponent(out Health health))
        {
            health.DealDamage(_damage);
            _rb.velocity = Vector2.zero;
            _rb.simulated = false;
            _renderer.enabled = false;
            Destroy(gameObject);
        }
    }
}
