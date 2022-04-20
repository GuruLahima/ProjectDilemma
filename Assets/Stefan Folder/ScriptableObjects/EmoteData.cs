using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "New EmoteData", menuName = "Workbench/ScriptableObjects/EmoteData", order = 2)]
public class EmoteData : ScriptableObject
{
  public string Key;
  public bool Ownable;
  public bool Owned;
  public string Parameter;
  public float ApproxDuration;
  public SelectionMenuContainer Icon;
  [ShowAssetPreview]
  public Sprite ico;
}
