using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void Update()
    {
        transform.Translate(GameManager.Instance.MovementSpeed() * Time.deltaTime * Vector3.down);
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
            AudioManager.Instance.Play(SoundType.PlayerHitObstacle);
        }
    }
}
