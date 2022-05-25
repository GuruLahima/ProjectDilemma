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
    void RPC_PerkActivated(int viewID)
    {
      var player = PhotonView.Find(viewID);
      player.GetComponent<PlayerSpot>().operatePerk.SYNC_ActivatePerk();
    }
    [PunRPC]
    void RPC_MagnifyingGlassActivated(int viewID)
    {
      var player = PhotonView.Find(viewID);
      player.GetComponent<PlayerSpot>().magnifyingGlass.SYNC_MagnifyingGlass();
    }

    [PunRPC]
    void RPC_LoadOutfit(int viewID, int[] clothesId)
    {
      PhotonView player = PhotonView.Find(viewID);
      PlayerSpot playerSpot = player.GetComponent<PlayerSpot>();
      playerSpot.clothesId = clothesId;
      playerSpot.outfitLoader.GenerateDefault();
    }

    [PunRPC]
    void RPC_SyncPlayerLoaded(int viewID)
    {
      PhotonView player = PhotonView.Find(viewID);
      PlayerSpot playerSpot = player.GetComponent<PlayerSpot>();
      if (playerSpot)
      {
        playerSpot.playerLoaded = true;
      }
      GameMechanic.Instance.CheckPlayersLoaded();
    }
    #endregion

  }
}