using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScore : MonoBehaviour
{
    public TextMeshProUGUI HighScoreTMP;
    public TextMeshProUGUI MyScoreTMP;
    IEnumerator coroutine;

    void Start()
    {
        IEnumerator coroutine;
    }
    private void OnEnable()
    {
        coroutine = MakeUpdate();

        StartCoroutine(coroutine);
    }

    private void OnDisable()
    {
        StopCoroutine(coroutine);
    }


    IEnumerator MakeUpdate()
    {
        try
        {
            var requestLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "BBTCHighscore", MaxResultsCount = 1 };
            PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeaderboard=>
            {
                foreach (PlayerLeaderboardEntry player in OnGetLeaderboard.Leaderboard)
                {
                    HighScoreTMP.text = player.StatValue.ToString();
                }
            }, OnErrorLeaderboard => { Debug.LogError(OnErrorLeaderboard.GenerateErrorReport()); });

            PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetHighscore=> 
            {
                foreach (var eachStat in OnGetHighscore.Statistics)
                {
                    switch (eachStat.StatisticName)
                    {
                        case "BBTCHighscore":
                            MyScoreTMP.text = eachStat.Value.ToString();
                            break;
                    }
                }
            },
            error => Debug.LogError(error.GenerateErrorReport()));
        }
        catch (Exception)
        {
            coroutine = MakeUpdate();

            StartCoroutine(coroutine);
        }

        yield return new WaitForSeconds(5);

        StartCoroutine(MakeUpdate());
    }
}
