using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New RigData", menuName = "Workbench/Rig/RigData", order = 1)]
public class RigData : ItemData
{
  public List<string> BonesData = new List<string>();
  public RiggableSkin SkinPrefab;
  public ClothingType type;
  public Sprite Icon;
  [TextArea]
  public string Description;
}
