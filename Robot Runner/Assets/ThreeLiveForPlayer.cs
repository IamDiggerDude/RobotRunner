using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThreeLiveForPlayer : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI RemainingTime;
    int checkServerHearts;
    int ServerTime;
    int PlayerTime;
    IEnumerator coroutine;

    public Image[] Hearts;
    protected int currentHearts;
    protected int serverHearts;
    public Button startButton;
    public Button restertButton;


    private void OnEnable()
    {
        coroutine = MakeUpdate();

        StartCoroutine(coroutine);
    }
    private void OnDisable()
    {
        StopCoroutine(coroutine);
    }

    void HeartsUpdate(GetPlayerStatisticsResult result)
    {
        currentHearts = result.Statistics.Find(stat => stat.StatisticName == "Lives").Value;

        startButton.gameObject.SetActive(currentHearts > 0);
        restertButton.gameObject.SetActive(currentHearts > 0);

        for (int i = 0; i < currentHearts; i++)
        {
            Hearts[i].color = Color.white;
        }
        for (int i = currentHearts; i < 3; i++)
        {
            Hearts[i].color = Color.black;
        }

        if (currentHearts < 3)
        {
            var requestTime = new GetTimeRequest { };
            PlayFabClientAPI.GetTime(requestTime, ServerTimeResult =>
            {
                PlayerTime = result.Statistics.Find(stat => stat.StatisticName == "Time").Value;
                Debug.Log("PlayerTime: " + PlayerTime);

                ServerTime = Convert.ToInt32(new TimeSpan(ServerTimeResult.Time.Ticks).TotalMinutes);
                Debug.Log("ServerTime B: " + ServerTime);

                RemainingTime.text = "Life will be restored in " + (60 - (ServerTime - PlayerTime)) + " minutes";

                Debug.Log("ServerTime A: " + ServerTime);

                switch (currentHearts)
                {
                    case 0:
                        {
                            //string RemainingTime = "Life will be restored in " + (60 - (ServerTime - PlayerTime)) + " minutes";
                            if (ServerTime >= PlayerTime + 180)
                            {
                                UpdateLives(currentHearts + 3);
                                UpdateTime(PlayerTime + 180);
                                //RemainingTime = "";
                                RemainingTime.text = "Restoring life, Wait a second...";
                                startButton.gameObject.SetActive(false);
                            }
                            else if (ServerTime >= PlayerTime + 120)
                            {
                                UpdateLives(currentHearts + 2);
                                UpdateTime(PlayerTime + 120);
                                //RemainingTime = "Life will be restored in " + (60 - (ServerTime - (PlayerTime + 120))) + " minutes";
                                RemainingTime.text = "Restoring life, Wait a second...";
                                startButton.gameObject.SetActive(false);
                            }
                            else if (ServerTime >= PlayerTime + 60)
                            {
                                UpdateLives(currentHearts + 1);
                                UpdateTime(PlayerTime + 60);
                                //RemainingTime = "Life will be restored in " + (60 - (ServerTime - (PlayerTime + 60))) + " minutes";
                                RemainingTime.text = "Restoring life, Wait a second...";
                                startButton.gameObject.SetActive(false);
                            }
                            Debug.Log("ServerTime IN 0: " + ServerTime);
                            //this.RemainingTime.text = RemainingTime;
                        }
                        break;
                    case 1:
                        {
                            //string RemainingTime = "Life will be restored in " + (60 - (ServerTime - PlayerTime)) + " minutes";
                            if (ServerTime >= PlayerTime + 120)
                            {
                                UpdateLives(currentHearts + 2);
                                UpdateTime(PlayerTime + 120);
                                //RemainingTime = "";
                                RemainingTime.text = "Restoring life, Wait a second...";
                                startButton.gameObject.SetActive(false);
                            }
                            else if (ServerTime >= PlayerTime + 60)
                            {
                                UpdateLives(currentHearts + 1);
                                UpdateTime(PlayerTime + 60);
                                //RemainingTime = "Life will be restored in " + (60 - (ServerTime - (PlayerTime + 60))) + " minutes";
                                RemainingTime.text = "Restoring life, Wait a second...";
                                startButton.gameObject.SetActive(false);
                            }
                            Debug.Log("ServerTime IN 1: " + ServerTime);
                            //this.RemainingTime.text = RemainingTime;
                        }
                        break;
                    case 2:
                        {
                            //string RemainingTime = "Life will be restored in " + (60 - (ServerTime - PlayerTime)) + " minutes";
                            if (ServerTime >= PlayerTime + 60)
                            {
                                UpdateLives(currentHearts + 1);
                                UpdateTime(PlayerTime + 60);
                                //RemainingTime = "";
                                RemainingTime.text = "Restoring life, Wait a second...";
                                startButton.gameObject.SetActive(false);
                            }
                            Debug.Log("ServerTime IN 2: " + ServerTime);
                            //this.RemainingTime.text = RemainingTime;
                        }
                        break;
                }
            }, ServerTimeError => { });

        }
        else
        {
            RemainingTime.text = "";
            startButton.gameObject.SetActive(true);
        }
    }
    IEnumerator MakeUpdate()
    {
        do
        {
            try
            {
                PlayFabClientAPI.GetPlayerStatistics(
        new GetPlayerStatisticsRequest(),
        HeartsUpdate,
        error => Debug.LogError(error.GenerateErrorReport()));
            }
            catch
            {
                coroutine = MakeUpdate();

                StartCoroutine(coroutine);
            }

            yield return new WaitForSeconds(5);
        }
        while (true);
    }    
    public void RemoveHearts()
    {
        if (currentHearts == 3)
        {
            Debug.Log("ServerTime RemoveHearts: " + ServerTime);
            UpdateTime(ServerTime);
        }
        UpdateLives(currentHearts -= 1);

        Debug.Log("On Remove Hearts " + currentHearts);
    }
    private void UpdateTime(long send)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdateTime", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { TimeValue = send }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, ECSR => { }, ECSE => { });
    }
    private void UpdateLives(int lives)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdateLives", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { LivesValue = lives }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, ECSR => { }, ECSE => { });
    }
}

