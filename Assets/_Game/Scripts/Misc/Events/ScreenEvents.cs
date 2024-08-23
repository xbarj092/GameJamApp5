using System;

public static class ScreenEvents
{
    public static event Action<GameScreenType> OnGameScreenOpened;
    public static void OnGameScreenOpenedInvoke(GameScreenType gameScreenType)
    {
        OnGameScreenOpened?.Invoke(gameScreenType);
    }

    public static event Action<GameScreenType> OnGameScreenClosed;
    public static void OnGameScreenClosedInvoke(GameScreenType gameScreenType)
    {
        OnGameScreenClosed?.Invoke(gameScreenType);
    }
}
