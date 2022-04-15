using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
  /// <summary>
  /// Temporary script for keeping all the projectiels
  /// </summary>
  public class ProjectileManager : MonoBehaviour
  {
    #region Singleton
    public static ProjectileManager Instance;
    private void Awake()
    {
      Instance = this;
    }
    #endregion

    public List<ProjectileData> allProjectiles = new List<ProjectileData>();
    public int GetProjectileIndex(ProjectileData projData)
    {
      int index = allProjectiles.IndexOf(projData);
      return index;
    }
  }
}
