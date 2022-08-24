using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Collections;
using Cinemachine;
using UnityEngine.Playables;
using NaughtyAttributes;
using GuruLaghima;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using DG.Tweening;
using TMPro;
using UnityEngine.Animations.Rigging;
using Photon.Voice.PUN;
using UnityEngine.GameFoundation;

namespace Workbench.ProjectDilemma
{
  public enum Choice
  {
    Kill,
    Save,
    None
  }
  public enum Outcome
  {
    Won,
    Lost,
    BothWon,
    BothLost,
  }
  //
  public class GameMechanic : MonoBehaviourPunCallbacks, ISequenceExecutor
  {
    public float discussionTime;
    IEnumerator currentCoroutine;

    public IEnumerator SequenceCoroutine(List<ControllableSequence.EventWithDuration> eventSequence, string name)
    {
      MyDebug.Log($"[{name}]", "started");

      foreach (ControllableSequence.EventWithDuration ev in eventSequence)
      {
        if (ev.shouldExecute)
        {
          ev.theEvent?.Invoke();
          if (ev.isCoroutine)
          {
            if (currentCoroutine != null)
            {
              StopCoroutine(currentCoroutine);
              yield return StartCoroutine(currentCoroutine);
            }
            else
            {
              MyDebug.Log("There is no current coroutine. Assign it in the UnityEvent handler by calling a void function");
              yield return new WaitForSeconds(ev.duration);
            }
          }
          else
          {
            yield return new WaitForSeconds(ev.duration);
          }
        }
      }

      MyDebug.Log($"[{name}]", "ended");

    }

    [Foldout("Sequences conditions")]
    [SerializeField] BoolReference bothVotedCondition;
    [Foldout("Sequences conditions")]
    [SerializeField] BoolReference oneVotedCondition;
    [Foldout("Sequences conditions")]
    [SerializeField] BoolReference noneVotedCondition;
    [Foldout("Sequences conditions")]
    [SerializeField] BoolReference oneBetrayedOneSaved;
    [Foldout("Sequences conditions")]
    [SerializeField] BoolReference bothBetrayedCondition;
    [Foldout("Sequences conditions")]
    [SerializeField] BoolReference bothSavedCondition;
    [Foldout("Sequences conditions")]
    [SerializeField] BoolReference localPlayerWonCondition;

    public static GameMechanic Instance;

    public static Action GameStarted;
    public static Action VotingEnded;
    public static Action DiscussionStarted;
    public static Action DiscussionEnded;

    public ControllableSequence gameSequence;
    public ControllableSequence postGameSequence;


    [Header("Visual Feedback Events - Phases")]

    [HorizontalLine(color: EColor.Blue)]
    #region Unity Events 

    [HorizontalLine(color: EColor.Red)]

    /***************** delineator (obviously) ***************/

    [Header("Visual Feedback Events - Choices")]

    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent OnYourChoiceMade;

    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent OnTheirChoiceMade;

    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent OnBothPlayersChose;

    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent OnRanOutOfTime;
    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent DiscussionEndedUnityEvent;

    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent OnExtraTimeVotesAvailable;

    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent OnExtraTimeVotesUnavailable;

    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent OnExtraTimeRequestCooldown;

    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent OnLocalRequestedExtraTime;

    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent OnOtherRequestedExtraTime;

    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent OnExtraTimeRequestFailed;

    [BoxGroup("Visual Feedback Events - Choices")]
    public UnityEvent OnExtraTimeAdded;

    [Foldout("End screen references")]
    public UnityEvent<string> On1StarsEarned;
    [Foldout("End screen references")]
    public UnityEvent<string> On2StarsEarned;
    [Foldout("End screen references")]
    public UnityEvent<string> On3StarsEarned;
    [Foldout("End screen references")]
    public UnityEvent OnAllRewardsClaimed;
    #endregion

    #region exposed fields
    [SerializeField] PlayableDirector timelineDirector;
    [SerializeField] PlayableAsset introTimeline;


    [InputAxis]
    [SerializeField] private string skipIntroButton;
    [SerializeField] GameObject skipIntroText;
    [SerializeField] GameObject waitingOnOtherPlayerToSkipIntroText;
    [HorizontalLine(color: EColor.White)]
    [SerializeField] public PlayerSpot playerOneSpot;
    [SerializeField] public PlayerSpot playerTwoSpot;
    [SerializeField] float extraTimeRequestCooldown;
    [SerializeField] float extraTimeRequestDuration;
    [SerializeField] float extraTimeAdded;
    [SerializeField] float extraTimeDiminishPerVote;
    [SerializeField] int extraTimeMaxVotes;
    [HideInInspector][SerializeField] int extraTimeVotes = 0;
    [HorizontalLine(color: EColor.White)]
    public UnityEvent OnEndOfCinematic;

    [HorizontalLine(color: EColor.White)]

    /// <summary>
    /// ! refactor  this using the loading screens package
    /// </summary>
    [Foldout("Transition screens")]
    [SerializeField] GameObject bothVotedScreen;

    [Foldout("Transition screens")]
    [SerializeField] GameObject theyDidntVoteScreen;
    [Foldout("Transition screens")]
    [SerializeField] GameObject youDidntVoteScreen;
    [Foldout("Transition screens")]
    [SerializeField] GameObject nobodyVotedScreen;
    [HorizontalLine(color: EColor.White)]


    [Foldout("Death sequences params")]
    [SerializeField] Transform deathPrefabSpawnPos;
    [Foldout("Death sequences params")]
    [SerializeField] DeathSequence outcomeSequence;
    [Foldout("Death sequences params")]
    [SerializeField] GameObject transitionToDeathSequenceScreen;
    [Foldout("Death sequences params")]
    [SerializeField] GameObject transitionToSalvationSequenceScreen;
    [Foldout("Death sequences params")]
    [SerializeField] float fadeInToDeathSequenceInterval;
    [Foldout("Death sequences params")]
    [SerializeField] float fadeOutToDeathSequenceInterval;

    [HorizontalLine(color: EColor.White)]
    [Foldout("Camera transition settings")]
    [SerializeField] GameObject postScreenCam;
    [Foldout("Camera transition settings")]
    [SerializeField] SplitScreenAnim playerSplitScreenAnim;
    [Foldout("Camera transition settings")]
    [SerializeField] SplitScreenAnim enemySplitScreenAnim;
    [Foldout("Camera transition settings")]
    [SerializeField] SplitScreenAnim defaultDeathCamAnim;
    [Foldout("Camera transition settings")]
    [SerializeField] SplitScreenAnim defaultBothWonCamAnim;
    [Foldout("Camera transition settings")]
    [SerializeField] Ease camSlideTypeForSuspense;
    [Range(0, 10)]
    [Foldout("Camera transition settings")]
    [SerializeField] float camSlideDurationForSuspense;
    [Foldout("Camera transition settings")]
    [SerializeField] Ease camSlideTypeForBothLost;
    [Range(0, 10)]
    [Foldout("Camera transition settings")]
    [SerializeField] float camSlideDurationForBothLost;
    [Foldout("Camera transition settings")]
    [SerializeField] Ease camSlideTypeForBothWon;
    [Range(0, 10)]
    [Foldout("Camera transition settings")]
    [SerializeField] float camSlideDurationForBothWon;
    [Foldout("Camera transition settings")]
    [SerializeField] Ease camSlideTypeForPlayerWon;
    [Range(0, 10)]
    [Foldout("Camera transition settings")]
    [SerializeField] float camSlideDurationForPlayerWon;
    [Foldout("Camera transition settings")]
    [SerializeField] Ease camSlideTypeForEnemyWon;
    [Range(0, 10)]
    [Foldout("Camera transition settings")]
    [SerializeField] float camSlideDurationForEnemyWon;

    [HorizontalLine(color: EColor.White)]
    [SerializeField] GameObject otherPlayerDecisionTextBetray;
    [SerializeField] GameObject otherPlayerDecisionTextSave;
    [SerializeField] GameObject localPlayerDecisionTextBetray;
    [SerializeField] GameObject localPlayerDecisionTextSave;
    [HorizontalLine(color: EColor.White)]
    [Foldout("End screen references")]
    [SerializeField] ValueCounter coinsCounter;
    [Foldout("End screen references")]
    [SerializeField] TextMeshProUGUI coinsCounterText;
    [Foldout("End screen references")]
    [SerializeField] ValueCounter experienceCounter;
    [Foldout("End screen references")]
    [SerializeField] ValueCounter levelBarCounter;
    [Foldout("End screen references")]
    [SerializeField] UnityEngine.UI.Slider experienceSlider;
    [Foldout("End screen references")]
    [SerializeField] TextMeshProUGUI levelText;
    [Foldout("End screen references")]
    [SerializeField] TextMeshProUGUI experienceText;
    [Foldout("End screen references")]
    [SerializeField] float rewardActivationSpeed;
    [Foldout("End screen references")]
    [SerializeField] float starActivationSpeed;
    [Foldout("End screen references")]
    [SerializeField] NumberCounter localPlayerPointsCounter;
    [Foldout("End screen references")]
    [SerializeField] NumberCounter otherPlayerPointsCounter;
    [Foldout("End screen references")]
    [SerializeField] PlayableDirector endScreenDirector;
    [Foldout("End screen references")]
    [SerializeField] PlayableAsset endScreenWonTimeline;
    [Foldout("End screen references")]
    [SerializeField] PlayableAsset endScreenLostTimeline;
    [Foldout("End screen references")]
    [SerializeField] PlayableAsset endScreenBothWonTimeline;
    [Foldout("End screen references")]
    [SerializeField] PlayableAsset endScreenBothLostTimeline;
    [Foldout("End screen references")]
    [SerializeField] GameObject postcardObj;
    [Foldout("End screen references")]
    [SerializeField] TextMeshProUGUI postcardNameField;
    [Foldout("End screen references")]
    [SerializeField] TMP_InputField postcardTextField;
    [Foldout("End screen references")]
    [SerializeField] private TextMeshProUGUI experienceBonusesText;
    [Foldout("End screen references")]
    [SerializeField] private TextMeshProUGUI coinsBonusesText;
    [Foldout("End screen references")]
    [SerializeField] private string victoryMessageStar;
    [Foldout("End screen references")]
    [SerializeField] private string questCompletedMessageStar;
    [Foldout("End screen references")]
    [SerializeField] private string perkCompletedMessageStar;
    [Foldout("End screen references")]
    [SerializeField] private TextMeshProUGUI extraVotesCounter;


