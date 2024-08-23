using System.Collections.Generic;
using UnityEngine;

public class BaseCanvasController : MonoBehaviour
{
    protected Dictionary<GameScreenType, GameScreen> _instantiatedScreens = new();

    private void OnEnable()
    {
        ScreenEvents.OnGameScreenOpened += ShowGameScreen;
        ScreenEvents.OnGameScreenClosed += CloseGameScreen;
    }

    private void OnDisable()
    {
        ScreenEvents.OnGameScreenOpened -= ShowGameScreen;
        ScreenEvents.OnGameScreenClosed -= CloseGameScreen;
    }

    private void ShowGameScreen(GameScreenType gameScreenType)
    {
        if ((_instantiatedScreens.ContainsKey(gameScreenType) && _instantiatedScreens[gameScreenType] == null) ||
            !_instantiatedScreens.ContainsKey(gameScreenType))
        {
            InstantiateScreen(gameScreenType);
        }

        _instantiatedScreens[gameScreenType].Open();
    }

    private void CloseGameScreen(GameScreenType gameScreenType)
    {
        if (_instantiatedScreens.ContainsKey(gameScreenType))
        {
            GameScreen screenInstance = GetActiveGameScreen(gameScreenType);
            InstantiateScreen(screenInstance);
            _instantiatedScreens[gameScreenType].Close();
            _instantiatedScreens.Remove(gameScreenType);
        }
    }

    private void InstantiateScreen(GameScreenType gameScreenType)
    {
        GameScreen screenInstance = GetRelevantScreen(gameScreenType);
        InstantiateScreen(screenInstance);
    }

    private void InstantiateScreen(GameScreen screenInstance)
    {
        if (screenInstance != null)
        {
            _instantiatedScreens[screenInstance.GameScreenType] = screenInstance;
            ScreenManager.Instance.SetActiveGameScreen(screenInstance);
        }
    }

    protected virtual GameScreen GetRelevantScreen(GameScreenType gameScreenType)
    {
        return gameScreenType switch
        {
            _ => null
        };
    }

    protected virtual GameScreen GetActiveGameScreen(GameScreenType gameScreenType)
    {
        return gameScreenType switch
        {
            _ => null
        };
    }
}
