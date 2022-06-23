using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
  /// <summary>
  /// Singleton script for storing all requisites for RPC calls that require ScriptableObjects (data) to resolve
  /// </summary>
  [CreateAssetMenu(fileName = "MasterData", menuName = "Workbench/ScriptableObjects/MasterData", order = 10)]
  public class MasterData : SingletonScriptableObject<MasterData>
  {
    #region Public Fields
    // !: To return index simply call the corresponding function
    // !: To return the data simply use the index from the allData[index] of the corresponding type
    public List<ProjectileData> allProjectiles = new List<ProjectileData>();
    public List<EmoteData> allEmotes = new List<EmoteData>();
    public List<PerkData> allPerks = new List<PerkData>();
    public List<RigData> allRigs = new List<RigData>();
    public List<RewardData> allRewards = new List<RewardData>();

    #endregion

    #region Public Methods
    public int GetProjectileIndex(ProjectileData projData)
    {
      return allProjectiles.IndexOf(projData);
    }
    public int GetAnimationIndex(EmoteData emote)
    {
      return allEmotes.IndexOf(emote);
    }
    public int GetPerkIndex(PerkData perk)
    {
      return allPerks.IndexOf(perk);
    }
    public int GetRigIndex(RigData rig)
    {
      return allRigs.IndexOf(rig);
    }
    public int[] GetTreeRigIds(List<ClothingTree> clothing)
    {
      List<int> rigIds = new List<int>();
      foreach (ClothingTree tree in clothing)
      {
        if (tree.Clothing != null)
        {
          rigIds.Add(MasterData.Instance.GetRigIndex(tree.Clothing));
        }
      }
      return rigIds.ToArray();
    }
    #endregion
  }
}
