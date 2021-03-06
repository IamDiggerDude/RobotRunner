﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

/// <summary>
/// The TrackManager handles creating track segments, moving them and handling the whole pace of the game.
/// 
/// The cycle is as follows:
/// - Begin is called when the game starts.
///     - if it's a first run, init the controller, collider etc. and start the movement of the track.
///     - if it's a rerun (after watching ads on GameOver) just restart the movement of the track.
/// - Update moves the character and - if the character reaches a certain distance from origin (given by floatingOriginThreshold) -
/// moves everything back by that threshold to "reset" the player to the origin. This allow to avoid floating point error on long run.
/// It also handles creating the tracks segements when needed.
/// 
/// If the player has no more lives, it pushes the GameOver state on top of the GameState without removing it. That way we can just go back to where
/// we left off if the player watches an ad and gets a second chance. If the player quits, then:
/// 
/// - End is called and everything is cleared and destroyed, and we go back to the Loadout State.
/// </summary>
public class TrackManager : MonoBehaviour
{
    public GameObject MyPlayer;
    public MeshFilter MySky;
    public ThemeData MyTheme;

	static public TrackManager instance { get { return s_Instance; } }
	static protected TrackManager s_Instance;

    static int s_StartHash = Animator.StringToHash("Start");

	public delegate int MultiplierModifier(int current);
	public MultiplierModifier modifyMultiply;

	[Header("Character & Movements")]
	public CharacterInputController characterController;
    public CharacterCollider characterColl;
	public float minSpeed = 5.0f;
	public float maxSpeed = 10.0f;
	public int speedStep = 4;
	public float laneOffset = 1.0f;

	public bool invincible = false;

	[Header("Objects")]
	public ConsumableDatabase consumableDatabase;
	public MeshFilter skyMeshFilter;

	[Header("Parallax")]
	public Transform parallaxRoot;
	public float parallaxRatio = 0.5f;

	public int trackSeed {  get { return m_TrackSeed; } set { m_TrackSeed = value; } }

    public float timeToStart { get { return m_TimeToStart; } }  // Will return -1 if already started (allow to update UI)

	public int score { get { return m_Score; } }
	public int multiplier {  get { return m_Multiplier; } }
	public float worldDistance {  get { return m_TotalWorldDistance; } }
	public float speed {  get { return m_Speed; } }
	public float speedRatio {  get { return (m_Speed - minSpeed) / (maxSpeed - minSpeed); } }

	public TrackSegment currentSegment { get { return m_Segments[0]; } }
	public List<TrackSegment> segments { get { return m_Segments; } }
	public ThemeData currentTheme { get { return m_CurrentThemeData; } }

	public bool isMoving {  get { return m_IsMoving; } }
	public bool isRerun { get { return m_Rerun; } set { m_Rerun = value; } }

	protected float m_TimeToStart = -1.0f;

	// If this is set to -1, random seed is init to system clock, otherwise init to that value
	// Allow to play the same game multiple time (useful to make specific competition/challenge fair between players)
	protected int m_TrackSeed = -1;

	protected float m_CurrentSegmentDistance;
	protected float m_TotalWorldDistance;
	protected bool m_IsMoving;
	protected float m_Speed;

    protected float m_TimeSincePowerup;     // The higher it goes, the higher the chance of spawning one
	protected float m_TimeSinceLastPremium;

	protected int m_Multiplier;

	protected List<TrackSegment> m_Segments = new List<TrackSegment>();
	protected List<TrackSegment> m_PastSegments = new List<TrackSegment>();
	protected int m_SafeSegementLeft;

	protected ThemeData m_CurrentThemeData;
	protected int m_CurrentZone;
	protected float m_CurrentZoneDistance;
	protected int m_PreviousSegment = -1;

	protected int m_Score;
	protected float m_ScoreAccum;
    protected bool m_Rerun;     // This lets us know if we are entering a game over (ads) state or starting a new game (see GameState)

    const float k_FloatingOriginThreshold = 10000f;

    protected const float k_CountdownToStartLength = 5f;
    protected const float k_CountdownSpeed = 1.5f;
    protected const float k_StartingSegmentDistance = 2f;
    protected const int k_StartingSafeSegments = 2;
    protected const int k_StartingCoinPoolSize = 256;
    protected const int k_DesiredSegmentCount = 3;
    protected const float k_SegmentRemovalDistance = -80f;
    protected const float k_Acceleration = 0.2f;

    protected void Awake()
	{
        m_ScoreAccum = 0.0f;
		s_Instance = this;
    }

