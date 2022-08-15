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

  [Header("UI stuff")]
  public GameObject notificationIcon;
  public GameObject quantityTextFieldParent;
  public Image rarityGraphic;
  [SerializeField] TextMeshProUGUI title;
  [SerializeField] TooltipTrigger tooltip; 
  [Header("References")]
  public ItemData whoDis;
  public InventoryView parentView;
  public NewItemsTracker notificationHandler;
  public RecyclerView recyclerView;
  public CombinerView combinerView;
  public RelicView relicView;
  public BookView bookView;
  public ShelfView shelfView;
  #endregion

  #region public fields 
  public bool usesRadialMenu = false;
  public List<DictWrapper<MMFeedbacks>> feedbacks = new List<DictWrapper<MMFeedbacks>>();
  public bool usedInRecycler;
  public bool usedForCombiner;
  public bool usedByRelics;
  public bool usedByBook;
  public bool usedInShelf;
  #endregion

  #region exposed fields

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
    if (tooltip)
    {
      tooltip.title = text;
    }
  }

  public void SetItemTooltip(string text)
  {
    if (tooltip)
    {
      tooltip.description = text;
    }
  }

  public void SetItemIcon(Sprite sprite)
  {
    if (tooltip)
    {
      tooltip.icon = sprite;
    }
  }

  public void HandleHover()
  {
    if (usedByBook)
    {
      bookView.ItemHovered(this);
    }
  }

  public void HandleClick()
  {
    if (usedInRecycler)
    {
      recyclerView.ItemSelected(this);
      return;
    }
    if (usedForCombiner)
    {
      combinerView.ItemSelected(this);
      return;
    }
    if (usesRadialMenu)
    {
      ShowRadialMenu();
    }
    if (usedByBook)
    {
      bookView.ItemSelected(this);
      return;
    }
    if (usedInShelf)
    {
      shelfView.ItemSelected(this);
      return;
    }
    if (usedByRelics)
    {
      relicView.ItemSelected(this);
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
    //don't do anything if we have already equipped this item
    // if (parentView.mustHaveOneEquippedAtAllTimes)
    //   if (parentView.equippedItem)
    //     if (parentView.equippedItem.whoDis == whoDis)
    //       return;

    // if this is a clothing item add it to character 
    if (GetComponentInChildren<ClothingPlaceholder>())
    {
      if (GetComponentInChildren<ClothingPlaceholder>().Clothing && equip)
      {
        GetComponentInChildren<ClothingPlaceholder>().AddToCharacter();
      }
    }


    if (InventoryData.Instance)
      InventoryData.Instance.Equip(whoDis, equip);

    if (equip)
    {
      // MyDebug.Log("AddCLothing:: Equipping", this.whoDis.name);
      // if only one item of this type can be equipped then unequip the previously equipped one.
      if (parentView.onlyOneItemPerInventory)
        if (parentView.equippedItem)
          if (parentView.equippedItem.whoDis != whoDis)
            parentView.UnequipPreviousitem();

      // record this item as the last equipped item
      parentView.equippedItem = this;
    }

    if (equip)
    {
      // MyDebug.Log("Equipping OnEquipped", whoDis.Key);

      OnEquipped?.Invoke();
    }
    else
    {
      // MyDebug.Log("Equipping OnUnquipped", whoDis.Key);

      OnUnequipped?.Invoke();
    }
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


