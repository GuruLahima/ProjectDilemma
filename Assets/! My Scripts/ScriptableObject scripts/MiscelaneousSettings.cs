#if UNITY_EDITOR
using UnityEditor;
using NaughtyAttributes;
#endif
using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using GuruLaghima;
using System.Linq;
using System;

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

  [Header("Upgrader settings")]
  public Sprite upgrader_mistery_mask_image;
  public Sprite upgrader_mistery_chest_image;
  public Sprite upgrader_mistery_pants_image;
  public Sprite upgrader_mistery_shoes_image;
  public Sprite upgrader_mistery_gloves_image;
  public Sprite upgrader_mistery_random_item_image;

  [Header("Item Rarity colors")]
  public MyGenericDictionary<ItemRarity, Color> dictionaryOfColorsByRarity = new MyGenericDictionary<ItemRarity, Color>();

  public Color GetRarityColor(ItemRarity rarity)
  {

    return dictionaryOfColorsByRarity.GetItems()[rarity];
  }

#if UNITY_EDITOR
  [MenuItem("CONTEXT/MiscelaneousSettings/Reset (Custom)")]
  static void CustomReset(MenuCommand command)
  {
    MiscelaneousSettings thisObj = (MiscelaneousSettings)command.context;
    thisObj.Reset();
  }

#endif
}
