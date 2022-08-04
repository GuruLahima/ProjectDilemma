using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New RigData", menuName = "Workbench/ScriptableObjects/RigData")]
public class RigData : ItemData
{
  [HorizontalLine]
  [Header("Clothing Info")]
  public List<string> BonesData = new List<string>();
  public RiggableSkin SkinPrefab;
  public ClothingType type;
  [TextArea]
  public string Description;
  [HorizontalLine]
  public bool partOfOutfit = false;
  [ShowIf("partOfOutfit")]
  public ItemSet outfit;
}
