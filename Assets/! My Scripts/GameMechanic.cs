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

namespace Workbench.ProjectDilemma
{
  public enum Choice
  {
    Kill,
    Save
  }
  public enum Outcome
  {
    Won,
    Lost,
    BothWon,
    BothLost,
    // TheyDidntVote,
    // YouDidntVote,
    // BothDidntVote
  }
  public class GameMechanic : MonoBehaviourPunCallbacks
  {
    public static GameMechanic Instance;

    public static Action GameStarted;
    public static Action VotingEnded;

    [Header("Visual Feedback Events - Phases")]
#if UNITY_EDITOR
    [HorizontalLine(color: EColor.Blue)]
#endif
    #region Unity Events  
#if UNITY_EDITOR
    [BoxGroup("Visual Feedback Events - Phases")]
#endif
    public UnityEvent OnEndOfCinematic;

#if UNITY_EDITOR
    [BoxGroup("Visual Feedback Events - Phases")]
#endif
    public EventEnclosure DiscussionPhaseEvents;

#if UNITY_EDITOR
    [BoxGroup("Visual Feedback Events - Phases")]
#endif
    public EventEnclosure TransitionPhaseEvents;


#if UNITY_EDITOR
    [BoxGroup("Visual Feedback Events - Phases")]
#endif
    public EventSequence SuspensePhaseSequence;

#if UNITY_EDITOR
    [BoxGroup("Visual Feedback Events - Phases")]
#endif
    public EventSequence NobodyVotedPhaseSequence;

#if UNITY_EDITOR
    [BoxGroup("Visual Feedback Events - Phases")]
#endif
    public EventSequence SomebodyVotedPhaseSequence;

#if UNITY_EDITOR
    [BoxGroup("Visual Feedback Events - Phases")]
#endif
    public EventEnclosure DeathChoiceScreenPhaseEvents;

#if UNITY_EDITOR
    [BoxGroup("Visual Feedback Events - Phases")]
#endif
    public EventEnclosure DeathSequencePhaseEvents;


#if UNITY_EDITOR
    [BoxGroup("Visual Feedback Events - Phases")]
#endif
    public EventEnclosure PostGameScreenPhaseEvents;

#if UNITY_EDITOR
    [HorizontalLine(color: EColor.Red)]
#endif

    /***************** delineator (obviously) ***************/

    [Header("Visual Feedback Events - Choices")]
#if UNITY_EDITOR
    [HideInInspector]
    [BoxGroup("Visual Feedback Events - Choices")]
#endif
    public UnityEvent OnYourChoiceMade;

#if UNITY_EDITOR
    [HideInInspector]
    [BoxGroup("Visual Feedback Events - Choices")]
#endif
    public UnityEvent OnTheirChoiceMade;

#if UNITY_EDITOR
    [HideInInspector]
    [BoxGroup("Visual Feedback Events - Choices")]
#endif
    public UnityEvent OnBothPlayersChose;

#if UNITY_EDITOR
    [HideInInspector]
    [BoxGroup("Visual Feedback Events - Choices")]
#endif
    public UnityEvent OnRanOutOfTime;
    #endregion

    #region exposed fields
    [SerializeField] PlayableDirector introDirector;
#if UNITY_EDITOR
    [InputAxis]
#endif
    [SerializeField] private string skipIntroButton;
    [SerializeField] GameObject skipIntroText;
    [SerializeField] GameObject waitingOnOtherPlayerToSkipIntroText;
    [HorizontalLine(color: EColor.White)]
    [SerializeField] PlayerSpot playerOneSpot;
    [SerializeField] PlayerSpot playerTwoSpot;
    [HorizontalLine(color: EColor.White)]

    [SerializeField] GameObject postGameScreen;
    [SerializeField] GameObject bothVotedScreen;
    [SerializeField] GameObject theyDidntVoteScreen;
    [SerializeField] GameObject youDidntVoteScreen;
    [SerializeField] GameObject nobodyVotedScreen;
    [HorizontalLine(color: EColor.White)]


    [BoxGroup("Death sequences params")]
    [SerializeField] Transform deathPrefabSpawnPos;
    [BoxGroup("Death sequences params")]
    [SerializeField] GameObject chosenDeathPrefab;
    // [BoxGroup("Death sequences params")]
    // [SerializeField] GameObject defaultDeathPrefab;
    // [BoxGroup("Death sequences params")]
    // [SerializeField] GameObject defaultWinPrefab;
    [BoxGroup("Death sequences params")]
    [SerializeField] GameObject transitionToDeathSequenceScreen;
    [BoxGroup("Death sequences params")]
    [SerializeField] GameObject transitionToSalvationSequenceScreen;
    [BoxGroup("Death sequences params")]
    [SerializeField] float fadeInToDeathSequenceInterval;
    [BoxGroup("Death sequences params")]
    [SerializeField] float fadeOutToDeathSequenceInterval;
    [BoxGroup("Death sequences params")]
    [SerializeField] PlayableDirector defaultWinDirector;
    [BoxGroup("Death sequences params")]
    [SerializeField] PlayableDirector defaultLossDirector;