    #endregion

    #region public fields
    public bool SkippedOutcomeSequence = false;

    [Foldout("Debug")]
    [ReadOnly]
    public bool canChoose = false;
    // If you have multiple custom events, it is recommended to define them in the used class
    [HorizontalLine(color: EColor.Green)]
    [HideInInspector]
    public MyEventsDictionary Outcomes;
    [HideInInspector] public PlayerSpot localPlayerSpot;
    [HideInInspector] public PlayerSpot otherPlayerSpot;
    [Foldout("Debug")]
    public int localPlayerPoints;
    [Foldout("Debug")]
    public int otherPlayerPoints;

    #endregion

    #region photon events
    public const byte DecisionEvent = 1;
    public const byte UniversalDeathChoiceEvent = 2;
    public const byte FinalNoteEvent = 3;
    public const byte AnimationEvent = 4;
    public const byte ScenarioDeathChoiceEvent = 5;
    public const byte CooperateSequenceChoiceEvent = 6;
    public const byte BothLoseSequenceChoiceEvent = 7;
    #endregion

    #region private fields
    IEnumerator gameTimerCoroutine;
    IEnumerator extraTimeRequestCoroutine;
    [Foldout("Debug")]
    [Dropdown("intValues")]
    [SerializeField] int debugPlayerSpot;
    private int[] intValues = new int[] { 1, 2 };
    [Foldout("Debug")]
    [OnValueChanged("OnValueChangedCallback")]
    [SerializeField] public Choice myChoice = Choice.None;
    [Foldout("Debug")]
    [OnValueChanged("OnValueChangedCallback")]
    [SerializeField] public Choice theirChoice = Choice.None;
    [Foldout("Debug")]
    [SerializeField] bool madeChoice = false;
    [Foldout("Debug")]
    [SerializeField] bool theyMadeChoice = false;
    private void OnValueChangedCallback()
    {
      votingOutcome = outcomeMatrix[myChoice][theirChoice];
      MyDebug.Log("VotingOutcome", votingOutcome.ToString());
    }

    /// <summary>
    /// this matrix denotes what outcome happens at which coombinations of choices
    /// Upper level key denotes the choice of the local player, the lower level key denotes other player choice
    /// works together with the "MyEventsDictionary OutcomesOutcomes" variable
    /// </summary>
    Dictionary<Choice, Dictionary<Choice, Outcome>> outcomeMatrix = new Dictionary<Choice, Dictionary<Choice, Outcome>>(){
      {Choice.Kill, new Dictionary<Choice, Outcome>(){
        {Choice.Kill, Outcome.BothLost},// I betray, they betray
        {Choice.Save, Outcome.Won},// I betray, they save
        {Choice.None, Outcome.Won}// I betray, they dont vote
      }},
      {Choice.Save, new Dictionary<Choice, Outcome>(){
        {Choice.Kill, Outcome.Lost},// I save, they betray
        {Choice.Save, Outcome.BothWon},// I save, they save
        {Choice.None, Outcome.Won}// I save, they dont vote
      }},
      {Choice.None, new Dictionary<Choice, Outcome>(){
        {Choice.Kill, Outcome.Lost},// I dont vote, they betray
        {Choice.Save, Outcome.Lost},// I dont vote, they save
        {Choice.None, Outcome.BothLost}// I dont vote, they dont vote
      }}
    };
    private bool winnerChoseDeath;
    private Outcome votingOutcome = Outcome.Won;
    private GameObject endOfDiscussionScreen;
    private GameObject otherPlayerDecisionText;
    private GameObject localPlayerDecisionText;
    private Animator activeDeathBookAnimator;
    int decisionsMade;
    private bool inOutcomeSequence;
    private float discussionTimer; // i extracted this timer because we are modifying it outside the function
    #endregion

    #region public methods
    public void StartGameSequence()
    {
      gameSequence.rootCoroutine = gameSequence.RunSequence(this);
      StartCoroutine(gameSequence.rootCoroutine);
    }

    public void InitializePlayers()
    {
      MyDebug.Log("Initializing players");
      // initialize players in their spots
      if (PhotonNetwork.IsConnected)
      {
        int playerSpot = (int)PhotonNetwork.LocalPlayer.CustomProperties[Keys.PLAYER_NUMBER];
        GetComponent<GameMechanic>().InitializeLocalPlayerSpot(playerSpot);
      }
      else if (ScenarioManager.instance.debugMode)
      {
        GetComponent<GameMechanic>().InitializeLocalPlayerSpot(debugPlayerSpot);
      }
    }

    public void Director_Stopped(PlayableDirector obj)
    {
      MyDebug.Log("Cinematic stopped");
      // reset timeline
      timelineDirector.time = 0;
      timelineDirector.Stop();
      timelineDirector.Evaluate();

      OnEndOfCinematic?.Invoke();
    }

    /// <summary>
    /// Choose whether to kill or save the other player. True for kill, false for save
    /// </summary>
    /// <param name="kill"></param>
    public void MakeDecision(bool kill)
    {
      if (!madeChoice)
      {
        if (ScenarioManager.instance.debugMode)
        {
          madeChoice = true;
          myChoice = kill ? Choice.Kill : Choice.Save;
          // decisionsMade++;
          GameEvents.OnLocalPlayerVoted?.Invoke(myChoice);
          // if both players made the choice execute the outcome
          // if (decisionsMade >= 2)
          {
            // we'll need to know the outcome so we can decide flows
            votingOutcome = outcomeMatrix[myChoice][theirChoice];

            // visual feedback
            OnBothPlayersChose?.Invoke();

          }
        }
        else
        {
          MyDebug.Log("Player made the decision to", kill ? "betray" : "save");
          int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
          object[] content = new object[] { senderID, kill, };
          RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
          PhotonNetwork.RaiseEvent(DecisionEvent, content, raiseEventOptions, SendOptions.SendReliable);
        }

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

      localPlayerSpot.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
      Invoke("InitVoice", 2f);
      localPlayerSpot.playerUsingThisSpot = PhotonNetwork.LocalPlayer;
      localPlayerSpot.playerModel.SetActive(true);
      localPlayerSpot.outlineComponent.enabled = true;
      DestroyImmediate(otherPlayerSpot.outlineComponent);
      localPlayerSpot.gameplayCamerasParent.SetActive(true); // the container for the cinemachine virtual cams
      localPlayerSpot.mainCam.gameObject.SetActive(true); // the cinemachine brain cam
      localPlayerSpot.killButton.interactable = true;
      localPlayerSpot.saveButton.interactable = true;
      // localPlayerSpot.projectileThrow.canon.playerHasControlOverCamera = true;
      localPlayerSpot.throwablesActivator.canon.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer);
      PlayerInputManager.Instance.throwablesActivator = localPlayerSpot.throwablesActivator;
      PlayerInputManager.Instance.emoteActivator = localPlayerSpot.emoteActivator;
      PlayerInputManager.Instance.perkActivator = localPlayerSpot.perkActivator;
      PlayerInputManager.Instance.abilityActivator = localPlayerSpot.abilityActivator;
      PlayerInputManager.Instance.questActivator = localPlayerSpot.questActivator;
      // we can maybe use SendMessage to call all of these together, but I will leave it be for now
      localPlayerSpot.throwablesActivator.Init();
      localPlayerSpot.emoteActivator.Init();
      localPlayerSpot.perkActivator.Init();
      // localPlayerSpot.outfitLoader.Init();
      localPlayerSpot.questActivator.Init();
      localPlayerSpot.relicActivator.Init();
      localPlayerSpot.extrasActivator.Init();
      localPlayerSpot.abilityActivator.Init();
      // localPlayerSpot.outfitLoader.Init(); // ? can the onwership transfer be screwing this call?  
      // populate list of owned death sequences
      localPlayerSpot.PopulateDeathBook(ScenarioManager.instance.thisScenario, DeathSequencesManager.Instance.universalDeathSequences);
      // assign current points to end screen counters
      // load point data about eachother
      if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(Keys.PLAYER_SOFT_CURRENCY_POINTS))
      {
        MyDebug.Log("Load points for local player");
        localPlayerPoints = (int)PhotonNetwork.LocalPlayer.CustomProperties[Keys.PLAYER_SOFT_CURRENCY_POINTS];
      }
      MyDebug.Log("localPlayerPoints", localPlayerPoints.ToString());
      localPlayerPointsCounter.Text.text = localPlayerPoints.ToString();


      otherPlayerSpot.playerUsingThisSpot = ScenarioManager.instance.otherPlayer;
      if (otherPlayerSpot.gameplayCamerasParent)
      {
        otherPlayerSpot.gameplayCamerasParent.SetActive(false);
        Destroy(otherPlayerSpot.gameplayCamerasParent);
      }
      otherPlayerSpot.mainCam.gameObject.SetActive(false);
      otherPlayerSpot.mainCam.gameObject.GetComponent<Camera>().cullingMask = otherPlayerSpot.mainCam.gameObject.GetComponent<Camera>().cullingMask | (1 << LayerMask.NameToLayer("Masks"));
      // otherPlayerSpot.killButton.interactable = false;
      // otherPlayerSpot.saveButton.interactable = false;
      otherPlayerSpot.GetComponent<CameraSwitcher>().enabled = false;
      otherPlayerSpot.throwablesActivator.canon.playerHasControlOverCamera = false;
      if (!PhotonNetwork.IsConnected)
      {
        otherPlayerSpot.outfitLoader.Init();
      }

