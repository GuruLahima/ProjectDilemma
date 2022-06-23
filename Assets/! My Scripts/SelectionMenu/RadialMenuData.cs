using System.Collections.Generic;
using GuruLaghima;
using NaughtyAttributes;
using UnityEngine;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New RadialMenuData", menuName = "Workbench/ScriptableObjects/RadialMenuData", order = 3)]
public class RadialMenuData : ScriptableObject
{

#if UNITY_EDITOR
  [SerializeField] bool restoreValuesOnAwake = true;
  private void OnEnable()
  {
    if (restoreValuesOnAwake)
    {
      for (int i = 0; i < orderedItems.Count; i++)
      {
        orderedItems[i] = null;
      }
    }
  }
#endif

  #region Public Fields
  public List<ItemData> orderedItems = new List<ItemData>(5);
  #endregion

  #region Public Methods

  #endregion

}