#region OldLives
/*
    int checkServerHearts;
    DateTime checkServerTime;
    DateTime checkCountTime;
    DateTime add1;
    DateTime add2;
    DateTime add3;
    IEnumerator coroutine;

    public Image[] Hearts;
    protected int currentHearts;
    protected int serverHearts;
    public Button startButton;
    public Button restertButton;


    private void Start()
    {
        StartCoroutine(Loading());
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



    void OnGetUpdateLives(GetPlayerStatisticsResult result)
    {
        foreach (var eachStat in result.Statistics)
        {
            switch (eachStat.StatisticName)
            {
                case "Lives":
                    currentHearts = Convert.ToInt32(eachStat.Value.ToString());
                    CurrentHearts(currentHearts);
                    break;
            }
        }
    }



    IEnumerator MakeUpdate()
    {

        try
        {
            PlayFabClientAPI.GetPlayerStatistics(
    new GetPlayerStatisticsRequest(),
    OnGetUpdateLives,
    error => Debug.LogError(error.GenerateErrorReport()));
        }
        catch (Exception e)
        {
            coroutine = MakeUpdate();

            StartCoroutine(coroutine);
        }

        yield return new WaitForSeconds(5);

        StartCoroutine(MakeUpdate());
    }

    IEnumerator Loading()
    {
        yield return new WaitForSeconds(10);
        
        GameObject load = GameObject.FindGameObjectWithTag("Loading");
    }



    void CurrentHearts(int numberHearts)
    {
        var requestTime = new GetTimeRequest { };
        switch (numberHearts) {
            case 3:
                for (int i = 0; i< numberHearts; i++)
                {
                    Hearts[i].color = Color.red;
                }
                startButton.gameObject.SetActive(true);
                restertButton.gameObject.SetActive(true);
                break;
            case 2:
                Hearts[0].color = Color.black;
                Hearts[1].color = Color.red;
                Hearts[2].color = Color.red;
                startButton.gameObject.SetActive(true);
                restertButton.gameObject.SetActive(true);

                //Time                
                PlayFabClientAPI.GetTime(requestTime, nolol =>
                {
                    PlayFabClientAPI.GetPlayerStatistics(
                        new GetPlayerStatisticsRequest(),
                        lol =>
                        {
                            foreach (var eachStat in lol.Statistics)
                            {
                                switch (eachStat.StatisticName)
                                {
                                    case "Time":

                                        checkServerTime = nolol.Time;
                                        TimeSpan t = TimeSpan.FromTicks(checkServerTime.Ticks);
                                        long get = Convert.ToInt32(t.TotalMinutes);
                                        if(get >= (Convert.ToInt32(eachStat.Value.ToString()) + 360))
                                        {
                                            UpdateLives(3);
                                        }
                                        break;
                                }
                            }
                        },
                        error => Debug.LogError(error.GenerateErrorReport()));
                }, noelol => { });
                break;
            case 1:
                Hearts[0].color = Color.black;
                Hearts[1].color = Color.black;
                Hearts[2].color = Color.red;
                startButton.gameObject.SetActive(true);
                restertButton.gameObject.SetActive(true);

                //Time                
                PlayFabClientAPI.GetTime(requestTime, nolol =>
                {
                    PlayFabClientAPI.GetPlayerStatistics(
                        new GetPlayerStatisticsRequest(),
                        lol =>
                        {
                            foreach (var eachStat in lol.Statistics)
                            {
                                switch (eachStat.StatisticName)
                                {
                                    case "Time":

                                        checkServerTime = nolol.Time;
                                        TimeSpan t = TimeSpan.FromTicks(checkServerTime.Ticks);
                                        long get = Convert.ToInt32(t.TotalMinutes);
                                        if (get >= (Convert.ToInt32(eachStat.Value.ToString()) + 720))
                                        {
                                            UpdateLives(3);
                                        }
                                        else if (get >= (Convert.ToInt32(eachStat.Value.ToString()) + 360))
                                        {
                                            UpdateLives(2);

                                            UpdateTime(get + 360);
                                        }
                                        break;
                                }
                            }
                        },
                        error => Debug.LogError(error.GenerateErrorReport()));
                }, noelol => { });
                break;
            case 0:
                Hearts[0].color = Color.black;
                Hearts[1].color = Color.black;
                Hearts[2].color = Color.black;
                startButton.gameObject.SetActive(false);
                restertButton.gameObject.SetActive(false);

                //Time                
                PlayFabClientAPI.GetTime(requestTime, nolol =>
                {
                    PlayFabClientAPI.GetPlayerStatistics(
                        new GetPlayerStatisticsRequest(),
                        lol =>
                        {
                            foreach (var eachStat in lol.Statistics)
                            {
                                switch (eachStat.StatisticName)
                                {
                                    case "Time":

                                        checkServerTime = nolol.Time;
                                        TimeSpan t = TimeSpan.FromTicks(checkServerTime.Ticks);
                                        long get = Convert.ToInt32(t.TotalMinutes);
                                        if (get >= (Convert.ToInt32(eachStat.Value.ToString()) + 1080))
                                        {
                                            UpdateLives(3);
                                        }
                                        else if (get >= (Convert.ToInt32(eachStat.Value.ToString()) + 720))
                                        {
                                            UpdateLives(2);

                                            UpdateTime(get + 720);
                                        }
                                        else if (get >= (Convert.ToInt32(eachStat.Value.ToString()) + 360))
                                        {
                                            UpdateLives(1);

                                            UpdateTime(get+360);
                                        }
                                        break;
                                }
                            }
                        },
                        error => Debug.LogError(error.GenerateErrorReport()));
                }, noelol => { });
                break;
                
            }

        //        PlayFabClientAPI.GetPlayerStatistics(
        //new GetPlayerStatisticsRequest(),
        //LUPD=> 
        //{
        //    foreach (var eachStat in LUPD.Statistics)
        //    {
        //        switch (eachStat.StatisticName)
        //        {
        //            case "Lives":
        //                Debug.Log("HP in Update " + eachStat.Value.ToString());
        //                break;
        //        }
        //    }},error => Debug.LogError(error.GenerateErrorReport()));

        //        PlayFabClientAPI.GetTime(requestTime, nolol =>
        //        {
        //            checkServerTime = nolol.Time;
        //            TimeSpan t = TimeSpan.FromTicks(checkServerTime.Ticks);
        //            long send = Convert.ToInt32(t.TotalMinutes);
        //            Debug.Log("Timespan in Update : " + send);
        //        }, noelol => { });


                     
    }

    public void RemoveHearts()
    {
        if (currentHearts>0)
        {
            if (currentHearts == 3)
            {
                var requestTime = new GetTimeRequest { };
                PlayFabClientAPI.GetTime(requestTime, nolol =>
                {
                    checkServerTime = nolol.Time;
                    TimeSpan t = TimeSpan.FromTicks(checkServerTime.Ticks);
                    long send = Convert.ToInt32(t.TotalMinutes);
                    Debug.Log("Timespan 4 : " + send);
                    UpdateTime(send);
                }, noelol => { });
            }

            currentHearts -= 1;
            UpdateLives(currentHearts);
        }

        Debug.Log("On Remove Hearts " + currentHearts);
               
        CurrentHearts(currentHearts);

    }



    private void UpdateTime(long send)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdateTime", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { TimeValue = send}, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, nonolol => { }, nonoelol => { });
    }

    private void UpdateLives(int lives)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdateLives", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { LivesValue = lives }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, qwe => { }, ewq => { });
    }
}
    */
#endregion OldLives