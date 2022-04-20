using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New PerkData", menuName = "Workbench/ScriptableObjects/PerkData", order = 3)]
public class PerkData : ScriptableObject
{
  public string Key;
  public int AmountOwned;
  public bool Ownable;
  public SelectionMenuContainer Icon;
  [ShowAssetPreview]
  public Sprite ico;
}
