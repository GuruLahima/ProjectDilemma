using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.GameFoundation;

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
      player.GetComponent<PlayerSpot>().emoteActivator.SYNC_Animation(emote);
    }

    [PunRPC]
    void RPC_PerkActivated(int viewID)
    {
      var player = PhotonView.Find(viewID);
      player.GetComponent<PlayerSpot>().perkActivator.SYNC_ActivatePerk();
    }
    [PunRPC]
    void RPC_AbilityActivated(int viewID)
    {
      var player = PhotonView.Find(viewID);
      player.GetComponent<PlayerSpot>().abilityActivator.SYNC_AbilityActivation();
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
    void RPC_SyncExtraTimeVote(int viewID, bool request)
    {
      PhotonView player = PhotonView.Find(viewID);
      PlayerSpot playerSpot = player.GetComponent<PlayerSpot>();
      if (playerSpot)
      {
        playerSpot.requestedExtraTime = request;
        if (!request)
        {
          GameMechanic.Instance.DeclineExtraTime();
        }
      }
      GameMechanic.Instance.CheckPlayersVotedExtraTime();
    }
    [PunRPC]
    void RPC_ForceExtraTimeVote(float time)
    {
      GameMechanic.Instance.AddExtraTimeDiscussion(time);
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
    [PunRPC]
    void RPC_AddTimeExtends(int amount)
    {
      GameMechanic.Instance.AddExtraTimeMaxVotes(amount);
    }

    [PunRPC]
    void RPC_RequestPlayerInfo()
    {
      if (PlayerData.Instance)
      {
        int[] outcomes = new int[PlayerData.Instance.latestGames.Count];
        int[] choices = new int[PlayerData.Instance.latestGames.Count];
        for (int i = 0; i < PlayerData.Instance.latestGames.Count; i++)
        {
          outcomes[i] = (int)PlayerData.Instance.latestGames[i].Outcome;
          choices[i] = (int)PlayerData.Instance.latestGames[i].Outcome;
        }
        string[] relics = new string[GameMechanic.Instance.localPlayerSpot.relicActivator.equipedRelics.Count];
        for (int i = 0; i < GameMechanic.Instance.localPlayerSpot.relicActivator.equipedRelics.Count; i++)
        {
          relics[i] = GameMechanic.Instance.localPlayerSpot.relicActivator.equipedRelics[i].inventoryitemDefinition.key;
        }
        string[] perks = new string[GameMechanic.Instance.localPlayerSpot.perkActivator.equipedPerks.Count];
        for (int i = 0; i < GameMechanic.Instance.localPlayerSpot.perkActivator.equipedPerks.Count; i++)
        {
          perks[i] = GameMechanic.Instance.localPlayerSpot.perkActivator.equipedPerks[i].inventoryitemDefinition.key;
        }

        string[] abilities = new string[0];
        if (GameMechanic.Instance.localPlayerSpot.abilityActivator.equipedAbility)
        {
          abilities = new string[1];
          abilities[0] = GameMechanic.Instance.localPlayerSpot.abilityActivator.equipedAbility.inventoryitemDefinition.key;
        }

        if (GameMechanic.Instance.otherPlayerSpot.GetComponent<PhotonView>())
        {
          Photon.Realtime.Player player = GameMechanic.Instance.otherPlayerSpot.GetComponent<PhotonView>().Owner;
          if (player != null)
          {
            this.photonView.RPC("RPC_LoadPlayerInfo", player, outcomes, choices, relics, perks, abilities);
          }
        }
      }
    }

    [PunRPC]
    void RPC_LoadPlayerInfo(int[] outcomes, int[] choices, string[] relics, string[] perks, string[] abilities)
    {
      for (int i = 0; i < outcomes.Length; i++)
      {
        GameMechanic.Instance.otherPlayerGames.Add(new GameInfo(outcomes[i], choices[i]));
      }
      for (int i = 0; i < relics.Length; i++)
      {
        InventoryItem inventoryItem = GameFoundationSdk.inventory.FindItem(relics[i]);
        if (inventoryItem != null)
        {
          ItemData item = inventoryItem.definition.GetStaticProperty("ingame_ScriptableObject").AsAsset<ItemData>();
          if (item is RelicData relic)
          {
            GameMechanic.Instance.otherPlayerRelics.Add(relic);
          }
        }
      }
      for (int i = 0; i < perks.Length; i++)
      {
        InventoryItem inventoryItem = GameFoundationSdk.inventory.FindItem(perks[i]);
        if (inventoryItem != null)
        {
          ItemData item = inventoryItem.definition.GetStaticProperty("ingame_ScriptableObject").AsAsset<ItemData>();
          if (item is PerkData perk)
          {
            GameMechanic.Instance.otherPlayerPerks.Add(perk);
          }
        }
      }
      for (int i = 0; i < abilities.Length; i++)
      {
        InventoryItem inventoryItem = GameFoundationSdk.inventory.FindItem(abilities[i]);
        if (inventoryItem != null)
        {
          ItemData item = inventoryItem.definition.GetStaticProperty("ingame_ScriptableObject").AsAsset<ItemData>();
          if (item is AbilityData ability)
          {
            GameMechanic.Instance.otherPlayerAbilities.Add(ability);
          }
        }
      }
      GameEvents.OnPlayerInfoLoaded?.Invoke();
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