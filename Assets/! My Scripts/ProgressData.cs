using GuruLaghima;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
  [CreateAssetMenu(fileName = "ProgressData", menuName = "Workbench/ScriptableObjects/ProgressData", order = 11)]
  public class ProgressData : SingletonScriptableObject<ProgressData>
  {
    private void OnEnable()
    {
      //initialization
      foreach (LevelRewards reward in RewardsPerLevel)
      {
        reward.IsClaimed = false;
      }
    }

    [NaughtyAttributes.HorizontalLine]
    public float[] ExperienceRequiredPerLevel;
    public List<LevelRewards> RewardsPerLevel = new List<LevelRewards>();

    [NaughtyAttributes.HorizontalLine]
    [Header("Experience Distribution")]
    public float xpBase;
    public float xpPercentBonusWin;
    public float xpPercentBonusLose;
    public float xpPercentBonusBothWin;
    public float xpPercentBonusBothLose;

    [NaughtyAttributes.HorizontalLine]
    [Header("Generating Rewards")]
    [SerializeField] private List<RewardData> possibleRewards = new List<RewardData>();
    [SerializeField] private List<RewardSortingData> possibleSortingRewards = new List<RewardSortingData>();
    [CustomTooltip("Minimum number of rewards for each level guaranteed")]
    [SerializeField] private int minPossibleRewards = 0;
    [CustomTooltip("How many times we try to randomize reward")]
    [SerializeField] private int maxPossibleRewards = 3;
    [CustomTooltip("Chance for a reward to be randomized")]
    [Range(0, 100)]
    [SerializeField] private int chanceForRewardPerLevel = 15;
    [CustomTooltip("Chance For reward vs sortingReward ratio")]
    [Range(0, 100)]
    [SerializeField] private int rewardSortingRewardRatio = 70;
    [CustomTooltip("Should chance scale when it doesnt occur")]
    [SerializeField] private bool pseudoDistribution = true;
    [CustomTooltip("Should a single reward repeat on the same level?")]
    [SerializeField] private bool uniqueRewards = false;
    [HideInInspector]
    [SerializeField] private string formula = "";
    [HideInInspector]
    [SerializeField] private int maxLevel = 0;

    public int GetLevel(int experience, out int experienceLeft)
    {
      int level = 0;

      int totalRequiredXp = 0; int lastLevelExperience = 0;
      for (int i = 0; i < ExperienceRequiredPerLevel.Length; i++)
      {
        totalRequiredXp += (int)ExperienceRequiredPerLevel[i];
        if (experience >= totalRequiredXp)
        {
          lastLevelExperience = totalRequiredXp;
          continue;
        }
        else
        {
          level = i;
          break;
        }
      }
      experienceLeft = experience - lastLevelExperience;
      return level;
    }

    public void GenerateLevels(int maxLevel, string formula)
    {
      ExperienceRequiredPerLevel = new float[maxLevel];
      for (int i = 0; i < maxLevel; i++)
      {
        if (!string.IsNullOrEmpty(formula))
        {
          string temp = formula.Replace("i", i.ToString());
          StringToFormula stf = new StringToFormula();
          double result = stf.Eval(temp);
          ExperienceRequiredPerLevel[i] = (float)result;
        }
      }

      if (RewardsPerLevel.Count > maxLevel)
      {
        RewardsPerLevel.RemoveRange(maxLevel, RewardsPerLevel.Count - maxLevel);
      }
      else
      {
        while (RewardsPerLevel.Count < maxLevel)
        {
          RewardsPerLevel.Add(new LevelRewards());
        }
      }
    }

    public void GenerateRewards()
    {
      // we cycle through all the levels and generate rewards
      for (int level = 0; level < RewardsPerLevel.Count; level++)
      {
        RewardsPerLevel[level].PredefinedRewards.Clear();
        RewardsPerLevel[level].RandomizedRewards.Clear();
        List<RewardData> rewardsLeft = new List<RewardData>(possibleRewards);
        List<RewardSortingData> sortingRewardsLeft = new List<RewardSortingData>(possibleSortingRewards);
        int generatedRewards = 0;
        int pseudoCounter = 0;
        while (generatedRewards < maxPossibleRewards)
        {
          RewardData _rewardData = null;
          RewardSortingData _rewardSortingData = null;
          float ratioChance = Random.Range(0, 100);
          if (generatedRewards < minPossibleRewards)
          {
            if (ratioChance < rewardSortingRewardRatio)
            {
              _rewardData = GetRandomReward(rewardsLeft);
            }
            else
            {
              _rewardSortingData = GetRandomSortingReward(sortingRewardsLeft);
            }
          }
          else
          {
            float chance = chanceForRewardPerLevel;
            if (pseudoDistribution)
            {
              chance = (6 / (100 / chanceForRewardPerLevel) - pseudoCounter) * 100;
            }
            if (Random.Range(0, 100) < chance)
            {
              if (ratioChance < rewardSortingRewardRatio)
              {
                _rewardData = GetRandomReward(rewardsLeft);
              }
              else
              {
                _rewardSortingData = GetRandomSortingReward(sortingRewardsLeft);
              }
            }
          }

          if (_rewardData != null)
          {
            if (uniqueRewards)
            {
              rewardsLeft.Remove(_rewardData);
            }
            RewardsPerLevel[level].PredefinedRewards.Add(_rewardData);
          }
          else if (_rewardSortingData != null)
          {
            if (uniqueRewards)
            {
              sortingRewardsLeft.Remove(_rewardSortingData);
            }
            RewardsPerLevel[level].RandomizedRewards.Add(_rewardSortingData);
          }
          // we increase the pseudo counter regardless if we using pseudo or not
          pseudoCounter++;
          //regardless if the rewad is null or not, we add 1
          generatedRewards++;
        }

      }
    }

    private RewardSortingData GetRandomSortingReward(List<RewardSortingData> sortingRewards)
    {
      return sortingRewards[Random.Range(0, sortingRewards.Count)];
    }
    private RewardData GetRandomReward(List<RewardData> rewards)
    {
      return rewards[Random.Range(0, rewards.Count)];
    }
  }

  [System.Serializable]
  public class LevelRewards
  {
    //
    public List<RewardData> PredefinedRewards = new List<RewardData>();
    public List<RewardSortingData> RandomizedRewards = new List<RewardSortingData>();
    // use this temporary and reset it every play
    public bool IsClaimed = false;
  }



