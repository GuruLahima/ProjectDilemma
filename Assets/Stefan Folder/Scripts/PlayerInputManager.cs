using NaughtyAttributes;
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
      public enum Mode{OnPress, Hold, OnRelease}
      [InputAxis]
      [AllowNesting]
      public string InputHotkey;
      public Mode InputMode;
      public UnityEvent InputAction;
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
            if (Input.GetButtonDown(_input.InputHotkey))
            {
              _input.InputAction?.Invoke();
            }
            break;
          case InputWrapper.Mode.Hold:
            if (Input.GetButton(_input.InputHotkey))
            {
              _input.InputAction?.Invoke();
              isHeld = true;
            }
            break;
          case InputWrapper.Mode.OnRelease:
            if (Input.GetButtonUp(_input.InputHotkey))
            {
              _input.InputAction?.Invoke();
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
    [SerializeField]public ProjectileThrow projectileThrow;
    public void ProjectileAim()
    {
      projectileThrow.Aim();
    }

    public void ProjectileThrow()
    {
      projectileThrow.Throw();
    }

    public void EmoteChoose()
    {

    }

    public void EmoteActivate()
    {

    }

    public void MagnifyingGlass()
    {

    }

    public void PerkActivate()
    {

    }

    #endregion
  }
}
