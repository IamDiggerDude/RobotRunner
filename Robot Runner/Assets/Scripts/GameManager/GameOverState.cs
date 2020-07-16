using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif
using System.Collections.Generic;
 
/// <summary>
/// state pushed on top of the GameManager when the player dies.
/// </summary>
public class GameOverState : AState
{
    [SerializeField]
    SkinSelection skinSelection;

    private int sendCoins;
    private int previousCoinValueServer;

    public TrackManager trackManager;
    public Canvas canvas;
    public MissionUI missionPopup;

	public AudioClip gameOverTheme;

	public Leaderboard miniLeaderboard;
	public Leaderboard fullLeaderboard;

    public InformatioAfterGame lead;

    public GameObject addButton;



    public override void Enter(AState from)
    {
        canvas.gameObject.SetActive(true);

        lead.Score.text = "SCORE: " +  trackManager.score.ToString();
        lead.BBTS.text =  "BBTS: "  + trackManager.characterController.coins.ToString();


        //miniLeaderboard.playerEntry.inputName.text = PlayerData.instance.previousName;
		//miniLeaderboard.playerEntry.score.text = trackManager.score.ToString();
		//miniLeaderboard.Populate();

        //if (PlayerData.instance.AnyMissionComplete())
        //    missionPopup.Open();
        //else
        //    missionPopup.gameObject.SetActive(false);

		CreditCoins();

		if (MusicPlayer.instance.GetStem(0) != gameOverTheme)
		{
            MusicPlayer.instance.SetStem(0, gameOverTheme);
			StartCoroutine(MusicPlayer.instance.RestartAllStems());
        }
    }

	public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);
        FinishRun();
    }

    public override string GetName()
    {
        return "GameOver";
    }

    public override void Tick()
    {
        
    }

	public void OpenLeaderboard()
	{
		//fullLeaderboard.forcePlayerDisplay = false;
		//fullLeaderboard.displayPlayer = true;
		//fullLeaderboard.playerEntry.playerName.text = miniLeaderboard.playerEntry.inputName.text;
		//fullLeaderboard.playerEntry.score.text = trackManager.score.ToString();

		//fullLeaderboard.Open();
    }

	public void GoToStore()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("shop", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }


    public void GoToLoadout()
    {
        trackManager.isRerun = false;
		manager.SwitchState("Loadout");
    }

    public void RunAgain()
    {
        trackManager.isRerun = false;
        manager.SwitchState("Game");
    }

    protected void CreditCoins()
	{
		PlayerData.instance.Save();

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        var transactionId = System.Guid.NewGuid().ToString();
        var transactionContext = "gameplay";
        var level = PlayerData.instance.rank.ToString();
        var itemType = "consumable";
        
        if (trackManager.characterController.coins > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Soft, // Currency type
                transactionContext,
                trackManager.characterController.coins,
                "fishbone",
                PlayerData.instance.coins,
                itemType,
                level,
                transactionId
            );
        }

        if (trackManager.characterController.premium > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Premium, // Currency type
                transactionContext,
                trackManager.characterController.premium,
                "anchovies",
                PlayerData.instance.premium,
                itemType,
                level,
                transactionId
            );
        }
#endif 
	}

	protected void FinishRun()
    {
		//if(miniLeaderboard.playerEntry.inputName.text == "")
		//{
		//	miniLeaderboard.playerEntry.inputName.text = "Trash Cat";
		//}
		//else
		//{
		//	PlayerData.instance.previousName = miniLeaderboard.playerEntry.inputName.text;
		//}

        PlayerData.instance.InsertScore(trackManager.score, "player" ); // вместо player имя игрока с таблицы плефаба

        CharacterCollider.DeathEvent de = trackManager.characterController.characterCollider.deathData;

        //------------------------------------Playfab highscore send to server 1 start--------------------------

        sendCoins = de.coins;

        //------------------------------------Achivements check-------------------------------------------------

        if (sendCoins >= 12000)
            CheckAndGrantSkin("Lolipop");
        if (sendCoins >= 18000)
            CheckAndGrantSkin("Rusty Military");
        if (sendCoins >= 25000)
            CheckAndGrantSkin("Military");

        //------------------------------------Playfab highscore send to server 1 end--------------------------

        //register data to analytics
#if UNITY_ANALYTICS
        AnalyticsEvent.GameOver(null, new Dictionary<string, object> {
            { "coins", de.coins },
            { "premium", de.premium },
            { "score", de.score },
            { "distance", de.worldDistance },
            { "obstacle",  de.obstacleType },
            { "theme", de.themeUsed },
            { "character", de.character },
        });
#endif
        //------------------------------------Playfab highscore send to server 2 start--------------------------

        PlayFabClientAPI.GetPlayerStatistics(
    new GetPlayerStatisticsRequest(),
    OnGetStats,
    error => Debug.LogError(error.GenerateErrorReport()));

        //------------------------------------Playfab highscore send to server 2 end--------------------------

        PlayerData.instance.Save();

        trackManager.End();
    }

    void CheckAndGrantSkin(string SkinName)
    {
        GetUserInventoryRequest items = new GetUserInventoryRequest { };
        PlayFabClientAPI.GetUserInventory(items, itemInfo =>
        {
            foreach (var eachStat in itemInfo.Inventory)
            {
                bool grant = true;
                if (eachStat.ItemId == SkinName)
                    grant = false;
                if (grant)
                {
                    var purchaseRequest = new PurchaseItemRequest();
                    purchaseRequest.CatalogVersion = "Skins";
                    purchaseRequest.ItemId = SkinName + "Bundle";
                    purchaseRequest.VirtualCurrency = "ST";
                    purchaseRequest.Price = 0;
                    PlayFabClientAPI.PurchaseItem(purchaseRequest, result => { Debug.Log(SkinName + " added"); }, error => { Debug.Log(SkinName + " was not added added"); });

                    for (int j = 0; j < skinSelection.SkinDetails.Length; j++)
                    {
                        if (skinSelection.SkinDetails[j].Name == SkinName)
                            skinSelection.Locked[j] = false;
                    }
                }
            }

        }, itemError => { itemError.GenerateErrorReport(); });
    }

    void OnGetStats(GetPlayerStatisticsResult result)
    {
        foreach (var eachStat in result.Statistics)
        {
            switch (eachStat.StatisticName)
            {
                case "Satoshi":
                    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                    {
                        FunctionName = "UpdateLeaderboard", // Arbitrary function name (must exist in your uploaded cloud.js file)
                        FunctionParameter = new { BBTCValue = sendCoins, BBTCHighscoreValue = sendCoins, SatoshiValue = sendCoins}, // The parameter provided to your function
                        GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
                    }, nothing1 => { }, nothing2 => { });                    
                    break;
            }
        }
    }

    //----------------
}