      // assign current points to end screen counters
      if (ScenarioManager.instance.otherPlayer != null && ScenarioManager.instance.otherPlayer.CustomProperties.ContainsKey(Keys.PLAYER_SOFT_CURRENCY_POINTS))
      {
        MyDebug.Log("Load points for other player");
        otherPlayerPoints = (int)ScenarioManager.instance.otherPlayer.CustomProperties[Keys.PLAYER_SOFT_CURRENCY_POINTS];
      }
      MyDebug.Log("otherPlayerPoints", otherPlayerPoints.ToString());
      otherPlayerPointsCounter.Text.text = otherPlayerPoints.ToString();

      //
      GameEvents.OnScenario?.Invoke(ScenarioManager.instance.thisScenario);

      if (PhotonNetwork.IsConnected)
      {
        RPCManager.Instance.photonView.RPC("RPC_SyncPlayerLoaded", RpcTarget.AllViaServer, localPlayerSpot.GetComponent<PhotonView>().ViewID);
      }
      else
      {
        localPlayerSpot.playerLoaded = true;
        otherPlayerSpot.playerLoaded = true;
        GameMechanic.Instance.CheckPlayersLoaded();
      }
    }

    public void SpawnDeathPrefab()
    {
      Instantiate(outcomeSequence, deathPrefabSpawnPos.position, deathPrefabSpawnPos.rotation);
    }

    public void AnimateSplitScreen()
    {
      // playerSplitScreenAnim.cam = localPlayerSpot.suspenseCam.GetComponent<Camera>();
      playerSplitScreenAnim.AnimateCameraWidth(1f, 0.5f, 0f, 0f, 0f, 0f, camSlideTypeForSuspense, camSlideDurationForSuspense);
      enemySplitScreenAnim.AnimateCameraWidth(0f, 0.5f, 1f, 0.5f, 0f, 0f, camSlideTypeForSuspense, camSlideDurationForSuspense);
    }

    public void SetupOtherPlayerCamForSplitScreen()
    {
      MyDebug.Log("SetupOtherPlayerCamForSplitScreen");
      otherPlayerSpot.playerCam = otherPlayerSpot.suspenseCam;
      otherPlayerSpot.playerCam.SetActive(true);
      enemySplitScreenAnim.cam = otherPlayerSpot.playerCam.GetComponent<Camera>();
      Rect newRect = otherPlayerSpot.playerCam.GetComponent<Camera>().rect;
      otherPlayerSpot.playerCam.GetComponent<Camera>().depth = 90;
      localPlayerSpot.playerCam.GetComponent<Camera>().depth = 91;
      newRect.width = 0.0f;
      newRect.x = 1f;
      otherPlayerSpot.playerCam.GetComponent<Camera>().rect = newRect;
    }
    public void SetupLocalPlayerCamForSplitScreen()
    {
      MyDebug.Log("SetupLocalPlayerCamForSplitScreen");
      localPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      localPlayerSpot.suspenseCam.SetActive(true);
      localPlayerSpot.playerCam = localPlayerSpot.suspenseCam;
      playerSplitScreenAnim.cam = localPlayerSpot.playerCam.GetComponent<Camera>();
    }
    public void ShowOtherPlayerDecisionAnim()
    {
      // either through animator or through coded IK motions, animate the voting movement
      if (theirChoice == Choice.Kill)
        otherPlayerSpot.playerModel.GetComponent<Animator>().SetTrigger("betray_owl");
      else if (theirChoice == Choice.Save)
        otherPlayerSpot.playerModel.GetComponent<Animator>().SetTrigger("cooperate_owl");
    }
    public void ShowOtherPlayerDecisionText()
    {
      // either through animator or through coded IK motions, animate the voting movement
      if (theirChoice == Choice.Kill)
      {
        if (otherPlayerDecisionTextBetray)
        {

          otherPlayerDecisionText = otherPlayerDecisionTextBetray;
          otherPlayerDecisionText.SetActive(true);
        }
      }
      else
      {
        if (otherPlayerDecisionTextSave)
        {

          otherPlayerDecisionText = otherPlayerDecisionTextSave;
          otherPlayerDecisionText.SetActive(true);
        }
      }
    }
    public void ShowLocalPlayerDecisionAnim()
    {
      // either through animator or through coded IK motions, animate the voting movement
      if (myChoice == Choice.Kill)
        localPlayerSpot.playerModel.GetComponent<Animator>().SetTrigger("betray_owl");
      else if (myChoice == Choice.Save)
        localPlayerSpot.playerModel.GetComponent<Animator>().SetTrigger("cooperate_owl");
    }
    public void HideDecisionTexts()
    {
      if (otherPlayerDecisionText)
        otherPlayerDecisionText.SetActive(false);
      if (localPlayerDecisionText)
        localPlayerDecisionText.SetActive(false);
    }

    public void ShowLocalPlayerDecisionText()
    {
      // either through animator or through coded IK motions, animate the voting movement
      if (myChoice == Choice.Kill)
      {
        if (localPlayerDecisionTextBetray)
        {
          localPlayerDecisionText = localPlayerDecisionTextBetray;
          localPlayerDecisionText.SetActive(true);
        }
      }
      else
      {
        if (localPlayerDecisionTextSave)
        {
          localPlayerDecisionText = localPlayerDecisionTextSave;
          localPlayerDecisionText.SetActive(true);
        }
      }
    }
    public void ShowBookAnimationBasedOnOutcome(float delay)
    {
      switch (votingOutcome)
      {
        case Outcome.Won:
          localPlayerSpot.deathBookForPlayerCam.SetActive(true);
          activeDeathBookAnimator = localPlayerSpot.deathBookForPlayerCam.GetComponent<Animator>();
          activeDeathBookAnimator.Play("death book slide up");
          // localPlayerSpot.deathBook.GetComponent<Animator>().Play;
          break;
        case Outcome.Lost:
          StartCoroutine(OtherPlayerChoosesDeath(delay));
          break;
        case Outcome.BothWon:
          break;
        case Outcome.BothLost:
          // dont show books because they dont get to choose. they both die an unexciting default death
          break;
      }

    }
    public void HideBookAnimation()
    {
      if (activeDeathBookAnimator)
        activeDeathBookAnimator.Play("death book slide down");
    }

    public void ZoomOutFromEnemy()
    {
      if (theirChoice == Choice.Kill && myChoice == Choice.Save)
        otherPlayerSpot.suspenseCam.GetComponent<Animator>().Play("Zoom out on enemy");
    }

    public void ShowEndOfDiscussionScreen()
    {
      if (madeChoice && theyMadeChoice)
      {
        // show both voted screen
        endOfDiscussionScreen = bothVotedScreen;
        endOfDiscussionScreen.SetActive(true);

      }
      else if (madeChoice)
      {
        // show 'Other player didnt participate in the game. They lose!' screen
        endOfDiscussionScreen = theyDidntVoteScreen;
        endOfDiscussionScreen.SetActive(true);

      }
      else if (theyMadeChoice)
      {
        // show 'You didn't participate in the game. You lose!
        endOfDiscussionScreen = youDidntVoteScreen;
        endOfDiscussionScreen.SetActive(true);
      }
      else
      {
        endOfDiscussionScreen = nobodyVotedScreen;
        endOfDiscussionScreen.SetActive(true);
      }
    }

    public void HideEndOfDiscussionScreen()
    {
      endOfDiscussionScreen.GetComponent<Animator>().SetTrigger("FadeOut");
    }

    public void ChooseUniversalDeathSequence(DeathSequence deathSequence)
    {
      MyDebug.Log("Chosen Death sequence is", deathSequence.labelText);
      // if (DeathSequencesManager.Instance.deathSequences.Contains(death))
      int deathSequenceIndex = DeathSequencesManager.Instance.universalDeathSequences.FindIndex((obj) => { return obj.deathSequence == deathSequence; });
      if (ScenarioManager.instance.debugMode)
      {
        outcomeSequence = deathSequence;
        winnerChoseDeath = true; // this should be assigned when the event is recieved, not here
      }
      else
      {
        // to do: notify other user AND local player of the death scenario they chose)
        int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
        object[] content = new object[] { senderID, deathSequenceIndex };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(UniversalDeathChoiceEvent, content, raiseEventOptions, SendOptions.SendReliable);
      }
    }


    public void ChooseScenarioDeathSequence(DeathSequence deathSequence)
    {
      MyDebug.Log("Chosen Death sequence is", deathSequence.labelText);
      // if (DeathSequencesManager.Instance.deathSequences.Contains(death))
      int deathSequenceIndex = ScenarioManager.instance.thisScenario.defaultDeathSequences.FindIndex((obj) => { return obj.deathSequence == deathSequence; });
      if (ScenarioManager.instance.debugMode && !PhotonNetwork.IsConnected)
      {
        outcomeSequence = deathSequence;
        winnerChoseDeath = true; // this should be assigned when the event is recieved, not here
      }
      else
      {
        // to do: notify other user AND local player of the death scenario they chose)
        int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
        object[] content = new object[] { senderID, deathSequenceIndex };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(ScenarioDeathChoiceEvent, content, raiseEventOptions, SendOptions.SendReliable);
      }
    }

