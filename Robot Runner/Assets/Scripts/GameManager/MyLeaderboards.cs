using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

using PlayFab.DataModels;
using PlayFab.ProfilesModels;

using PlayFab.Json;

public class MyLeaderboards : MonoBehaviour
{
    public GameObject leaderboardPanel;
    public GameObject listingPrefab;
    public Transform  listingContainer;
    public bool displayPlayer = true;

    public void Open()
    {
        gameObject.SetActive(true);
        GetLeaderboarder();
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void GetLeaderboarder()
    {
        var requestLeaderboard = new GetLeaderboardRequest { StartPosition = 0, StatisticName = "PlayerHighScore", MaxResultsCount = 20 };
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeadboard, OnErrorLeaderboard);
    }
    void OnGetLeadboard(GetLeaderboardResult result)
    {
        leaderboardPanel.SetActive(true);
        //Debug.Log(result.Leaderboard[0].StatValue);
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            GameObject tempListing = Instantiate(listingPrefab, listingContainer);
            Listening LL = tempListing.GetComponent<Listening>();
            LL.Name.text = player.DisplayName;
            LL.Score.text = player.StatValue.ToString();
            //LL.Reward.text = player.
            Debug.Log(player.DisplayName + ": " + player.StatValue);
        }
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
    void OnErrorLeaderboard(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
}
