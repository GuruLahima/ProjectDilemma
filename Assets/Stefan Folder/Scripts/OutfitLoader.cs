using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace Workbench.ProjectDilemma
{
  public class OutfitLoader : MonoBehaviourPun
  {

    public ClothingData clothingData;

    PlayerSpot playerSpot;

    void Awake()
    {
      playerSpot = GetComponent<PlayerSpot>();

    }


    public void Init()
    {

      if (GameMechanic.Instance.localPlayerSpot != playerSpot) return;


      List<int> rigIds = new List<int>();
      foreach (ClothingTree tree in clothingData.Clothes)
      {
        if (tree.Clothing != null)
        {
          rigIds.Add(MasterData.Instance.GetRigIndex(tree.Clothing));
        }
      }
      if (PhotonNetwork.IsConnected)
      {
        RPCManager.Instance.photonView.RPC("RPC_LoadOutfit", RpcTarget.AllViaServer, photonView.ViewID, rigIds.ToArray());
      }
      else
      {
        LoadOutfit(rigIds.ToArray(), playerSpot.character, playerSpot.rigRoot);
      }
    }

    public void LoadOutfit(int[] rigIds, Transform character, Transform rigRoot)
    {
      for (int i = 0; i < rigIds.Length; i++)
      {
        RigData rig = MasterData.Instance.allRigs[rigIds[i]];
        var newSkin = Instantiate(rig.SkinPrefab, character);
        RigManipulator.ReassignBones(rigRoot, rig, newSkin.skinMeshRenderer);
      }
    }

  }
}