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
    GameObject DetailsPanel;
    [SerializeField]
    TextMeshProUGUI Details;
    [SerializeField]
    GameObject BuyButton;

    [SerializeField]
    TrackManager TrackManager;
    [SerializeField]
    Button ChooseButton;
    [SerializeField]
    GameObject StaticViewUpload;
    [SerializeField]
    TextMeshProUGUI ViewModeLabel;
    string[] PlayableObjects;
    public bool[] Locked;
    int Chosen = 0;
    const float distanceStep = 0.7f;
    const int speed = 10;
    bool view = true;
    IEnumerator enumerator;
    GameObject[] SkinsView;

    public SkinDetails[] SkinDetails = new SkinDetails[]{ new SkinDetails("Agent", 15000), new SkinDetails("Blue Lightning", 40000), new SkinDetails("Classic", 0), new SkinDetails("Golden Rush", 25000), new SkinDetails("Hive", 40000),
    new SkinDetails("Junkyard Metal", 25000), new SkinDetails("Junkyard", 15000), new SkinDetails("Lolipop", "Make a single run with result over 12000 coins") ,new SkinDetails("Low Level Security", "Make a single run without power-ups with result over 10000 coins") ,new SkinDetails("Military", "Make a single run with result over 25000 coins"), new SkinDetails("Neon Agent", "Take place in top 1% in tournament"),
    new SkinDetails("Omnimon", "Take place in top 5% in tournament"), new SkinDetails("RedLine", 25000), new SkinDetails("Ruby Agent", "Take first place in tournament"), new SkinDetails("Rusty Military", "Make a single run with result over 18000 coins"), new SkinDetails("Security", "Make a single run without power-ups with result over 15000 coins"), new SkinDetails("Spaceman", "Take place in top 10% in tournament"), };


    private void Start()
    {
        Load();
    }

    public void Load()
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

        if(SkinsView!=null)
        for(int i=0;i< SkinsView.Length;i++)
        {
            GameObject toDestroy = SkinsView[i];
            SkinsView[i] = null;
            Destroy(toDestroy);
        }

        SkinsView = new GameObject[LoadedSkins.Length];

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
                SkinsView[i] = ShowSkin;

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
                view = true;
                ViewModeLabel.text = "Power-up";
                StaticViewUpload.transform.GetChild(i).GetComponent<Skin>().ShowNormalSkin(true);
            }
        }

        ChooseButton.interactable = !Locked[index];
        if (Locked[index])
        {
            ChooseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Locked";
            DetailsPanel.SetActive(true);
            if (SkinDetails[index].Cost == 0)
            {
                Details.text = SkinDetails[index].Details;
                BuyButton.SetActive(false);
            }
            else
            {
                Details.text = "Buy for " + SkinDetails[index].Cost + " coins";
                BuyButton.SetActive(true);
            }
        }
        else
        {
            ChooseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Choose";
            DetailsPanel.SetActive(false);
        }

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

    public void ChangeView()
    {
        view = !view;
        ViewModeLabel.text = view ? "Power-up" : "Normal";
        StaticViewUpload.transform.GetChild(Chosen).GetComponent<Skin>().ShowNormalSkin(view);
    }
    public void Choose()
    {
        TrackManager.MyPlayer = Resources.Load<GameObject>("Skins/Playable/" + PlayableObjects[Chosen]); ;
        PlayerPrefs.SetString("Skin", PlayableObjects[Chosen]);
    }
    
    public void GrantSkin()
    {
        var purchaseRequest = new PurchaseItemRequest();
            purchaseRequest.CatalogVersion = "Skins";
            purchaseRequest.ItemId = SkinDetails[Chosen].Name+"Bundle";
            purchaseRequest.VirtualCurrency = "ST";
            purchaseRequest.Price = 0;
            PlayFabClientAPI.PurchaseItem(purchaseRequest, result => { Debug.Log(SkinDetails[Chosen].Name +" added"); }, error=> { Debug.Log(SkinDetails[Chosen].Name + " was not added added"); });
    }
    public void BuySkin()
    {
        PlayFabClientAPI.GetPlayerStatistics(
new GetPlayerStatisticsRequest(),
SatoshiInfo =>
{
    foreach (var eachStat in SatoshiInfo.Statistics)
    {
        switch (eachStat.StatisticName)
        {
            case "Satoshi":
                if (eachStat.Value >= SkinDetails[Chosen].Cost)
                {
                    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                    {
                        FunctionName = "UpdateSatoshi", // Arbitrary function name (must exist in your uploaded cloud.js file)
                        FunctionParameter = new { SatoshiValue = -SkinDetails[Chosen].Cost }, // The parameter provided to your function
                        GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
                    }, nothing1 => { }, nothing2 => { });

                    GrantSkin();

                    Locked[Chosen] = false;
                    ChooseButton.interactable = !Locked[Chosen];
                    ChooseButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Choose";
                    DetailsPanel.SetActive(false);
                }
                break;
        }
    }
},
error => Debug.LogError(error.GenerateErrorReport()));
    }
}

public class SkinDetails
{
    public string Name { get; set; }
    public string Details { get; set; }
    public int Cost { get; set; }

    public SkinDetails(string name, string details)
    {
        Name = name;
        Details = details;
        Cost = 0;
    }
    public SkinDetails(string name, int cost)
    {
        Name = name;
        Details = "";
        Cost = cost;
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