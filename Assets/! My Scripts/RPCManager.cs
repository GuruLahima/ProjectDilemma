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
    void RPC_ThrowProjectile(int viewId, int projectileIndex, Vector3 startPosition, Vector3 direction, float strength, float customGravity)
    {
      Projectile projectile = MasterData.Instance.allProjectiles[projectileIndex].Prefab;
      if (projectile)
      {
        var proj = Instantiate(projectile);
        proj.SetProjectile(viewId, startPosition, direction, strength, customGravity);
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
      LoadOutfit(viewID, clothesId);
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

    [PunRPC]
    void RPC_EvaluatePlayerIncomes(object points, object rank, object xp)
    {
      long _points = 0, _rank = 0, _xp = 0;
      if (points is long lPoints)
      {
        _points = lPoints;
      }
      if (rank is long lRank)
      {
        _rank = lRank;
      }
      if (xp is long lXp)
      {
        _xp = lXp;
      }
      GameMechanic.Instance.ActivateEndScreenCountersOtherPlayer(_points, _rank, _xp);
    }
    #endregion

    #region public static methods

    public static void LoadOutfit(int viewID, int[] clothesId)
    {
      PhotonView player = PhotonView.Find(viewID);
      PlayerSpot playerSpot = player.GetComponent<PlayerSpot>();
      playerSpot.clothesId = clothesId;
      playerSpot.outfitLoader.GenerateDefault();
    }

    #endregion

  }
}