using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GuruLaghima;
using NaughtyAttributes;

namespace Workbench.ProjectDilemma
{
  /// <summary>
  /// This script is to be attached to the object it is modifying on its ROOT bone of the skinned mesh renderer;
  /// </summary>
  public class OutfitGenerator : MonoBehaviour
  {
    #region Enum Declaration
    public enum PlayerNumeration : byte { Player1 = 1, Player2 = 2}
    #endregion

    #region Public Fields
 
    public PlayerNumeration player = PlayerNumeration.Player1;
    [CustomTooltip("Which protocol for loading should be used?")]
    public bool autoGenerateOnStart;
    public Transform RigRoot
    {
      get
      {
        if (_overrideRigRoot)
        {
          return _overrideRigRoot;
        }
        else
        {
          return transform;
        }
      }
    }
    #endregion

    #region Exposed Private Fields
    [CustomTooltip("By default uses the transform it is attached to")]
    [SerializeField] private Transform _overrideRigRoot;
    #endregion

    #region MonoBehavior Callbacks
    private void Start()
    {
      if (autoGenerateOnStart)
      {
        TryPopulateCharacters();
      }
    }
    #endregion

    #region Public Methods
    public void TryPopulateCharacters()
    {
      if (GameMechanic.Instance /*&& Photon.Pun.PhotonNetwork.IsConnected*/)
      {
        if (player == PlayerNumeration.Player1)
        {
          if (GameMechanic.Instance.playerOneSpot)
          {
            PopulateCharacters(GameMechanic.Instance.playerOneSpot.clothesId, RigRoot);
          }
        }
        else if (player == PlayerNumeration.Player2)
        {
          if (GameMechanic.Instance.playerTwoSpot)
          {
            PopulateCharacters(GameMechanic.Instance.playerTwoSpot.clothesId, RigRoot);
          }
        }
      }
    }
    //=========================OUTFIT-GENERATION=========================//
    public void PopulateCharacters(int[] rigIds, Transform rigRoot)
    {
      for (int i = 0; i < rigIds.Length; i++)
      {
        RigData rig = MasterData.Instance.allRigs[rigIds[i]];
        var newSkin = Instantiate(rig.SkinPrefab, rigRoot.parent);
        Rig.ReassignBones(rigRoot, rig, newSkin.skinMeshRenderer);
      }
    }
    public void PopulateCharacters(List<ClothingTree> clothes, Transform rigRoot)
    {
      for (int i = 0; i < clothes.Count; i++)
      {
        var newSkin = Instantiate(clothes[i].Clothing.SkinPrefab, rigRoot.parent);
        Rig.ReassignBones(rigRoot, clothes[i].Clothing, newSkin.skinMeshRenderer);
      }
    }
    //===================================================================//
    #endregion
  }
}