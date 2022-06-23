#if UNITY_EDITOR
using NaughtyAttributes;
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "InGameSettings", menuName = "Workbench/ScriptableObjects/InGameSettings", order = 1)]
public class InGameSettings : SingletonScriptableObject<InGameSettings>
{
  public int screenResolutionOption;
  [Header("Audio settings")]
  public float masterVolume;
  public float masterVolume_min;
  public float masterVolume_max;
  [Header("")]

  public float musicVolume;
  public float musicVolume_min;
  public float musicVolume_max;
  [Header("")]

  public float soundFXVolume;
  public float soundFXVolume_min;
  public float soundFXVolume_max;
  [Header("")]
  public float ambianceVolume;
  public float ambianceVolume_min;
  public float ambianceVolume_max;
  [Header("")]
#if UNITY_EDITOR
  [Dropdown("GetVoiceChatMode")]
#endif
  public int voiceChatSetting;
  public float voiceVolume;
  public float voiceVolume_min;
  public float voiceVolume_max;
#if UNITY_EDITOR
  private DropdownList<int> GetVoiceChatMode()
  {
    return new DropdownList<int>()
    {
      { "Always On",  0 },
      { "Push-to-talk",   1 },
      { "Off",     2 }
    };
  }
#endif
  [Header("")]
  public float mouseSensitivity;
  public float mouseSensitivity_min;
  public float mouseSensitivity_max;
  [Header("")]
  public int qualityLevelOption;
  public bool isFullScreenOn;
  public bool isVSyncOn;

  public bool crosshairOn;
  public bool invertAxis;
  [Header("")]
  public float brightness;
  public float brightness_min;
  public float brightness_max;

  [Header("Accessiblity options")]
  public bool stickyKeys = true;

#if UNITY_EDITOR

  [MenuItem("CONTEXT/InGameSettings/Reset (Custom)")]
  static void CustomReset(MenuCommand command)
  {
    InGameSettings thisObj = (InGameSettings)command.context;
    thisObj.Reset();
  }
#endif
}