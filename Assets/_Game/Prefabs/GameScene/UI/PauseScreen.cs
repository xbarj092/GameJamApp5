using UnityEngine;

public class PauseScreen : GameScreen
{
    // bound from inspector
    public void Resume()
    {
        CloseScreen();
    }

    // bound from inspector
    public void Restart()
    {
        SceneLoadManager.Instance.RestartGame();
    }

    // bound from inspector
    public void GoMenu()
    {
        SceneLoadManager.Instance.GoGameToMenu();
    }

    // bound from inspector
    public void Exit()
    {
        Application.Quit();
    }

    // bound from inspector
    public void ToggleSound()
    {
        // TODO - sounds
    }
}
