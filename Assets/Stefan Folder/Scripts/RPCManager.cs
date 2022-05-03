using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Workbench.ProjectDilemma
{
  public class RPCManager : MonoBehaviourPunCallbacks
  {
    #region Singleton
    public static RPCManager Instance;
    private void Awake()
    {
      if (!Instance)
      {
        Instance = this;
      }
      else if (Instance != this)
      {
        Destroy(this);
      }
    }
    #endregion

    #region Pun RPCs
    [PunRPC]
    void RPC_ThrowProjectile(int projectileIndex, Vector3 startPosition, Vector3 direction, float strength, float customGravity)
    {
      Projectile projectile = MasterData.Instance.allProjectiles[projectileIndex].Prefab;
      if (projectile)
      {
        var proj = Instantiate(projectile);
        proj.SetProjectile(startPosition, direction, strength, customGravity);
      }
    }

    [PunRPC]
    void RPC_UserEmote(int viewID, int emoteIndex)
    {
      var player = PhotonView.Find(viewID);
      EmoteData emote = MasterData.Instance.allEmotes[emoteIndex];
      player.GetComponent<PlayerSpot>().playerEmote.SYNC_Animation(emote);
    }

    [PunRPC]
    void RPC_LoadOutfit(int viewID, int[] clothesId)
    {
      var pl = PhotonView.Find(viewID);
      var pSpot = pl.GetComponent<PlayerSpot>();
      if (pSpot)
      {
        pSpot.outfitLoader.LoadOutfit(clothesId, pSpot.character, pSpot.rigRoot);
      }
    }
    #endregion

  }
}