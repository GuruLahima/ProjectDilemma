using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
  /// <summary>
  /// Singleton script for storing all requisites for RPC calls that require ScriptableObjects (data) to resolve
  /// </summary>
  public class MasterData : MonoBehaviour
  {
    #region Singleton
    public static MasterData Instance;
    private void Awake()
    {
      Instance = this;
    }
    #endregion

    // !: To return index simply call the corresponding function
    // !: To return the data simply use the index from the allData[index] of the corresponding type

    public List<ProjectileData> allProjectiles = new List<ProjectileData>();
    public int GetProjectileIndex(ProjectileData projData)
    {
      int index = allProjectiles.IndexOf(projData);
      return index;
    }

    public List<EmoteData> allEmotes = new List<EmoteData>();
    public int GetAnimationIndex(EmoteData emote)
    {
      int index = allEmotes.IndexOf(emote);
      return index;
    }

    public List<PerkData> allPerks = new List<PerkData>();
    public int GetPerkIndex(PerkData perk)
    {
      int index = allPerks.IndexOf(perk);
      return index;
    }

    public List<RigData> allRigs = new List<RigData>();
    public int GetRigIndex(RigData rig)
    {
      int index = allRigs.IndexOf(rig);
      return index;
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
  }
}