    public void StartMove(bool isRestart = true)
	{
		m_IsMoving = true;
		if(isRestart)
			m_Speed = minSpeed;
	}

	public void StopMove()
	{
		m_IsMoving = false;
	}

	IEnumerator WaitToStart()
	{
		characterController.character.animator.Play(s_StartHash);
		float length = k_CountdownToStartLength;
		m_TimeToStart = length;

		while(m_TimeToStart >= 0)
		{
			yield return null;
			m_TimeToStart -= Time.deltaTime * k_CountdownSpeed;
		}

		m_TimeToStart = -1;

		if (m_Rerun)
		{
			// Make invincible on rerun, to avoid problems if the character died in front of an obstacle
			characterController.characterCollider.SetInvincible();
		}

		characterController.StartRunning();
		StartMove();
	}

	public void Begin()
	{
        Debug.Log("Track Manager 1");
		if (!m_Rerun)
        {
            Debug.Log("Track Manager 2");
            if (m_TrackSeed != -1)
            {
                Debug.Log("Track Manager 3.1");
                Random.InitState(m_TrackSeed);
            }
            else
            {
                Debug.Log("Track Manager 3.2");
                Random.InitState((int)System.DateTime.Now.Ticks);
            }

            Debug.Log("Track Manager 4");
            // Since this is not a rerun, init the whole system (on rerun we want to keep the states we had on death)
            m_CurrentSegmentDistance = k_StartingSegmentDistance;
            Debug.Log("Track Manager 5");
            m_TotalWorldDistance = 0.0f;
            Debug.Log("Track Manager 6");

            characterController.gameObject.SetActive(true);
            Debug.Log("Track Manager 7");

            // Spawn the player
            Character player = Instantiate(MyPlayer.transform.GetChild(5).GetComponent<Character>()/*CharacterDatabase.GetCharacter(PlayerData.instance.characters[PlayerData.instance.usedCharacter])*/, Vector3.zero, Quaternion.identity);
            Debug.Log("Track Manager 8");
            player.transform.SetParent(characterController.characterCollider.transform, false);
            Debug.Log("Track Manager 9");
            Camera.main.transform.SetParent(characterController.transform, true);
            Debug.Log("Track Manager 10");


            player.SetupAccesory(PlayerData.instance.usedAccessory);
            Debug.Log("Track Manager 11");

            characterController.character = player;
            Debug.Log("Track Manager 12");
            characterColl.character = player;
            Debug.Log("Track Manager 13");
            characterController.trackManager = this;
            Debug.Log("Track Manager 14");

            characterController.Init();
            Debug.Log("Track Manager 15");
            characterController.CheatInvincible(invincible);
            Debug.Log("Track Manager 16");

            m_CurrentThemeData = MyTheme; //ThemeDatabase.GetThemeData(PlayerData.instance.themes[PlayerData.instance.usedTheme]);

            foreach(var th in PlayerData.instance.themes)
            {
                Debug.Log(th);
            }

            Debug.Log("Track Manager 17");
            m_CurrentZone = 0;
            Debug.Log("Track Manager 18");
            m_CurrentZoneDistance = 0;
            Debug.Log("Track Manager 19");

            skyMeshFilter.sharedMesh = m_CurrentThemeData.skyMesh;//MySky.mesh;
            Debug.Log("Track Manager 20");
            RenderSettings.fogColor = m_CurrentThemeData.fogColor;//Color.black;
            Debug.Log("Track Manager 21");
            RenderSettings.fog = true;
            Debug.Log("Track Manager 22");

            gameObject.SetActive(true);
            Debug.Log("Track Manager 23");
            characterController.gameObject.SetActive(true);
            Debug.Log("Track Manager 24");
            characterController.coins = 0;
            Debug.Log("Track Manager 25");
            characterController.premium = 0;
            Debug.Log("Track Manager 26");

            m_Score = 0;
            Debug.Log("Track Manager 27");
            m_ScoreAccum = 0;
            Debug.Log("Track Manager 28");

            m_SafeSegementLeft = k_StartingSafeSegments;
            Debug.Log("Track Manager 29");

            Coin.coinPool = new Pooler(MyTheme.collectiblePrefab/*currentTheme.collectiblePrefab*/, k_StartingCoinPoolSize);
            Debug.Log("Track Manager 30");

            PlayerData.instance.StartRunMissions(this);
            Debug.Log("Track Manager 31");

/*#if UNITY_ANALYTICS
            AnalyticsEvent.GameStart(new Dictionary<string, object>
            {
                { "theme", m_CurrentThemeData.themeName},
                { "character", player.characterName },
                { "accessory",  PlayerData.instance.usedAccessory >= 0 ? player.accessories[PlayerData.instance.usedAccessory].accessoryName : "none"}
            });
#endif
            Debug.Log("Track Manager 32");*/
        }
        Debug.Log("Track Manager 33");

        characterController.Begin();
        Debug.Log("Track Manager 34");
        StartCoroutine(WaitToStart());
        Debug.Log("Track Manager 35");
    }

