using UnityEngine;
using NaughtyAttributes;
using UnityEngine.GameFoundation;
using Workbench.ProjectDilemma;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item Set", menuName = "Workbench/ScriptableObjects/ItemSet", order = 1)]
public class ItemSet : ItemData
{
  [HorizontalLine]
  [Foldout("Outfit Info")]
  public string outfitTitle;
  [Foldout("Outfit Info")]
  public Sprite tierIcon;
  [Foldout("Outfit Info")]
  public RigData head;
  [Foldout("Outfit Info")]
  public RigData chest;
  [Foldout("Outfit Info")]
  public RigData legs;
  [Foldout("Outfit Info")]
  public RigData hands;
  [Foldout("Outfit Info")]
  public RigData feet;

  public List<RigData> GetItems()
  {
    List<RigData> tempCol = new List<RigData>();
    tempCol.Add(this.head);
    tempCol.Add(this.chest);
    tempCol.Add(this.legs);
    tempCol.Add(this.hands);
    tempCol.Add(this.feet);
    return tempCol;
  }

  [Foldout("Outfit Info")]
  public ItemSet NextTier;


}
