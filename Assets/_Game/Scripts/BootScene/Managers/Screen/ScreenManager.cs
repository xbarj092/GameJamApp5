using UnityEngine;

public class ScreenManager : MonoSingleton<ScreenManager>
{
    [field: SerializeField] public GameScreen ActiveGameScreen { get; private set; }

    public void SetActiveGameScreen(GameScreen screen)
    {
        ActiveGameScreen = screen;
    }
}
