﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Launcher.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in "PUN Basic tutorial" to connect, and join/create room automatically
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

using NaughtyAttributes;
using System.Collections.Generic;
using GuruLaghima;

namespace Workbench.ProjectDilemma
{
#pragma warning disable 649

  /// <summary>
  /// Launch manager. Connect, join a random room or create one if none or all full.
  /// </summary>
  public class LauncherScript : MonoBehaviourPunCallbacks
  {
    #region public fields
    public static LauncherScript instance;

    public static bool introPlayed = false;

    #endregion

    #region Private Serializable Fields
    [SerializeField] List<GameObject> introObjects = new List<GameObject>();

    [Tooltip("The Ui Text to inform the user about the connection progress")]
    [SerializeField]
    Text feedbackText;

    [Tooltip("The maximum number of players per room")]
    [SerializeField]
    byte maxPlayersPerRoom = 2;

    [Tooltip("The input field for the nickname")]
    [SerializeField]
    TMP_InputField _inputField;
    [SerializeField]
    TMP_Text idCardNickname;
    [SerializeField]
    TMP_Text idCardXPLabel;
    [SerializeField]
    Slider idCardXPSlider;
    [SerializeField]
    TMP_Text idCardRank;
    [SerializeField]
    TMP_Text idCardPoints;

    [Tooltip("The input field for the nickname")]
    [SerializeField]
    Button quickMatchButton;
    [Tooltip("The input field for the nickname")]
    [SerializeField]
    [Dropdown("Regions")]
    string region;
    private List<string> Regions
    {
      get
      {
        return new List<string>() {
      "asia",
      "au",
      "cae",
      "cn",
      "eu",
      "in",
      "jp",
      "ru",
      "rue",
      "za",
      "sa",
      "kr",
      "tr",
      "us",
      "usw"
     };
      }
    }

    [SerializeField] float loadScenarioDelay;

#if UNITY_EDITOR
    [Scene]
#endif
    [SerializeField] string debugScenario;

    [SerializeField] bool loadDebugScenario = false;
    #endregion

    #region Private Fields
    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";

    RoomOptions roomOptions;
    AppSettings appSettings;

    #endregion

    #region Unity Events
    [Foldout("Visual Feedback Events")]
    public UnityEvent OnClickedOnQuickMatch;
    [Foldout("Visual Feedback Events")]
    public UnityEvent OnConnectedEvent;
    [Foldout("Visual Feedback Events")]
    public UnityEvent OnJoinedRoomEvent;
    [Foldout("Visual Feedback Events")]
    public UnityEvent OnSecondPlayerEnteredRoomEvent;
    [Foldout("Visual Feedback Events")]
    public UnityEvent OnFailedToFindRandomRoomEvent;
    [Foldout("Visual Feedback Events")]
    public UnityEvent OnDisconnectedEvent;
    private Player otherPlayer;
    private static int lastScenario;
    #endregion


    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
      // singleton code
      if (instance == null)
        instance = this;
      else
        Destroy(this.gameObject);

      // #Critical
      // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
      PhotonNetwork.AutomaticallySyncScene = true;

      // initializations
      appSettings = new AppSettings();
      appSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
      appSettings.AppVersion = gameVersion;
      appSettings.FixedRegion = region;
      roomOptions = new RoomOptions();
      roomOptions.IsVisible = true;
      roomOptions.PublishUserId = true;
      roomOptions.MaxPlayers = maxPlayersPerRoom; // should be exposed as option for master clients
      roomOptions.CustomRoomPropertiesForLobby = new string[] { Keys.MAP_PROP_KEY };

      _inputField.onValueChanged.AddListener(delegate (string m) { SetPlayerName(m); }); // for some reason dropdowns override their default behaviour if given listener via code
      quickMatchButton.onClick.AddListener(delegate () { QuickMatch(); }); // for some reason dropdowns override their default behaviour if given listener via code
    }
    public override void OnEnable()
    {
      base.OnEnable();

      if (!introPlayed)
        introPlayed = true;
      else
        introObjects.ForEach((obj) => { obj.SetActive(false); });

    }
    public override void OnDisable()
    {
      base.OnDisable();

    }

