using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Workbench.ProjectDilemma
{
  /// <summary>
  /// Please rename this script I couldnt come up with a name
  /// </summary>
  public class OperatePerk : MonoBehaviourPun
  {
    public List<PerkData> ownedPerks = new List<PerkData>();
    /// <summary>
    /// List of currently active perks on this player
    /// </summary>
    [HideInInspector] public List<PerkData> activePerks = new List<PerkData>();

    public SelectionMenu selectionMenu;
    public CursorManager cursorManager;
    private CanvasGroup canvasGroup;

    private void Start()
    {
      canvasGroup = selectionMenu.GetComponent<CanvasGroup>();
      if (photonView.IsMine)
      {
        foreach (PerkData perk in ownedPerks)
        {
          if (perk.Icon)
          {
            var ico = Instantiate(perk.Icon, selectionMenu.transform);
            ico.container = perk;
            if (perk.ico) ico.image.sprite = perk.ico;
          }
        }
      }
    }

    public void Pick()
    {
      // cancel the action if there is no more perks to activate
      if (selectionMenu.transform.childCount <= 0) return;

      canvasGroup.alpha = 1; canvasGroup.blocksRaycasts = true; canvasGroup.interactable = true;
      cursorManager.SetLockMode(CursorLockMode.Confined);
      cursorManager.SetVisibility(true);
    }

    public void Activate()
    {
      canvasGroup.alpha = 0; canvasGroup.blocksRaycasts = false; canvasGroup.interactable = false;
      cursorManager.SetLockMode(CursorLockMode.Locked);
      cursorManager.SetVisibility(false);

      if (selectionMenu.LastSelectedObject != null)
      {
        var container = selectionMenu.LastSelectedObject.GetComponent<SelectionMenuContainer>();
        if (container)
        {
          if (container.container is PerkData)
          {
            activePerks.Add(container.container as PerkData);
            Destroy(container.gameObject, 0.1f);
            if (PhotonNetwork.IsConnected)
            {
              //do stuff here
            }
            else
            {

            }
          }
        }
      }
    }

  }
}