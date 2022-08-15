using System.Collections.Generic;
using UnityEngine;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New ObjectiveData", menuName = "Workbench/ScriptableObjects/ObjectiveData/ObjectiveData", order = 3)]
public class ObjectiveData : ItemData
{
  #region Public Fields
  public SelectionMenuContainer Icon;
  public List<BonusModifiersConditionWrapper> AllBonusModifiers = new List<BonusModifiersConditionWrapper>();
  public ObjectiveBase objective;
  #endregion

  #region Public Methods
  public BonusModifiersWrapper[] GetBonusModifiers(ObjectiveCondition condition)
  {
    foreach (BonusModifiersConditionWrapper pmw in AllBonusModifiers)
    {
      if (pmw.Condition == condition)
      {
        return pmw.Modifiers;
      }
    }
    return new BonusModifiersWrapper[0];
  }
  #endregion
}
namespace Workbench.ProjectDilemma
{
  [System.Serializable]
  public class BonusModifiersConditionWrapper
  {
    public ObjectiveCondition Condition;
    public BonusModifiersWrapper[] Modifiers;
  }
  [System.Serializable]
  public class BonusModifiersWrapper
  {
    public BonusModifierData ModifierKey;
    public UpgradeData UpgradeableBy;
    public string Description;
    //---------------------------
    public float ModifierValueFloat;
    public bool ModifierValueBool;
    public string ModifierValueString;

    /// <returns>Value for the modifier corresponding to the modifier type</returns>
    public object GetValue()
    {
      return ModifierKey.Type switch
      {
        BonusModifierData.BonusModifierType.Float => ModifierValueFloat,
        BonusModifierData.BonusModifierType.Bool => ModifierValueBool,
        BonusModifierData.BonusModifierType.String => ModifierValueString,
        _ => ModifierValueFloat,
      };
    }
  }
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