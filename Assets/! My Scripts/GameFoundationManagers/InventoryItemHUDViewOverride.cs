using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.GameFoundation.Components;
using TMPro;
using UnityEngine.Events;
using System;
using GuruLaghima.ProjectDilemma;
using MoreMountains.Feedbacks;
using GuruLaghima;

public class InventoryItemHUDViewOverride : InventoryItemHudView
{
  public GameObject notificationIcon;
  public bool usesRadialMenu = false;
  public ItemData whoDis;

  public InventoryView parentView;

  public UnityEvent OnEquipped;
  public UnityEvent OnUnequipped;

  public List<DictWrapper<MMFeedbacks>> feedbacks = new List<DictWrapper<MMFeedbacks>>();

  // public Dictionary<string, MMFeedbacks> feedbacks;


  [SerializeField] TextMeshProUGUI title;
  /// <summary>
  ///     Sets Inventory Item should be displayed by this view.
  /// </summary>
  /// <param name="itemDefinition">
  ///     The Inventory Item definition that should be displayed.
  /// </param>
  public void SetItemTitle(string text)
  {
    title.text = text;
  }

  public void HandleClick()
  {
    if (usesRadialMenu)
    {
      ShowRadialMenu();

    }
    else
    {
      Equip();
    }
  }

  public void ShowRadialMenu()
  {
    // how do I know which radial menu? reference to the Inventoryview that spawned this. that unventory view should know that. and lalso have a reference to that radial menu
    parentView.ShowRadialMenuSequence(this);
  }

  public void Equip()
  {
    if (ItemSettings.Instance)
      ItemSettings.Instance.Equip(whoDis, !whoDis.Equipped);

    if (parentView.onlyOneItemPerInventory)
    {
      parentView.UnequipPreviousitem();
      if (whoDis.Equipped)
        parentView.equippedItem = this;

    }

    if (whoDis.Equipped)
      OnEquipped?.Invoke();
    else
      OnUnequipped?.Invoke();
  }
  public void Equip(bool equip)
  {
    if (ItemSettings.Instance)
      ItemSettings.Instance.Equip(whoDis, equip);

    if (whoDis.Equipped)
      OnEquipped?.Invoke();
    else
      OnUnequipped?.Invoke();
  }

  public void SetItemData(ItemData inventoryItemType)
  {
    whoDis = inventoryItemType;
    if (whoDis.Equipped)
      OnEquipped?.Invoke();
    else
      OnUnequipped?.Invoke();
  }
}
