using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Collections;
using Cinemachine;
#if UNITY_EDITOR
using NaughtyAttributes;
#endif
using GuruLaghima;

namespace Workbench.ProjectDilemma
{
  public enum Choice
  {
    Kill,
    Save
  }
  public class GameMechanic : MonoBehaviourPunCallbacks
  {
    public static GameMechanic Instance;

    public static Action GameStarted;

    #region Unity Events  

#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent TimerStarted;
#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent TimerFinished;
#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent OnMyChoiceMade;
#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent OnTheirChoiceMade;
#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent OnBothPlayersChose;
    #endregion

    #region exposed fields
    [Tooltip("The cameras we shuffle through")]
    [SerializeField] List<CinemachineVirtualCamera> gameplayCameras;
    [SerializeField] PlayerSpot playerOneSpot;
    [SerializeField] PlayerSpot playerTwoSpot;
    public PlayerSpot localPlayerSpot;
    public PlayerSpot otherPlayerSpot;
    [SerializeField] float delayBeforeRankScreen;
    [SerializeField] GameObject rankScreen;
    [SerializeField] float delayBeforeMainMenu;
    [SerializeField] float discussionTime = 60f;
    #endregion

    #region public fields
    // If you have multiple custom events, it is recommended to define them in the used class
    public const byte DecisionEvent = 1;
    public MyEventsDictionary Outcomes;
    public bool canChoose = false;
    #endregion

    #region private fields
    int decisionsMade;
    IEnumerator gameTimerCoroutine;

    Choice myChoice;
    Choice theirChoice;
    bool madeChoice = false;

    /// <summary>
    /// this matrix denotes what outcome happens at which coombinations of choices
    /// Upper level key denotes the choice of the local player, the lower level key denotes other player choice
    /// works together with the "MyEventsDictionary OutcomesOutcomes" variable
    /// </summary>
    Dictionary<Choice, Dictionary<Choice, UnityEvent>> decisionMatrix;
    #endregion

    #region public methods

    /// <summary>
    /// Choose whether to kill or save the other player. True for kill, false for save
    /// </summary>
    /// <param name="kill"></param>
    public void MakeDecision(bool kill)
    {
      if (!madeChoice)
      {

        int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
        object[] content = new object[] { kill, senderID };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(DecisionEvent, content, raiseEventOptions, SendOptions.SendReliable);
      }
    }

    public void StartTimer()
    {
      if (gameTimerCoroutine == null)
      {
        gameTimerCoroutine = GameTimer(discussionTime);
        StartCoroutine(gameTimerCoroutine);
      }
    }

    public void InitializeLocalPlayerSpot(int playerSpot)
    {
      MyDebug.Log("Player spot", playerSpot.ToString());
      if (playerSpot == 1)
      {
        localPlayerSpot = playerOneSpot;
        otherPlayerSpot = playerTwoSpot;
      }
      else
      {
        localPlayerSpot = playerTwoSpot;
        otherPlayerSpot = playerOneSpot;
      }

      localPlayerSpot.playerModel.SetActive(true);
      localPlayerSpot.gameplayCamerasParent.SetActive(true);
      localPlayerSpot.playerCam.SetActive(true);
      localPlayerSpot.killButton.enabled = true;
      localPlayerSpot.saveButton.enabled = true;
      localPlayerSpot.timer.enabled = true;
      // localPlayerSpot.playerModel = Instantiate(playerOnePrefab, localPlayerSpot.playerModelSpawnPos.transform.position, localPlayerSpot.playerModelSpawnPos.transform.rotation, localPlayerSpot.transform);

      if (otherPlayerSpot.gameplayCamerasParent)
      {
        otherPlayerSpot.gameplayCamerasParent.SetActive(false);
        Destroy(otherPlayerSpot.gameplayCamerasParent);
      }
      otherPlayerSpot.playerCam.SetActive(false);
      otherPlayerSpot.killButton.enabled = false;
      otherPlayerSpot.saveButton.enabled = false;
      otherPlayerSpot.timer.enabled = false;
      otherPlayerSpot.GetComponent<CameraSwitcher>().enabled = false;
      // otherPlayerSpot.playerModel = Instantiate(playerTwoPrefab, otherPlayerSpot.playerModelSpawnPos.transform.position, otherPlayerSpot.playerModelSpawnPos.transform.rotation, playerTwoSpot.transform);

      StartTimer();
      GameStarted?.Invoke();

    }

