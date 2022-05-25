using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This is NOT and should NOT be a singleton script
/// <para>This script should operate by itself!</para>
/// </summary>
public class HotkeyManager : KeybindModificationCallbacks
{
  // !important: if eventually in-game settings for changing hotkeys are added, we need a static action to which this script
  // can subscribe and be called whenever new hotkeys have been applied to change the texts according to the new hotkeys

  [System.Serializable]
  public class HotkeyGroup
  {
    [InputName]
    public string Hotkey;
    public TextMeshProUGUI HotkeyNameField;
  }
  [SerializeField] List<HotkeyGroup> Hotkeys = new List<HotkeyGroup>();
  private HotkeyGroup GetHotkeyByName(string buttonName)
  {
    foreach (HotkeyGroup hg in Hotkeys)
    {
      if (string.Equals(hg.Hotkey, buttonName))
      {
        return hg;
      }
    }
    return null;
  }

#if UNITY_EDITOR
  private void OnValidate()
  {
    foreach (HotkeyGroup hg in Hotkeys)
    {
      if (hg.HotkeyNameField)
      {
        hg.HotkeyNameField.text = InputManager.GetKeyNameByInput(hg.Hotkey);
      }
    }
  }
#endif

  private void Start()
  {
    foreach (HotkeyGroup hg in Hotkeys)
    {
      if (hg.HotkeyNameField)
      {
        hg.HotkeyNameField.text = InputManager.GetKeyNameByInput(hg.Hotkey);
      }
    }
  }
  public override void OnKeyChanged(string buttonName)
  {
    SetKeyText(buttonName);
  }
  public override void OnKeyAdded(string buttonName)
  {
    SetKeyText(buttonName);
  }
  public override void OnKeyRemoved(string buttonName)
  {
    SetKeyText(buttonName);
  }

  void SetKeyText(string buttonName)
  {
    var hg = GetHotkeyByName(buttonName);
    if (hg != null)
    {
      if (hg.HotkeyNameField)
      {
        hg.HotkeyNameField.text = InputManager.GetKeyNameByInput(hg.Hotkey);
      }
    }
  }
}
