using UnityEngine;

public class MenuCanvasController : BaseCanvasController
{
    private void Awake() {
        Time.timeScale = 1;
    }

    [SerializeField] private MenuMainButtons _menuMainButtonsPrefab;

    protected override GameScreen GetRelevantScreen(GameScreenType gameScreenType)
    {
        return gameScreenType switch
        {
            GameScreenType.MenuMain => Instantiate(_menuMainButtonsPrefab, transform),
            _ => base.GetRelevantScreen(gameScreenType),
        };
    }
}
