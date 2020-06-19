using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
public class UnityAds : MonoBehaviour
{
#if !UNITY_WEBGL
    private void Start()
    {
        rewardsunityvideo();
    }
    public void rewardsunityvideo()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            Advertisement.Show("rewardedVideo");
        }
    }
#endif
}
