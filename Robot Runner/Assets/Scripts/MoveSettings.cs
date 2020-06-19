using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!PlayerPrefs.HasKey("MoveType"))
        {
            PlayerPrefs.SetInt("MoveType",1);
            PlayerPrefs.Save();
        }

    }

    public void UseSlideCotroll()
    {
        PlayerPrefs.SetInt("MoveType", 1);
        PlayerPrefs.Save();
    }
    public void UseTouchCotroll()
    {
        PlayerPrefs.SetInt("MoveType", 2);
        PlayerPrefs.Save();
    }
}
