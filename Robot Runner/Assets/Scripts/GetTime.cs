using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class GetTime : MonoBehaviour
{
    IEnumerator coroutine;
    TextMeshProUGUI TimeTMP;

    private void Start()
    {
        TimeTMP = this.GetComponent<TextMeshProUGUI>();
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
            var requestTime = new GetTimeRequest { };
            PlayFabClientAPI.GetTime(requestTime, onGetTimeResult =>
            {
                //TimeTMP.text = onGetTimeResult.Time.ToShortTimeString() + ", " + onGetTimeResult.Time.ToLongDateString();
                TimeTMP.text = "Tournament ends: " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(onGetTimeResult.Time.Month);

            }, onGetTimeError => { });
        }
        catch (Exception e)
        {
            coroutine = MakeUpdate();

            StartCoroutine(coroutine);
        }
        yield return new WaitForSeconds(5);

        StartCoroutine(MakeUpdate());
    }
}
