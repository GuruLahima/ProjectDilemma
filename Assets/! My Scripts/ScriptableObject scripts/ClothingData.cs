using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New ClothingData", menuName = "Workbench/ScriptableObjects/ClothingData", order = 4)]
public class ClothingData : ScriptableObject
{
  

  private void OnEnable()
  {
    if (defaultClothing && !IsDefault)
    {
      SaveClothes(defaultClothing.Clothes);
    }
  }

  public bool IsPreviewClothing;
  public ClothingData currentClothing;

  [Foldout("Defaults")]
  public bool IsDefault;
  [Foldout("Defaults")]
  public ClothingData defaultClothing;

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
    _clothes = new List<ClothingTree>();
    foreach (ClothingTree ct in clothes)
    {
      ClothingTree tree = new ClothingTree(ct.Type, ct.Clothing);
      _clothes.Add(tree);
    }
  }
}
