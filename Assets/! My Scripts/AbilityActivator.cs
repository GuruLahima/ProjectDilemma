using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

namespace Workbench.ProjectDilemma
{
  public class AbilityActivator : BaseActivatorComponent
  {
    #region Exposed Private Fields
    [SerializeField] List<TextMeshProUGUI> recentGames = new List<TextMeshProUGUI>(); //temp
    [SerializeField] float displayDuration;
    [Tooltip("The name of the player is added on the end of this message")]
    [SerializeField] string noInfoMessagePrefix;
    [Tooltip("The name of the player is added on the start of this message")]
    [SerializeField] string noInfoMessageSuffix;
    #endregion

    #region Public Methods
    /// <summary>
    /// Since this will require things such as getting player's last x games CHOICE and the OUTCOME of the game
    /// I will simply leave an empty panel to show for x seconds and we will re-do this script when everything else is done
    /// </summary>
    public void Use()
    {
      if (OnCooldown) return;
      AddCooldown();

      ActiveState = true;
      string PLAYERNAME;
      if (PhotonNetwork.IsConnected)
      {
        PLAYERNAME = GameMechanic.Instance.otherPlayerSpot.GetComponent<PhotonView>().Owner.NickName;
      }
      else
      {
        PLAYERNAME = GameMechanic.Instance.otherPlayerSpot.name;
      }
      for (int i = 0; i < recentGames.Count; i++)
      {
        recentGames[i].text = $"{noInfoMessagePrefix} {noInfoMessageSuffix}";
      }
      if (PhotonNetwork.IsConnected)
      {
        int viewID = GameMechanic.Instance.localPlayerSpot.GetComponent<PhotonView>().ViewID;
        RPCManager.Instance.photonView.RPC("RPC_MagnifyingGlassActivated", RpcTarget.AllViaServer, viewID);
      }
      else
      {
        SYNC_MagnifyingGlass();
      }
      StartCoroutine(Display());
    }
    #endregion

    #region Private Methods
    IEnumerator Display()
    {
      yield return new WaitForSeconds(displayDuration);
      ActiveState = false;
    }
    #endregion

    #region Called by RPC
    /// <summary>
    /// This method is invoked through an RPC call
    /// <para>Here we can initialize stuff that we want both players to see ex: animations</para>
    /// </summary>
    public void SYNC_MagnifyingGlass()
    {
      Debug.Log("RPC TEST MAGNIFYING GLASS");
    }
    #endregion
  }
}