#if UNITY_EDITOR
  [CustomEditor(typeof(ProgressData))]
  public class ProgressDataEditor : Editor
  {
    private bool initDone = false;
    private GUIStyle displayStyle;
    ProgressData _target;
    SerializedProperty formula;
    SerializedProperty maxLevel;
    private void InitStyles()
    {
      initDone = true;
      displayStyle = new GUIStyle(GUI.skin.textField)
      {
        alignment = TextAnchor.MiddleLeft,
        margin = new RectOffset(),
        padding = new RectOffset(),
        fontSize = 12,
        fontStyle = FontStyle.Normal
      };
    }
    private void OnEnable()
    {
      _target = (ProgressData)this.target;
      formula = this.serializedObject.FindProperty("formula");
      maxLevel = this.serializedObject.FindProperty("maxLevel");
    }
    public override void OnInspectorGUI()
    {
      if (!initDone)
        InitStyles();

      this.serializedObject.Update();

      EditorGUILayout.LabelField("Use 'i' to display the level in the formula");
      formula.stringValue = EditorGUILayout.TextField("Formula", formula.stringValue, displayStyle);
      maxLevel.intValue = EditorGUILayout.IntField("Max Level", maxLevel.intValue, displayStyle);
      if (GUILayout.Button("Generate Levels", EditorStyles.radioButton))
      {
        if (_target)
        {
          _target.GenerateLevels(maxLevel.intValue, formula.stringValue);
        }
      }
      if (GUILayout.Button("Generate Rewards", EditorStyles.radioButton))
      {
        if (_target)
        {
          _target.GenerateRewards();
        }
      }

      serializedObject.ApplyModifiedProperties();

      DrawDefaultInspector();
    }
  }
#endif
}