    /// <summary>
    /// Invoked through UnityEvent (one of the phases)
    /// </summary>
    public void SelectRandomDefaultDeathSequence()
    {
      // we decide which default death will occur to the loser at the master client, and then sync it with event
      if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
      {
        // if (DeathSequencesManager.Instance.deathSequences.Contains(death))
        int deathSequenceIndex = UnityEngine.Random.Range(0, ScenarioManager.instance.thisScenario.defaultDeathSequences.Count);
        if (ScenarioManager.instance.debugMode && !PhotonNetwork.IsConnected)
        {
          outcomeSequence = ScenarioManager.instance.thisScenario.defaultDeathSequences[deathSequenceIndex].deathSequence;
          winnerChoseDeath = true; // this should be assigned when the event is recieved, not here
        }
        else
        {
          MyDebug.Log("Chosen Death sequence is", deathSequenceIndex);
          // to do: notify other user AND local player of the death scenario they chose)
          int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
          object[] content = new object[] { senderID, deathSequenceIndex };
          RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
          PhotonNetwork.RaiseEvent(ScenarioDeathChoiceEvent, content, raiseEventOptions, SendOptions.SendReliable);
        }
      }

    }

    /// <summary>
    /// Invoked through UnityEvent (one of the phases)
    /// </summary>
    public void SelectRandomDefaultWinSequence()
    {
      // we decide which default death will occur to the loser at the master client, and then sync it with event
      if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
      {
        // if (DeathSequencesManager.Instance.deathSequences.Contains(death))
        int cooperateSequenceIndex = UnityEngine.Random.Range(0, ScenarioManager.instance.thisScenario.defaultBothCooperateSequences.Count);
        if (ScenarioManager.instance.debugMode && !PhotonNetwork.IsConnected)
        {
          MyDebug.Log("Choosing a random default win");
          outcomeSequence = ScenarioManager.instance.thisScenario.defaultBothCooperateSequences[cooperateSequenceIndex].deathSequence;
          winnerChoseDeath = true; // this should be assigned when the event is recieved, not here
        }
        else
        {
          // to do: notify other user AND local player of the death scenario they chose)
          MyDebug.Log("Chosen Death sequence is", cooperateSequenceIndex);
          int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
          object[] content = new object[] { senderID, cooperateSequenceIndex };
          RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
          PhotonNetwork.RaiseEvent(CooperateSequenceChoiceEvent, content, raiseEventOptions, SendOptions.SendReliable);
        }
      }

    }

    /// <summary>
    /// Invoked through UnityEvent (one of the phases)
    /// </summary>
    public void SelectRandomDefaultBothLoseSequence()
    {
      // we decide which default death will occur to the loser at the master client, and then sync it with event
      if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
      {
        // if (DeathSequencesManager.Instance.deathSequences.Contains(death))
        int deathSequenceIndex = UnityEngine.Random.Range(0, ScenarioManager.instance.thisScenario.defaultBothLoseSequences.Count);
        if (ScenarioManager.instance.debugMode && !PhotonNetwork.IsConnected)
        {
          outcomeSequence = ScenarioManager.instance.thisScenario.defaultBothLoseSequences[deathSequenceIndex].deathSequence;
          winnerChoseDeath = true; // this should be assigned when the event is recieved, not here
        }
        else
        {
          // to do: notify other user AND local player of the death scenario they chose)
          MyDebug.Log("Chosen Death sequence is", deathSequenceIndex);
          int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
          object[] content = new object[] { senderID, deathSequenceIndex };
          RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
          PhotonNetwork.RaiseEvent(BothLoseSequenceChoiceEvent, content, raiseEventOptions, SendOptions.SendReliable);
        }
      }

    }

    public void ShowPostScreenAnimations()
    {
      MyDebug.Log(votingOutcome.ToString());
      switch (votingOutcome)
      {
        case Outcome.Won:
          endScreenDirector.playableAsset = endScreenWonTimeline;
          break;
        case Outcome.Lost:
          endScreenDirector.playableAsset = endScreenLostTimeline;
          break;
        case Outcome.BothWon:
          endScreenDirector.playableAsset = endScreenBothWonTimeline;
          break;
        case Outcome.BothLost:
          endScreenDirector.playableAsset = endScreenBothLostTimeline;
          break;
        default:
          break;
      }
      endScreenDirector.Play();
    }

    public void ShowPostcardForWinner()
    {
      MyDebug.Log("ShowPostcardForWinner");
      if (votingOutcome == Outcome.Won)
      {
        postcardObj.SetActive(true);

        // focus the input field
        postcardTextField.Select();
        postcardTextField.ActivateInputField();

        if (!ScenarioManager.instance.debugMode || PhotonNetwork.IsConnected)
          postcardNameField.text = otherPlayerSpot.playerUsingThisSpot.NickName.Substring(0, otherPlayerSpot.playerUsingThisSpot.NickName.IndexOf("#"));
      }
    }

    public void SendFinalNoteToEnemy()
    {
      postcardObj.SetActive(false);

      MyDebug.Log("Sending final note");
      int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
      string sender = "<UNKNOWN>";
      if (localPlayerSpot.playerUsingThisSpot.NickName.IndexOf("#") >= 0)
        sender = localPlayerSpot.playerUsingThisSpot.NickName.Substring(0, localPlayerSpot.playerUsingThisSpot.NickName.IndexOf("#"));
      object[] content = new object[] { senderID, postcardTextField.text, sender };
      RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well
      PhotonNetwork.RaiseEvent(FinalNoteEvent, content, raiseEventOptions, SendOptions.SendReliable);
    }
    public void AnimatePlayer(string animation)
    {
      MyDebug.Log("Animating player over the network");

      int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
      object[] content = new object[] { senderID, animation, "Trigger", null, localPlayerSpot.playerSpot };
      RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
      PhotonNetwork.RaiseEvent(AnimationEvent, content, raiseEventOptions, SendOptions.SendReliable);
    }

    // referenced in inspector
    public void PrepareForEndScreen()
    {
      MyDebug.Log("PrepareForEndScreen");
      ActivateProperModelForEndScreen();

      int ourTotalPoints = RewardCalculator.Instance.CalculatePointsAfterOutcome();
      coinsCounter.newAmount = ourTotalPoints;
      if (BankManager.Instance)
        BankManager.Instance.WalletEvaluate(ourTotalPoints);
      localPlayerPointsCounter.newAmount = localPlayerPoints + ourTotalPoints;

      Currency rankAsCurrency = GameFoundationSdk.catalog.Find<Currency>(Keys.CURRENCY_RANK);
      int ourTotalRank = RewardCalculator.Instance.CalculateRankAfterOutcome();
      GameFoundationSdk.wallet.Set(rankAsCurrency, GameFoundationSdk.wallet.Get(rankAsCurrency) + ourTotalRank);

      Currency xpAsCurrency = GameFoundationSdk.catalog.Find<Currency>(Keys.CURRENCY_XP);
      int ourTotalXp = (int)GameFoundationSdk.wallet.Get(xpAsCurrency);
      ourTotalXp += RewardCalculator.Instance.CalculateXPAfterOutcome();
      GameFoundationSdk.wallet.Set(xpAsCurrency, GameFoundationSdk.wallet.Get(xpAsCurrency) + ourTotalXp);

      int ourLevel = ProgressData.Instance.GetLevel(ourTotalXp, out int experienceLeft);

      experienceSlider.maxValue = MiscelaneousSettings.Instance.levelsDistribution[ourLevel];
      levelText.text = ourLevel.ToString();
      experienceText.text = $"{experienceLeft}/{MiscelaneousSettings.Instance.levelsDistribution[ourLevel]}";
      experienceCounter.newAmount = ourTotalXp;
      levelBarCounter.newAmount = experienceLeft;

      RewardCalculator.Instance.CalculateRewardsAfterOutcome();
      SyncDataToOtherPlayer();

    }

    public void LevelBar()
    {
      levelBarCounter.Value = levelBarCounter.newAmount;
    }

    public void ActivateProperModelForEndScreen()
    {
      // activate proper model on endscreen
      localPlayerSpot.endScenePlayerModelLeft.SetActive(true);
      localPlayerSpot.endScenePlayerModelRight.SetActive(false);
      otherPlayerSpot.endScenePlayerModelLeft.SetActive(false);
      otherPlayerSpot.endScenePlayerModelRight.SetActive(true);

    }

    // we send an RPC to other player containing our total points, rank, xp..
    public void SyncDataToOtherPlayer()
    {
      if (PhotonNetwork.IsConnected)
      {
        if (GameFoundationSdk.IsInitialized)
        {
          Currency softCurrency = GameFoundationSdk.catalog.Find<Currency>(Keys.CURRENCY_SOFT);
          Currency rankAsCurrency = GameFoundationSdk.catalog.Find<Currency>(Keys.CURRENCY_RANK);
          Currency xpAsCurrency = GameFoundationSdk.catalog.Find<Currency>(Keys.CURRENCY_XP);
          RPCManager.Instance.photonView.RPC("RPC_EvaluatePlayerIncomes", GameMechanic.Instance.otherPlayerSpot.GetComponent<PhotonView>().Owner,
            GameFoundationSdk.wallet.Get(softCurrency), GameFoundationSdk.wallet.Get(rankAsCurrency), GameFoundationSdk.wallet.Get(xpAsCurrency));
        }
        else
        {
          RPCManager.Instance.photonView.RPC("RPC_EvaluatePlayerIncomes", GameMechanic.Instance.otherPlayerSpot.GetComponent<PhotonView>().Owner,
            0, 0, 0);
        }
      }
      else
      {
        ActivateEndScreenCountersOtherPlayer(0, 0, 0);
      }
    }

    // we recieve the rpc from the other player containing their total points, rank, xp..
    public void ActivateEndScreenCountersOtherPlayer(long theirPoints, long theirRank, long theirXp)
    {
      otherPlayerPointsCounter.newAmount = (int)theirPoints;
    }

    public void DisableFog()
    {
      RenderSettings.fog = false;
    }

