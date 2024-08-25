using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dan.Main;
using Dan.Models;
using UnityEngine.Events;
using TMPro;
using System;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] string publicKey;
    [SerializeField] private List<Item> items;
    public UnityEvent OnHighScoresGet;

    private void Awake() {
        foreach(var i in items) {
            i.Name.text = "XXX";
            i.Score.text = "0";
        }
    }

    private void Start() {
        GetHighScores();
    }

    private string SetTimeText(int num) {
        TimeSpan time = TimeSpan.FromSeconds(num);

        string timeString = "";

        if(time.Hours > 0) {
            timeString += $"{time.Hours}h ";
        }
        if(time.Minutes > 0) {
            timeString += $"{time.Minutes}m ";
        }
        if(time.Seconds > 0 || timeString == "") {
            timeString += $"{time.Seconds}s";
        }

        return timeString.Trim();
    }

    private void OnEnable() {
        StartCoroutine(UpdateLeaderboard());
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    IEnumerator UpdateLeaderboard() {
        while(true) { 
            yield return new WaitForSecondsRealtime(3);
            ShowLeaderboard();
            yield return new WaitForSecondsRealtime(2);
        }
    }

    public void ShowLeaderboard() {
        GetHighScores();
    }

    public void GetHighScores() {
        LeaderboardCreator.GetLeaderboard(publicKey, OnGetHighScores);
    }

    private void OnGetHighScores(Entry[] entries) {
        int i = 0;
        foreach(var entry in entries) {
            if(i == 10)
                return;
            items[i].Name.text = entry.Username;
            items[i].Score.text = SetTimeText(entry.Score);
            i++;
        }
        OnHighScoresGet.Invoke();
    }


}