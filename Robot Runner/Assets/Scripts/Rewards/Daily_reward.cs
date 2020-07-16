using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class Daily_reward : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI RewardLabel;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("Day") || !PlayerPrefs.HasKey("Month"))
        {
            PlayerPrefs.SetInt("Day", System.DateTime.Now.Day);
            PlayerPrefs.SetInt("Month", System.DateTime.Now.Month);
            GetRandom();
        }

        if (PlayerPrefs.GetInt("Day") != System.DateTime.Now.Day || PlayerPrefs.GetInt("Month") != System.DateTime.Now.Month)
        {
            GetRandom();
        }
        else
        gameObject.SetActive(false);

        PlayerPrefs.SetInt("Day", System.DateTime.Now.Day-1);
        PlayerPrefs.SetInt("Month", System.DateTime.Now.Month);
    }

    void GetRandom()
    {
        var rand = new System.Random();
        int r = rand.Next(1, 101);
        Debug.Log("Random reward:" + r);
        if (r <= 60)
        {
            r = rand.Next(1, 8);
            switch (r)
            {
                case 1:
                    AddSatoshi(500);
                    break;
                case 2:
                    AddSatoshi(1000);
                    break;
                case 3:
                    AddSatoshi(1500);
                    break;
                case 4:
                    AddSatoshi(2000);
                    break;
                case 5:
                    AddSatoshi(2500);
                    break;
                case 6:
                    AddSatoshi(3500);
                    break;
                case 7:
                    AddSatoshi(5000);
                    break;
                default:
                    break;
            }
        }
        else if (r > 60 && r <= 93)
        {
            r = rand.Next(1, 4);
            switch (r)
            {
                case 1:
                    AddSatoshi(7000);
                    break;
                case 2:
                    AddSatoshi(10000);
                    break;
                case 3:
                    AddSatoshi(15000);
                    break;
                default:
                    break;
            }
        }
        else
        {
            r = rand.Next(1, 8);
            switch (r)
            {
                case 1:
                    GrantSkin("Agent");
                    break;
                case 2:
                    GrantSkin("Blue Lightning");
                    break;
                case 3:
                    GrantSkin("Golden Rush");
                    break;
                case 4:
                    GrantSkin("Hive");
                    break;
                case 5:
                    GrantSkin("Junkyard Metal");
                    break;
                case 6:
                    GrantSkin("Junkyard");
                    break;
                case 7:
                    GrantSkin("RedLine ");
                    break;
                default:
                    break;
            }
        }
    }

    void AddSatoshi(int value)
    {
        RewardLabel.text = value + " Satoshi";
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdateSatoshi", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { SatoshiValue = value }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, nothing1 => { }, nothing2 => { });
    }

    void GrantSkin(string skinName)
    {
        RewardLabel.text = skinName + " skin";
        GetUserInventoryRequest items = new GetUserInventoryRequest { };
        PlayFabClientAPI.GetUserInventory(items, itemInfo =>
        {
            bool grant = true;
            foreach (var eachStat in itemInfo.Inventory)
                if (skinName == eachStat.ItemId)
                    grant = false;

            if (grant)
            {
                var purchaseRequest = new PurchaseItemRequest();
                purchaseRequest.CatalogVersion = "Skins";
                purchaseRequest.ItemId = skinName + "Bundle";
                purchaseRequest.VirtualCurrency = "ST";
                purchaseRequest.Price = 0;
                PlayFabClientAPI.PurchaseItem(purchaseRequest, result => { Debug.Log(skinName + " added"); }, error => { Debug.Log(skinName + " was not added added"); });
            }

        }, itemError => { itemError.GenerateErrorReport(); });
    }
}
