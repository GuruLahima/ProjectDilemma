using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Workbench.ProjectDilemma
{
  /// <summary>
  /// Please rename this script I couldnt come up with a name
  /// </summary>
  public class OperatePerk : InputBaseComponent
  {
    [SerializeField] Transform perkSlot;
    [SerializeField] CanvasGroup canvasGroup;

    /// <summary>
    /// List of currently owned perks on this player
    /// </summary>
    public PerkData ownedPerk;

    
    public CursorManager cursorManager;

    public override void Init()
    {
      if (ownedPerk.Icon)
      {
        var ico = Instantiate(ownedPerk.Icon, perkSlot.transform);
        ico.container = ownedPerk;
        if (ownedPerk.ico) ico.image.sprite = ownedPerk.ico;
      }
    }

    public void SwitchState()
    {
      if(canvasGroup.alpha > 0)
      {
        Hide();
      }
      else
      {
        Show();
      }
    }

    private void Show()
    {
      if (OnCooldown) return;

      AddCooldown();

      ActiveState = true;
      canvasGroup.alpha = 1;
      canvasGroup.interactable = true;
      canvasGroup.blocksRaycasts = true;
      cursorManager.SetLockMode(CursorLockMode.Confined);
      cursorManager.SetVisibility(true);
    }

    private void Hide()
    {
      if (OnCooldown) return;

      AddCooldown();

      ActiveState = false;
      canvasGroup.alpha = 0;
      canvasGroup.interactable = false;
      canvasGroup.blocksRaycasts = false;
      cursorManager.SetLockMode(CursorLockMode.Locked);
      cursorManager.SetVisibility(false);
    }

    /// <summary>
    /// This method is probably going to be called via UI button
    /// </summary>
    public void ActivatePerk()
    {

    }
  }
}