public class GameManager : MonoSingleton<GameManager>
{
    private int _secondsPassed = 0;
    public int SecondsPassed => _secondsPassed;

    private int _maxSpeed = 4;

    public float MovementSpeed()
    {
        float movementSpeed = 2 + _secondsPassed * 0.01f;
        if (movementSpeed > _maxSpeed)
        {
            return _maxSpeed;
        }

        return movementSpeed;
    }

    private void Start()
    {
        InvokeRepeating(nameof(IncrementSeconds), 0, 1);
    }

    private void IncrementSeconds()
    {
        _secondsPassed++;
    }
}
