using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.Events;
using GuruLaghima;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
namespace Workbench.ProjectDilemma
{
  public class PlayerSpot : MonoBehaviour
  {
    #region public fields
    public Camera deathCam;
    public Camera surviveCam;
    public GameObject suspenseCam;
    public GameObject gameplayCamerasParent;
    public GameObject playerCam;
    public GameObject nameplateCanvas;
    public TextMeshProUGUI nameplateText;

    [HorizontalLine(color: EColor.White)]
    public GameObject playerModelSpawnPos;
    public GameObject playerModel;
    public GameObject endScenePlayerModelLeft;
    public GameObject endScenePlayerModelRight;
    [HorizontalLine(color: EColor.White)]
    public GameObject votingMechanism;
    public Collider killButton;
    public Collider saveButton;
    public TextMeshProUGUI timer;
    public GameObject decisionTextSave;
    public GameObject decisionTextBetray;
    public GameTimer gameTimer;
    [HorizontalLine(color: EColor.White)]
    public GameObject deathBookForPlayerCam;
    public GameObject deathBookForEnemyCam;
    public GameObject myDeathBook;
    public Image deathBookLeftPage;
    public Image deathBookRightPage;
    public DeathSequence_UI_Item deathItemPrefab;

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
    public void PopulateDeathBook(List<OwnableDeathSequence> collection)
    {
      foreach (OwnableDeathSequence item in collection)
      {
        DeathSequence_UI_Item tmpItem = Instantiate(deathItemPrefab.gameObject, deathBookLeftPage.transform, false).GetComponent<DeathSequence_UI_Item>();
        tmpItem.deathSequence = item.deathSequence;
        if (tmpItem.backgroundImg)
          tmpItem.backgroundImg.sprite = item.deathSequence.backgroundSprite;
        if (tmpItem.label)
          tmpItem.label.text = item.deathSequence.labelText;
        if (!item.owned)
        {
          tmpItem.backgroundImg.material = tmpItem.unavailableMat;
          tmpItem.GetComponent<Button>().interactable = false;
        }
        else
          tmpItem.GetComponent<Button>().onClick.AddListener(delegate () { GameMechanic.Instance.ChooseDeathSequence(tmpItem.deathSequence); });
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