    private void Start()
    {

      // string defaultName = (Input.mousePosition.x * Input.mousePosition.y).ToString();
      string defaultName = "";

      if (_inputField != null)
      {
        if (PlayerPrefs.HasKey(Keys.PLAYER_NAME))
        {
          defaultName = PlayerPrefs.GetString(Keys.PLAYER_NAME);
          _inputField.text = defaultName;
        }
      }
      // initialize idcard details
      if (PlayerPrefs.HasKey(Keys.PLAYER_NAME))
      {
        idCardNickname.text = PlayerPrefs.GetString(Keys.PLAYER_NAME);
      }
      int xp = PlayerPrefs.GetInt(Keys.PLAYER_XP, 0);
      int accumulatedLevelThreshold = 0;
      int currentLevel = 0;
      for (int i = 0; i < MiscelaneousSettings.Instance.levelsDistribution.Count; i++)
      {
        int levelThreshold = MiscelaneousSettings.Instance.levelsDistribution[i];
        accumulatedLevelThreshold += levelThreshold;
        if (xp >= accumulatedLevelThreshold)
        {
          currentLevel = i;
        }
        else
        {
          break;
        }
      }
      idCardXPSlider.minValue = 0;
      idCardXPSlider.maxValue = MiscelaneousSettings.Instance.levelsDistribution[currentLevel];
      idCardXPSlider.value = MiscelaneousSettings.Instance.levelsDistribution[currentLevel + 1] - (accumulatedLevelThreshold - xp);
      idCardXPLabel.text = "Level " + currentLevel;
      idCardRank.text = "Rank. " + PlayerPrefs.GetInt(Keys.PLAYER_RANK, 0).ToString();
      idCardPoints.text = "Points: " + PlayerPrefs.GetInt(Keys.PLAYER_POINTS_PREF, 0).ToString();


      // we are appending a deliminator and a random color to prevent duplicate of nicknames (because my colored chat system depends on unique nicknames)
      PhotonNetwork.NickName = defaultName + "#" + Random.ColorHSV();
    }

    bool muted;
    private void Update()
    {
      // debug commands
      if (Input.GetKey("left ctrl"))
      {

        if (Input.GetKeyDown("h"))
        {
          MyDebug.Log("Clear player prefs", "h");
          PlayerPrefs.DeleteAll();
        }
        if (Input.GetKeyDown("m"))
        {
          MyDebug.Log("Mute shit", "l");
          muted = !muted;
          Object[] objs = GameObject.FindObjectsOfType(typeof(AudioListener));
          foreach (Object item in objs)
          {
            MyDebug.Log("Mute shit", item.name);
            ((AudioListener)item).enabled = false;
          }
        }
      }
    }

    #endregion


    #region Public Methods

    /// <summary>
    /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
    /// </summary>
    /// <param name="value">The name of the Player</param>
    public void SetPlayerName(string value)
    {
      // #Important
      if (string.IsNullOrEmpty(value))
      {
        MyDebug.LogError("Player Name is null or empty");
        return;
      }
      PhotonNetwork.NickName = value + "#" + Random.ColorHSV();
      idCardNickname.text = value; // sync id card nickname as well

      PlayerPrefs.SetString(Keys.PLAYER_NAME, value);
    }

    public void QuickMatcWithDelay(float delay)
    {
      if (!isConnecting)
        Invoke("QuickMatch", delay);
    }

    /// <summary>
    /// Start the connection process.
    /// </summary>
    public void QuickMatch()
    {
      if (feedbackText)
        // we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
        feedbackText.text = "";

      // keep track of the will to join a room, because when we come back from the game we will get a callback that we are connected, so we need to know what to do then
      isConnecting = true;


      // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
      if (PhotonNetwork.IsConnected)
      {
        JoinRandomRoom();
      }
      else
      {
        ConnectToMaster();
      }

      OnClickedOnQuickMatch?.Invoke();
    }

    public void Disconnect()
    {
      PhotonNetwork.Disconnect();
    }


    #endregion


    #region MonoBehaviourPunCallbacks CallBacks
    // below, we implement some callbacks of PUN
    // you can find PUN's callbacks in the class MonoBehaviourPunCallbacks


    /// <summary>
    /// Called after the connection to the master is established and authenticated
    /// </summary>
    public override void OnConnectedToMaster()
    {
      LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
      MyDebug.Log("Connected to region: " + PhotonNetwork.CloudRegion);

      JoinRandomRoom();

      // visual stuff
      OnConnectedEvent?.Invoke();
    }

    /// <summary>
    /// Called when a JoinRandom() call failed. The parameter provides ErrorCode and message.
    /// </summary>
    /// <remarks>
    /// Most likely all rooms are full or no rooms are available. <br/>
    /// </remarks>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
      LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");

      CreateRoom();

