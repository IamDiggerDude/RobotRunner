using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfabData : MonoBehaviour
{
    public static PlayfabData SharedInstance;
    private string playfabUserId;
    private string sessionToken;
    public string SessionToken { get => sessionToken; set => sessionToken = value; }
    public string PlayfabUserId { get => playfabUserId; set => playfabUserId = value; }

    private void Start()
    {
        SharedInstance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
