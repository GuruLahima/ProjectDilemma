using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RigData", menuName = "Workbench/Rig/RigData", order = 1)]
public class RigData : ScriptableObject
{
  public string Key;
  public List<string> BonesData = new List<string>();
  public RiggableSkin SkinPrefab;
  public ClothingType type;
}
