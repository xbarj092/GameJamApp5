using UnityEngine;

public class GameScreen : MonoBehaviour
{
    public GameScreenType GameScreenType;

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public virtual void Close()
    {
        Destroy(gameObject);
    }

    public void CloseScreen()
    {
        ScreenEvents.OnGameScreenClosedInvoke(GameScreenType);
    }
}
