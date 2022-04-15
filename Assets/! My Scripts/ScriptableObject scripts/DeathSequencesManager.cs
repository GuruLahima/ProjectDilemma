using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;
using UnityEngine;
using GuruLaghima;



namespace Workbench.ProjectDilemma

{
  [CreateAssetMenu(fileName = "DeathSequencesManager", menuName = "Workbench/ScriptableObjects/DeathSequencesManager", order = 1)]
  public class DeathSequencesManager : SingletonScriptableObject<DeathSequencesManager>
  {
    // added to the book of death
    public List<OwnableDeathSequence> universalDeathSequences = new List<OwnableDeathSequence>();

#if UNITY_EDITOR

    [MenuItem("CONTEXT/DeathSequencesManager/Reset (Custom)")]
    static void CustomReset(MenuCommand command)
    {
      DeathSequencesManager thisObj = (DeathSequencesManager)command.context;
      thisObj.Reset();
    }
#endif

  }

  [System.Serializable]
  public class OwnableDeathSequence
  {
    public bool owned = false;
    public DeathSequence deathSequence;
  }
}