	public void End()
	{
	    foreach (TrackSegment seg in m_Segments)
	    {
	        Destroy(seg.gameObject);
	    }

	    for (int i = 0; i < m_PastSegments.Count; ++i)
	    {
	        Destroy(m_PastSegments[i].gameObject);
	    }

		m_Segments.Clear();
		m_PastSegments.Clear();

		characterController.End();

		gameObject.SetActive(false);
		Destroy(characterController.character.gameObject);
		characterController.character = null;

        Camera.main.transform.SetParent(null);

        characterController.gameObject.SetActive(false);

		for (int i = 0; i < parallaxRoot.childCount; ++i) 
		{
			Destroy (parallaxRoot.GetChild(i).gameObject);
		}

		//if our consumable wasn't used, we put it back in our inventory
		if (characterController.inventory != null) 
		{
            PlayerData.instance.Add(characterController.inventory.GetConsumableType());
			characterController.inventory = null;
		}
	}


	void Update ()
    {
        while (m_Segments.Count < k_DesiredSegmentCount)
		{
			SpawnNewSegment();
        }

        if (parallaxRoot != null && currentTheme.cloudPrefabs.Length > 0)
        {
            while (parallaxRoot.childCount < currentTheme.cloudNumber)
			{
				float lastZ = parallaxRoot.childCount == 0 ? 0 : parallaxRoot.GetChild(parallaxRoot.childCount - 1).position.z + currentTheme.cloudMinimumDistance.z;

				GameObject obj = Instantiate(currentTheme.cloudPrefabs[Random.Range(0, currentTheme.cloudPrefabs.Length)]);
				obj.transform.SetParent(parallaxRoot, false);

				obj.transform.localPosition = 
					Vector3.up * (currentTheme.cloudMinimumDistance.y + (Random.value - 0.5f) * currentTheme.cloudSpread.y) 
					+ Vector3.forward * (lastZ  + (Random.value - 0.5f) * currentTheme.cloudSpread.z)
					+ Vector3.right * (currentTheme.cloudMinimumDistance.x + (Random.value - 0.5f) * currentTheme.cloudSpread.x);

				obj.transform.localScale = obj.transform.localScale * (1.0f + (Random.value - 0.5f) * 0.5f);
				obj.transform.localRotation = Quaternion.AngleAxis(Random.value * 360.0f, Vector3.up);
			}
        }

        if (!m_IsMoving)
			return;

        float scaledSpeed = m_Speed * Time.deltaTime;
        m_ScoreAccum += scaledSpeed;
        m_CurrentZoneDistance += scaledSpeed;

        int intScore = Mathf.FloorToInt(m_ScoreAccum);
        if (intScore != 0) AddScore(intScore);
        m_ScoreAccum -= intScore;

        m_TotalWorldDistance += scaledSpeed;
        m_CurrentSegmentDistance += scaledSpeed;

        if (m_CurrentSegmentDistance > m_Segments[0].worldLength)
        {
            m_CurrentSegmentDistance -= m_Segments[0].worldLength;

			// m_PastSegments are segment we already passed, we keep them to move them and destroy them later 
			// but they aren't part of the game anymore 
			m_PastSegments.Add(m_Segments[0]);
			m_Segments.RemoveAt(0);

		}

        Vector3 currentPos;
        Quaternion currentRot;
        Transform characterTransform = characterController.transform;

        m_Segments[0].GetPointAtInWorldUnit(m_CurrentSegmentDistance, out currentPos, out currentRot);


        // Floating origin implementation
        // Move the whole world back to 0,0,0 when we get too far away.
        bool needRecenter = currentPos.sqrMagnitude > k_FloatingOriginThreshold;

        // Parallax Handling
        if (parallaxRoot != null)
        {
            Vector3 difference = (currentPos - characterTransform.position) * parallaxRatio;
            int count = parallaxRoot.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform cloud = parallaxRoot.GetChild(i);
				cloud.position += difference - (needRecenter ? currentPos : Vector3.zero);
			}
        }

