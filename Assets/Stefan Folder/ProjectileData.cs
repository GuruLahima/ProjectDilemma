using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Workbench.ProjectDilemma;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New ProjectileData", menuName = "Workbench/ScriptableObjects/ProjectileData", order = 1)]
public class ProjectileData : ScriptableObject
{
  public string Key;
  public int AmountOwned;
  public bool Ownable;
  public Projectile Prefab;

#if UNITY_EDITOR
  [Button("Generate Key")] [System.Obsolete]
  void GenerateKey()
  {
    Key = this.name + "_projectile";
  }
#endif
}
