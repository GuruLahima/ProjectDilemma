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
      public bool PersistentInput;
      public bool TriggersHeld;
      [HideInInspector]
      public bool? IsActivated;
    }

    [NaughtyAttributes.InfoBox("Override Inputs ignores input conditions")]
    [SerializeField] private List<InputWrapper> OverrideInputs = new List<InputWrapper>();
    [SerializeField] private List<InputWrapper> Inputs = new List<InputWrapper>();
    public List<string> inputConditions;

    public UnityEvent OnButtonHeldOrActive;
    public UnityEvent OnButtonReleasedOrInactive;
    bool IsHeld
    {
      get
      {

        return _isHeld;
      }
      set
      {
        _isHeld = value;
        if (_isHeld)
        {
          OnButtonHeldOrActive?.Invoke();
        }
        else
        {
          OnButtonReleasedOrInactive?.Invoke();
        }
      }
    }
    private bool _isHeld = false;

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
      ProcessOverrideInputs();
      if (InputEnabled)
        ProcessInput();
    }

    private void ProcessOverrideInputs()
    {
      foreach (InputWrapper _input in OverrideInputs)
      {
        switch (_input.InputMode)
        {
          case InputWrapper.Mode.OnPress:
            if (InputManager.GetButtonDown(_input.InputHotkey) && (!IsHeld || _input.PersistentInput))
            {
              _input.InputAction?.Invoke();
            }
            break;
          case InputWrapper.Mode.Hold:
            if (InputManager.GetButton(_input.InputHotkey) && (!IsHeld || _input.IsActivated == true || _input.PersistentInput))
            {
              _input.InputAction?.Invoke();
              _input.IsActivated = true;
              if (_input.TriggersHeld)
              {
                IsHeld = true;
              }
            }
            else if (_input.IsActivated == true)
            {
              _input.IsActivated = false;
              if (_input.TriggersHeld)
              {
                IsHeld = false;
              }
            }
            break;
          case InputWrapper.Mode.OnRelease:
            if (InputManager.GetButtonUp(_input.InputHotkey) && (!IsHeld || _input.PersistentInput))
            {
              _input.InputAction?.Invoke();
            }
            break;
          case InputWrapper.Mode.StateSwitch:
            // we initiate the state
            if (InputManager.GetButtonDown(_input.InputHotkey) && (!IsHeld || _input.IsActivated == true || _input.PersistentInput)) // special condition
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
              if (_input.TriggersHeld)
              {
                IsHeld = true;
              }
            }
            // we only ever want this to execute once, then we set the state => null
            else if (_input.IsActivated == false)
            {
              _input.ReverseInputAction?.Invoke();
              _input.IsActivated = null;
              if (_input.TriggersHeld)
              {
                IsHeld = false;
              }
            }
            break;
          default:
            break;
        }
      }
    }

    private void ProcessInput()
    {
      foreach (InputWrapper _input in Inputs)
      {
        switch (_input.InputMode)
        {
          case InputWrapper.Mode.OnPress:
            if (InputManager.GetButtonDown(_input.InputHotkey) && (!IsHeld || _input.PersistentInput))
            {
              _input.InputAction?.Invoke();
            }
            break;
          case InputWrapper.Mode.Hold:
            if (InputManager.GetButton(_input.InputHotkey) && (!IsHeld || _input.IsActivated == true || _input.PersistentInput))
            {
              _input.InputAction?.Invoke();
              _input.IsActivated = true;
              if (_input.TriggersHeld)
              {
                IsHeld = true;
              }
            }
            else if (_input.IsActivated == true)
            {
              _input.IsActivated = false;
              if (_input.TriggersHeld)
              {
                IsHeld = false;
              }
            }
            break;
          case InputWrapper.Mode.OnRelease:
            if (InputManager.GetButtonUp(_input.InputHotkey) && (!IsHeld || _input.PersistentInput))
            {
              _input.InputAction?.Invoke();
            }
            break;
          case InputWrapper.Mode.StateSwitch:
            // we initiate the state
            if (InputManager.GetButtonDown(_input.InputHotkey) && (!IsHeld || _input.IsActivated == true || _input.PersistentInput)) // special condition
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
              if (_input.TriggersHeld)
              {
                IsHeld = true;
              }
            }
            // we only ever want this to execute once, then we set the state => null
            else if (_input.IsActivated == false)
            {
              _input.ReverseInputAction?.Invoke();
              _input.IsActivated = null;
              if (_input.TriggersHeld)
              {
                IsHeld = false;
              }
            }
            break;
          default:
            break;
        }
      }
    }
    #endregion

    #region Input Actions
    [HideInInspector] public ThrowablesActivator throwablesActivator;
    [HideInInspector] public EmoteActivator emoteActivator;
    [HideInInspector] public PerkActivator perkActivator;
    [HideInInspector] public AbilityActivator abilityActivator;
    public void ProjectileAim()
    {
      throwablesActivator.Aim();
    }

    public void ProjectileThrow()
    {
      throwablesActivator.Throw();
    }

    public void ProjectilePick()
    {
      throwablesActivator.Pick();
    }

    public void ProjectileSet()
    {
      throwablesActivator.Set();
    }

    public void EmoteChoose()
    {
      emoteActivator.Pick();
    }

    public void EmoteActivate()
    {
      emoteActivator.Emote();
    }

    public void MagnifyingGlass()
    {
      abilityActivator.Use();
    }

    public void PerkShow()
    {
      perkActivator.ShowPerk();
    }
    
    public void PerkHide()
    {
      perkActivator.HidePerk();
    }

    // we are calling this via UI button
    public void PerkActivate()
    {
      perkActivator.ActivatePerk();
    }

    public void InChatVoice()
    {
      if (!inputConditions.Contains(ICKeys.USING_COMPUTER))
      {
        inputConditions.Add(ICKeys.USING_COMPUTER);
      }
    }

    public void ExitChatVoice()
    {
      if (inputConditions.Contains(ICKeys.USING_COMPUTER))
      {
        inputConditions.Remove(ICKeys.USING_COMPUTER);
      }
    }

    public void SetCameraControl(bool value)
    {
      if (GameMechanic.Instance.localPlayerSpot.mainCam)
      {
        GameMechanic.Instance.localPlayerSpot.mainCam.playerHasControlOverCamera = value;
      }
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