    public void ShowNameplates()
    {
      if (localPlayerSpot.playerUsingThisSpot != null)
        localPlayerSpot.nameplateText.text = localPlayerSpot.playerUsingThisSpot.NickName.Substring(0, localPlayerSpot.playerUsingThisSpot.NickName.IndexOf("#") < 0 ? 0 : localPlayerSpot.playerUsingThisSpot.NickName.IndexOf("#"));
      localPlayerSpot.nameplateCanvas.SetActive(true);

      if (otherPlayerSpot.playerUsingThisSpot != null)
        otherPlayerSpot.nameplateText.text = otherPlayerSpot.playerUsingThisSpot.NickName.Substring(0, otherPlayerSpot.playerUsingThisSpot.NickName.IndexOf("#") < 0 ? 0 : otherPlayerSpot.playerUsingThisSpot.NickName.IndexOf("#"));
      otherPlayerSpot.nameplateCanvas.SetActive(true);
    }

    public void Disconnect()
    {
      PhotonNetwork.Disconnect();

    }

    public void DecideOutcome()
    {
      votingOutcome = outcomeMatrix[myChoice][theirChoice];
      // we call a static event (used by quests)
      GameEvents.OnOutcome?.Invoke(votingOutcome);
      // setup outcome conditions
      MyDebug.Log("Voting outcome", votingOutcome.ToString());

      if (!madeChoice)
      {
        if (theyMadeChoice)
          oneVotedCondition.boolWrapper.Value = true;
        else
          noneVotedCondition.boolWrapper.Value = true;
      }
      else if (!theyMadeChoice)
      {
        localPlayerWonCondition.boolWrapper.Value = true;
        oneVotedCondition.boolWrapper.Value = true;
      }
      else
      {
        bothVotedCondition.boolWrapper.Value = true;
        MyDebug.Log("My choice", myChoice.ToString());
        MyDebug.Log("Their choice", theirChoice.ToString());
        switch (votingOutcome)
        {
          case Outcome.Won:
            localPlayerWonCondition.boolWrapper.Value = true;

            oneBetrayedOneSaved.boolWrapper.Value = true;
            break;
          case Outcome.Lost:
            oneBetrayedOneSaved.boolWrapper.Value = true;
            break;
          case Outcome.BothLost:
            bothBetrayedCondition.boolWrapper.Value = true;
            break;
          case Outcome.BothWon:
            bothSavedCondition.boolWrapper.Value = true;
            break;
          default:
            break;
        }
      }

    }
    public void OnPlayerDisconnect()
    {
      MyDebug.Log("OnPlayerDisconnect");
      votingOutcome = Outcome.Won;
      // PrepareForEndScreen();


      // stop game sequence
      StopCoroutine(gameSequence.rootCoroutine);

      // consider it as another end of discussion phase
      DiscussionEnded?.Invoke();
      DiscussionEndedUnityEvent?.Invoke();

      // run postgame screen sequence
      postGameSequence.rootCoroutine = postGameSequence.RunSequence(this);
      StartCoroutine(postGameSequence.rootCoroutine);
    }

    public void RequestExtraTime(bool state)
    {
      if (PhotonNetwork.IsConnected)
      {
        RPCManager.Instance.photonView.RPC("RPC_SyncExtraTimeVote", RpcTarget.AllViaServer, localPlayerSpot.GetComponent<PhotonView>().ViewID, state);
      }
      else
      {
        localPlayerSpot.requestedExtraTime = state;
        //otherPlayerSpot.requestedExtraTime = true;
        if (!state)
        {
          DeclineExtraTime();
        }
        GameMechanic.Instance.CheckPlayersVotedExtraTime();
      }
      if (state)
      {
        StartCoroutine(ExtraTimeRequestCooldown());
      }
    }

    public void DeclineExtraTime()
    {
      if (extraTimeRequestCoroutine != null)
      {
        StopCoroutine(extraTimeRequestCoroutine);
      }
      localPlayerSpot.requestedExtraTime = false;
      otherPlayerSpot.requestedExtraTime = false;
      OnExtraTimeRequestFailed?.Invoke();
    }

    // normaly we call this function via RPC twice, it fails the first time when only one of the player is loaded
    // and succeeds the second time when both players load.
    public void CheckPlayersLoaded()
    {
      if (localPlayerSpot && otherPlayerSpot)
      {
        if (localPlayerSpot.playerLoaded && otherPlayerSpot.playerLoaded)
        {
          MyDebug.Log("Both players have successfuly loaded", Color.green);
          GameEvents.OnBothPlayerLoaded?.Invoke();
          GameMechanic.Instance.localPlayerSpot.outfitLoader.Init();
        }
        else
        {
          MyDebug.Log("Waiting for both players to load", Color.yellow);
        }
      }
    }
    public void CheckPlayersVotedExtraTime()
    {
      if (localPlayerSpot && otherPlayerSpot)
      {
        if (localPlayerSpot.requestedExtraTime && otherPlayerSpot.requestedExtraTime)
        {
          //reset values for the next vote
          localPlayerSpot.requestedExtraTime = false;
          otherPlayerSpot.requestedExtraTime = false;
          MyDebug.Log("Both players voted for extra time", Color.green);
          AddExtraTimeDiscussion();
          GameEvents.OnVotedExtraTime?.Invoke();
        }
        else if (otherPlayerSpot.requestedExtraTime)
        {
          MyDebug.Log("Other player voted for extra time", Color.green);
          OnOtherRequestedExtraTime?.Invoke();
          if (extraTimeRequestCoroutine != null)
          {
            StopCoroutine(extraTimeRequestCoroutine);
          }
          extraTimeRequestCoroutine = ExtraTimeRequestInProgress();
          StartCoroutine(extraTimeRequestCoroutine);
        }
        else if (localPlayerSpot.requestedExtraTime)
        {
          MyDebug.Log("Local player voted for extra time", Color.green);
          OnLocalRequestedExtraTime?.Invoke();
          if (extraTimeRequestCoroutine != null)
          {
            StopCoroutine(extraTimeRequestCoroutine);
          }
          extraTimeRequestCoroutine = ExtraTimeRequestInProgress();
          StartCoroutine(extraTimeRequestCoroutine);
        }
        else
        {
          MyDebug.Log("Waiting for both players to vote for extra time", Color.yellow);
        }
      }
    }

    public void AddExtraTimeDiscussion()
    {
      if (extraTimeVotes >= extraTimeMaxVotes)
      {
        return;
      }
      discussionTimer += extraTimeAdded;
      extraTimeAdded -= extraTimeDiminishPerVote;
      extraTimeVotes++;
      if (extraVotesCounter)
        extraVotesCounter.text = (extraTimeMaxVotes - extraTimeVotes).ToString();
      CheckExtraTimeAvailability();
      OnExtraTimeAdded?.Invoke();
      localPlayerSpot.gameTimer.ResetTimer(discussionTimer + localPlayerSpot.gameTimer.CurrentTime); //
    }

    /// <summary>
    /// Increase or decrease max votes for extra time
    /// </summary>
    /// <param name="value"></param>
    public void AddExtraTimeMaxVotes(int value)
    {
      extraTimeMaxVotes += value;
      if (extraVotesCounter)
        extraVotesCounter.text = (extraTimeMaxVotes - extraTimeVotes).ToString();
      CheckExtraTimeAvailability();
    }

    /// <summary>
    /// Call this check everytime we modify max votes or current votes
    /// </summary>
    private void CheckExtraTimeAvailability()
    {
      if (extraTimeVotes < extraTimeMaxVotes)
      {
        OnExtraTimeVotesAvailable?.Invoke();
      }
      else
      {
        OnExtraTimeVotesUnavailable?.Invoke();
      }
    }

    public void SkipOutcomeSequence()
    {
      if (inOutcomeSequence)
        SkippedOutcomeSequence = true;
    }

    public void GameEnded()
    {
      GameEvents.OnGameEnded?.Invoke();
    }
    #endregion

    #region IEnumerator wrappers
    public void STartTimelineWrapper()
    {
      currentCoroutine = StartTimeline();
    }
    public void DiscussionPhaseInvoker(float duration)
    {
      currentCoroutine = DiscussionPhaseCoroutine(discussionTime);
    }
    public void DeathChoiceCoroutineWrapper(float interval)
    {
      currentCoroutine = DeathChoiceCoroutine(interval);
    }
    public void ShowOutcomeSequenceWrapper()
    {
      currentCoroutine = OutcomeSequencePhase();
    }
    public void StarRewardsSequenceWrapper()
    {
      currentCoroutine = StarRewardsCoroutine();
    }
    public void ClaimRewardsSequenceWrapper()
    {
      currentCoroutine = ClaimRewardsCoroutine();
    }
    public void ExperienceRewardsSequenceWrapper()
    {
      currentCoroutine = ExperienceRewardsCoroutine();
    }
    public void CoinsRewardsSequenceWrapper()
    {
      currentCoroutine = CoinsRewardsCoroutine();
    }
    #endregion

    #region Monobehaviour callbacks

    private void Awake()
    {
      if (!Instance)
        Instance = this;
      else
        Destroy(this);

      if (timelineDirector != null)
      {
        timelineDirector.played += Director_Played;
        timelineDirector.stopped += Director_Stopped;
      }

      if (skipIntroText)
        skipIntroText.SetActive(true);
      if (waitingOnOtherPlayerToSkipIntroText)
        waitingOnOtherPlayerToSkipIntroText.SetActive(false);
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
      if (timelineDirector != null)
      {
        timelineDirector.played -= Director_Played;
        timelineDirector.stopped -= Director_Stopped;
      }
      if (gameTimerCoroutine != null)
        StopCoroutine(gameTimerCoroutine);

      // return fog to normal state
      RenderSettings.fog = true;


      StopAllCoroutines();
    }