      // visual stuff
      OnFailedToFindRandomRoomEvent?.Invoke();
    }

    /// <summary>
    /// Called when entering a room (by creating or joining it). Called on all clients (including the Master Client).
    /// </summary>
    /// <remarks>
    /// </remarks>
    public override void OnJoinedRoom()
    {
      LogFeedback("<Color=Green>OnJoinedRoom</Color> with " + PhotonNetwork.CurrentRoom.PlayerCount + " Player(s)");
      OnJoinedRoomEvent?.Invoke();

      // send points info about player
      Hashtable temphashtable = new Hashtable();
      temphashtable.Add(Keys.PLAYER_POINTS, PlayerPrefs.GetInt(Keys.PLAYER_POINTS_PREF, 0));
      PhotonNetwork.LocalPlayer.SetCustomProperties(temphashtable);

      if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
      {
        // visual stuff. probably for triggering transition between main menu and scenario
        OnSecondPlayerEnteredRoomEvent?.Invoke();
      }


      if (PhotonNetwork.IsMasterClient)
      {
        // mark this room as available to be joined
        Hashtable ht = new Hashtable {
        { Keys.MAP_PROP_KEY, "in_queue"}
         };
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
      }
    }

    /// <summary>
    /// Note: this method is not called on the client that entered the room
    /// </summary>
    /// <param name="player"></param>
    public override void OnPlayerEnteredRoom(Player player)
    {
      otherPlayer = player;

      Hashtable temphashtable = new Hashtable();

      if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
      {
        // visual stuff. probably for triggering transition between main menu and scenario
        OnSecondPlayerEnteredRoomEvent?.Invoke();
      }

      // #Critical: We only load level if the second player entered too
      if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
      {
        // determine who sits where (player number can be  1 or 2)
        int my_player_number = Random.Range(0, 2);
        temphashtable = new Hashtable();
        temphashtable.Add(Keys.PLAYER_NUMBER, (my_player_number) + 1);
        PhotonNetwork.LocalPlayer.SetCustomProperties(temphashtable);
        temphashtable = new Hashtable();
        temphashtable.Add(Keys.PLAYER_NUMBER, (1 - my_player_number) + 1);
        otherPlayer.SetCustomProperties(temphashtable);

        // here we make sure other clients use master client room settings
        Hashtable ht = new Hashtable {
        { Keys.MAP_PROP_KEY, "in_game"}
         };
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        PhotonNetwork.CurrentRoom.IsOpen = false; // other players shouldnt be able to join it after the game started

        // 

        Invoke("LoadRandomScenario", loadScenarioDelay);
      }
    }

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
      LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);

      OnDisconnectedEvent?.Invoke();

      // clear photon properties so next game starts with clean slate
      if (PhotonNetwork.IsMasterClient)
      {
        PhotonNetwork.CurrentRoom.SetCustomProperties(ResetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties));
      }
      PhotonNetwork.LocalPlayer.SetCustomProperties(ResetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties));

      isConnecting = false;
    }

    #endregion

    #region private methods

    void ConnectToMaster()
    {
      LogFeedback("Connecting...");

      // #Critical, we must first and foremost connect to Photon Online Server.
      PhotonNetwork.ConnectUsingSettings();
      PhotonNetwork.GameVersion = this.gameVersion;
    }

    void JoinRandomRoom()
    {
      LogFeedback("Joining Random Room...");

      // we don't want to do anything if we are not attempting to join a room. 
      // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
      // we don't want to do anything.
      if (isConnecting)
      {
        Hashtable expectedCustomRoomProperties = new Hashtable { { Keys.MAP_PROP_KEY, "in_queue" } };
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 2);
      }
    }

    private void CreateRoom()
    {
      // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
      PhotonNetwork.CreateRoom(null, roomOptions);
    }

    private void LoadRandomScenario()
    {
      // determine scenario to load
      // string scenarioName = PickRandomScenario();
      string scenarioName = RotateScenarios();
      MyDebug.Log("We are loading ", scenarioName);

      // #Critical
      // Load the Room Level. 
      PhotonNetwork.LoadLevel(scenarioName);
    }

    private string RotateScenarios()
    {
      if (loadDebugScenario)
        return debugScenario;

      lastScenario = lastScenario == 1 ? 0 : 1;
      string chosenScenario = "";

      if (ScenariosManager.Instance.approvedScenariosNames.Count > 0)
      {
        chosenScenario = ScenariosManager.Instance.approvedScenariosNames[lastScenario];
      }
      return chosenScenario;
    }

    private string PickRandomScenario()
    {
      if (loadDebugScenario)
        return debugScenario;

      int randomInt = TotallyRandomNumber(0, ScenariosManager.Instance.approvedScenariosNames.Count);
      string chosenScenario = "";

      if (ScenariosManager.Instance.approvedScenariosNames.Count > 0)
      {
        chosenScenario = ScenariosManager.Instance.approvedScenariosNames[randomInt];
      }
      return chosenScenario;
    }

    /// <summary>
    /// Returns a random integer between min(included) and max(excluded)
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private int TotallyRandomNumber(int min, int max)
    {
      return UnityEngine.Random.Range(min, max);
    }

    /// <summary>
    /// Logs the feedback in the UI view for the player, as opposed to inside the Unity Editor for the developer.
    /// </summary>
    /// <param name="message">Message.</param>
    void LogFeedback(string message)
    {
      MyDebug.Log(message, Color.cyan);

      // we do not assume there is a feedbackText defined.
      if (feedbackText == null)
      {
        return;
      }

      // add new messages as a new line and at the bottom of the log.
      feedbackText.text += System.Environment.NewLine + message;
    }

    Hashtable ResetCustomProperties(Hashtable properties)
    {
      Hashtable temphashtable = new Hashtable();
      foreach (var item in properties)
      {
        temphashtable.Add(item.Key, null);
      }
      return temphashtable;
    }

    #endregion

  }
}