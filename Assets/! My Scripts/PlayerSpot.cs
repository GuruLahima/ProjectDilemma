using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.Events;
using GuruLaghima;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.Animations.Rigging;
using cakeslice;

namespace Workbench.ProjectDilemma
{
  public class PlayerSpot : MonoBehaviour
  {
    #region public fields
    public int playerSpot;
    // public Camera deathCam;
    // public Camera surviveCam;
    public GameObject gameplayCamerasParent; // the container for the cinemachine virtual cams
    public GameObject suspenseCam;
    public GameObject playerCam;// the active virtual cam
    // we need a reference to this specific cam because it is the most important one and many things depend on it, like throwables
    public GameObject firstPersonPlayerCam;
    public GameObject mainCam; // the cinemachine brain cam
    public OutlineEffect outlineComponent;
    public GameObject nameplateCanvas;
    public TextMeshProUGUI nameplateText;

    [HorizontalLine(color: EColor.White)]
    public GameObject playerModelSpawnPos;
    public GameObject playerModel;
    public UnityEngine.Animations.Rigging.Rig playerRig;
    public GameObject endScenePlayerModelLeft;
    public GameObject endScenePlayerModelRight;
    [HorizontalLine(color: EColor.White)]
    public GameObject votingMechanism;
    public Button killButton;
    public Button saveButton;
    // public TextMeshProUGUI timer; // deprecated, using UI overlay now
    public GameObject decisionTextSave;
    public GameObject decisionTextBetray;
    public GameTimer gameTimer;
    public GameTimer extraTimer;
    [HorizontalLine(color: EColor.White)]
    public GameObject deathBookForPlayerCam;
    public GameObject deathBookForEnemyCam;
    public GameObject myDeathBook;
    public Image deathBookLeftPage;
    public Image deathBookRightPage;
    public DeathSequence_UI_Item deathItemPrefab;
    [HorizontalLine(color: EColor.White)]
    public ThrowablesActivator throwablesActivator;
    public EmoteActivator emoteActivator;
    public PerkActivator perkActivator;
    public AbilityActivator abilityActivator;
    public QuestActivator questActivator;
    public RelicActivator relicActivator;
    public ExtrasActivator extrasActivator;
    public OutfitLoader outfitLoader;
    [HorizontalLine(color: EColor.White)]
    public Transform character;
    public Transform rigRoot;
    public int[] clothesId;

    [ReadOnly] public bool playerLoaded;
    [ReadOnly] public bool requestedExtraTime;

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

    /// <summary>
    /// this matrix denotes what outcome happens at which coombinations of choices
    /// Upper level key denotes the choice of the local player, the lower level key denotes other player choice
    /// works together with the "MyEventsDictionary OutcomesOutcomes" variable
    /// </summary>
    public Dictionary<Choice, Dictionary<Choice, UnityEvent>> decisionMatrix;
    public MyEventsDictionary Outcomes;
    #endregion


    #region exposed fields
    [HideInInspector] public Player playerUsingThisSpot;
    #endregion


    #region private fields

    #endregion  


    #region public methods
    public void PopulateDeathBook(Scenario thisScenario, List<OwnableDeathSequence> universalDeathSequences)
    {
      // add death sequences from this scenario to the death book
      foreach (OwnableDeathSequence item in thisScenario.defaultDeathSequences)
      {
        DeathSequence_UI_Item tmpItem = Instantiate(deathItemPrefab.gameObject, deathBookLeftPage.transform, false).GetComponent<DeathSequence_UI_Item>();
        tmpItem.deathSequence = item.deathSequence;
        if (tmpItem.backgroundImg && item.deathSequence.backgroundSprite)
          tmpItem.backgroundImg.sprite = item.deathSequence.backgroundSprite;
        if (tmpItem.label)
          tmpItem.label.text = item.deathSequence.labelText;
        if (!item.owned)
        {
          tmpItem.backgroundImg.material = tmpItem.unavailableMat;
          tmpItem.GetComponent<Button>().interactable = false;
        }
        else
          tmpItem.GetComponent<Button>().onClick.AddListener(delegate () { GameMechanic.Instance.ChooseScenarioDeathSequence(tmpItem.deathSequence); });
      }
      // add universal death sequences to the death book
      foreach (OwnableDeathSequence item in universalDeathSequences)
      {
        DeathSequence_UI_Item tmpItem = Instantiate(deathItemPrefab.gameObject, deathBookLeftPage.transform, false).GetComponent<DeathSequence_UI_Item>();
        tmpItem.deathSequence = item.deathSequence;
        if (tmpItem.backgroundImg && item.deathSequence.backgroundSprite)
          tmpItem.backgroundImg.sprite = item.deathSequence.backgroundSprite;
        if (tmpItem.label)
          tmpItem.label.text = item.deathSequence.labelText;
        if (!item.owned)
        {
          tmpItem.backgroundImg.material = tmpItem.unavailableMat;
          tmpItem.GetComponent<Button>().interactable = false;
        }
        else
          tmpItem.GetComponent<Button>().onClick.AddListener(delegate () { GameMechanic.Instance.ChooseUniversalDeathSequence(tmpItem.deathSequence); });
      }

    }
    #endregion


    #region MonoBehaviour callbacks

    #endregion


    #region private methods

    #endregion


    #region networking code

    #endregion
    // Start is called before the first frame update
    void Start()
    {
      decisionMatrix = new Dictionary<Choice, Dictionary<Choice, UnityEvent>>(){
        {Choice.Kill, new Dictionary<Choice, UnityEvent>(){
/*           {Choice.Kill, Outcomes.GetItems()["Kill Each Other"]},
          {Choice.Save, Outcomes.GetItems()["I kill they save"]} */
        }},
        {Choice.Save, new Dictionary<Choice, UnityEvent>(){
/*           {Choice.Kill, Outcomes.GetItems()["They kill I save"]},
          {Choice.Save, Outcomes.GetItems()["Save each other"]} */
        }}
      };
    }

  }
}