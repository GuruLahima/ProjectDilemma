using System;
using GuruLaghima;
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

  public static void ToggleLockMode()
  {
    CursorLockMode mode = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
    Cursor.lockState = mode;
    MyDebug.Log("LockState", mode.ToString());
  }
  public static void ToggleVisibility()
  {
    Cursor.visible = !Cursor.visible;
  }
  public static void SetLockMode(CursorLockMode mode)
  {
    MyDebug.Log("LockState", mode.ToString());
    Cursor.lockState = mode;
  }
  /// <summary>
  /// an overeload of SetLockMode but one that can be called via UnityEvent.
  /// 0 = CursorLockMode.None;
  /// 1 = CursorLockMode.Confined;
  /// 2 = CursorLockMode.Locked;
  /// </summary>
  /// <param name="lockMode"></param>
  public void SetLockMode(int lockMode)
  {
    CursorLockMode mode;
    switch (lockMode)
    {
      case 0:
        mode = CursorLockMode.None;
        break;
      case 1:
        mode = CursorLockMode.Confined;
        break;
      case 2:
        mode = CursorLockMode.Locked;
        break;
      default:
        mode = CursorLockMode.None;
        break;
    }
    Cursor.lockState = mode;
    MyDebug.Log("LockState", mode.ToString());
  }
  public static void SetVisibility(bool visible)
  {
    Cursor.visible = visible;
  }
}