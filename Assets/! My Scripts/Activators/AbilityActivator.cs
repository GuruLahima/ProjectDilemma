using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Linq;

namespace Workbench.ProjectDilemma
{
  public class AbilityActivator : BaseActivatorComponent
  {
    #region Exposed Private Fields
    [SerializeField] private UnityEngine.UI.Image abilitiesIcon;
    [SerializeField] private TooltipTrigger tooltip;
    [SerializeField] private Color activatedAbilitiesColor;
    #endregion

    #region Private Fields
    [HideInInspector]
    public AbilityData equipedAbility;

    private AbilityBase _equipedAbilityBase;

    private bool abiliyUsed = false;
    #endregion

    #region Public Methods
    public override void Init()
    {
      equipedAbility = InventoryData.Instance.abilities.Find((obj) => { return obj.Owned && obj.Equipped; });
      if (equipedAbility)
      {
        if (abilitiesIcon)
        {
          abilitiesIcon.sprite = equipedAbility.ico;
        }
        if (tooltip)
        {
          tooltip.title = equipedAbility.inventoryitemDefinition.displayName;        
          tooltip.description = (string)equipedAbility.inventoryitemDefinition.GetStaticProperty("description");
        }
      }
    }
    /// <summary>
    /// Since this will require things such as getting player's last x games CHOICE and the OUTCOME of the game
    /// I will simply leave an empty panel to show for x seconds and we will re-do this script when everything else is done
    /// </summary>
    public void Use()
    {
      if (OnCooldown) return;

      if (abiliyUsed) return;

      if (equipedAbility)
      {
        if (abilitiesIcon)
          abilitiesIcon.color = activatedAbilitiesColor;
        abiliyUsed = true;
        AddCooldown();
        ActiveState = true;
        _equipedAbilityBase = Instantiate(equipedAbility.Ability);
        _equipedAbilityBase.ActivateAbility();
        if (PhotonNetwork.IsConnected)
        {
          int viewID = GameMechanic.Instance.localPlayerSpot.GetComponent<PhotonView>().ViewID;
          RPCManager.Instance.photonView.RPC("RPC_AbilityActivated", RpcTarget.AllViaServer, viewID);
        }
        else
        {
          SYNC_AbilityActivation();
        }
      }
    }
    #endregion

    #region Private Methods

    #endregion

    #region Called by RPC
    /// <summary>
    /// This method is invoked through an RPC call
    /// <para>Here we can initialize stuff that we want both players to see ex: animations</para>
    /// </summary>
    public void SYNC_AbilityActivation()
    {
      Debug.Log("player has activated ability");
    }
    #endregion
  }
}