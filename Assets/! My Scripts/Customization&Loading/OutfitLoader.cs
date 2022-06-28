using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;

namespace Workbench.ProjectDilemma
{
  /// <summary>
  /// Outfit loader is to be used outside lobby
  /// <para>Im using PlayerSpot to store the Clothes ids, this script is only used for initiating use OutfitGenerator</para>
  /// </summary>
  [RequireComponent(typeof(PlayerSpot))]
  public class OutfitLoader : MonoBehaviourPun
  {
    #region Exposed Private Fields
    [SerializeField] ClothingData clothingData;
    [SerializeField] public OutfitGenerator defaultGeneratedClothes;
    [SerializeField] public OutfitGenerator endScreenClothesGenerator;
    #endregion

    #region Private Fields
    private PlayerSpot playerSpot;
    #endregion

    #region MonoBehavior
    void Awake()
    {
      playerSpot = GetComponent<PlayerSpot>();
    }
    #endregion

    #region Public Methods
    public void Init()
    {
      // check if this is the local player spot
      if (PhotonNetwork.IsConnected) // this check ensures we load both outfits locally even when we are not connected to photon
        if (GameMechanic.Instance.localPlayerSpot != playerSpot) return;

      int[] rigIds = MasterData.Instance.GetTreeRigIds(clothingData.Clothes);
      if (PhotonNetwork.IsConnected)
      {
        RPCManager.Instance.photonView.RPC("RPC_LoadOutfit", RpcTarget.AllViaServer, photonView.ViewID, rigIds);
      }
      else
      {
        Debug.LogWarning("Trying to load outfit but photon is not connected");
        // circumventing photon because we are not connected
        RPCManager.LoadOutfit(photonView.ViewID, rigIds);
      }
    }

    public void GenerateDefault()
    {
      // this happens on each instance of OutfitLoader separately when called via the RPC method
      if (defaultGeneratedClothes) defaultGeneratedClothes.enabled = true;
      if (endScreenClothesGenerator) endScreenClothesGenerator.enabled = true;
    }
    #endregion
  }
}



