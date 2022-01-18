// --------------------------------------------------------------------------------------------------------------------
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
using TMPro;

using Photon.Realtime;
using Photon.Pun;

using HelperScripts;
using NaughtyAttributes;
using System;
using UnityEngine.Events;

namespace Workbench.ProjectDilemma
{
#pragma warning disable 649

  /// <summary>
  /// Launch manager. Connect, join a random room or create one if none or all full.
  /// </summary>
  public class LauncherScript : MonoBehaviourPunCallbacks
  {
    #region public fields


    #endregion

    #region Private Serializable Fields

    [Tooltip("The Ui Text to inform the user about the connection progress")]
    [SerializeField]
    private Text feedbackText;

    [Tooltip("The maximum number of players per room")]
    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    [Tooltip("The input field for the nickname")]
    [SerializeField]
    private TMP_InputField _inputField;
    [SerializeField]
    [Tooltip("The input field for the nickname")]
    private Button quickMatchButton;

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

    [Scene]
    [SerializeField] string debugScenario;


    #endregion

    #region Unity Events
    [Foldout("Events for visual feedback")]
    public UnityEvent OnClickedOnQuickMatch;
    [Foldout("Events for visual feedback")]
    public UnityEvent OnConnectedEvent;
    [Foldout("Events for visual feedback")]
    public UnityEvent OnJoinedRoomEvent;
    [Foldout("Events for visual feedback")]
    public UnityEvent OnSecondPlayerEnteredRoomEvent;
    [Foldout("Events for visual feedback")]
    public UnityEvent OnFailedToFindRandomRoomEvent;
    [Foldout("Events for visual feedback")]
    public UnityEvent OnDisconnectedEvent;
    #endregion


    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {

      // #Critical
      // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
      PhotonNetwork.AutomaticallySyncScene = true;

      _inputField.onValueChanged.AddListener(delegate (string m) { SetPlayerName(m); }); // for some reason dropdowns override their default behaviour if given listener via code
      quickMatchButton.onClick.AddListener(delegate () { QuickMatch(); }); // for some reason dropdowns override their default behaviour if given listener via code
    }

    private void Start()
    {

      string defaultName = string.Empty;

      if (_inputField != null)
      {
        if (PlayerPrefs.HasKey(Keys.PLAYER_NAME))
        {
          defaultName = PlayerPrefs.GetString(Keys.PLAYER_NAME);
          _inputField.text = defaultName;
        }
      }

      PhotonNetwork.NickName = defaultName;
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
      PhotonNetwork.NickName = value;

      PlayerPrefs.SetString(Keys.PLAYER_NAME, value);
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
    }

    public override void OnPlayerEnteredRoom(Player player)
    {
      // #Critical: We only load level if the second player entered too
      if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient)
      {
        LoadRandomScenario();

        // visual stuff
        OnSecondPlayerEnteredRoomEvent?.Invoke();
      }
    }

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
      LogFeedback("<Color=Red>OnDisconnected</Color> " + cause);

      OnDisconnectedEvent?.Invoke();

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

        // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
        PhotonNetwork.JoinRandomRoom();
      }
    }

    private void CreateRoom()
    {
      // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
      PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom });
    }

    private void LoadRandomScenario()
    {
      // determine scenario to load
      string scenarioName = PickRandomScenario();
      MyDebug.Log("We are loading ", scenarioName);

      // #Critical
      // Load the Room Level. 
      PhotonNetwork.LoadLevel(scenarioName);
    }

    private string PickRandomScenario()
    {
#if ALEK_DEBUG_ON
      return debugScenario;
#else
      // TODO: pick from an ABSOLUTELY random scenario from a compiled list of scenarios
      return debugScenario;
#endif
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

    #endregion

  }
}