    private void Start()
    {

      // initialize game properly depending if its in debug mode or not
      if (ScenarioManager.instance.debugMode && !PhotonNetwork.IsConnected)
      {
        votingOutcome = outcomeMatrix[myChoice][theirChoice];
      }
      else
      {
        madeChoice = false;
        theyMadeChoice = false;
        // we fetch points from network properties
        localPlayerPoints = 0;
        otherPlayerPoints = 0;
      }
      bothBetrayedCondition.boolWrapper.Value = false;
      bothSavedCondition.boolWrapper.Value = false;
      bothVotedCondition.boolWrapper.Value = false;
      noneVotedCondition.boolWrapper.Value = false;
      oneVotedCondition.boolWrapper.Value = false;
      oneBetrayedOneSaved.boolWrapper.Value = false;
      localPlayerWonCondition.boolWrapper.Value = false;
      if (extraVotesCounter)
        extraVotesCounter.text = (extraTimeMaxVotes - extraTimeVotes).ToString();
    }

    private void Update()
    {
      if (localPlayerSpot != null)
        if (localPlayerSpot.GetComponent<CameraSwitcher>() != null)
          localPlayerSpot.GetComponent<CameraSwitcher>().SwitchCamera();
    }
    #endregion

    #region coroutines
    IEnumerator GameSequence()
    {
      GameStarted?.Invoke();
      yield return gameSequence.RunSequence(this);
    }

    IEnumerator SkipIntroCoroutine()
    {

      while (!Input.GetButtonDown(skipIntroButton))
      {
        yield return null;
      }

      // show player he skipped but is waiting on other player
      if (skipIntroText)
        skipIntroText.SetActive(false);
      if (waitingOnOtherPlayerToSkipIntroText)
        waitingOnOtherPlayerToSkipIntroText.SetActive(true);

      // tell other player we are skipping
      Hashtable ht = new Hashtable {
        { Keys.SKIP_INTRO, "skipped"}
         };
      PhotonNetwork.LocalPlayer.SetCustomProperties(ht);

    }

    IEnumerator DiscussionPhaseCoroutine(float duration)
    {
      MyDebug.Log("DiscussionPhase coroutine started");

      DiscussionStarted?.Invoke();
      localPlayerSpot.TimerStarted?.Invoke(); // one of these are probably unnecessary
      localPlayerSpot.gameTimer.StartTimer(duration); // one of these are probably unnecessary

      canChoose = true;

      // this phase lasts until time runs out or both players choose
      discussionTimer = duration;
      while (!(madeChoice && theyMadeChoice) && discussionTimer > 0)
      {
        discussionTimer -= Time.deltaTime;
        yield return null;
      }

      canChoose = false;
      if (discussionTimer > 0)
      {
        GameEvents.OnPlayersChoseTimeLeft?.Invoke(discussionTimer);
      }
      MyDebug.Log("DiscussionPhase coroutine ended");

      localPlayerSpot.TimerFinished?.Invoke();

      // if no decision was made maybe hook up some visuals
      if (!madeChoice)
        OnRanOutOfTime?.Invoke();

      /*       try
            {
              DiscussionEnded?.Invoke();
            }
            catch (Exception ex)
            {

            }
            try
            {
              DiscussionEndedUnityEvent?.Invoke();
            }
            catch (Exception ex)
            {

            } */
      DiscussionEnded?.Invoke();
      DiscussionEndedUnityEvent?.Invoke();


    }
    IEnumerator DeathChoiceCoroutine(float chooseDuration)
    {

      MyDebug.Log("DeathChoicePhase", "started");

      // this phase lasts until time runs out or player chooses death
      float timer = chooseDuration;
      while (!winnerChoseDeath && timer > 0)
      {
        timer -= Time.deltaTime;
        yield return null;
      }

      MyDebug.Log("DeathChoicePhase", "ended");

    }
    IEnumerator OutcomeSequencePhase()
    {

      MyDebug.Log("OutcomeSequencePhase", "started");

      yield return StartCoroutine(ShowOutcomeSequence());

      // we wait for the sequence to end to show the end screen
      if (outcomeSequence)
      {
        float timer = outcomeSequence.GetComponent<DeathSequence>().duration;
        MyDebug.Log("OutcomeSequencePhase", "chosen death sequence started");
        inOutcomeSequence = true;
        while (timer > 0)
        {
          timer -= Time.deltaTime;

          // allow sequence to be skipped
          if (SkippedOutcomeSequence)
          {
            timer = 0;
            SkippedOutcomeSequence = false;
          }

          yield return null;
        }
        inOutcomeSequence = false;

        MyDebug.Log("OutcomeSequencePhase", "chosen death sequence ended");
      }

      MyDebug.Log("OutcomeSequencePhase", "ended");

    }

    IEnumerator StartTimeline()
    {
      MyDebug.Log("StartTimelineCoroutine Start");
      timelineDirector.playableAsset = introTimeline;
      timelineDirector.Play();
      yield return StartCoroutine(SkipIntroCoroutine());
      MyDebug.Log("StartTimelineCoroutine end");
    }

    IEnumerator OtherPlayerChoosesDeath(float delay)
    {

      // zoom in on other player
      otherPlayerSpot.playerCam.GetComponent<Animator>().SetTrigger("ZoomIn");
      otherPlayerSpot.playerCam.GetComponent<Animator>().Play("Zoom in on enemy");


      yield return new WaitForSeconds(delay);

      // then show book of death
      otherPlayerSpot.deathBookForEnemyCam.SetActive(true);
      activeDeathBookAnimator = otherPlayerSpot.deathBookForPlayerCam.GetComponent<Animator>();
      activeDeathBookAnimator.Play("death book slide up");
    }
    IEnumerator LocalPlayerChoosesDeath()
    {

      // zoom in on other player
      // localPlayerSpot.playerCam.GetComponent<Animator>().SetTrigger("ZoomIn");
      localPlayerSpot.deathBookForPlayerCam.SetActive(true);

      yield return new WaitForSeconds(2f);

      // then show book of death
    }

    IEnumerator ShowOutcomeSequence()
    {
      if (madeChoice && theyMadeChoice)
      {
        switch (votingOutcome)
        {
          case Outcome.Won:
            yield return StartCoroutine(YouWon());
            break;
          case Outcome.Lost:
            yield return StartCoroutine(YouLost());
            break;
          case Outcome.BothWon:
            yield return StartCoroutine(BothWon());
            break;
          case Outcome.BothLost:
            yield return StartCoroutine(BothLost());
            break;
        }
      }
      else
      {
        if (theyMadeChoice)
        {
          yield return StartCoroutine(YouLost());
        }
        else if (madeChoice)
        {
          yield return StartCoroutine(YouWon());
        }
        else
        {
          yield return StartCoroutine(BothLost());
        }
      }
    }

    IEnumerator BothLost()
    {
      MyDebug.Log("Both lost");
      if (madeChoice && theyMadeChoice)
      {

        defaultDeathCamAnim.cam.gameObject.SetActive(true);
        defaultDeathCamAnim.AnimateCameraWidth(0f, 1f, 0.5f, 0f, 0f, 0f, camSlideTypeForBothLost, camSlideDurationForBothLost);
        enemySplitScreenAnim.AnimateCameraWidth(0.5f, 0f, 0.5f, 1f, 0f, 0f, camSlideTypeForBothLost, camSlideDurationForBothLost);
        playerSplitScreenAnim.AnimateCameraWidth(0.5f, 0f, 0f, 0f, 0f, 0f, camSlideTypeForBothLost, camSlideDurationForBothLost);
        yield return new WaitForSeconds(camSlideDurationForBothLost);
        // fade out screen
        transitionToDeathSequenceScreen.SetActive(true);
        yield return new WaitForSeconds(fadeInToDeathSequenceInterval);
        transitionToDeathSequenceScreen.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(fadeOutToDeathSequenceInterval);
      }
      // disable all other cameras
      /*       if (otherPlayerSpot.playerCam)
              otherPlayerSpot.playerCam.SetActive(false);
            if (localPlayerSpot.playerCam)
              localPlayerSpot.playerCam.SetActive(false);
            defaultDeathCamAnim.cam.gameObject.SetActive(false);
            localPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
            otherPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras(); */
      DisableAllCameras();

      // play random Both Lost death (the choice should already have been made when the voting ended)
      if (outcomeSequence != null)
      {
        //before the death prefab is spawned we must disable all other active cameras.
        // I don't care much for this particular implementation. should be better
        foreach (Camera cam in Camera.allCameras)
        {
          cam.enabled = false;
        }
        var _deathSequence = Instantiate(outcomeSequence, deathPrefabSpawnPos.position, deathPrefabSpawnPos.rotation);
        _deathSequence.ActivateOutcome(DeathSequence.OutcomeSequence.Both);
      }

    }

