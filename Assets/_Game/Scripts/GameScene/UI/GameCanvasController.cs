using UnityEngine;

public class GameCanvasController : BaseCanvasController
{
    [SerializeField] private GameOverScreen _gameOverScreenPrefab;
    [SerializeField] private PauseScreen _pauseScreenPrefab;

    protected override GameScreen GetRelevantScreen(GameScreenType gameScreenType)
    {
        return gameScreenType switch
        {
            GameScreenType.GameOver => Instantiate(_gameOverScreenPrefab, transform),
            GameScreenType.Pause => Instantiate(_pauseScreenPrefab, transform),
            _ => base.GetRelevantScreen(gameScreenType),
        };
    }
}
