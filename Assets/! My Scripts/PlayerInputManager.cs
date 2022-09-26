using System;
using System.Collections;
using System.Collections.Generic;
using GuruLaghima;
using NaughtyAttributes;
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
      public string ActionName;
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
      public List<InputCondition> allowEvenWhenTheseConditionsAreTrue = new List<InputCondition>();
    }

    [NaughtyAttributes.InfoBox("Override Inputs ignores input conditions")]
    [SerializeField] private List<InputWrapper> OverrideInputs = new List<InputWrapper>();
    [SerializeField] private List<InputWrapper> Inputs = new List<InputWrapper>();
    public List<InputCondition> inputConditions;

    [HorizontalLine]
    [Header("Events")]
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
      Instance = null;

    }

    private void Update()
    {
      ProcessOverrideInputs();
      // if (InputEnabled) // ! I needed a finer approach to deciding if an input should be blocked or not
      ProcessInput();
    }

    public void ExecuteInputAction(string name)
    {
      InputWrapper input = Inputs.Find((input) =>
      {
        return input.ActionName == name;
      });
      if (input != null)
        input.InputAction?.Invoke();
    }

    public void ExecuteInputActionButAlsoProcessTheInput(string name)
    {
      InputWrapper input = Inputs.Find((input) =>
      {
        return input.ActionName == name;
      });
      if (input != null)
      {
        CircumventedProcessInput(input);
        // input.InputAction?.Invoke();
      }
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

    private void CircumventedProcessInput(InputWrapper _input)
    {
      switch (_input.InputMode)
      {
        case InputWrapper.Mode.OnPress:
          // if ((!IsHeld || _input.PersistentInput))
          {
            _input.InputAction?.Invoke();
          }
          break;
        case InputWrapper.Mode.Hold:
          if ((!IsHeld || _input.IsActivated == true || _input.PersistentInput))
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
          if ((!IsHeld || _input.PersistentInput))
          {
            _input.InputAction?.Invoke();
          }
          break;
        case InputWrapper.Mode.StateSwitch:
          // we initiate the state
          if ((!IsHeld || _input.IsActivated == true || _input.PersistentInput)) // special condition
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

    bool inputBlocked = false;
    private void ProcessInput()
    {
      foreach (InputWrapper _input in Inputs)
      {
        // check if this input is allowed even though the input is blocked
        if (!InputEnabled)
        {
          inputBlocked = true;

          foreach (InputCondition condition in inputConditions)
          {

            if (_input.allowEvenWhenTheseConditionsAreTrue.Contains(condition))
            {
              inputBlocked = false;
            }
            else
            {
              inputBlocked = true;
              break;
            }

          }

          // skip this input because it's blocked by conditions
          if (inputBlocked)
          {
            continue;
          }
        }

        // process the input
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
    [HideInInspector] public QuestActivator questActivator;
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

    public void AbilityActivate()
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

    public void RerollSnatchQuest()
    {
      questActivator.RerollSnatchQuest();
    }

    public void SetHeld(bool state)
    {
      IsHeld = state;
    }

    public void InChatVoice()
    {
      if (!inputConditions.Contains(InputCondition.USING_COMPUTER))
      {
        inputConditions.Add(InputCondition.USING_COMPUTER);
      }
    }

    public void ExitChatVoice()
    {
      if (inputConditions.Contains(InputCondition.USING_COMPUTER))
      {
        inputConditions.Remove(InputCondition.USING_COMPUTER);
      }
    }

    public void OnEnterRadialMenu()
    {
      if (!inputConditions.Contains(InputCondition.IN_RADIALMENU))
      {
        inputConditions.Add(InputCondition.IN_RADIALMENU);
      }
    }
    public void OnExitRadialMenu()
    {
      if (inputConditions.Contains(InputCondition.IN_RADIALMENU))
      {
        inputConditions.Remove(InputCondition.IN_RADIALMENU);
      }
    }

    public void OnStartedAiming()
    {
      if (!inputConditions.Contains(InputCondition.IS_AIMING))
      {
        inputConditions.Add(InputCondition.IS_AIMING);
      }
    }

    public void OnFinishedAiming()
    {
      if (inputConditions.Contains(InputCondition.IS_AIMING))
      {
        inputConditions.Remove(InputCondition.IS_AIMING);
      }
    }

    public void OnChoosingPerk()
    {
      if (!inputConditions.Contains(InputCondition.CHOOSING_PERK))
      {
        inputConditions.Add(InputCondition.CHOOSING_PERK);
      }
    }
    public void OnPerkChosen()
    {
      if (inputConditions.Contains(InputCondition.CHOOSING_PERK))
      {
        inputConditions.Remove(InputCondition.CHOOSING_PERK);
      }
    }

    public void ActivateQuestState(string stateName)
    {
      GameMechanic.Instance.localPlayerSpot.questActivator.ActivateQuestState(stateName);
    }

    public void SetCameraControl(bool value)
    {
      if (GameMechanic.Instance.localPlayerSpot.firstPersonPlayerCam)
      {
        GameMechanic.Instance.localPlayerSpot.firstPersonPlayerCam.GetComponent<SimpleCameraController>().playerHasControlOverCamera = value;
      }
    }

    public void CoinFlip()
    {
      if (GameMechanic.Instance)
      {
        GameMechanic.Instance.localPlayerSpot.extrasActivator.CoinFlip();
      }
    }

    public void DiceRoll()
    {
      if (GameMechanic.Instance)
      {
        GameMechanic.Instance.localPlayerSpot.extrasActivator.DiceRoll();
      }
    }

    #endregion

    #region Private Methods
    private void OnDiscussionEnded()
    {
      if (!inputConditions.Contains(InputCondition.DISCUSSION_ENDED))
      {
        inputConditions.Add(InputCondition.DISCUSSION_ENDED);
      }
    }
    #endregion
  }
}

/// <summary>
/// if any of these conditions exist in the conditions list the input is blocked
/// </summary>
public enum InputCondition
{
  DISCUSSION_ENDED,
  EMOTE_PLAYING,
  USING_COMPUTER,
  IN_RADIALMENU,
  IS_AIMING,
  CHOOSING_PERK
}
