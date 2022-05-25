using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New PerkData", menuName = "Workbench/ScriptableObjects/PerkData", order = 3)]
public class PerkData : ItemData
{
  #region Public Fields
  public SelectionMenuContainer Icon;
  [ShowAssetPreview]
  public Sprite ico;
  public List<PerkModifiersWrapper> AllPerkModifiers = new List<PerkModifiersWrapper>();
  public ObjectiveBase quest;
  #endregion

  #region Public Methods
  public PerkBonusModifiers[] GetPerkModifiers(ObjectiveCondition condition)
  {
    foreach (PerkModifiersWrapper pmw in AllPerkModifiers)
    {
      if (pmw.Condition == condition)
      {
        return pmw.Modifiers;
      }
    }
    return new PerkBonusModifiers[0];
  }
  #endregion

  #region Class Definition
  /// <summary>
  /// Class exclusive to PerkData for storing modifiers with their conditional objective
  /// </summary>
  [System.Serializable]
  public class PerkModifiersWrapper
  {
    public ObjectiveCondition Condition;
    public PerkBonusModifiers[] Modifiers;
  }
  /// <summary>
  /// Class used for assigning values to keys (PerkKeys)
  /// </summary>
  [System.Serializable]
  public class PerkBonusModifiers
  {
    public enum ModifierType : byte { Float, Bool }
    [PerkKey]
    public string ModifierKey;
    public ModifierType ModifierValueType;

    [AllowNesting]
    [ShowIf("ModifierValueType", ModifierType.Float)]
    public float ModifierValueFloat;

    [AllowNesting]
    [ShowIf("ModifierValueType", ModifierType.Bool)]
    public bool ModifierValueBool;

    /// <returns>Value for the modifier corresponding to the modifier type</returns>
    public object GetValue()
    {
      return ModifierValueType switch
      {
        ModifierType.Float => ModifierValueFloat,
        ModifierType.Bool => ModifierValueBool,
        _ => ModifierValueFloat,
      };
    }
  }
  #endregion
}
public enum ObjectiveCondition
{
  Complete,
  Failed,
  BetrayCooperate,
  CooperateBetray,
  CooperateCooperate,
  BetrayBetray,
  CompletedWon,
  CompletedLost,
  FailedWon,
  FailedLost
}