    #endregion
    #region Monobehaviour callbacks

    private void Awake()
    {
      if (!Instance)
        Instance = this;
      else
        Destroy(this);
    }

    public override void OnEnable()
    {
      base.OnEnable();
      PhotonNetwork.NetworkingClient.EventReceived += OnAnyEvent;
    }

    public override void OnDisable()
    {
      base.OnDisable();
      PhotonNetwork.NetworkingClient.EventReceived -= OnAnyEvent;

      if (gameTimerCoroutine != null)
        StopCoroutine(gameTimerCoroutine);
    }

    private void Start()
    {
      // initialise the decision matrix
      decisionMatrix = new Dictionary<Choice, Dictionary<Choice, UnityEvent>>(){
      {Choice.Kill, new Dictionary<Choice, UnityEvent>(){
        {Choice.Kill, Outcomes.GetItems()["Kill Each Other"]},
        {Choice.Save, Outcomes.GetItems()["I kill they save"]}
      }},
      {Choice.Save, new Dictionary<Choice, UnityEvent>(){
        {Choice.Kill, Outcomes.GetItems()["They kill I save"]},
        {Choice.Save, Outcomes.GetItems()["Save each other"]}
      }}
    };
    }

    private void Update()
    {
      if (localPlayerSpot != null)
        localPlayerSpot.GetComponent<CameraSwitcher>().SwitchCamera();
    }
    #endregion
    #region private methods

    IEnumerator GameTimer(float v)
    {
      MyDebug.Log("GameTimer", "started");
      canChoose = true;
      TimerStarted?.Invoke(); // two of these are probably unnecessary
      localPlayerSpot.TimerStarted?.Invoke(); // two of these are probably unnecessary
      localPlayerSpot.gameTimer.StartTimer(v); // two of these are probably unnecessary

      yield return new WaitForSeconds(v);
      MyDebug.Log("GameTimer", "ended");
      canChoose = false;

      TimerFinished?.Invoke();
      localPlayerSpot.TimerFinished?.Invoke();
    }

    void OnAnyEvent(EventData photonEvent)
    {
      byte eventCode = photonEvent.Code;
      if (eventCode == DecisionEvent)
      {
        // extract info from received data
        object[] data = (object[])photonEvent.CustomData;
        Choice choice = (bool)data[0] ? Choice.Kill : Choice.Save;
        int senderID = (int)data[1];

        // who's choice it was
        if (senderID == PhotonNetwork.LocalPlayer.ActorNumber)
        {
          myChoice = choice;
          madeChoice = true;
          OnMyChoiceMade?.Invoke();
          localPlayerSpot.OnMyChoiceMade?.Invoke();
          otherPlayerSpot.OnTheirChoiceMade?.Invoke();
        }
        else
        {
          theirChoice = choice;
          OnTheirChoiceMade?.Invoke();
          otherPlayerSpot.OnMyChoiceMade?.Invoke();
          localPlayerSpot.OnTheirChoiceMade?.Invoke();
        }

        decisionsMade++;

        // if both players made the choice execute the outcome
        if (decisionsMade >= 2)
        {
          // visual feedback
          OnBothPlayersChose?.Invoke();

          // visual feedback
          decisionMatrix[myChoice][theirChoice]?.Invoke();
          localPlayerSpot.decisionMatrix[myChoice][theirChoice]?.Invoke();
          otherPlayerSpot.decisionMatrix[theirChoice][myChoice]?.Invoke();

          Invoke("ShowRankScreen", delayBeforeRankScreen);
        }

      }
    }

    void ShowRankScreen()
    {
      rankScreen.SetActive(true);
      Invoke("ReturnToMainMenu", delayBeforeMainMenu);

    }
    void ReturnToMainMenu()
    {
      PhotonNetwork.Disconnect();

    }

    #endregion

  }
}