using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New ProjectileData", menuName = "Workbench/ScriptableObjects/ProjectileData", order = 1)]
public class ProjectileData : ItemData
{
  [Header("Constant data - projectiles")]

  public Projectile Prefab;
  public SelectionMenuContainer Icon;
}
