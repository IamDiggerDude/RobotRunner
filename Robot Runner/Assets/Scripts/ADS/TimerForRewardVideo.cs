using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class TimerForRewardVideo : MonoBehaviour
{
    //public Text Timerads;
    public Button watchButton;
    private ulong LastWatchVideo;
    public float msReady = 100000.0f;
     void Start()
    {
        watchButton.GetComponent<Button>();
        LastWatchVideo = ulong.Parse(PlayerPrefs.GetString("lastwath"));
        if (!IsButtonVideoReaddy())
        {
            watchButton.interactable = false;
        }
    }

    void Update()
    {
        if (!watchButton.IsInteractable())
        {
            if (IsButtonVideoReaddy())
            {
                watchButton.interactable = true;
              
                return;
            }
            ulong diff = ((ulong)DateTime.Now.Ticks - LastWatchVideo);
            ulong m = diff / TimeSpan.TicksPerMillisecond;
            float secondsLeft = (float)(msReady - m) / 1000.0f;
            string r = "";
                r += ((int)secondsLeft / 3600).ToString() + "h ";
            secondsLeft -= ((int)secondsLeft / 3600) * 3600;
            r += ((int)secondsLeft / 60).ToString("00") + "m ";

            r += ((int)secondsLeft %60).ToString("00") + "s ";
            //Timerads.text = r;


        }
    }

    public void watchVideoButton()
    {
        LastWatchVideo = (ulong)DateTime.Now.Ticks;
        PlayerPrefs.SetString("lastwath", LastWatchVideo.ToString());
        watchButton.interactable = false;
    }
    private bool IsButtonVideoReaddy()
    {
            ulong diff = ((ulong)DateTime.Now.Ticks - LastWatchVideo);
            ulong m = diff / TimeSpan.TicksPerMillisecond;
            float secondsLeft = (float)(msReady - m) / 1000.0f;
            if (secondsLeft < 0)
        {
            //Timerads.text = "Ready!";
            return true;
        }
                
            return false;
    }
}
