#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSettings", menuName = "Workbench/ScriptableObjects/ItemSettings", order = 1)]
public class ItemSettings : SingletonScriptableObject<ItemSettings>
{
  
  public List<ProjectileData> throwables = new List<ProjectileData>();

  public void FetchDataFromServer()
  {
    FetchThrowablesData();
  }

  private void FetchThrowablesData()
  {

  }
}