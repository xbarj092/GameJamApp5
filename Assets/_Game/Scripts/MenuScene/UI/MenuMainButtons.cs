using UnityEngine;

public class MenuMainButtons : GameScreen
{
    public void PlayTheGame()
    {
        SceneLoadManager.Instance.GoMenuToGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