        if (needRecenter)
        {
            int count = m_Segments.Count;
            for (int i = 0; i < count; i++)
            {
				m_Segments[i].transform.position -= currentPos;
            }

            count = m_PastSegments.Count;
            for (int i = 0; i < count; i++)
            {
				m_PastSegments[i].transform.position -= currentPos;
            }

            // Recalculate current world position based on the moved world
            m_Segments[0].GetPointAtInWorldUnit(m_CurrentSegmentDistance, out currentPos, out currentRot);
        }

        characterTransform.rotation = currentRot;
        characterTransform.position = currentPos;

        if (parallaxRoot != null && currentTheme.cloudPrefabs.Length > 0)
        {
            for (int i = 0; i < parallaxRoot.childCount; ++i)
			{
				Transform child = parallaxRoot.GetChild(i);

				// Destroy unneeded clouds
				if ((child.localPosition - currentPos).z < -50)
					Destroy(child.gameObject);
			}
        }

        // Still move past segment until they aren't visible anymore.
        for (int i = 0; i < m_PastSegments.Count; ++i)
        {
            if ((m_PastSegments[i].transform.position - currentPos).z < k_SegmentRemovalDistance)
			{
				m_PastSegments[i].Cleanup();
				m_PastSegments.RemoveAt(i);
				i--;
			}
        }

        PowerupSpawnUpdate();

        if (m_Speed < maxSpeed)
            m_Speed += k_Acceleration * Time.deltaTime;
		else
			m_Speed = maxSpeed;

        m_Multiplier = 1 + Mathf.FloorToInt((m_Speed - minSpeed) / (maxSpeed - minSpeed) * speedStep);

        if (modifyMultiply != null)
        {
            foreach (MultiplierModifier part in modifyMultiply.GetInvocationList())
            {
                m_Multiplier = part(m_Multiplier);
            }
        }

        //check for next rank achieved
        int currentTarget = (PlayerData.instance.rank + 1) * 300;
        if (m_TotalWorldDistance > currentTarget)
        {
            PlayerData.instance.rank += 1;
            PlayerData.instance.Save();
#if UNITY_ANALYTICS
            //"level" in our game are milestone the player have to reach : one every 300m
            AnalyticsEvent.LevelUp(PlayerData.instance.rank);
#endif
        }

