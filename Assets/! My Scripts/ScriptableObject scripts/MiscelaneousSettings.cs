#if UNITY_EDITOR
using UnityEditor;
using NaughtyAttributes;
#endif
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using GuruLaghima;
using System.Linq;

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
  [CustomTooltip("The time the chat is blocked after a massage is sent in lobby and in-game chat")]
  public float spamProtectInterval = 3f;
  [Header("Points distribution")]
  public int pointsForWin = 50;
  public int pointsForLoss = -50;
  public int pointsForBothWon = 10;
  public int pointsForBothLost = -50;
  [Header("XP distribution")]
  public int xpForWin;
  public int xpForLoss;
  public int xpForBothWon;
  public int xpForBothLost;
  [CustomTooltip("Enter values for how much each level needs XP to be reached")]
  public List<int> levelsDistribution = new List<int>();
  [Header("Rank distribution")]
  public int rankForWin;
  public int rankForLoss;
  public int rankForBothWin;
  public int rankForBothLost;

  public ValidationSystem<RewardSortingData> HighestRewardTierQualification(float value) => MiscelaneousSettings.Instance.rewardTiers.Where((x) => x.MinimumBetMultiplier <= value).Max();

  [Header("Reward Validation System")]
  public List<ValidationSystem<RewardSortingData>> rewardTiers = new List<ValidationSystem<RewardSortingData>>();

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