    IEnumerator BothWon()
    {
      MyDebug.Log("Both won");
      defaultBothWonCamAnim.cam.gameObject.SetActive(true);
      defaultBothWonCamAnim.AnimateCameraWidth(0f, 1f, 0.5f, 0f, 0f, 0f, camSlideTypeForBothWon, camSlideDurationForBothWon);
      enemySplitScreenAnim.AnimateCameraWidth(0.5f, 0f, 0.5f, 1f, 0f, 0f, camSlideTypeForBothWon, camSlideDurationForBothWon);
      playerSplitScreenAnim.AnimateCameraWidth(0.5f, 0f, 0f, 0f, 0f, 0f, camSlideTypeForBothWon, camSlideDurationForBothWon);
      yield return new WaitForSeconds(camSlideDurationForBothWon);
      // fade out screen
      transitionToSalvationSequenceScreen.SetActive(true);
      yield return new WaitForSeconds(fadeInToDeathSequenceInterval);
      transitionToSalvationSequenceScreen.GetComponent<Animator>().SetTrigger("FadeOut");
      yield return new WaitForSeconds(fadeOutToDeathSequenceInterval);
      // disable all other cameras
      /*       otherPlayerSpot.playerCam.SetActive(false);
            localPlayerSpot.playerCam.SetActive(false);
            defaultDeathCamAnim.cam.enabled = false;
            localPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
            otherPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras(); */
      DisableAllCameras();
      // play victory
      if (outcomeSequence)
      {
        //before the death prefab is spawned we must disable all other active cameras.
        // I don't care much for this particular implementation. should be better
        foreach (Camera cam in Camera.allCameras)
        {
          cam.enabled = false;
        }
        var _deathSequence = Instantiate(outcomeSequence, deathPrefabSpawnPos.position, deathPrefabSpawnPos.rotation);
        _deathSequence.ActivateOutcome(DeathSequence.OutcomeSequence.Both);
      }
    }

    IEnumerator YouLost()
    {
      MyDebug.Log("You lost");
      if (madeChoice)
      {
        playerSplitScreenAnim.AnimateCameraWidth(0.5f, 1f, 0f, 0f, 0f, 0f, camSlideTypeForEnemyWon, camSlideDurationForEnemyWon);
        enemySplitScreenAnim.AnimateCameraWidth(0.5f, 0f, 0.5f, 1f, 0f, 0f, camSlideTypeForEnemyWon, camSlideDurationForEnemyWon);
        yield return new WaitForSeconds(camSlideDurationForEnemyWon);
        // fade out screen
        transitionToDeathSequenceScreen.SetActive(true);
        yield return new WaitForSeconds(fadeInToDeathSequenceInterval);
        transitionToDeathSequenceScreen.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(fadeOutToDeathSequenceInterval);
      }
      // disable all other cameras
      /*       otherPlayerSpot.playerCam.SetActive(false);
            localPlayerSpot.playerCam.SetActive(false);
            defaultDeathCamAnim.cam.gameObject.SetActive(false);
            localPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
            otherPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras(); */
      DisableAllCameras();
      // play chosen death scene (by other player, or a random default one)
      if (outcomeSequence != null)
      {
        //before the death prefab is spawned we must disable all other active cameras.
        // I don't care much for this particular implementation. should be better
        foreach (Camera cam in Camera.allCameras)
        {
          cam.enabled = false;
        }
        var _deathSequence = Instantiate(outcomeSequence, deathPrefabSpawnPos.position, deathPrefabSpawnPos.rotation);
        _deathSequence.ActivateOutcome((DeathSequence.OutcomeSequence)otherPlayerSpot.playerSpot);
      }
    }

    IEnumerator YouWon()
    {
      MyDebug.Log("You won");
      if (theyMadeChoice)
      {
        // animate camera
        enemySplitScreenAnim.AnimateCameraWidth(enemySplitScreenAnim.cam.rect.width, 1f, enemySplitScreenAnim.cam.rect.min.x, 0f, 0f, 0f, camSlideTypeForPlayerWon, camSlideDurationForPlayerWon);
        playerSplitScreenAnim.AnimateCameraWidth(playerSplitScreenAnim.cam.rect.width, 0f, 0f, 0f, 0f, 0f, camSlideTypeForPlayerWon, camSlideDurationForPlayerWon);
        yield return new WaitForSeconds(camSlideDurationForPlayerWon);
        // fade out screen
        transitionToDeathSequenceScreen.SetActive(true);
        yield return new WaitForSeconds(fadeInToDeathSequenceInterval);
        transitionToDeathSequenceScreen.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(fadeOutToDeathSequenceInterval);
      }
      // disable all other cameras
      DisableAllCameras();
      // play chosen death scene (or a random default one if the other player didnt vote)
      if (outcomeSequence != null)
      {
        //before the death prefab is spawned we must disable all other active cameras.
        // I don't care much for this particular implementation. should be better
        foreach (Camera cam in Camera.allCameras)
        {
          cam.enabled = false;
        }
        var _deathSequence = Instantiate(outcomeSequence, deathPrefabSpawnPos.position, deathPrefabSpawnPos.rotation);
        _deathSequence.ActivateOutcome((DeathSequence.OutcomeSequence)localPlayerSpot.playerSpot);
      }
    }

    IEnumerator StarRewardsCoroutine()
    {
      MyDebug.Log("StarRewardsCoroutine");
      int stars = 0;
      if (votingOutcome == Outcome.BothWon || votingOutcome == Outcome.Won)
      {
        stars++;
        ActivateStarEvents(stars, victoryMessageStar);
        yield return new WaitForSeconds(starActivationSpeed);
      }
      if (localPlayerSpot.perkActivator.PerkActivated)
      {
        stars++;
        ActivateStarEvents(stars, perkCompletedMessageStar);
        yield return new WaitForSeconds(starActivationSpeed);
      }
      foreach (RewardData reward in MasterData.Instance.allRewards)
      {
        if (reward.NewlyAdded)
        {
          stars++;
          ActivateStarEvents(stars, questCompletedMessageStar);
          yield return new WaitForSeconds(starActivationSpeed);
          break;
        }
      }
    }
    IEnumerator ClaimRewardsCoroutine()
    {
      if (RewardDisplayManager.Instance)
      {
        RewardDisplayManager.Instance.CreateRewards();
        yield return new WaitForSeconds(0.3f);
        yield return new WaitUntil(() => RewardDisplayManager.Instance.listOfRewardPlaceholders.Count <= 0);
      }
      else
      {
        yield return null;
      }
    }
    IEnumerator CoinsRewardsCoroutine()
    {
      coinsCounterText.color = coinsCounter.newAmount >= 0 ? Color.green : Color.red;
      coinsCounter.Duration = rewardActivationSpeed + RewardDisplayManager.Instance.CoinsRewards.Count * rewardActivationSpeed - Mathf.Epsilon;
      coinsCounter.Value = coinsCounter.newAmount;
      int i = 0;
      coinsBonusesText.text = "";
      WaitForSeconds Wait = new WaitForSeconds(rewardActivationSpeed);
      while (i < RewardDisplayManager.Instance.CoinsRewards.Count)
      {
        if (i > 0)
        {
          coinsBonusesText.text += "\n";
        }
        coinsBonusesText.text += $"{RewardDisplayManager.Instance.CoinsRewards[i]}";
        i++;
        yield return Wait;
      }
    }
    IEnumerator ExperienceRewardsCoroutine()
    {
      MyDebug.Log("ExperienceRewardsCoroutine");

      experienceCounter.Duration = rewardActivationSpeed + RewardDisplayManager.Instance.ExperienceRewards.Count * rewardActivationSpeed - Mathf.Epsilon;
      experienceCounter.Value = experienceCounter.newAmount;
      int i = 0;
      experienceBonusesText.text = "";
      WaitForSeconds Wait = new WaitForSeconds(rewardActivationSpeed);
      while (i < RewardDisplayManager.Instance.ExperienceRewards.Count)
      {
        if (i > 0)
        {
          experienceBonusesText.text = "\n";
        }
        experienceBonusesText.text += $"{RewardDisplayManager.Instance.ExperienceRewards[i]}";
        i++;
        yield return Wait;
      }
      yield return new WaitForSeconds(2f);
    }
    IEnumerator ExtraTimeRequestInProgress()
    {
      yield return new WaitForSeconds(extraTimeRequestDuration);
      // after the time has expired we reset both to false
      localPlayerSpot.requestedExtraTime = false;
      otherPlayerSpot.requestedExtraTime = false;
      OnExtraTimeRequestFailed?.Invoke();
    }
    IEnumerator ExtraTimeRequestCooldown()
    {
      OnExtraTimeRequestCooldown?.Invoke();
      yield return new WaitForSeconds(extraTimeRequestCooldown);
      CheckExtraTimeAvailability();
    }
    #endregion

    #region private methods

    public void DisableAllCameras()
    {
      MyDebug.Log("DisableAllCameras");

      // disable all other cameras
      if (otherPlayerSpot.playerCam)
        otherPlayerSpot.playerCam.SetActive(false);
      if (otherPlayerSpot.suspenseCam)
        otherPlayerSpot.suspenseCam.SetActive(false);
      if (otherPlayerSpot.playerCam)
        localPlayerSpot.playerCam.SetActive(false);
      if (defaultDeathCamAnim.cam)
        defaultDeathCamAnim.cam.gameObject.SetActive(false);
      if (localPlayerSpot.GetComponent<CameraSwitcher>())
        localPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      if (otherPlayerSpot.GetComponent<CameraSwitcher>())
        otherPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      // //before the death prefab is spawned we must disable all other active cameras.
      // // I don't care much for this particular implementation. should be better
      // foreach (Camera cam in Camera.allCameras)
      // {
      //   cam.enabled = false;
      // }
    }

    // public void ShowPostscreenCam()
    // {
    //   MyDebug.Log("ShowPostscreenCam");
    //   postScreenCam.SetActive(true);
    //   postScreenCam.GetComponent<Camera>().enabled = true;
    // }

    void ActivateStarEvents(int star, string message)
    {
      switch (star)
      {
        case 1:
          On1StarsEarned?.Invoke(message);
          break;
        case 2:
          On2StarsEarned?.Invoke(message);
          break;
        case 3:
          On3StarsEarned?.Invoke(message);
          break;
        default:
          break;
      }
    }
    void InitVoice()
    {
      localPlayerSpot.GetComponent<PhotonVoiceView>().Init();
    }
    void Director_Played(PlayableDirector obj)
    {
      MyDebug.Log("Cinematic played");

    }
    void OtherPlayerOutcome()
    {
      otherPlayerSpot.decisionMatrix[theirChoice][myChoice]?.Invoke();
    }

