using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New EmoteData", menuName = "Workbench/ScriptableObjects/EmoteData", order = 2)]
public class EmoteData : ItemData
{
  [Header("Constant data - emotes")]
  public bool Ownable;
  public string Parameter;
  public float ApproxDuration;
  public List<FXWrapper> FXOnCast = new List<FXWrapper>();
  public List<FXWrapper> FXOnEnd = new List<FXWrapper>();
  public SelectionMenuContainer Icon;
}
