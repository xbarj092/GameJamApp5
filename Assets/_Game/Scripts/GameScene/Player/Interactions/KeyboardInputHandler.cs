using UnityEngine;

public class KeyboardInputHandler : IInteractionHandler
{
    public void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Player player = GameObject.FindObjectOfType<Player>();
            if (ScreenManager.Instance.ActiveGameScreen != null &&
                ScreenManager.Instance.ActiveGameScreen.GameScreenType == GameScreenType.GameOver ||
                (player != null && player.IsDead()))
            {
                return;
            }

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