    /*     void ShowPostGameScreen()
        {
          if (postGameScreen) postGameScreen.SetActive(true);
        } */

    void UniversalDeathChoiceEventFunc(EventData photonEvent)
    {

      {
        // extract info from received data
        object[] data = (object[])photonEvent.CustomData;
        int senderID = (int)data[0];
        int deathSequenceIndex = (int)data[1];

        winnerChoseDeath = true;

        if (DeathSequencesManager.Instance.universalDeathSequences.Count > 0
        && DeathSequencesManager.Instance.universalDeathSequences.Count > deathSequenceIndex
        && deathSequenceIndex >= 0)
          outcomeSequence = DeathSequencesManager.Instance.universalDeathSequences[deathSequenceIndex].deathSequence;

        MyDebug.Log("outcomeSequence.name", outcomeSequence.name);

        // who's choice it was
        if (senderID == PhotonNetwork.LocalPlayer.ActorNumber)
        {
          // OnYourChoiceMade?.Invoke();
          // localPlayerSpot.OnMyChoiceMade?.Invoke();
          // otherPlayerSpot.OnTheirChoiceMade?.Invoke();
        }
        else
        {
          // OnTheirChoiceMade?.Invoke();
          // otherPlayerSpot.OnMyChoiceMade?.Invoke();
          // localPlayerSpot.OnTheirChoiceMade?.Invoke();
        }
      }

    }

    void ScenarioDeathChoiceEventFunc(EventData photonEvent)
    {

      // extract info from received data
      object[] data = (object[])photonEvent.CustomData;
      int senderID = (int)data[0];
      int deathSequenceIndex = (int)data[1];

      winnerChoseDeath = true;

      Scenario currentScenario = ScenarioManager.instance.thisScenario;
      if (currentScenario.defaultDeathSequences.Count > 0
      && currentScenario.defaultDeathSequences.Count > deathSequenceIndex
      && deathSequenceIndex >= 0)
        outcomeSequence = currentScenario.defaultDeathSequences[deathSequenceIndex].deathSequence;
      MyDebug.Log("outcomeSequence.name", outcomeSequence.name);

      // who's choice it was
      if (senderID == PhotonNetwork.LocalPlayer.ActorNumber)
      {
        // OnYourChoiceMade?.Invoke();
        // localPlayerSpot.OnMyChoiceMade?.Invoke();
        // otherPlayerSpot.OnTheirChoiceMade?.Invoke();
      }
      else
      {
        // OnTheirChoiceMade?.Invoke();
        // otherPlayerSpot.OnMyChoiceMade?.Invoke();
        // localPlayerSpot.OnTheirChoiceMade?.Invoke();
      }

    }

    void CooperateSequenceChoiceEventFunc(EventData photonEvent)
    {

      // extract info from received data
      object[] data = (object[])photonEvent.CustomData;
      int senderID = (int)data[0];
      int cooperateSequenceIndex = (int)data[1];

      // winnerChoseDeath = true;

      Scenario currentScenario = ScenarioManager.instance.thisScenario;
      if (currentScenario.defaultBothCooperateSequences.Count > 0
      && currentScenario.defaultBothCooperateSequences.Count > cooperateSequenceIndex
      && cooperateSequenceIndex >= 0)
        outcomeSequence = currentScenario.defaultBothCooperateSequences[cooperateSequenceIndex].deathSequence;
      MyDebug.Log("outcomeSequence.name", outcomeSequence.name);

    }
    void BothLoseSequenceChoiceEventFunc(EventData photonEvent)
    {

      // extract info from received data
      object[] data = (object[])photonEvent.CustomData;
      int senderID = (int)data[0];
      int deathSequenceIndex = (int)data[1];

      // winnerChoseDeath = true;

      Scenario currentScenario = ScenarioManager.instance.thisScenario;
      if (currentScenario.defaultBothLoseSequences.Count > 0
      && currentScenario.defaultBothLoseSequences.Count > deathSequenceIndex
      && deathSequenceIndex >= 0)
        outcomeSequence = currentScenario.defaultBothLoseSequences[deathSequenceIndex].deathSequence;
      MyDebug.Log("outcomeSequence.name", outcomeSequence.name);

    }

    void FinalNoteEventFunc(EventData photonEvent)
    {
      // extract info from received data
      object[] data = (object[])photonEvent.CustomData;
      int senderID = (int)data[0];
      string textSent = (string)data[1];
      string nickname = (string)data[2];

      MyDebug.Log("FinalNote text", textSent);
      MyDebug.Log("FinalNote nickname", nickname);

      // who's choice it was
      if (senderID == PhotonNetwork.LocalPlayer.ActorNumber)
      {
        MyDebug.Log("I sent this note", textSent);

        // play animation for sending mail
        postcardObj.SetActive(false);
      }
      else
      {
        MyDebug.Log("Other player sent this note.", textSent);

        // save the text locally. in json maybe? could do, could do.

        FinalNote newNote = new FinalNote(nickname, textSent);
        FinalNoteCardHandler.SaveFinalNote(newNote);
      }
    }
    void AnimationEventFunc(EventData photonEvent)
    {
      MyDebug.Log("AnimationEvent triggered");
      // extract info from received data
      object[] data = (object[])photonEvent.CustomData;
      int senderID = (int)data[0];
      string animatorParameter = (string)data[1];
      string parameterType = (string)data[2];
      object parameterValue = (object)data[3];
      int playerSpot = (int)data[4];
      // Rig playerRig;
      Animator anim;
      MyDebug.Log("Parameters:");
      MyDebug.Log("animatorParameter:", animatorParameter);
      MyDebug.Log("parameterType:", parameterType);
      // MyDebug.Log("parameterValue:", parameterValue.ToString());
      MyDebug.Log("playerSpot:", playerSpot.ToString());

      if (playerSpot == 1)
      {
        // playerRig = playerOneSpot.playerRig;
        anim = playerOneSpot.playerModel.GetComponent<Animator>();
      }
      else
      {
        // playerRig = playerTwoSpot.playerRig;
        anim = playerTwoSpot.playerModel.GetComponent<Animator>();
      }

      switch (parameterType)
      {
        case "Trigger":
          anim.SetTrigger(animatorParameter);
          // playerRig.weight = 1f; // I decided to animate this value within the animation itself
          break;
        case "Bool":
          anim.SetBool(animatorParameter, (bool)parameterValue);
          // playerRig.weight = 1f;
          break;
        case "Float":
          anim.SetFloat(animatorParameter, (float)parameterValue);
          // playerRig.weight = 1f;
          break;
        case "Int":
          anim.SetInteger(animatorParameter, (int)parameterValue);
          // playerRig.weight = 1f;
          break;
        default:
          break;
      }
    }

    void DecisionEventFunc(EventData photonEvent)
    {
      // extract info from received data
      object[] data = (object[])photonEvent.CustomData;
      int senderID = (int)data[0];
      Choice choice = (bool)data[1] ? Choice.Kill : Choice.Save;

      // who's choice it was
      if (senderID == PhotonNetwork.LocalPlayer.ActorNumber)
      {
        myChoice = choice;
        madeChoice = true;
        OnYourChoiceMade?.Invoke();
        localPlayerSpot.OnMyChoiceMade?.Invoke();
        otherPlayerSpot.OnTheirChoiceMade?.Invoke();
        GameEvents.OnLocalPlayerVoted?.Invoke(choice);
      }
      else
      {
        theirChoice = choice;
        theyMadeChoice = true;
        OnTheirChoiceMade?.Invoke();
        otherPlayerSpot.OnMyChoiceMade?.Invoke();
        localPlayerSpot.OnTheirChoiceMade?.Invoke();
        GameEvents.OnOtherPlayerVoted?.Invoke(choice);
      }

      decisionsMade++;

      // update the outcome so we can control code flow
      votingOutcome = outcomeMatrix[myChoice][theirChoice];

      // if both players made the choice
      if (decisionsMade >= 2)
      {
        // visual feedback
        OnBothPlayersChose?.Invoke();
      }
    }

    void OnAnyEvent(EventData photonEvent)
    {
      byte eventCode = photonEvent.Code;

      switch (eventCode)
      {
        case DecisionEvent:
          DecisionEventFunc(photonEvent);
          break;
        case UniversalDeathChoiceEvent:
          UniversalDeathChoiceEventFunc(photonEvent);
          break;
        case FinalNoteEvent:
          FinalNoteEventFunc(photonEvent);
          break;
        case AnimationEvent:
          AnimationEventFunc(photonEvent);
          break;
        case ScenarioDeathChoiceEvent:
          ScenarioDeathChoiceEventFunc(photonEvent);
          break;
        case CooperateSequenceChoiceEvent:
          CooperateSequenceChoiceEventFunc(photonEvent);
          break;
        case BothLoseSequenceChoiceEvent:
          BothLoseSequenceChoiceEventFunc(photonEvent);
          break;
        default:
          break;
      }


    }

    #endregion

    #region DEBUG
    [Button("Other player makes choice")]
    void DebugOtherPlayerChoice()
    {
      //theirChoice = theirChoice;
      theyMadeChoice = true;
      //OnTheirChoiceMade?.Invoke();
      //otherPlayerSpot.OnMyChoiceMade?.Invoke();
      //localPlayerSpot.OnTheirChoiceMade?.Invoke();
      GameEvents.OnOtherPlayerVoted?.Invoke(theirChoice);
    }
    [Button("Other player requests extra time")]
    void DebugOtherPlayerRequestsExtraTime()
    {
      otherPlayerSpot.requestedExtraTime = true;
      GameMechanic.Instance.CheckPlayersVotedExtraTime();
    }
    [Button("Other player leaves")]
    void DebugOtherPlayerLeaves()
    {
      OnPlayerDisconnect();
    }
    #endregion
  }
}