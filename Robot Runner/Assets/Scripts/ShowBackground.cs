using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class ShowBackground : MonoBehaviour
{
    public VideoPlayer MyPlayer;
    public Camera UICam;
    private IEnumerator enumerator;

    private void Start()
    {
        UICam.gameObject.SetActive(false);
        UICam.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            try
            {
                MyPlayer.Play();
            }
            catch (Exception e)
            {
                Debug.LogError(e.GetBaseException());
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            try
            {
                MyPlayer.Stop();
            }
            catch (Exception e)
            {
                Debug.LogError(e.GetBaseException());
            }
        }
    }

    public void PlayVideo()
    {
        UICam.enabled = true;
        MyPlayer.enabled = true;
        MyPlayer.Play();
    }

    public void StopVideo()
    {
        MyPlayer.Pause();
        MyPlayer.enabled = false;
        UICam.enabled = false;
    }

}
