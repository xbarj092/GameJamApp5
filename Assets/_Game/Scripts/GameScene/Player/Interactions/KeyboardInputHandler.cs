using UnityEngine;

public class KeyboardInputHandler : IInteractionHandler
{
    public void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ScreenManager.Instance.ActiveGameScreen != null &&
                ScreenManager.Instance.ActiveGameScreen.GameScreenType == GameScreenType.Pause)
            {
                ScreenEvents.OnGameScreenClosedInvoke(GameScreenType.Pause);
            }
            else
            {
                ScreenEvents.OnGameScreenOpenedInvoke(GameScreenType.Pause);
            }
        }
    }
}
