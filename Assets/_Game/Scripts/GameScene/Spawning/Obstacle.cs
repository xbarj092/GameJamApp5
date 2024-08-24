using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 2f;

    private void Update()
    {
        transform.Translate(_moveSpeed * Time.deltaTime * Vector3.down);
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.Death();
        }
    }
}