    [HorizontalLine(color: EColor.White)]
    [SerializeField] SplitScreenAnim playerSplitScreenAnim;
    [SerializeField] SplitScreenAnim enemySplitScreenAnim;
    [SerializeField] SplitScreenAnim defaultDeathCamAnim;
    [SerializeField] SplitScreenAnim defaultBothWonCamAnim;
    [SerializeField] Ease camSlideTypeForSuspense;
    [Range(0, 10)]
    [SerializeField] float camSlideDurationForSuspense;
    [SerializeField] Ease camSlideTypeForBothLost;
    [Range(0, 10)]
    [SerializeField] float camSlideDurationForBothLost;
    [SerializeField] Ease camSlideTypeForBothWon;
    [Range(0, 10)]
    [SerializeField] float camSlideDurationForBothWon;
    [SerializeField] Ease camSlideTypeForPlayerWon;
    [Range(0, 10)]
    [SerializeField] float camSlideDurationForPlayerWon;
    [SerializeField] Ease camSlideTypeForEnemyWon;
    [Range(0, 10)]
    [SerializeField] float camSlideDurationForEnemyWon;

    [HorizontalLine(color: EColor.White)]
    [SerializeField] GameObject otherPlayerDecisionTextBetray;
    [SerializeField] GameObject otherPlayerDecisionTextSave;
    [SerializeField] GameObject localPlayerDecisionTextBetray;
    [SerializeField] GameObject localPlayerDecisionTextSave;
    [HorizontalLine(color: EColor.White)]
    [SerializeField] NumberCounter victoryPointsCounter;
    [SerializeField] NumberCounter localPlayerPointsCounter;
    [SerializeField] NumberCounter otherPlayerPointsCounter;
    [SerializeField] PlayableDirector endScreenDirector;
    [SerializeField] PlayableAsset endScreenWonTimeline;
    [SerializeField] PlayableAsset endScreenLostTimeline;
    [SerializeField] PlayableAsset endScreenBothWonTimeline;
    [SerializeField] PlayableAsset endScreenBothLostTimeline;
    [SerializeField] GameObject postcardObj;
    [SerializeField] TextMeshProUGUI postcardNameField;
    [SerializeField] TMP_InputField postcardTextField;

    #endregion

    #region public fields
    [Foldout("Debug")]
    [ReadOnly]
    public bool canChoose = false;
    // If you have multiple custom events, it is recommended to define them in the used class
#if UNITY_EDITOR
    [HorizontalLine(color: EColor.Green)]
    [HideInInspector]
#endif
    public MyEventsDictionary Outcomes;
    [HideInInspector] public PlayerSpot localPlayerSpot;
    [HideInInspector] public PlayerSpot otherPlayerSpot;
    [Foldout("Debug")]
    public int localPlayerPoints;
    [Foldout("Debug")]
    public int otherPlayerPoints;

    public const byte DecisionEvent = 1;
    public const byte DeathChoiceEvent = 2;
    public const byte FinalNoteEvent = 3;
    #endregion

    #region private fields
    IEnumerator gameTimerCoroutine;
    IEnumerator gameCycleCor;

    [Foldout("Debug")]
    [Dropdown("intValues")]
    [SerializeField] int debugPlayerSpot;
    private int[] intValues = new int[] { 1, 2 };
    [Foldout("Debug")]
    [OnValueChanged("OnValueChangedCallback")]
    [SerializeField] Choice myChoice;
    [Foldout("Debug")]
    [OnValueChanged("OnValueChangedCallback")]
    [SerializeField] Choice theirChoice;
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
    Dictionary<Choice, Dictionary<Choice, UnityEvent>> decisionMatrix;
    // somewhat newer way of handling outcomes than the decisionMatrix
    Dictionary<Choice, Dictionary<Choice, Outcome>> outcomeMatrix = new Dictionary<Choice, Dictionary<Choice, Outcome>>(){
      {Choice.Kill, new Dictionary<Choice, Outcome>(){
        {Choice.Kill, Outcome.BothLost},
        {Choice.Save, Outcome.Won}
      }},
      {Choice.Save, new Dictionary<Choice, Outcome>(){
        {Choice.Kill, Outcome.Lost},
        {Choice.Save, Outcome.BothWon}
      }}
    };
    private bool winnerChoseDeath;
    private Outcome votingOutcome = Outcome.Won;
    private GameObject endOfDiscussionScreen;
    private GameObject otherPlayerDecisionText;
    private GameObject localPlayerDecisionText;
    private Animator activeDeathBookAnimator;
    int decisionsMade;
    #endregion

    #region public methods
    public void StartGameCycle()
    {
      gameCycleCor = GameCycle();
      StartCoroutine(gameCycleCor);
    }

    public void Director_Stopped(PlayableDirector obj)
    {
      MyDebug.Log("Cinematic stopped");
      // reset timeline
      introDirector.time = 0;
      introDirector.Stop();
      introDirector.Evaluate();

      OnEndOfCinematic?.Invoke();

      InitializePlayers();
    }


