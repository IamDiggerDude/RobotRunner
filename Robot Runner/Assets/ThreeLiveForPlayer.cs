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
    DateTime checkServerTime;
    DateTime checkCountTime;
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
        do
        {
            try
            {
                PlayFabClientAPI.GetPlayerStatistics(
        new GetPlayerStatisticsRequest(),
        OnGetUpdateLives,
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
    void CurrentHearts(int numberHearts)
    {
        if (numberHearts > 0)
        {
            startButton.gameObject.SetActive(true);
            restertButton.gameObject.SetActive(true);
        }
        else
        {
            startButton.gameObject.SetActive(false);
            restertButton.gameObject.SetActive(false);
        }

        for (int i = 0; i < numberHearts; i++)
        {
            Hearts[i].color = Color.white;
        }
        for (int i = numberHearts; i < 3; i++)
        {
            Hearts[i].color = Color.black;
        }

        if (currentHearts < 3)
        {
            var requestTime = new GetTimeRequest { };
            PlayFabClientAPI.GetTime(requestTime, ServerTimeResult =>
            {
                PlayFabClientAPI.GetPlayerStatistics(
                    new GetPlayerStatisticsRequest(),
                    PlayerTimeResult =>
                    {
                        foreach (var eachStat in PlayerTimeResult.Statistics)
                        {
                            switch (eachStat.StatisticName)
                            {
                                case "Time":

                                    int sendTime = 0;

                                    checkServerTime = ServerTimeResult.Time;
                                    TimeSpan t = TimeSpan.FromTicks(checkServerTime.Ticks);
                                    long get = Convert.ToInt32(t.TotalMinutes);

                                    if (get >= (Convert.ToInt32(eachStat.Value) + 180) && currentHearts == 0)
                                    {
                                        Debug.Log("get: " + get);
                                        Debug.Log("eachStat: " + Convert.ToInt32(eachStat.Value));
                                        UpdateTime(Convert.ToInt32(eachStat.Value.ToString()) + 180);
                                        UpdateLives(3);
                                        RemainingTime.text = "";
                                        break;
                                    }
                                    else if (get >= (Convert.ToInt32(eachStat.Value) + 120) && (currentHearts == 0 || currentHearts == 1))
                                    {
                                        Debug.Log("get: " + get);
                                        Debug.Log("eachStat: " + Convert.ToInt32(eachStat.Value));
                                        UpdateTime(Convert.ToInt32(eachStat.Value.ToString()) + 120);
                                        UpdateLives(currentHearts+2);
                                        if(currentHearts == 0)
                                            RemainingTime.text = "Life will be restored in: " + ((Convert.ToInt32(eachStat.Value.ToString()) + 180) - get) + "m";
                                        else
                                            RemainingTime.text = "";
                                        break;
                                    }
                                    else if (get >= (Convert.ToInt32(eachStat.Value) + 60) && (currentHearts == 0 || currentHearts == 1 || currentHearts == 2))
                                    {
                                        Debug.Log("get: " + get);
                                        Debug.Log("eachStat: " + Convert.ToInt32(eachStat.Value));
                                        UpdateTime(Convert.ToInt32(eachStat.Value.ToString()) + 60);
                                        UpdateLives(currentHearts+1);
                                        if (currentHearts == 0)
                                            RemainingTime.text = "Life will be restored in: " + ((Convert.ToInt32(eachStat.Value.ToString()) + 60) - get) + "m";
                                        else if (currentHearts == 1)
                                            RemainingTime.text = "Life will be restored in: " + ((Convert.ToInt32(eachStat.Value.ToString()) + 120) - get) + "m";
                                        else
                                            RemainingTime.text = "";
                                        break;
                                    }
                                    RemainingTime.text = "Life will be restored in: " + ((Convert.ToInt32(eachStat.Value.ToString()) + 60) - get) + "m";
                                    break;
                                    /*if (get >= (Convert.ToInt32(eachStat.Value) + 60))
                                    {
                                        Debug.Log("get: " + get);
                                        Debug.Log("eachStat: " + Convert.ToInt32(eachStat.Value) + 60);
                                        UpdateTime(Convert.ToInt32(eachStat.Value.ToString()) + 60);
                                        UpdateLives(currentHearts + 1);
                                        break;
                                    }*/
                            }
                        }
                    },
                    PlayerTimeError => Debug.LogError(PlayerTimeError.GenerateErrorReport()));
            }, ServerTimeError => { });
        }
        else
            RemainingTime.text = "";
    }
    public void RemoveHearts()
    {
        if (currentHearts > 0)
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
            FunctionParameter = new { TimeValue = send }, // The parameter provided to your function
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