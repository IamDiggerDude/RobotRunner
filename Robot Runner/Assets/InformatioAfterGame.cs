using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;
using System;

public class InformatioAfterGame : MonoBehaviour
{
    public TextMeshProUGUI Score;
    public TextMeshProUGUI BBTS;
    public TextMeshProUGUI Percent;
    private int previousCoinValue;
    private string s;
#if UNITY_ANDROID  || UNITY_IPHONE
    private AdMobHandler ad;
#endif

    public void Open()
    {
#if UNITY_ANDROID || UNITY_IOS
        ad.rewardBasedVideo.Show();
#endif
        gameObject.SetActive(true);
        Data();
    }

    public void Data()
    {

        Score.SetText("SCORE: " + PlayerData.instance.highscores.ToString());
        BBTS.SetText("BBTC: " + PlayerData.instance.coins.ToString());
        Percent.SetText(PlayerData.instance.rank.ToString());

    }

}
