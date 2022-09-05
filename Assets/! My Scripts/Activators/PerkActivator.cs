using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using MoreMountains.Feedbacks;
using GuruLaghima;
using System;
using UnityEngine.Events;
using System.Linq;

namespace Workbench.ProjectDilemma
{
  /// <summary>
  /// Handles all perk related stuff
  /// </summary>
  public class PerkActivator : BaseActivatorComponent
  {

    #region events

    public UnityEvent ChoosingPerk;
    public UnityEvent PerkChosen;

    #endregion

    #region Public Property
    public bool PerkActivated
    {
      get
      {
        return perkActivated;
      }
    }
    public bool PerkCompleted
    {
      get
      {
        return bonusAdded;
      }
    }
    #endregion

    #region Public Fields
    public PerkData ownedPerk;
    [HideInInspector]
    public List<PerkData> equipedPerks = new List<PerkData>();
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
    [SerializeField] TooltipTrigger perkTooltip;
    [SerializeField] DiamonMenuEntry menuEntry;
    #endregion

    #region Private Fields
    private ObjectiveBase activeQuest;
    private bool bonusAdded = false;
    private bool perkActivated = false;
    #endregion

    #region Public Methods
    public override void Init()
    {
      // saving it here for syncing data //
      equipedPerks = InventoryData.Instance.activePerks.Where((obj) => { return obj.Owned && obj.Equipped; }).ToList();
      perkChooser.playerPerkFeature = this;
      perkChooser.Init();
    }

    public void ShowPerk()
    {
      MyDebug.Log("Show perks window");
      if (perkActivated)
        return;
      if (showChooseScreenFeedbacks.IsPlaying)
      {
        return;
      }
      if (ActiveState)
      {
        // HidePerk();
        return;
      }
      ChoosingPerk?.Invoke();
      MyDebug.Log("Showing perks window");
      ActiveState = true;
/*       showChooseScreenFeedbacks.StopFeedbacks();
      showChooseScreenFeedbacks.ResetFeedbacks();
      showChooseScreenFeedbacks.Direction = MMFeedbacks.Directions.TopToBottom; */
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
      Invoke("DisableScreen", 1f);
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
      activeQuest = Instantiate(ownedPerk.objective, perkCanvasSlot);
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

      // perk showcase tooltip
      perkTooltip.title = ownedPerk.inventoryitemDefinition.displayName;
      string descriptionKey = ProjectDilemmaCatalog.Items.perk_doubleDown.StaticProperties.description;
      perkTooltip.description = ownedPerk.inventoryitemDefinition.GetStaticProperty(descriptionKey);

      // diamond menu icon change
      // first swap the default icon with the previous icon to complete the illusion of rotation of the items (because that's how the MMfeedbacks are set up)
      if (menuEntry.chosenItemIcon.sprite)
        menuEntry.defaultIcon.sprite = menuEntry.chosenItemIcon.sprite;
      menuEntry.chosenItemIcon.sprite = ownedPerk.ico;
      menuEntry.switchToChosenIconFeedbacks.PlayFeedbacks();

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

      MyDebug.Log("Perk " + activeQuest + " has been activated");
    }

    /// <summary>
    /// This method is invoked through an RPC call
    /// <para>Here we can initialize stuff that we want both players to see ex: animations</para>
    /// </summary>
    public void SYNC_ActivatePerk()
    {
      MyDebug.Log("RPC TEST PERK ACTIVATION");
    }
    public void AddPerkBonuses(ObjectiveCondition condition)
    {
      if (bonusAdded == true)
      {
        return;
      }
      bonusAdded = true;

      BonusModifiersWrapper[] mod = ownedPerk.GetBonusModifiers(condition);
      foreach (BonusModifiersWrapper bmw in mod)
      {
        BonusModifierManager.Instance.AddModifier(bmw);
      }
    }
    #endregion

    #region Private Methods

    #endregion
  }
}