        PlayerData.instance.UpdateMissions(this);
        MusicPlayer.instance.UpdateVolumes(speedRatio);
    }

    public void PowerupSpawnUpdate()
	{
		m_TimeSincePowerup += Time.deltaTime;
		m_TimeSinceLastPremium += Time.deltaTime;
	}

	public void SpawnNewSegment()
    {
        if (m_CurrentThemeData.zones[m_CurrentZone].length < m_CurrentZoneDistance)
        {
            m_CurrentZone += 1;
			if (m_CurrentZone >= m_CurrentThemeData.zones.Length)
				m_CurrentZone = 0;

			m_CurrentZoneDistance = 0;
        }

        int segmentUse = Random.Range(0, m_CurrentThemeData.zones[m_CurrentZone].prefabList.Length);
        if (segmentUse == m_PreviousSegment) segmentUse = (segmentUse + 1) % m_CurrentThemeData.zones[m_CurrentZone].prefabList.Length;

        TrackSegment segmentToUse = m_CurrentThemeData.zones[m_CurrentZone].prefabList[segmentUse];
        TrackSegment newSegment = Instantiate(segmentToUse, Vector3.zero, Quaternion.identity);

        foreach (Transform part in newSegment.transform)
        {
            if (part.name != "ObjectRoot")
            {
                part.gameObject.isStatic = true;
                foreach (Transform childpart in part)
                {
                    childpart.gameObject.isStatic = true;
                    foreach (Transform childchildpart in childpart)
                    {
                        childchildpart.gameObject.isStatic = true;
                    }
                }
            }
        }



        Vector3 currentExitPoint;
        Quaternion currentExitRotation;
        if (m_Segments.Count > 0)
        {
            m_Segments[m_Segments.Count - 1].GetPointAt(1.0f, out currentExitPoint, out currentExitRotation);
		}
		else
        {
            currentExitPoint = transform.position;
			currentExitRotation = transform.rotation;
		}

        newSegment.transform.rotation = currentExitRotation;

        Vector3 entryPoint;
        Quaternion entryRotation;
        newSegment.GetPointAt(0.0f, out entryPoint, out entryRotation);


        Vector3 pos = currentExitPoint + (newSegment.transform.position - entryPoint);
        newSegment.transform.position = pos;
        newSegment.manager = this;

        newSegment.transform.localScale = new Vector3((/*Random.value > 0.5f ? -1 :*/ 1), 1, 1);
        newSegment.objectRoot.localScale = new Vector3(1.0f/newSegment.transform.localScale.x, 1, 1);

        if (m_SafeSegementLeft <= 0)
        {
            SpawnObstacle(newSegment);
        }
        else
        {
            m_SafeSegementLeft -= 1;
        }

        m_Segments.Add(newSegment);
    }


	public void SpawnObstacle(TrackSegment segment)
	{
		if (segment.possibleObstacles.Length != 0)
		{
			for (int i = 0; i < segment.obstaclePositions.Length; ++i)
			{
				segment.possibleObstacles[Random.Range(0, segment.possibleObstacles.Length)].Spawn(segment, segment.obstaclePositions[i]);
			}
		}

		SpawnCoinAndPowerup(segment);
	}

	public void SpawnCoinAndPowerup(TrackSegment segment)
	{
		const float increment = 1.5f;
		float currentWorldPos = 0.0f;
		int currentLane = Random.Range(0,3);
        int q = 0;
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), OnGetStats, error => Debug.LogError(error.GenerateErrorReport()));
        void OnGetStats(GetPlayerStatisticsResult result)
        {

            foreach (var eachStat in result.Statistics)
            {

                switch (eachStat.StatisticName)
                {
                    case "BBTC":
                        q = eachStat.Value;
                        break;
                }
            }
        }

        float powerupChance = Mathf.Clamp01(Mathf.Floor(m_TimeSincePowerup) * 0.5f * 0.001f);
		float premiumChance = Mathf.Clamp01(q * 50/100 * 0.5f * 0.0001f);

		while (currentWorldPos < segment.worldLength)
		{
			Vector3 pos;
			Quaternion rot;
			segment.GetPointAtInWorldUnit(currentWorldPos, out pos, out rot);


			bool laneValid = true;
			int testedLane = currentLane;
			while(Physics.CheckSphere(pos + ((testedLane - 1) * laneOffset * (rot*Vector3.right)), 0.4f, 1<<9))
			{
				testedLane = (testedLane + 1) % 3;
				if (currentLane == testedLane)
				{
                    // Couldn't find a valid lane.
					laneValid = false;
					break;
				}
			}

			currentLane = testedLane;

			if(laneValid)
			{
				pos = pos + ((currentLane - 1) * laneOffset * (rot * Vector3.right));


                GameObject toUse;
				if (Random.value < powerupChance)
				{
                    int picked = Random.Range(0, consumableDatabase.consumbales.Length);

                    //if the powerup can't be spawned, we don't reset the time since powerup to continue to have a high chance of picking one next track segment
                    if (consumableDatabase.consumbales[picked].canBeSpawned)
                    {
                        // Spawn a powerup instead.
                        m_TimeSincePowerup = 0.0f;
                        powerupChance = 0.0f;

                        toUse = Instantiate(consumableDatabase.consumbales[picked].gameObject, pos, rot) as GameObject;
                        toUse.transform.SetParent(segment.transform, true);
                    }
				}
				else if (Random.value < premiumChance)
				{
					m_TimeSinceLastPremium = 0.0f;
					premiumChance = 0.0f;

					toUse = Instantiate(currentTheme.premiumCollectible, pos, rot);
					toUse.transform.SetParent(segment.transform, true);
				}
				else
				{
					toUse = Coin.coinPool.Get(pos, rot);
					toUse.transform.SetParent(segment.collectibleTransform, true);
				}

				
			}

			currentWorldPos += increment;
		}

	}


    public void AddScore(int amount)
	{
		int finalAmount = amount;
		m_Score += finalAmount * m_Multiplier;

        //PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        //{
        //    FunctionName = "UpdateLeaderboard", // Arbitrary function name (must exist in your uploaded cloud.js file)
        //    FunctionParameter = new { BBTCValue = m_Score.ToString() , BBTCHighscoreValue = m_Score.ToString()}, // The parameter provided to your function
        //    GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        //}, OnCloudUpdateStats, OnErrorShared);


    }

    private static void OnCloudUpdateStats(ExecuteCloudScriptResult result)
    {

    }

    private static void OnErrorShared(PlayFabError error)
    {
    }
}