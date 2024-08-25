public class GameManager : MonoSingleton<GameManager>
{
    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            GameEvents.OnScoreChangedInvoke();
        }
    }

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

    public void ResetStats()
    {
        CancelInvoke();
        Score = 0;
        _secondsPassed = 0;
        InvokeRepeating(nameof(IncrementSeconds), 0, 1);
    }
}
