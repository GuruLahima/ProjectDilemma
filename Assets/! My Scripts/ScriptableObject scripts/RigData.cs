using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New RigData", menuName = "Workbench/ScriptableObjects/RigData")]
public class RigData : ItemData
{
  public List<string> BonesData = new List<string>();
  public RiggableSkin SkinPrefab;
  public ClothingType type;
  [TextArea]
  public string Description;
}
