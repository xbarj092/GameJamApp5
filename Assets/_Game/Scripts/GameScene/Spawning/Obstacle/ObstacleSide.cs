using UnityEngine;

public class ObstacleSide : MonoBehaviour
{
    [SerializeField] private bool _left;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Player player))
        {
            int lineIndex = player.CurrentLine + (_left ? -1 : 1);
            player.Damage(20);
            player.MoveToLine(lineIndex);
            AudioManager.Instance.Play(SoundType.PlayerHitObstacle);
        }
    }
}
