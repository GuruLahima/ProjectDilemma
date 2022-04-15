using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;
using UnityEngine;
using GuruLaghima;



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

  }
}