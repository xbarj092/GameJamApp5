using UnityEngine;
using Dan.Main;
using TMPro;
using UnityEngine.Events;

public class ScoreSender : MonoBehaviour
{
    [SerializeField] string publicKey;
    [SerializeField] private TMP_InputField nickname;
    [SerializeField] private TextMeshProUGUI score;

    [SerializeField] private Item playerData;
    [SerializeField] private GameObject sendButton;

    private int currentScore;

    public UnityEvent OnScoreSend;

    private void Awake()
    {
        nickname.text = PlayerPrefs.GetString("Nickname", "");
        currentScore = GameManager.Instance.Score;
        
        score.text = SetTimeText(currentScore);
    }

    private string SetTimeText(int num)
    {
        return num.ToString();
    }

    public void SendScore()
    {
        sendButton.SetActive(false);

        PlayerPrefs.SetString("Nickname", nickname.text);
        string name = nickname.text != "" ? nickname.text : "Anonym";
        nickname.text = name;
        LeaderboardCreator.UploadNewEntry(publicKey, name, currentScore, OnScoreUploaded);
    }

    private void OnScoreUploaded(bool done)
    {
        OnScoreSend.Invoke();
        LeaderboardCreator.ResetPlayer();
        gameObject.SetActive(false);
    }
}
