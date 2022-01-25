#if UNITY_EDITOR
using UnityEditor;
using NaughtyAttributes;
#endif
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "VersionSettingsAsset", menuName = "Workbench/ScriptableObjects/MiscelaneousSettings", order = 1)]
public class MiscelaneousSettings : SingletonScriptableObject<MiscelaneousSettings>
{
#if UNITY_EDITOR
  [Scene]
#endif
  public string gameplayScene = "Map 01";
#if UNITY_EDITOR
  [Scene]
#endif
  public string mainMenuScene = "Main Menu";

  [Header("Chat settings")]
  [Tooltip("The time the chat is blocked after a massage is sent in lobby and in-game chat")]
  public float spamProtectInterval = 3f;

  [Header("Sound")]
  public AudioMixerGroup defaultSFXMixerGroup;
#if UNITY_EDITOR
  [MenuItem("CONTEXT/MiscelaneousSettings/Reset (Custom)")]
  static void CustomReset(MenuCommand command)
  {
    MiscelaneousSettings thisObj = (MiscelaneousSettings)command.context;
    thisObj.Reset();
  }
#endif
}