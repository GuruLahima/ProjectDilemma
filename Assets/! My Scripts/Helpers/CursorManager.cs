using System;
using NaughtyAttributes;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

  [BoxGroup("OnStart")]
  [SerializeField] bool toggleCursorOnStart;
  [BoxGroup("OnStart")]
  [SerializeField] bool setCursorOnStart;
  [ShowIf("setCursorOnStart")]
  [BoxGroup("OnStart")]
  [SerializeField] bool visibilityOnStart;
  [ShowIf("setCursorOnStart")]
  [BoxGroup("OnStart")]
  [SerializeField] CursorLockMode lockModeOnStart;
  [BoxGroup("OnDisable")]
  [SerializeField] bool toggleCursorOnDisable;
  [BoxGroup("OnDisable")]
  [SerializeField] bool setCursorOnDisable;
  [BoxGroup("OnDisable")]
  [ShowIf("setCursorOnDisable")]
  [SerializeField] bool visibilityOnDisable;
  [BoxGroup("OnDisable")]
  [ShowIf("setCursorOnDisable")]
  [SerializeField] CursorLockMode lockModeOnDisable;

  /*private void Start()
  {
    if (toggleCursorOnStart)
    {
      ToggleLockMode();
      ToggleVisibility();
    }
    if (setCursorOnStart)
    {
      SetLockMode(lockModeOnStart);
      SetVisibility(visibilityOnStart);
    }
  } */
    private void OnEnable()
    {
        if (toggleCursorOnStart)
        {
            ToggleLockMode();
            ToggleVisibility();
        }
        if (setCursorOnStart)
        {
            SetLockMode(lockModeOnStart);
            SetVisibility(visibilityOnStart);
        }
    }
    private void OnDisable()
  {
    if (toggleCursorOnDisable)
    {
      ToggleLockMode();
      ToggleVisibility();
    }
    if (setCursorOnDisable)
    {
      SetLockMode(lockModeOnDisable);
      SetVisibility(visibilityOnDisable);
    }
  }

  public void ToggleLockMode()
  {
    Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
  }
  public void ToggleVisibility()
  {
    Cursor.visible = !Cursor.visible;
  }
  public void SetLockMode(CursorLockMode mode)
  {
    Cursor.lockState = mode;
  }
  public void SetVisibility(bool visible)
  {
    Cursor.visible = visible;
  }
}