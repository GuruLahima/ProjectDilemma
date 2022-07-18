using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;
using UnityEngine;
using GuruLaghima;
using System.Linq;

namespace Workbench.ProjectDilemma
{
  [CreateAssetMenu(fileName = "Scenario", menuName = "Workbench/ScriptableObjects/Scenario", order = 1)]
  public class Scenario : ScriptableObject
  {
    // added to the list of deaths a player can choose from when they win,
    // but also randomly selected for when a player loses from being AFK
    public List<OwnableDeathSequence> defaultDeathSequences = new List<OwnableDeathSequence>();
    // alwyays randomly chosen
    public List<OwnableDeathSequence> defaultBothLoseSequences = new List<OwnableDeathSequence>();
    // alwyays randomly chosen
    public List<OwnableDeathSequence> defaultBothCooperateSequences = new List<OwnableDeathSequence>();


    public ValidationSystem<RelicData> HighestRelicTierQualification(float value) => this.scenarioSpecificRelicTiers.Where((x) => x.MinimumBetMultiplier <= value).Max();

    [HorizontalLine]
    public List<ValidationSystem<RelicData>> scenarioSpecificRelicTiers = new List<ValidationSystem<RelicData>>();

    [HorizontalLine]
    public List<BonusModifiersWrapper> scenarioBonusModifiers = new List<BonusModifiersWrapper>();
  }
}