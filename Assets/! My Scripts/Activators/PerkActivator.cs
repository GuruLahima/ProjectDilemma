using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using MoreMountains.Feedbacks;
using GuruLaghima;
using System;
using UnityEngine.Events;

namespace Workbench.ProjectDilemma
{
  /// <summary>
  /// Handles all perk related stuff
  /// </summary>
  public class PerkActivator : BaseActivatorComponent
  {

    #region events

    public UnityEvent PerkChosen;

    #endregion

    #region Public Fields
    public PerkData ownedPerk;
    #endregion

    #region Exposed Private Fields
    [SerializeField] Transform perkCanvasSlot;
    [SerializeField] Transform perkBuffSlot;
    [SerializeField] public Transform chooseAPerkWindow;
    [SerializeField] MMFeedbacks showChooseScreenFeedbacks;
    [SerializeField] MMFeedbacks hideChooseScreenFeedbacks;
    [SerializeField] MMFeedbacks disableChooseScreenFeedbacks;
    [SerializeField] ActivePerksChooser perkChooser;
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] UpgradeData upgradePerkBonus;
    #endregion

    #region Private Fields
    private ObjectiveBase activeQuest;
    private bool bonusAdded = false;
    private bool perkActivated = false;
    #endregion

    #region Public Methods
    public override void Init()
    {
      perkChooser.playerPerkFeature = this;
      perkChooser.Init();

    }

    public void ShowPerk()
    {
      if (perkActivated)
        return;
      if (showChooseScreenFeedbacks.IsPlaying)
      {
        return;
      }
      if (ActiveState)
      {
        HidePerk();
        return;
      }
      MyDebug.Log("Show perks window");
      ActiveState = true;
      CursorManager.SetLockMode(CursorLockMode.Confined);
      CursorManager.SetVisibility(true);
      showChooseScreenFeedbacks.StopFeedbacks();
      showChooseScreenFeedbacks.ResetFeedbacks();
      showChooseScreenFeedbacks.Direction = MMFeedbacks.Directions.TopToBottom;
      showChooseScreenFeedbacks.PlayFeedbacks();
    }
    public void HidePerk()
    {
      if (perkActivated)
        return;
      if (hideChooseScreenFeedbacks.IsPlaying || showChooseScreenFeedbacks.IsPlaying)
      {
        return;
      }

      MyDebug.Log("Hide perks window");

      ActiveState = false;
      //CursorManager.SetLockMode(CursorLockMode.Locked);
      //CursorManager.SetVisibility(false);
      try
      {
        hideChooseScreenFeedbacks.PlayFeedbacks();
      }
      catch (Exception ex)
      {
        MyDebug.Log(ex);
      }
    }

    public void DisablePerkWindow()
    {
      MyDebug.Log("Disable perks window");

      CursorManager.SetLockMode(CursorLockMode.Locked);
      CursorManager.SetVisibility(false);
      Invoke("DisableScreen", 1.5f);
    }

    void DisableScreen()
    {
      chooseAPerkWindow.gameObject.SetActive(false);
    }

    public void ActivatePerk()
    {
      if (perkActivated)
        return;

      PerkChosen?.Invoke();

      #region LOCAL STUFF
      perkActivated = true;
      MyDebug.Log("Perks window perk activated");
      activeQuest = Instantiate(ownedPerk.objective, perkCanvasSlot ?? transform);
      GameEvents.OnPerkActivated?.Invoke(ownedPerk);

      // show info about perk in the upper left corner
      if (ownedPerk.Icon && perkBuffSlot)
      {
        var ico = Instantiate(ownedPerk.Icon, perkBuffSlot);
        ico.container = ownedPerk;
        if (ownedPerk.ico)
        {
          ico.image.sprite = ownedPerk.ico;
        }
        perkBuffSlot.GetComponent<CanvasGroup>().alpha = 1;
      }
      #endregion

      #region INTERNET STUFF
      if (PhotonNetwork.IsConnected)
      {
        int viewID = GameMechanic.Instance.localPlayerSpot.GetComponent<PhotonView>().ViewID;
        RPCManager.Instance.photonView.RPC("RPC_PerkActivated", RpcTarget.AllViaServer, viewID);
      }
      else
      {
        SYNC_ActivatePerk();
      }
      #endregion

      Debug.Log("Perk " + activeQuest + " has been activated");
    }

    /// <summary>
    /// This method is invoked through an RPC call
    /// <para>Here we can initialize stuff that we want both players to see ex: animations</para>
    /// </summary>
    public void SYNC_ActivatePerk()
    {
      Debug.Log("RPC TEST PERK ACTIVATION");
    }
    public void AddPerkBonuses(ObjectiveCondition condition)
    {
      if (bonusAdded == true)
      {
        return;
      }
      bonusAdded = true;

      ObjectiveData.BonusModifiersWrapper[] mod = ownedPerk.GetBonusModifiers(condition);
      foreach (ObjectiveData.BonusModifiersWrapper pm in mod)
      {
        BonusModifierManager.Instance.AddModifier(pm.ModifierKey, pm.GetValue());
      }
    }
    #endregion

    #region Private Methods

    #endregion
  }
}