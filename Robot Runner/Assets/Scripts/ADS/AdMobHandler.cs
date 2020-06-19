using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;
using System;
using GoogleMobileAds.Api;
using UnityEngine.UI;


public class AdMobHandler : MonoBehaviour
{
    public RewardBasedVideoAd rewardBasedVideo;
    public Button ShowABtn;
    bool onetime = false;



    // Start is called before the first frame update
    void Awake()
    {
        ShowABtn.onClick.AddListener(() => ShowABtnHandler());
        //ShowABtn.gameObject.SetActive(false);


        #if UNITY_ANDROID
        string appId = "ca-app-pub-3940256099942544~3347511713";
#elif UNITY_IPHONE
            string appId = "ca-app-pub-3940256099942544~1458002511";
#else
            string appId = "unexpected_platform";
#endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);

        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

        // Called when an ad request has successfully loaded.
        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for watching a video.
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
        
        this.RequestRewardBasedVideo();
        //UserOptToWatchAd();
    }

    private void Start()
    {
        onetime = false;
        PlayerPrefs.SetInt("FromButton", 0);
    }

    private void RequestRewardBasedVideo()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            string adUnitId = "unexpected_platform";
#endif

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded video ad with the request.
        this.rewardBasedVideo.LoadAd(request, adUnitId);
    }

    private void UserOptToWatchAd()
    {
        try
        {
            if (rewardBasedVideo.IsLoaded())
            {
                rewardBasedVideo.Show();
            }
            else
            {
                ShowRewardedAd();
            }
        }
        catch (Exception e)
        {
            Debug.Log("Shit: " + e.Message);
        }

    }

    public void ShowABtnHandler()
    {
        PlayerPrefs.SetInt("FromButton", 1);
        UserOptToWatchAd();
    }

   
    // Update is called once per frame
    void FixedUpdate()
    {
  
            if(!onetime)
            {
                onetime = true;
                UserOptToWatchAd();
            }
        
    }

    public void ShowRewardedAd()
    {
        #if UNITY_ANDROID
        if (Advertisement.IsReady("3175095"))
        {
           
            {
                ShowABtn.gameObject.SetActive(true);
            }
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("3175095", options);
        }
        else
        {
           /* if (ShowABtn.IsActive())
            {
                ShowABtn.gameObject.SetActive(false);
            }*/
        }
#elif UNITY_IPHONE

        if (Advertisement.IsReady("3175094"))
        {
           
                ShowABtn.gameObject.SetActive(true);
            
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("3175094", options);
        }
        else
        {
           /* if (ShowABtn.IsActive())
            {
                ShowABtn.gameObject.SetActive(false);
            }*/
        }
#endif
    }

#if UNITY_ANDROID || UNITY_IPHONE
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                //Debug.Log("The ad was successfully shown.");
                if (PlayerPrefs.GetInt("FromButton") == 1)
                {
                    PlayerPrefs.SetInt("FromReward", 1);
                  
                    SceneManager.LoadScene(0);
                }
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
}
#endif

    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
        //statusLink.text += "HandleRewardBasedVideoLoaded event received [] ";
        //ShowABtn.gameObject.SetActive(true);
        
            ShowABtn.gameObject.SetActive(true);
        
    }

    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
       // statusLink.text +="HandleRewardBasedVideoFailedToLoad event received with message: "+ args.Message;
    }

    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
       // statusLink.text += "HandleRewardBasedVideoOpened event received";
    }

    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
        //statusLink.text += "HandleRewardBasedVideoStarted event received";
    }

    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
       // statusLink.text += "HandleRewardBasedVideoClosed event received";
        //this.RequestRewardBasedVideo();
    }

    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        //string type = args.Type;
        //double amount = args.Amount;
        if (PlayerPrefs.GetInt("FromButton") == 1)
        {
            PlayerPrefs.SetInt("FromReward", 1);

            SceneManager.LoadScene(0);
        }
        // statusLink.text +=
        //"HandleRewardBasedVideoRewarded event received for "
        // + amount.ToString() + " " + type;
    }

    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
        //statusLink.text += "HandleRewardBasedVideoLeftApplication event received";
    }
}
