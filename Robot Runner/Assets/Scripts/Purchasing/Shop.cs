using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shop : MonoBehaviour
{
    IEnumerator coroutine;
    //public TextMeshProUGUI currentSSTMP;
    public TextMeshProUGUI currentSTMP;
    string itemInstanceID = "";

    public void BuyWithSatoshi()
    {
        PlayFabClientAPI.GetPlayerStatistics(
new GetPlayerStatisticsRequest(),
LivesInfo =>
{
    foreach (var eachStat in LivesInfo.Statistics)
    {
        switch (eachStat.StatisticName)
        {
            case "Lives":
                if(eachStat.Value<3 && Convert.ToInt32(currentSTMP.text)>=5000)
                {
                        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                        {
                            FunctionName = "UpdateSatoshi", // Arbitrary function name (must exist in your uploaded cloud.js file)
                            FunctionParameter = new { SatoshiValue = -5000 }, // The parameter provided to your function
                            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
                        }, nothing1 => { }, nothing2 => { });

                    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                    {
                        FunctionName = "UpdateLives", // Arbitrary function name (must exist in your uploaded cloud.js file)
                        FunctionParameter = new { LivesValue = eachStat.Value + 1 }, // The parameter provided to your function
                        GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
                    }, nothing1 => { }, nothing2 => { });
                }
                break;
        }
    }
},
error => Debug.LogError(error.GenerateErrorReport()));
    }

    /*public void UseSuperSatoshi()
    {
        PlayFabClientAPI.GetPlayerStatistics(
new GetPlayerStatisticsRequest(),
LivesInfo =>
{
    foreach (var eachStat in LivesInfo.Statistics)
    {
        switch (eachStat.StatisticName)
        {
            case "Lives":
                if (eachStat.Value < 3)
                {
                    var consumeitem = new ConsumeItemRequest {ConsumeCount =1 , ItemInstanceId = itemInstanceID };
                    PlayFabClientAPI.ConsumeItem(consumeitem, Consumed => { Debug.Log("Consumed!"); }, NotConsumed => { Debug.Log("Not consumed!"); });

                    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                    {
                        FunctionName = "UpdateLives", // Arbitrary function name (must exist in your uploaded cloud.js file)
                        FunctionParameter = new { LivesValue = 3 }, // The parameter provided to your function
                        GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
                    }, nothing1 => { }, nothing2 => { });
                }
                break;
        }
    }
},
error => Debug.LogError(error.GenerateErrorReport()));

    }*/

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
        do
        {
            try
            {
                /*var items = new GetUserInventoryRequest();
                PlayFabClientAPI.GetUserInventory(items, itemInfo =>
                {
                    foreach (var eachStat in itemInfo.Inventory)
                    {
                        switch (eachStat.ItemId)
                        {
                            case "SS":
                                currentSSTMP.SetText(eachStat.RemainingUses.ToString());
                                itemInstanceID = eachStat.ItemInstanceId;
                                break;
                            default:
                                currentSSTMP.SetText("0");
                                break;
                        }
                    }

                }, itemError => { itemError.GenerateErrorReport(); });*/



                PlayFabClientAPI.GetPlayerStatistics(
                new GetPlayerStatisticsRequest(),
                SatoshiInfo =>
                {
                    foreach (var eachStat in SatoshiInfo.Statistics)
                    {
                        switch (eachStat.StatisticName)
                        {
                            case "Satoshi":
                                currentSTMP.SetText(eachStat.Value.ToString());
                                break;
                        }
                    }
                },
                SatoshiError => Debug.LogError(SatoshiError.GenerateErrorReport()));
            }
            catch (Exception)
            {
                coroutine = MakeUpdate();

                StartCoroutine(coroutine);
            }

            yield return new WaitForSeconds(5);
        } while (true);
    }
}
