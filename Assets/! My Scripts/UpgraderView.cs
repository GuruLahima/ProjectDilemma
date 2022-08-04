using UnityEngine;

using UnityEngine.GameFoundation;

using Workbench.ProjectDilemma;

using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using TMPro;

namespace GuruLaghima.ProjectDilemma
{


  public class UpgraderView : MonoBehaviour
  {

    public ItemSet selectedOutfit;


    #region exposed fields
    [SerializeField] TextMeshProUGUI outfitTitleLabel;

    [SerializeField] GameObject tier_1_slot;
    [SerializeField] Image tier_1_sprite;
    [SerializeField] GameObject tier_2_slot;
    [SerializeField] Image tier_2_sprite;
    [SerializeField] GameObject tier_3_slot;
    [SerializeField] Image tier_3_sprite;

    [SerializeField] GameObject headUpgrade;
    [SerializeField] GameObject chestUpgrade;
    [SerializeField] GameObject legsUpgrade;
    [SerializeField] GameObject glovesUpgrade;
    [SerializeField] GameObject feetUpgrade;
    [SerializeField] Transform rightPanel;
    [SerializeField] List<OutfitItemUpgradePanel> rightPanelItems;
    [SerializeField] EquipModal equipModal;
    [SerializeField] UnityEvent OnUpgrade;
    #endregion

    #region private fields
    private int selectedTierIndex;

    #endregion

    #region Monobehaviors

    #endregion

    #region public methods

    public List<ItemSet> GetTiers()
    {
      List<ItemSet> newList = new List<ItemSet>();

      ItemSet currentTier = selectedOutfit;
      while (currentTier)
      {
        newList.Add(currentTier);

        //
        currentTier = currentTier.NextTier;
      }

      return newList;
    }

    // intended to be called from inspector (on click of image)
    public void SelectTier(int tierIndex)
    {
      selectedTierIndex = tierIndex;
      ClearPreviousSelection();

      MyDebug.Log("ShowOutfit:: selected Tier index", tierIndex);
      rightPanel.gameObject.SetActive(true);

      ItemSet selectedTier = GetTiers()[tierIndex - 1];
      MyDebug.Log("ShowOutfit:: selectedTier", selectedTier.name);

      // populate the right panel with all the relevant information
      int i = 0;
      foreach (OutfitItemUpgradePanel item in rightPanelItems)
      {

        item.currentTierItem = selectedTier.GetItems()[i];
        if (item.currentTierItem)
        {
          item.itemTypeLabel.text = item.currentTierItem.type.ToString();
          item.currentTierItemAmount.text = item.currentTierItem.AmountOwned.ToString();
          item.currentTierItemImage.sprite = item.currentTierItem.ico;
          MyDebug.Log("ShowOutfit:: item.currentTierItem", item.currentTierItem.name);
        }

        if (selectedTier.NextTier)
        {
          item.nextTierItem = selectedTier.NextTier.GetItems()[i];
          if (item.nextTierItem)
          {
            item.nextTierItemRequiredAmount.text = item.currentTierItem.amountNeededTorUpgrade.ToString();
            item.nextTierItemImage.sprite = item.nextTierItem.ico;
            MyDebug.Log("ShowOutfit:: item.nextTierItem", item.nextTierItem.name);
          }
        }

        if (item.currentTierItem.AmountOwned >= item.currentTierItem.amountNeededTorUpgrade)
        {
          item.upgradeButton.interactable = true;
          item.upgradeButton.onClick.AddListener(() =>
          {
            // do the actual upgrade here
            Upgrade(item);
            // visual/audio feedback
            item.OnUpgrade?.Invoke();
          });

        }
        i++;
      }
    }


    // TODO: this should be driven from the OutfitItemUpgradePanel itself, with exposed UnityEvents for MMFeedbacks
    private void ClearPreviousSelection()
    {
      foreach (OutfitItemUpgradePanel item in rightPanelItems)
      {
        item.itemTypeLabel.text = "N/A";
        item.currentTierItem = null;
        item.currentTierItemAmount.text = "";
        item.currentTierItemImage.sprite = null;

        item.nextTierItem = null;
        item.nextTierItemRequiredAmount.text = "";
        item.nextTierItemImage.sprite = null;

        item.upgradeButton.onClick.RemoveAllListeners();
        item.upgradeButton.interactable = false;
      }

    }

    public void ShowOutfitTiers(ItemSet outfit)
    {
      selectedOutfit = outfit;
      foreach (RigData item in selectedOutfit.GetItems())
      {

        MyDebug.Log("ShowOutfit", item.name);
      }

      tier_1_slot.SetActive(false);
      tier_2_slot.SetActive(false);
      tier_3_slot.SetActive(false);
      rightPanel.gameObject.SetActive(false);
      outfitTitleLabel.text = outfit.outfitTitle;

      List<ItemSet> tiers = GetTiers();
      MyDebug.Log("ShowOutfit:: tiersCount", tiers.Count);

      if (tiers.Count > 0)
      {
        tier_1_slot.SetActive(true);
        tier_1_sprite.sprite = tiers[0].tierIcon;
      }
      if (tiers.Count > 1)
      {
        tier_2_slot.SetActive(true);
        tier_2_sprite.sprite = tiers[1].tierIcon;
      }
      if (tiers.Count > 2)
      {
        tier_3_slot.SetActive(true);
        tier_3_sprite.sprite = tiers[3].tierIcon;
      }

      // select first tier immediately. maybe. we'll see.
      // SelectTier(1);
    }
    #endregion

    #region private 

    private void Upgrade(OutfitItemUpgradePanel itemUpgradePanel)
    {
      MyDebug.Log("ShowOutfit:: Upgrading ", itemUpgradePanel.currentTierItem.inventoryitemDefinition.displayName);
      // actually create the instance of the rewarded item
      InventoryItem newItem = InventoryManager.instance.AddItem(itemUpgradePanel.nextTierItem.inventoryitemDefinition.key);

      ItemData itemData = itemUpgradePanel.nextTierItem;

      // discard X amount of duplicates (X being the number of duplicates necessary to produce the upgrade)
      ItemList duplicates = GameFoundationSdk.inventory.CreateList();
      int someInt = GameFoundationSdk.inventory.FindItems(itemUpgradePanel.currentTierItem.inventoryitemDefinition, duplicates, true);
      if (duplicates.Count > 1)
      {
        int limit = duplicates.Count;
        for (int i = 0; i < limit - 1; i++)
        {
          MyDebug.Log("ShowOutfit:: Deleting ", itemUpgradePanel.currentTierItem.inventoryitemDefinition.displayName);

          GameFoundationSdk.inventory.Delete(duplicates[0]);
        }
      }

      // refresh the upgrade panel
      Invoke("RefreshUpgradePanel", 0.5f);

      // show equip modal while passing the necessary information
      equipModal.ShowModal(itemData);

      // play upgrading feedback
      OnUpgrade?.Invoke();
    }

    void RefreshUpgradePanel()
    {
      SelectTier(selectedTierIndex);
    }

    #endregion
  }
}