    /// <summary>
    /// Choose whether to kill or save the other player. True for kill, false for save
    /// </summary>
    /// <param name="kill"></param>
    public void MakeDecision(bool kill)
    {
      if (!madeChoice)
      {

        int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
        object[] content = new object[] { senderID, kill, };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(DecisionEvent, content, raiseEventOptions, SendOptions.SendReliable);
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

      localPlayerSpot.playerUsingThisSpot = PhotonNetwork.LocalPlayer;
      localPlayerSpot.playerModel.SetActive(true);
      localPlayerSpot.gameplayCamerasParent.SetActive(true);
      localPlayerSpot.playerCam.SetActive(true);
      localPlayerSpot.killButton.enabled = true;
      localPlayerSpot.saveButton.enabled = true;
      localPlayerSpot.timer.enabled = true;
      // populate list of owned death sequences
      localPlayerSpot.PopulateDeathBook(DeathSequencesManager.Instance.universalDeathSequences);
      // assign current points to end screen counters
      // load point data about eachother
      if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(Keys.PLAYER_POINTS))
      {
        MyDebug.Log("Load points for local player");
        localPlayerPoints = (int)PhotonNetwork.LocalPlayer.CustomProperties[Keys.PLAYER_POINTS];
      }
      MyDebug.Log("localPlayerPoints", localPlayerPoints.ToString());
      localPlayerPointsCounter.Text.text = localPlayerPoints.ToString();

      // localPlayerSpot.playerModel = Instantiate(playerOnePrefab, localPlayerSpot.playerModelSpawnPos.transform.position, localPlayerSpot.playerModelSpawnPos.transform.rotation, localPlayerSpot.transform);

      otherPlayerSpot.playerUsingThisSpot = ScenarioManager.instance.otherPlayer;
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
      // assign current points to end screen counters
      if (ScenarioManager.instance.otherPlayer != null && ScenarioManager.instance.otherPlayer.CustomProperties.ContainsKey(Keys.PLAYER_POINTS))
      {
        MyDebug.Log("Load points for other player");
        otherPlayerPoints = (int)ScenarioManager.instance.otherPlayer.CustomProperties[Keys.PLAYER_POINTS];
      }
      MyDebug.Log("otherPlayerPoints", otherPlayerPoints.ToString());
      otherPlayerPointsCounter.Text.text = otherPlayerPoints.ToString();
      // otherPlayerSpot.playerModel = Instantiate(playerTwoPrefab, otherPlayerSpot.playerModelSpawnPos.transform.position, otherPlayerSpot.playerModelSpawnPos.transform.rotation, playerTwoSpot.transform);

    }

    public void SpawnDeathPrefab()
    {
      Instantiate(chosenDeathPrefab, deathPrefabSpawnPos.position, deathPrefabSpawnPos.rotation);
    }

    public void AnimateSplitScreen()
    {
      playerSplitScreenAnim.cam = localPlayerSpot.GetComponent<PlayerSpot>().playerCam.GetComponent<Camera>();
      playerSplitScreenAnim.AnimateCameraWidth(1f, 0.5f, 0f, 0f, 0f, 0f, camSlideTypeForSuspense, camSlideDurationForSuspense);
      enemySplitScreenAnim.AnimateCameraWidth(0f, 0.5f, 1f, 0.5f, 0f, 0f, camSlideTypeForSuspense, camSlideDurationForSuspense);
    }

