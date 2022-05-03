using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Workbench.ProjectDilemma
{
  public class PlayerInputManager : MonoBehaviour
  {
    #region Singleton
    public static PlayerInputManager Instance;
    private void Awake()
    {
      Instance = this;
    }
    #endregion

    #region User Input
    [System.Serializable]
    public class InputWrapper
    {
      public enum Mode : byte { OnPress, Hold, OnRelease, StateSwitch }
      [InputName]
      public string InputHotkey;
      public Mode InputMode;
      public UnityEvent InputAction;
      public UnityEvent ReverseInputAction;
      [HideInInspector]
      public bool? IsActivated;
    }

    [SerializeField] private List<InputWrapper> Inputs = new List<InputWrapper>();
    public List<string> inputConditions;

    public bool InputEnabled
    {
      get
      {
        if (inputConditions.Count > 0)
          return false;
        else
          return true;
      }
    }

    private void OnEnable()
    {
      GameMechanic.DiscussionEnded += OnDiscussionEnded;
    }
    private void OnDisable()
    {
      GameMechanic.DiscussionEnded -= OnDiscussionEnded;
    }

    private void Update()
    {
      if (InputEnabled)
        ProcessInput();
    }

    private void ProcessInput()
    {
      bool isHeld = false;
      foreach (InputWrapper _input in Inputs)
      {
        switch (_input.InputMode)
        {
          case InputWrapper.Mode.OnPress:
            if (InputManager.GetButtonDown(_input.InputHotkey))
            {
              _input.InputAction?.Invoke();
            }
            break;
          case InputWrapper.Mode.Hold:
            if (InputManager.GetButton(_input.InputHotkey))
            {
              _input.InputAction?.Invoke();
              isHeld = true;
            }
            break;
          case InputWrapper.Mode.OnRelease:
            if (InputManager.GetButtonUp(_input.InputHotkey))
            {
              _input.InputAction?.Invoke();
            }
            break;
          case InputWrapper.Mode.StateSwitch:
            // we initiate the state
            if (InputManager.GetButtonDown(_input.InputHotkey))
            {
              // switch the state (true => false) / (false, null => true)
              if (_input.IsActivated == true)
              {
                _input.IsActivated = false;
              }
              else
              {
                _input.IsActivated = true;
              }
            }
            // init depending on the current state
            if (_input.IsActivated == true)
            {
              _input.InputAction?.Invoke();
            }
            // we only ever want this to execute once, then we set the state => null
            else if (_input.IsActivated == false)
            {
              _input.ReverseInputAction?.Invoke();
              _input.IsActivated = null;
            }
            break;
          default:
            break;
        }
        if (isHeld) break; // this will force disable actions while an ability is being held
      }
    }
    #endregion


    #region Input Actions
    [Space(20)]
    [HideInInspector] public ProjectileThrow projectileThrow;
    [Space(20)]
    [HideInInspector] public PlayerEmote playerEmote;
    [Space(20)]
    [HideInInspector] public OperatePerk operatePerk;
    [Space(20)]
    [HideInInspector] public MagnifyingGlass magnifyingGlass;
    public void ProjectileAim()
    {
      projectileThrow.Aim();
    }

    public void ProjectileThrow()
    {
      projectileThrow.Throw();
    }

    public void ProjectilePick()
    {
      projectileThrow.Pick();
    }

    public void ProjectileSet()
    {
      projectileThrow.Set();
    }

    public void EmoteChoose()
    {
      playerEmote.Pick();
    }

    public void EmoteActivate()
    {
      playerEmote.Emote();
    }

    public void MagnifyingGlass()
    {
      magnifyingGlass.Use();
    }

    public void PerkSelect()
    {
      operatePerk.SwitchState();
    }
    #endregion

    #region Private Methods
    private void OnDiscussionEnded()
    {
      if (!inputConditions.Contains(ICKeys.DISCUSSION_ENDED))
      {
        inputConditions.Add(ICKeys.DISCUSSION_ENDED);
      }
    }
    #endregion
  }
}

/// <summary>
/// ICKeys short for Input Condition Keys is a static class that contains bunch of predefined strings used as input conditions
/// </summary>
public static class ICKeys
{
  public const string DISCUSSION_ENDED = "discussionEnded";
  public const string EMOTE_PLAYING = "emotePlaying";
  public const string USING_COMPUTER = "usingComputer";
}
