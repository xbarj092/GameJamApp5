using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private Transform _shootTransform;

    private float[] _positions = { -2.5f, 0f, 2.5f };
    private int _currentLine = 1;
    public float _moveSpeed = 20f;

    private float _keyPressDelay = 0.1f;
    private float _timeSinceLastKeyPress = 0f;
    private bool _isWaitingForInput = false;
    private KeyCode _lastKeyPressed;

    public float BonusDamage;
    public float BaseDamage = 10;

    private bool _isMoving = false;

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

                if (!_isMoving)
                {
                    Shoot();
                }
                else
                {
                    Debug.Log("Cannot shoot while moving!");
                }
                return;
            }
        }

        _isWaitingForInput = true;
        _timeSinceLastKeyPress = 0f;
        _lastKeyPressed = key;
    }

    private void Shoot()
    {
        Projectile projectile = Instantiate(_projectilePrefab, _shootTransform.position, Quaternion.identity);
        projectile.Init(BaseDamage + BonusDamage);
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
        if (_currentLine > 0)
        {
            _currentLine--;
            _isMoving = true;
        }
    }

    private void MoveRight()
    {
        if (_currentLine < _positions.Length - 1)
        {
            _currentLine++;
            _isMoving = true;
        }
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = new(_positions[_currentLine], transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            _isMoving = false;
        }
    }
}