    public void SetupOtherPlayerCamForSplitScreen()
    {
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
      localPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      localPlayerSpot.suspenseCam.SetActive(true);
      localPlayerSpot.playerCam = localPlayerSpot.suspenseCam;
      // localPlayerSpot.playerCam.GetComponent<Camera>().depth = 102;
      playerSplitScreenAnim.cam = localPlayerSpot.playerCam.GetComponent<Camera>();
    }
    public void ShowOtherPlayerDecisionAnim()
    {
      // either through animator or through coded IK motions, animate the voting movement
      otherPlayerSpot.playerModel.GetComponent<Animator>().SetTrigger("decision");
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
      localPlayerSpot.playerModel.GetComponent<Animator>().SetTrigger("decision");
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
    public void ShowDeathSequence()
    {
      if (madeChoice && theyMadeChoice)
      {
        switch (votingOutcome)
        {
          case Outcome.Won:
            StartCoroutine(YouWon());
            break;
          case Outcome.Lost:
            StartCoroutine(YouLost());
            break;
          case Outcome.BothWon:
            StartCoroutine(BothWon());
            break;
          case Outcome.BothLost:
            StartCoroutine(BothLost());
            break;
        }
      }
      else
      {
        if (theyMadeChoice)
        {
          StartCoroutine(YouLost());
        }
        else if (madeChoice)
        {
          StartCoroutine(YouWon());
        }
        else
        {
          StartCoroutine(BothLost());
        }
      }
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

    public void ChooseDeathSequence(DeathSequence deathSequence)
    {
      MyDebug.Log("Chosen Death sequence is", deathSequence.labelText);
      // if (DeathSequencesManager.Instance.deathSequences.Contains(death))
      int deathSequenceIndex = DeathSequencesManager.Instance.universalDeathSequences.FindIndex((obj) => { return obj.deathSequence == deathSequence; });
      if (ScenarioManager.instance.debugMode)
      {
        chosenDeathPrefab = deathSequence.gameObject;
        winnerChoseDeath = true; // this should be assigned when the event is recieved, not here
      }
      else
      {
        // to do: notify other user AND local player of the death scenario they chose)
        int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
        object[] content = new object[] { senderID, deathSequenceIndex };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(DeathChoiceEvent, content, raiseEventOptions, SendOptions.SendReliable);
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
      if (votingOutcome == Outcome.Won)
      {
        postcardObj.SetActive(true);

        // focus the input field
        postcardTextField.Select();
        postcardTextField.ActivateInputField();

        if (!ScenarioManager.instance.debugMode)
          postcardNameField.text = otherPlayerSpot.playerUsingThisSpot.NickName.Substring(0, otherPlayerSpot.playerUsingThisSpot.NickName.IndexOf("#"));
      }
    }

    public void SendFinalNoteToEnemy()
    {
      postcardObj.SetActive(false);

      MyDebug.Log("Sending final note");
      int senderID = PhotonNetwork.LocalPlayer.ActorNumber;
      object[] content = new object[] { senderID, postcardTextField.text, postcardNameField.text };
      RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
      PhotonNetwork.RaiseEvent(FinalNoteEvent, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void ActivateProperModelForEndScreen()
    {
      // activate proper model on endscreen
      localPlayerSpot.endScenePlayerModelLeft.SetActive(true);
      localPlayerSpot.endScenePlayerModelRight.SetActive(false);
      otherPlayerSpot.endScenePlayerModelLeft.SetActive(false);
      otherPlayerSpot.endScenePlayerModelRight.SetActive(true);

    }

    public void CalculatePointsAfterOutcome()
    {
      // set up some stuff depending on outcome
      switch (votingOutcome)
      {
        case Outcome.Won:
          //calculate points for each player
          PlayerPrefs.SetInt(Keys.PLAYER_POINTS_PREF, localPlayerPoints + MiscelaneousSettings.Instance.pointsForWin);
          localPlayerPointsCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.pointsForWin;
          otherPlayerPointsCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.pointsForLoss;
          victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.pointsForWin;
          break;
        case Outcome.Lost:
          PlayerPrefs.SetInt(Keys.PLAYER_POINTS_PREF, localPlayerPoints + MiscelaneousSettings.Instance.pointsForLoss);
          localPlayerPointsCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.pointsForLoss;
          otherPlayerPointsCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.pointsForWin;
          victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.pointsForLoss;
          break;
        case Outcome.BothWon:
          PlayerPrefs.SetInt(Keys.PLAYER_POINTS_PREF, localPlayerPoints + MiscelaneousSettings.Instance.pointsForBothWon);
          localPlayerPointsCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.pointsForBothWon;
          otherPlayerPointsCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.pointsForBothWon;
          victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.pointsForBothWon;
          break;
        case Outcome.BothLost:
          PlayerPrefs.SetInt(Keys.PLAYER_POINTS_PREF, localPlayerPoints + MiscelaneousSettings.Instance.pointsForBothLost);
          localPlayerPointsCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.pointsForBothLost;
          otherPlayerPointsCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.pointsForBothLost;
          victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.pointsForBothLost;
          break;
        default:
          break;
      }
    }
    public void CalculateXPAfterOutcome()
    {
      // set up some stuff depending on outcome
      switch (votingOutcome)
      {
        case Outcome.Won:
          //calculate points for each player
          PlayerPrefs.SetInt(Keys.PLAYER_XP, PlayerPrefs.GetInt(Keys.PLAYER_XP, 0) + MiscelaneousSettings.Instance.xpForWin);
          // localPlayerXPCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.xpForWin;
          // otherPlayerXPCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.xpForLoss;
          // victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.xpForWin;
          break;
        case Outcome.Lost:
          PlayerPrefs.SetInt(Keys.PLAYER_XP, PlayerPrefs.GetInt(Keys.PLAYER_XP, 0) + MiscelaneousSettings.Instance.xpForLoss);
          // localPlayerXPCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.xpForLoss;
          // otherPlayerXPCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.xpForWin;
          // victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.xpForLoss;
          break;
        case Outcome.BothWon:
          PlayerPrefs.SetInt(Keys.PLAYER_XP, PlayerPrefs.GetInt(Keys.PLAYER_XP, 0) + MiscelaneousSettings.Instance.xpForBothWon);
          // localPlayerXPCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.xpForBothWon;
          // otherPlayerXPCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.xpForBothWon;
          // victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.xpForBothWon;
          break;
        case Outcome.BothLost:
          PlayerPrefs.SetInt(Keys.PLAYER_XP, PlayerPrefs.GetInt(Keys.PLAYER_XP, 0) + MiscelaneousSettings.Instance.xpForBothLost);
          // localPlayerXPCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.xpForBothLost;
          // otherPlayerXPCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.xpForBothLost;
          // victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.xpForBothLost;
          break;
        default:
          break;
      }
    }
    public void CalculateRankAfterOutcome()
    {
      // set up some stuff depending on outcome
      switch (votingOutcome)
      {
        case Outcome.Won:
          //calculate points for each player
          PlayerPrefs.SetInt(Keys.PLAYER_RANK, PlayerPrefs.GetInt(Keys.PLAYER_RANK, 0) + MiscelaneousSettings.Instance.rankForWin);
          // localPlayerRankCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.rankForWin;
          // otherPlayerRankCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.rankForLoss;
          // victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.rankForWin;
          break;
        case Outcome.Lost:
          PlayerPrefs.SetInt(Keys.PLAYER_RANK, PlayerPrefs.GetInt(Keys.PLAYER_RANK, 0) + MiscelaneousSettings.Instance.rankForLoss);
          // localPlayerRankCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.rankForLoss;
          // otherPlayerRankCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.rankForWin;
          // victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.rankForLoss;
          break;
        case Outcome.BothWon:
          PlayerPrefs.SetInt(Keys.PLAYER_RANK, PlayerPrefs.GetInt(Keys.PLAYER_RANK, 0) + MiscelaneousSettings.Instance.rankForBothWin);
          // localPlayerRankCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.rankForBothWin;
          // otherPlayerRankCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.rankForBothWin;
          // victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.rankForBothWin;
          break;
        case Outcome.BothLost:
          PlayerPrefs.SetInt(Keys.PLAYER_RANK, PlayerPrefs.GetInt(Keys.PLAYER_RANK, 0) + MiscelaneousSettings.Instance.rankForBothLost);
          // localPlayerRankCounter.newAmount = localPlayerPoints += MiscelaneousSettings.Instance.rankForBothLost;
          // otherPlayerRankCounter.newAmount = otherPlayerPoints += MiscelaneousSettings.Instance.rankForBothLost;
          // victoryPointsCounter.newAmount = MiscelaneousSettings.Instance.rankForBothLost;
          break;
        default:
          break;
      }
    }

    public void DisableFog()
    {
      RenderSettings.fog = false;
    }

    public void ShowNameplates()
    {
      if (localPlayerSpot.playerUsingThisSpot != null)
        localPlayerSpot.nameplateText.text = localPlayerSpot.playerUsingThisSpot.NickName.Substring(0, localPlayerSpot.playerUsingThisSpot.NickName.IndexOf("#"));
      localPlayerSpot.nameplateCanvas.SetActive(true);

      if (otherPlayerSpot.playerUsingThisSpot != null)
        otherPlayerSpot.nameplateText.text = otherPlayerSpot.playerUsingThisSpot.NickName.Substring(0, otherPlayerSpot.playerUsingThisSpot.NickName.IndexOf("#"));
      otherPlayerSpot.nameplateCanvas.SetActive(true);
    }
    #endregion


    #region Monobehaviour callbacks

    private void Awake()
    {
      if (!Instance)
        Instance = this;
      else
        Destroy(this);

      if (introDirector != null)
      {
        introDirector.played += Director_Played;
        introDirector.stopped += Director_Stopped;
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
      if (introDirector != null)
      {
        introDirector.played -= Director_Played;
        introDirector.stopped -= Director_Stopped;
      }
      if (gameTimerCoroutine != null)
        StopCoroutine(gameTimerCoroutine);
      if (gameCycleCor != null)
        StopCoroutine(gameCycleCor);

      // return fog to normal state
      RenderSettings.fog = true;


      StopAllCoroutines();
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
      // initialize the outcome matrix (same as decision matrix, just newer)
      outcomeMatrix = new Dictionary<Choice, Dictionary<Choice, Outcome>>(){
        {Choice.Kill, new Dictionary<Choice, Outcome>(){
          {Choice.Kill, Outcome.BothLost},
          {Choice.Save, Outcome.Won}
        }},
        {Choice.Save, new Dictionary<Choice, Outcome>(){
          {Choice.Kill, Outcome.Lost},
          {Choice.Save, Outcome.BothWon}
        }}
      };

      // initialize game properly depending if its in debug mode or not
      if (ScenarioManager.instance.debugMode)
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
    }

    private void Update()
    {
      if (localPlayerSpot != null)
        if (localPlayerSpot.GetComponent<CameraSwitcher>() != null)
          localPlayerSpot.GetComponent<CameraSwitcher>().SwitchCamera();
    }
    #endregion
    #region private methods
    IEnumerator GameCycle()
    {
      // if there is a timeline wait for the end of it to start the game, else start it now
      if (introDirector != null && introDirector.playableAsset != null)
        yield return StartCoroutine(StartTimeline());
      // initialie players
      yield return StartCoroutine(InitializePlayers());
      // game started. start timer
      GameStarted?.Invoke();
      yield return StartCoroutine(DiscussionPhase());
      yield return StartCoroutine(TransitionPhase());
      if (madeChoice && theyMadeChoice)
      {
        yield return StartCoroutine(SuspensePhase());
        // if both players saved or killed don't start DeathChoicePhase
        if (!((theirChoice == Choice.Kill && myChoice == Choice.Kill) || (theirChoice == Choice.Save && myChoice == Choice.Save)))
          yield return StartCoroutine(DeathChoicePhase());
      }
      else if (madeChoice || theyMadeChoice)
        yield return StartCoroutine(SomebodyVotedPhase());
      else
        yield return StartCoroutine(NobodyVotedPhase());

      yield return StartCoroutine(DeathSequencePhase());
      yield return StartCoroutine(PostGameScreenPhase());

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

    IEnumerator InitializePlayers()
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
      yield return null;
    }
    IEnumerator DiscussionPhase()
    {
      yield return new WaitForSeconds(DiscussionPhaseEvents.BeforePause);

      MyDebug.Log("DiscussionPhase", "started");
      canChoose = true;

      // show feedback stuff
      try
      {
        DiscussionPhaseEvents.OnStarted?.Invoke(); // two of these are probably unnecessary
      }
      catch (System.Exception ex)
      {
        MyDebug.Log(ex.ToString());
      }
      localPlayerSpot.TimerStarted?.Invoke(); // two of these are probably unnecessary
      localPlayerSpot.gameTimer.StartTimer(DiscussionPhaseEvents.Interval); // two of these are probably unnecessary

      // this phase lasts until time runs out or both players choose
      float timer = DiscussionPhaseEvents.Interval;
      while (!(madeChoice && theyMadeChoice) && timer > 0)
      {
        timer -= Time.deltaTime;
        yield return null;
      }

      MyDebug.Log("DiscussionPhase", "ended");
      canChoose = false;

      // show feedback stuff
      try
      {
        DiscussionPhaseEvents.OnEnded?.Invoke();
      }
      catch (System.Exception ex)
      {
        MyDebug.Log(ex.ToString());
      }
      localPlayerSpot.TimerFinished?.Invoke();

      // force decision if no decision was made
      // if (!madeChoice)
      //   ForceDecisionForLocalPlayer();

      // handle case when someone (or both) didnt vote
      if (!madeChoice)
      {
        if (theyMadeChoice)
          votingOutcome = Outcome.Lost;
        else
          votingOutcome = Outcome.BothLost;
      }
      else if (!theyMadeChoice)
      {
        if (madeChoice)
          votingOutcome = Outcome.Won;
        else
          votingOutcome = Outcome.BothLost;
      }

      yield return new WaitForSeconds(DiscussionPhaseEvents.AfterPause);
    }
    IEnumerator TransitionPhase()
    {
      yield return new WaitForSeconds(TransitionPhaseEvents.BeforePause);

      MyDebug.Log("TransitionPhase", "started");

      // show feedback stuff
      TransitionPhaseEvents.OnStarted?.Invoke();

      yield return new WaitForSeconds(TransitionPhaseEvents.Interval);

      MyDebug.Log("TransitionPhase", "ended");

      // show feedback stuff
      TransitionPhaseEvents.OnEnded?.Invoke();

      yield return new WaitForSeconds(TransitionPhaseEvents.AfterPause);
    }
    IEnumerator SuspensePhase()
    {
      yield return new WaitForSeconds(SuspensePhaseSequence.BeforePause);

      MyDebug.Log("SuspensePhase", "started");

      // show feedback stuff
      SuspensePhaseSequence.OnStarted?.Invoke();

      foreach (EventEnclosure evEn in SuspensePhaseSequence.eventSequence)
      {
        yield return new WaitForSeconds(evEn.BeforePause);
        evEn.OnStarted?.Invoke();
        yield return new WaitForSeconds(evEn.Interval);
        evEn.OnEnded?.Invoke();
        yield return new WaitForSeconds(evEn.BeforePause);
      }

      MyDebug.Log("SuspensePhase", "ended");

      // show feedback stuff
      SuspensePhaseSequence.OnEnded?.Invoke();

      yield return new WaitForSeconds(SuspensePhaseSequence.AfterPause);
    }

    IEnumerator NobodyVotedPhase()
    {
      yield return new WaitForSeconds(NobodyVotedPhaseSequence.BeforePause);

      MyDebug.Log("NobodyVotedPhase", "started");

      // show feedback stuff
      NobodyVotedPhaseSequence.OnStarted?.Invoke();

      foreach (EventEnclosure evEn in NobodyVotedPhaseSequence.eventSequence)
      {
        yield return new WaitForSeconds(evEn.BeforePause);
        evEn.OnStarted?.Invoke();
        yield return new WaitForSeconds(evEn.Interval);
        evEn.OnEnded?.Invoke();
        yield return new WaitForSeconds(evEn.BeforePause);
      }

      MyDebug.Log("NobodyVotedPhase", "ended");

      // show feedback stuff
      DeathSequencePhaseEvents.OnEnded?.Invoke();

      yield return new WaitForSeconds(NobodyVotedPhaseSequence.AfterPause);
    }
    IEnumerator SomebodyVotedPhase()
    {
      yield return new WaitForSeconds(SomebodyVotedPhaseSequence.BeforePause);

      MyDebug.Log("SomebodyVotedPhase", "started");

      // show feedback stuff
      SomebodyVotedPhaseSequence.OnStarted?.Invoke();

      foreach (EventEnclosure evEn in SomebodyVotedPhaseSequence.eventSequence)
      {
        yield return new WaitForSeconds(evEn.BeforePause);
        evEn.OnStarted?.Invoke();
        yield return new WaitForSeconds(evEn.Interval);
        evEn.OnEnded?.Invoke();
        yield return new WaitForSeconds(evEn.BeforePause);
      }

      MyDebug.Log("SomebodyVotedPhase", "ended");

      // show feedback stuff
      DeathSequencePhaseEvents.OnEnded?.Invoke();

      yield return new WaitForSeconds(SomebodyVotedPhaseSequence.AfterPause);
    }

    IEnumerator DeathChoicePhase()
    {
      yield return new WaitForSeconds(DeathChoiceScreenPhaseEvents.BeforePause);

      MyDebug.Log("DeathChoicePhase", "started");

      // show feedback stuff
      DeathChoiceScreenPhaseEvents.OnStarted?.Invoke();

      // this phase lasts until time runs out or player chooses death
      float timer = DeathChoiceScreenPhaseEvents.Interval;
      while (!winnerChoseDeath && timer > 0)
      {
        timer -= Time.deltaTime;
        yield return null;
      }

      MyDebug.Log("DeathChoicePhase", "ended");

      // show feedback stuff
      DeathChoiceScreenPhaseEvents.OnEnded?.Invoke();

      yield return new WaitForSeconds(DeathChoiceScreenPhaseEvents.AfterPause);
    }
    IEnumerator DeathSequencePhase()
    {
      yield return new WaitForSeconds(DeathSequencePhaseEvents.BeforePause);

      MyDebug.Log("DeathSequencePhase", "started");

      // show feedback stuff
      DeathSequencePhaseEvents.OnStarted?.Invoke();

      yield return new WaitForSeconds(DeathSequencePhaseEvents.Interval);

      MyDebug.Log("DeathSequencePhase", "ended");

      // show feedback stuff
      DeathSequencePhaseEvents.OnEnded?.Invoke();

      yield return new WaitForSeconds(DeathSequencePhaseEvents.AfterPause);
    }
    IEnumerator PostGameScreenPhase()
    {
      yield return new WaitForSeconds(PostGameScreenPhaseEvents.BeforePause);

      MyDebug.Log("PostGameScreenPhase", "started");

      // show feedback stuff
      PostGameScreenPhaseEvents.OnStarted?.Invoke();

      ShowPostGameScreen();

      yield return new WaitForSeconds(PostGameScreenPhaseEvents.Interval);

      MyDebug.Log("PostGameScreenPhase", "ended");

      // show feedback stuff
      PostGameScreenPhaseEvents.OnEnded?.Invoke();

      yield return new WaitForSeconds(PostGameScreenPhaseEvents.AfterPause);

      ReturnToMainMenu();
    }

    IEnumerator StartTimeline()
    {
      introDirector.Play();
      yield return StartCoroutine(SkipIntroCoroutine());
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

    private void Director_Played(PlayableDirector obj)
    {
      MyDebug.Log("Cinematic played");

    }

    private void ForceDecisionForLocalPlayer()
    {
      MakeDecision(true);
      OnRanOutOfTime?.Invoke();
    }

    IEnumerator BothLost()
    {
      MyDebug.Log("Both lost");
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
      // disable all other cameras
      otherPlayerSpot.playerCam.SetActive(false);
      localPlayerSpot.playerCam.SetActive(false);
      defaultDeathCamAnim.cam.gameObject.SetActive(false);
      localPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      otherPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      // play commn death
      // Instantiate(defaultDeathPrefab, deathPrefabSpawnPos.position, deathPrefabSpawnPos.rotation);
      if (defaultLossDirector)
        defaultLossDirector.Play();
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
      otherPlayerSpot.playerCam.SetActive(false);
      localPlayerSpot.playerCam.SetActive(false);
      defaultDeathCamAnim.cam.enabled = false;
      localPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      otherPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      // play victory
      // Instantiate(defaultWinPrefab, deathPrefabSpawnPos.position, deathPrefabSpawnPos.rotation);
      if (defaultWinDirector)
        defaultWinDirector.Play();
    }

    IEnumerator YouLost()
    {
      MyDebug.Log("You lost");
      playerSplitScreenAnim.AnimateCameraWidth(0.5f, 1f, 0f, 0f, 0f, 0f, camSlideTypeForEnemyWon, camSlideDurationForEnemyWon);
      enemySplitScreenAnim.AnimateCameraWidth(0.5f, 0f, 0.5f, 1f, 0f, 0f, camSlideTypeForEnemyWon, camSlideDurationForEnemyWon);
      yield return new WaitForSeconds(camSlideDurationForEnemyWon);
      // fade out screen
      transitionToDeathSequenceScreen.SetActive(true);
      yield return new WaitForSeconds(fadeInToDeathSequenceInterval);
      transitionToDeathSequenceScreen.GetComponent<Animator>().SetTrigger("FadeOut");
      yield return new WaitForSeconds(fadeOutToDeathSequenceInterval);
      // disable all other cameras
      otherPlayerSpot.playerCam.SetActive(false);
      localPlayerSpot.playerCam.SetActive(false);
      defaultDeathCamAnim.cam.gameObject.SetActive(false);
      localPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      otherPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      // play chosen death scene (by other player)
      if (chosenDeathPrefab != null)
        Instantiate(chosenDeathPrefab, deathPrefabSpawnPos.position, deathPrefabSpawnPos.rotation);
    }

    IEnumerator YouWon()
    {
      MyDebug.Log("You won");
      // animate camera
      enemySplitScreenAnim.AnimateCameraWidth(enemySplitScreenAnim.cam.rect.width, 1f, 0.5f, 0f, 0f, 0f, camSlideTypeForPlayerWon, camSlideDurationForPlayerWon);
      playerSplitScreenAnim.AnimateCameraWidth(0.5f, 0f, 0f, 0f, 0f, 0f, camSlideTypeForPlayerWon, camSlideDurationForPlayerWon);
      yield return new WaitForSeconds(camSlideDurationForPlayerWon);
      // fade out screen
      transitionToDeathSequenceScreen.SetActive(true);
      yield return new WaitForSeconds(fadeInToDeathSequenceInterval);
      transitionToDeathSequenceScreen.GetComponent<Animator>().SetTrigger("FadeOut");
      yield return new WaitForSeconds(fadeOutToDeathSequenceInterval);
      // disable all other cameras
      otherPlayerSpot.playerCam.SetActive(false);
      localPlayerSpot.playerCam.SetActive(false);
      defaultDeathCamAnim.cam.gameObject.SetActive(false);
      localPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      otherPlayerSpot.GetComponent<CameraSwitcher>().DisableGameplayCameras();
      // play chosen death scene
      if (chosenDeathPrefab != null)
        Instantiate(chosenDeathPrefab, deathPrefabSpawnPos.position, deathPrefabSpawnPos.rotation);
    }

    void OtherPlayerOutcome()
    {
      otherPlayerSpot.decisionMatrix[theirChoice][myChoice]?.Invoke();
    }

    void ShowPostGameScreen()
    {
      if (postGameScreen) postGameScreen.SetActive(true);
    }
    void ReturnToMainMenu()
    {
      PhotonNetwork.Disconnect();

    }

    private void DeathChoiceEventFunc(EventData photonEvent)
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
          chosenDeathPrefab = DeathSequencesManager.Instance.universalDeathSequences[deathSequenceIndex].deathSequence.gameObject;

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

    private void FinalNoteEventFunc(EventData photonEvent)
    {
      // extract info from received data
      object[] data = (object[])photonEvent.CustomData;
      int senderID = (int)data[0];
      string textSent = (string)data[1];
      string nickname = (string)data[2];

      // who's choice it was
      if (senderID == PhotonNetwork.LocalPlayer.ActorNumber)
      {
        // play animation for sending mail
        postcardObj.SetActive(false);
      }
      else
      {
        // save the text locally. in json maybe? could do, could do.
        MyDebug.Log("FinalNote text", textSent);
        MyDebug.Log("FinalNote nickname", nickname);

        FinalNote newNote = new FinalNote(nickname, textSent);
        FinalNoteCardHandler.SaveFinalNote(newNote);
      }
    }

    private void DecisionEventFunc(EventData photonEvent)
    {
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
        }
        else
        {
          theirChoice = choice;
          theyMadeChoice = true;
          OnTheirChoiceMade?.Invoke();
          otherPlayerSpot.OnMyChoiceMade?.Invoke();
          localPlayerSpot.OnTheirChoiceMade?.Invoke();
        }

        decisionsMade++;

        // if both players made the choice execute the outcome
        if (decisionsMade >= 2)
        {
          // we'll need to know the outcome so we can decide flows
          votingOutcome = outcomeMatrix[myChoice][theirChoice];

          // visual feedback
          OnBothPlayersChose?.Invoke();


          // visual feedback
          // decisionMatrix[myChoice][theirChoice]?.Invoke();
          // localPlayerSpot.decisionMatrix[myChoice][theirChoice]?.Invoke();
          // call other choices with delay
          // Invoke("OtherPlayerOutcome", otherPlayerOutcomeDelay);

          // logic event
          VotingEnded?.Invoke();
        }

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
        case DeathChoiceEvent:
          DeathChoiceEventFunc(photonEvent);
          break;
        case FinalNoteEvent:
          FinalNoteEventFunc(photonEvent);
          break;
        default:
          break;
      }


    }

    #endregion

  }
}