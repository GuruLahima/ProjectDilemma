using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.GameFoundation.Components;
using TMPro;
using UnityEngine.Events;
using System;

public class InventoryItemHUDViewOverride : InventoryItemHudView
{
  public ItemData whoDis;

  public UnityEvent OnEquipped;
  public UnityEvent OnUnequipped;


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

  public void Equip(bool equip)
  {
    if (ItemSettings.Instance)
      ItemSettings.Instance.Equip(whoDis, equip);
  }
  public void Equip()
  {
    if (ItemSettings.Instance)
      ItemSettings.Instance.Equip(whoDis, !whoDis.Equipped);

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
