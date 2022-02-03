using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.Events;
using GuruLaghima;

namespace Workbench.ProjectDilemma
{
  public class PlayerSpot : MonoBehaviour
  {
    #region public fields
    public GameObject gameplayCamerasParent;
    public GameObject playerCam;
    public GameObject playerModelSpawnPos;
    public Collider killButton;
    public Collider saveButton;
    public TextMeshProUGUI timer;
    public GameObject playerModel;
    public GameTimer gameTimer;

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

    #endregion


    #region private fields

    #endregion


    #region public methods

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
          {Choice.Kill, Outcomes.GetItems()["Kill Each Other"]},
          {Choice.Save, Outcomes.GetItems()["I kill they save"]}
        }},
        {Choice.Save, new Dictionary<Choice, UnityEvent>(){
          {Choice.Kill, Outcomes.GetItems()["They kill I save"]},
          {Choice.Save, Outcomes.GetItems()["Save each other"]}
        }}
      };
    }

    // Update is called once per frame
    void Update()
    {

    }
  }
}