using UnityEngine;

public class MenuEnemy : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Vector2 _targetPosition;

    public void SetDestination(Vector2 destination)
    {
        _targetPosition = destination;

        Vector2 direction = (_targetPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        float multiplier = Random.Range(0.8f, 1.21f);
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * multiplier * Time.deltaTime);
        if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
        {
            Destroy(gameObject);
        }
    }
}
