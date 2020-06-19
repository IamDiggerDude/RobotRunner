using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfabLeaderboard : MonoBehaviour
{

    public GameObject leaderboardPanel;
    public GameObject listingPrefab;
    public Transform listingContainer;
    public bool displayPlayer = true;

    private void OnEnable()
    {
        foreach (Transform child in listingContainer)
        {
            Destroy(child.gameObject);
        }
        GetLeaderboard();
    }

    public void GetLeaderboard()
    {
        var requestLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "BBTC", MaxResultsCount = 10 };
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeaderboard, OnErrorLeaderboard);
    }

    void OnGetLeaderboard(GetLeaderboardResult result)
    {
        int kol = 0, i = 1;
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            kol++;
            GameObject tempListing = Instantiate(listingPrefab, listingContainer);
            Listening LL = tempListing.GetComponent<Listening>();
            LL.Name.text = player.DisplayName;
            LL.Score.text = player.StatValue.ToString();
            switch (i)
            {
                case 1:
                    LL.Reward.text = "No reward";
                    i++;
                    break;
                case 2:
                    LL.Reward.text = "No reward";
                    i++;
                    break;
                case 3:
                    LL.Reward.text = "No reward";
                    i++;
                    break;
                default:
                    LL.Reward.text = "No reward";
                    break;
            }
            Debug.Log(player.DisplayName + ": " + player.StatValue);
        }

        for (int k = 0; k < 10 - kol; k++)
        {
            GameObject tempListing = Instantiate(listingPrefab, listingContainer);
            Listening LL = tempListing.GetComponent<Listening>();
            LL.Name.text = "";
            LL.Score.text = "";
            LL.Reward.text = "";
        }
    }

    void OnErrorLeaderboard(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    public void Open()
    {
        gameObject.SetActive(true);
        GetLeaderboard();
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void CloseLeaderboardPanel()
    {
        leaderboardPanel.SetActive(false);
        if (listingContainer != null)
        {
            for (int i = listingContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(listingContainer.GetChild(i).gameObject);
            }
        }
    }
}
