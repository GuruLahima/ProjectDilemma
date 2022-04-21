using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New ClothingData", menuName = "Workbench/ScriptableObjects/ClothingData", order = 4)]
public class ClothingData : ScriptableObject
{
  public bool IsDefault;
  public bool IsEquiped;

  /// <summary>
  /// To modify the clothes of this ClothingData, simply call the SaveClothes method
  /// </summary>
  public List<ClothingTree> Clothes
  {
    get
    {
      return _clothes;
    }
  }
  [SerializeField] private List<ClothingTree> _clothes = new List<ClothingTree>();
  public void SaveClothes(List<ClothingTree> clothes)
  {
    EditorUtility.SetDirty(this);
    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    _clothes = new List<ClothingTree>();
    foreach (ClothingTree ct in clothes)
    {
      ClothingTree tree = new ClothingTree();
      tree.Clothing = ct.Clothing;
      tree.Type = ct.Type;
      _clothes.Add(tree);
    }
    EditorUtility.SetDirty(this);
    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
  }
}
