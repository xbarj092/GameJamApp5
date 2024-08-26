using TMPro;
using UnityEngine;

public class GameOverScreen : GameScreen
{
    [SerializeField] private TMP_Text _timeText;

    private void Awake()
    {
        AudioManager.Instance.Stop(SoundType.GameAmbience);
        AudioManager.Instance.Play(SoundType.MenuAmbience);
    }

    // bound from inspector
    public void PlayAgain()
    {
        SceneLoadManager.Instance.RestartGame();
    }

    // bound from inspector
    public void MainMenu()
    {
        SceneLoadManager.Instance.GoGameToMenu();
    }
}
