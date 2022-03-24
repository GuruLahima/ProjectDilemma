using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using NaughtyAttributes;
using UnityEngine;
using GuruLaghima;



namespace Workbench.ProjectDilemma

{
  [CreateAssetMenu(fileName = "InGameSettingSO", menuName = "Workbench/ScriptableObjects/InGameSettingSO", order = 1)]
  public class InGameSettingSO : ScriptableObject
  {
    public string settingKey;
  }
}