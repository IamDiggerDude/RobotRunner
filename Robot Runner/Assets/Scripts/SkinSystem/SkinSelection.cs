using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using PlayFabLogin;
using TMPro;
using UnityEngine.UI;

public class SkinSelection : MonoBehaviour
{
    [SerializeField]
    TrackManager TrackManager;
    [SerializeField]
    Button ChooseButton;
    [SerializeField]
    GameObject StaticViewUpload;
    string[] PlayableObjects;
    bool[] Locked;
    int Chosen = 0;
    const float distanceStep = 0.7f;
    const int speed = 10;
    IEnumerator enumerator;

    private void Start()
    {
        int BeginWith = 2;
        if (!PlayerPrefs.HasKey("Skin"))
        {
            PlayerPrefs.SetString("Skin", "Classic");
        }

        enumerator = Move();

        GameObject[] LoadedSkins = Resources.LoadAll<GameObject>("Skins/Static");
        GameObject[] GetPlayableObjectsNames = Resources.LoadAll<GameObject>("Skins/Playable");
        PlayableObjects = new string[GetPlayableObjectsNames.Length];
        Locked = new bool[GetPlayableObjectsNames.Length];
        for (int i = 0; i < PlayableObjects.Length; i++)
        {
            PlayableObjects[i] = GetPlayableObjectsNames[i].name;
        }


        GetUserInventoryRequest items = new GetUserInventoryRequest { };
        PlayFabClientAPI.GetUserInventory(items, itemInfo =>
        {
            for (int i = 0; i < LoadedSkins.Length; i++)
            {
                Locked[i] = true;
                foreach (var eachStat in itemInfo.Inventory)
                {
                    if (LoadedSkins[i].name.Remove(LoadedSkins[i].name.Length - 6, 6) == eachStat.ItemId)
                    {
                        Locked[i] = false;
                    }
                }

                GameObject ShowSkin = Instantiate(LoadedSkins[i], new Vector3(distanceStep * i + StaticViewUpload.transform.position.x, StaticViewUpload.transform.position.y, StaticViewUpload.transform.position.z), Quaternion.Euler(new Vector3(0, 180, 0)));
                ShowSkin.name = ShowSkin.name.Remove(ShowSkin.name.Length - 13, 13);
                ShowSkin.transform.SetParent(StaticViewUpload.transform);

                if (StaticViewUpload.transform.GetChild(i).name == PlayerPrefs.GetString("Skin"))
                {
                    BeginWith = i;
                }
            }

            Show(BeginWith);
            Choose();
        }, itemError => { itemError.GenerateErrorReport(); });
    }

    public void Show(int index)
    {
        for (int i = 0; i < StaticViewUpload.transform.childCount; i++)
        {
            if (i == index)
            {
                StaticViewUpload.transform.GetChild(i).transform.localScale = Vector3.one;
            }
            else
            {
                StaticViewUpload.transform.GetChild(i).transform.localScale = Vector3.one / 5;
                StaticViewUpload.transform.GetChild(i).GetComponent<Skin>().ShowNormalSkin(true);
            }
        }

        ChooseButton.interactable = !Locked[index];
        if (Locked[index])
            ChooseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Locked";
        else
            ChooseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Choose";

        Chosen = index;
        if (enumerator!=null)
            StopCoroutine(enumerator);
        enumerator = Move();
        StartCoroutine(enumerator);
    }

