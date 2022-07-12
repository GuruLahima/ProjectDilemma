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
using Workbench.ProjectDilemma;
using UnityEngine.UI;

public class InventoryItemHUDViewOverride : InventoryItemHudView
{
  #region references

  public GameObject notificationIcon;
  public ItemData whoDis;

  public InventoryView parentView;
  public NewItemsTracker notificationHandler;
  public RecyclerView recyclerView;
  #endregion

  #region public fields 
  public bool usesRadialMenu = false;
  public List<DictWrapper<MMFeedbacks>> feedbacks = new List<DictWrapper<MMFeedbacks>>();
  public bool usedInRecycler;
  #endregion

  #region exposed fields

  [SerializeField] TextMeshProUGUI title;
  public MMFeedbacks notificationPopFeedback;
  #endregion

  public UnityEvent OnEquipped;
  public UnityEvent OnUnequipped;


  #region monobehaviours

  #endregion

  #region public methods


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
    if (usedInRecycler)
    {
      recyclerView.ItemSelected(this);
      return;
    }
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
    Equip(!whoDis.Equipped);
  }
  public void Equip(bool equip)
  {
    if (parentView.mustHaveOneEquippedAtAllTimes)
    {
      if (parentView.equippedItem == this)
        return;
    }

    // if this is a clothing item add it to character 
    if (GetComponentInChildren<ClothingPlaceholder>().Clothing && equip)
    {
      GetComponentInChildren<ClothingPlaceholder>().AddToCharacter();
    }

    if (InventoryData.Instance)
      InventoryData.Instance.Equip(whoDis, equip);

    // if only one item of this type can be equipped then unequip the previously equipped one.
    if (parentView.onlyOneItemPerInventory)
    {
      parentView.UnequipPreviousitem();
      if (equip)
        parentView.equippedItem = this;
    }

    if (equip)
      OnEquipped?.Invoke();
    else
      OnUnequipped?.Invoke();
  }

  public void SetItemData(ItemData inventoryItemType)
  {
    whoDis = inventoryItemType;
  }

  public void ShowItemInfoInInventory()
  {
    if (parentView.hoveredItemIcon)
    {
      parentView.hoveredItemIcon.sprite = this.whoDis.inventoryitemDefinition.GetStaticProperty(ProjectDilemmaCatalog.Items.throwable_egg.StaticProperties.inventory_icon).AsAsset<Sprite>();
      parentView.hoveredItemTitle.text = this.whoDis.inventoryitemDefinition.displayName;
      parentView.hoveredItemDesc.text = (string)this.whoDis.inventoryitemDefinition.GetStaticProperty(ProjectDilemmaCatalog.Items.throwable_egg.StaticProperties.description);
    }
  }
  public void ResetInfoInInventory()
  {
    if (parentView.hoveredItemIcon)
    {
      parentView.hoveredItemIcon.sprite = parentView.defaultItemIcon;
      parentView.hoveredItemTitle.text = parentView.defaultItemTitle;
      parentView.hoveredItemDesc.text = parentView.defaultItemDesc;
    }
  }
  public void RemoveNotificationIcon()
  {
    if (whoDis.NewlyAdded)
    {

      // tell GameFoundation the item was hovered
      if (InventoryData.Instance)
        InventoryData.Instance.RemoveFromNewlyAdded(whoDis);

      // relfect those changes in the UI
      this.notificationIcon.SetActive(false);
      notificationPopFeedback.StopFeedbacks();
      Destroy(notificationPopFeedback);

      // notify parentView that this item was checked out so it can decide if the other icons in the hierarchy should be disabled too
      // parentView.NewItems--;
      notificationHandler.NewItems--;
    }
  }
  #endregion
}


