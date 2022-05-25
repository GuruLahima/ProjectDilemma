using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using NaughtyAttributes;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using GuruLaghima;

namespace Workbench.ProjectDilemma
{

  public class ScenarioManager : MonoBehaviourPunCallbacks
  {

    // public static ScenariosManager 

    #region public fields
    public static ScenarioManager instance;
    public bool debugMode;
    #endregion


    #region exposed fields
#if UNITY_EDITOR
    [Scene]
#endif
    [SerializeField] string mainMenuScene;
    [SerializeField] public Scenario thisScenario;
    [SerializeField] public GameObject GameFoundationObj;

    #endregion


    #region private fields
    #endregion

    #region Unity Events
#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent OnLoadOfScenario;

#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent OnDisconnectedEvent;
#if UNITY_EDITOR
    [Foldout("Visual Feedback Events")]
#endif
    public UnityEvent OnOtherPlayerLeftTheRoom;
    private int playersLoadedScene;

    #endregion


    #region public methods


    #endregion


    #region MonoBehaviour callbacks
    private void Awake()
    {
      if (instance == null)
        instance = this;
      else
        Destroy(this.gameObject);

      // this ensures no matter what we do during testing and debugging when we run the game from main menu it doesn't enter debug
      if (PhotonNetwork.IsConnected)
        debugMode = false;
    }


    public override void OnEnable()
    {
      base.OnEnable();
      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
      base.OnDisable();

      SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
      if (debugMode)
      {

        GetComponent<GameMechanic>().StartGameSequence();
        GameFoundationObj.SetActive(true);

      }
#else
        GameFoundationObj.SetActive(false);

#endif

    }

    // Update is called once per frame
    // void Update()
    // {


    // }
    #endregion


    #region private methods

    Hashtable ResetCustomProperties(Hashtable properties)
    {
      Hashtable temphashtable = new Hashtable();
      foreach (var item in properties)
      {
        temphashtable.Add(item.Key, null);
      }
      return temphashtable;
    }



    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      MyDebug.Log("OnSceneLoaded: " + scene.name);

      // do some kind of transition from main menu to scenario
      if (scene.name != mainMenuScene)
        OnLoadOfScenario?.Invoke();

      // update network properties: a player loaded the gameplay scene
      Hashtable hashtable = new Hashtable()
      {
        {Keys.MAP_PROP_KEY, "Gameplay Level" }
      };
      PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);



      // if (GetComponent<PlayableDirector>().playableAsset == null)
      // {
      //   GetComponent<GameMechanic>().Director_Stopped(null);
      // }


    }



    #endregion


    #region networking code

    public override void OnPlayerLeftRoom(Player player)
    {
      if (GameMechanic.Instance.canChoose)
        // when other player leaves skip to postgame screen (only if we havent reached end of discussion time)
        GameMechanic.Instance.OnPlayerDisconnect();

      OnOtherPlayerLeftTheRoom?.Invoke();
    }

    /// <summary>
    /// Called after disconnecting from the Photon server.
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
      MyDebug.Log("OnDisconnected", cause.ToString(), Color.red);

      OnDisconnectedEvent?.Invoke();

      // clear photon properties so next game starts with clean slate
      if (PhotonNetwork.IsMasterClient)
      {
        PhotonNetwork.CurrentRoom.SetCustomProperties(ResetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties));
      }
      PhotonNetwork.LocalPlayer.SetCustomProperties(ResetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties));

      // if we are not in main menu load main menu
      if (SceneManager.GetActiveScene().name != mainMenuScene)
        PhotonNetwork.LoadLevel(mainMenuScene);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
      /* hacky soultion to a problem i dont understand.
       * it looks like this callback is being called on a destryoed SceneOverlord from previous game in the same session
       */
      if (this == null)
        return;

      object props;

      /*       if (propertiesThatChanged.TryGetValue(Keys.DAYPHASE_START, out props))
            {
              // startTime = (double)props;
              float phaseDuration = (float)props;

            } */

    }

    [HideInInspector] public Player otherPlayer;

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {

      MyDebug.Log("OnPlayerPropertiesUpdate", targetPlayer.NickName);
      if (targetPlayer != PhotonNetwork.LocalPlayer)
        otherPlayer = targetPlayer;

      if (changedProps.ContainsKey(Keys.MAP_PROP_KEY))
      {
        playersLoadedScene++;

        if (playersLoadedScene == 2)
        {
          GetComponent<GameMechanic>().StartGameSequence();
        }
      }

      // skip intro when both players agree
      if (changedProps.ContainsKey(Keys.SKIP_INTRO))
      {
        if (otherPlayer != null)
          if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(Keys.SKIP_INTRO) && otherPlayer.CustomProperties.ContainsKey(Keys.SKIP_INTRO))
            GetComponent<GameMechanic>().Director_Stopped(null);
      }

    }

    #endregion

  }
}
