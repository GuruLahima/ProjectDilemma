using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New RadialMenuData", menuName = "Workbench/ScriptableObjects/RadialMenuData", order = 3)]
public class RadialMenuData : ScriptableObject
{
  #region Public Fields
  public List<ItemData> orderedItems = new List<ItemData>();
  #endregion

  #region Public Methods

  #endregion

}
