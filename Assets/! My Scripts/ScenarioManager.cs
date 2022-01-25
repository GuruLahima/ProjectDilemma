using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using NaughtyAttributes;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System;
using Photon.Pun;
using Photon.Realtime;

namespace Workbench.ProjectDilemma
{

  public class ScenarioManager : MonoBehaviourPunCallbacks
  {

    #region public fields
    public static ScenarioManager instance;
    #endregion


    #region exposed fields
    [SerializeField] GameObject gameplayCamera;
#if UNITY_EDITOR
    [Scene]
#endif
    [SerializeField] string mainMenuScene;
    #endregion


    #region private fields
    PlayableDirector director;
    #endregion

    #region Unity Events
#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent OnLoadOfScenario;
#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent OnEndOfCinematic;
#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent OnDisconnectedEvent;
#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent OnOtherPlayerLeftTheRoom;
    #endregion


    #region public methods
    public void StartTimeline()
    {
      director.Play();
    }
    #endregion


    #region MonoBehaviour callbacks
    private void Awake()
    {
      if (instance == null)
        instance = this;
      else
        Destroy(this.gameObject);

      director = GetComponent<PlayableDirector>();
      director.played += Director_Played;
      director.stopped += Director_Stopped;
    }


    public override void OnEnable()
    {
      base.OnEnable();
      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
      base.OnDisable();
      director.played -= Director_Played;
      director.stopped -= Director_Stopped;
      SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      MyDebug.Log("OnSceneLoaded: " + scene.name);

      // do some kind of transition from main menu to scenario
      if (scene.name != mainMenuScene)
        OnLoadOfScenario?.Invoke();
    }

    // Start is called before the first frame update
    // void Start()
    // {

    // }

    // Update is called once per frame
    // void Update()
    // {

    // }
    #endregion


    #region private methods
    private void Director_Stopped(PlayableDirector obj)
    {
      MyDebug.Log("Cinematic stopped");
      // reset timeline
      director.time = 0;
      director.Stop();
      director.Evaluate();

      OnEndOfCinematic?.Invoke();
    }

    private void Director_Played(PlayableDirector obj)
    {
      MyDebug.Log("Cinematic played");

    }


    #endregion


    #region networking code

    public override void OnPlayerLeftRoom(Player player)
    {
      // when other player left return to main menu
      PhotonNetwork.Disconnect();

      OnOtherPlayerLeftTheRoom?.Invoke();
    }

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
      MyDebug.Log("OnDisconnected", cause.ToString(), Color.red);

      OnDisconnectedEvent?.Invoke();



      // if we are not in main menu load main menu
      if (SceneManager.GetActiveScene().name != mainMenuScene)
        PhotonNetwork.LoadLevel(mainMenuScene);
    }



    #endregion



  }
}
