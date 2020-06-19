using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class exitButton : MonoBehaviour
{
public void exitapp()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