    IEnumerator Move()
    {
        Vector3 destination = -Vector3.right * Chosen * distanceStep + Vector3.forward * StaticViewUpload.transform.position.z + Vector3.up * StaticViewUpload.transform.position.y;
        do
        {
            StaticViewUpload.transform.position = Vector3.MoveTowards(StaticViewUpload.transform.position, destination, speed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
            if (StaticViewUpload.transform.position == destination)
                break;
        }
        while (true);
    }

    public void ChangeView(bool view)
    {
        StaticViewUpload.transform.GetChild(Chosen).GetComponent<Skin>().ShowNormalSkin(view);
    }
    public void Choose()
    {
        TrackManager.MyPlayer = Resources.Load<GameObject>("Skins/Playable/" + PlayableObjects[Chosen]); ;
        PlayerPrefs.SetString("Skin", PlayableObjects[Chosen]);
    }
}

#region Old_Skins
/*
using System.Collections;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.UI;

public class SkinSelection : MonoBehaviour
{
    [SerializeField]
    TrackManager TrackManager;
    [SerializeField]
    Button ChooseButton;
    [SerializeField]
    GameObject StaticViewUpload;
    string[] PlayableObjects;
    int Chosen = 0;
    const float distanceStep = 0.7f;
    const int speed = 10;
    IEnumerator enumerator;

    private void Start()
    {
        int BeginWith = 2;
        if(!PlayerPrefs.HasKey("Skin"))
        {
            PlayerPrefs.SetString("Skin", "Classic");
        }

        enumerator = Move();

        GameObject[] LoadedSkins = Resources.LoadAll<GameObject>("Skins/Static");
        GameObject[] GetPlayableObjectsNames = Resources.LoadAll<GameObject>("Skins/Playable");
        PlayableObjects = new string[GetPlayableObjectsNames.Length];
        for(int i=0;i<PlayableObjects.Length;i++)
        {
            PlayableObjects[i] = GetPlayableObjectsNames[i].name;
        }
        GameObject Locked = Resources.Load<GameObject>("Skins/Locked/Locked");


        GetUserInventoryRequest items = new GetUserInventoryRequest { };
        PlayFabClientAPI.GetUserInventory(items, itemInfo =>
        {
            for (int i = 0; i < LoadedSkins.Length; i++)
            {
                bool isLocked = true;
                foreach (var eachStat in itemInfo.Inventory)
                {
                    if (LoadedSkins[i].name.Remove(LoadedSkins[i].name.Length - 6, 6) == eachStat.ItemId)
                    {
                        isLocked = false;
                    }
                }
                if (isLocked)
                {
                    GameObject ShowSkin = Instantiate(Locked, new Vector3(distanceStep * i + StaticViewUpload.transform.position.x, StaticViewUpload.transform.position.y, StaticViewUpload.transform.position.z), Quaternion.Euler(new Vector3(0,180,0)));
                    ShowSkin.name = ShowSkin.name.Remove(ShowSkin.name.Length - 7, 7);
                    ShowSkin.transform.SetParent(StaticViewUpload.transform);
                }
                else
                {
                    GameObject ShowErrorSkin = Instantiate(LoadedSkins[i], new Vector3(distanceStep * i + StaticViewUpload.transform.position.x, StaticViewUpload.transform.position.y, StaticViewUpload.transform.position.z), Quaternion.Euler(new Vector3(0, 180, 0)));
                    ShowErrorSkin.name = ShowErrorSkin.name.Remove(ShowErrorSkin.name.Length - 13, 13);
                    ShowErrorSkin.transform.SetParent(StaticViewUpload.transform);
                }

                if(StaticViewUpload.transform.GetChild(i).name == PlayerPrefs.GetString("Skin"))
                {
                    BeginWith = i;
                }
            }

            Show(BeginWith);
            Choose();
        }, itemError => { itemError.GenerateErrorReport(); });
    }

    public void Show(int index)
    {
        for (int i = 0; i< StaticViewUpload.transform.childCount; i++)
        {
            if(i==index)
            {
                StaticViewUpload.transform.GetChild(i).transform.localScale = Vector3.one;
            }
            else
            {
                StaticViewUpload.transform.GetChild(i).transform.localScale = Vector3.one / 5;
                StaticViewUpload.transform.GetChild(i).GetComponent<Skin>().ShowNormalSkin(true);
            }
        }
        if(StaticViewUpload.transform.GetChild(index).name == "Locked")
        {
            ChooseButton.interactable = false;
        }
        else
        {
            ChooseButton.interactable = true;
        }
        Chosen = index;
        StopCoroutine(enumerator);
        enumerator = Move();
        StartCoroutine(enumerator);
    }

    IEnumerator Move()
    {
        Vector3 destination = -Vector3.right * Chosen * distanceStep + Vector3.forward * StaticViewUpload.transform.position.z + Vector3.up * StaticViewUpload.transform.position.y;
        do
        {
            StaticViewUpload.transform.position = Vector3.MoveTowards(StaticViewUpload.transform.position, destination, speed*Time.deltaTime);
            yield return new WaitForFixedUpdate();
            if (StaticViewUpload.transform.position == destination)
                break;
        }
        while (true);
    }

    public void ChangeView(bool view)
    {
        StaticViewUpload.transform.GetChild(Chosen).GetComponent<Skin>().ShowNormalSkin(view);
    }
    public void Choose()
    {
        TrackManager.MyPlayer = Resources.Load<GameObject>("Skins/Playable/" + PlayableObjects[Chosen]); ;
        PlayerPrefs.SetString("Skin", PlayableObjects[Chosen]);
    }
}
*/
#endregion Old_Skins