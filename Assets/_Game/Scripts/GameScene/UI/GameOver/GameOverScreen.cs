using System;
using TMPro;
using UnityEngine;

public class GameOverScreen : GameScreen
{
    [SerializeField] private TMP_Text _timeText;

    private const string TIME_TEXT_PREFIX = "You survived for ";

    /*private void Awake()
    {
        SetTimeText();
    }

    private void SetTimeText()
    {
        TimeSpan time = TimeSpan.FromSeconds(LocalDataStorage.Instance.PlayerData.PlayerStats.TimeAlive);

        string timeString = "";

        if (time.Hours > 0)
        {
            timeString += $"{time.Hours}h ";
        }
        if (time.Minutes > 0)
        {
            timeString += $"{time.Minutes}m ";
        }
        if (time.Seconds > 0 || timeString == "")
        {
            timeString += $"{time.Seconds}s";
        }

        _timeText.text = TIME_TEXT_PREFIX + timeString.Trim();
    }*/

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
