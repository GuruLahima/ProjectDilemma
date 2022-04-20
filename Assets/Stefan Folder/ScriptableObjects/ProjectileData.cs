using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "New ProjectileData", menuName = "Workbench/ScriptableObjects/ProjectileData", order = 1)]
public class ProjectileData : ScriptableObject
{
  public string Key;
  public int AmountOwned;
  public bool Ownable;
  public Projectile Prefab;
  public SelectionMenuContainer Icon;
  [ShowAssetPreview]
  public Sprite ico;
}
