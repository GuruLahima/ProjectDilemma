using UnityEngine;
using GuruLaghima;
using System.Collections.Generic;
using UnityEngine.GameFoundation;
using UnityEngine.GameFoundation.Components;
using System.Linq;
using Workbench.ProjectDilemma;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using MoreMountains.Feedbacks;
using NaughtyAttributes;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace GuruLaghima.ProjectDilemma
{


  public class UpgraderView : MonoBehaviour
  {

    #region public fields
    [SerializeField]
    [HideInInspector] public int SelectedSlot { get; set; }

    #endregion

    #region UI references
    [SerializeField] Image slot_1_Image;
    [SerializeField] Image slot_2_Image;
    [SerializeField] Sprite defaultSlotSprite;
    [SerializeField] GameObject slot_1_TextObj;
    [SerializeField] GameObject slot_2_TextObj;
    [SerializeField] Button upgradeButton;
    [SerializeField] Image upgradedItemSlotImage;

    #endregion

    #region exposed fields

    [SerializeField] UnityEvent OnItemSelected;
    [SerializeField] UnityEvent OnItemUpgraded;
    [SerializeField] UnityEvent OnUpgraderUnlocked;
    #endregion

    #region property keys


    #endregion

    #region private fields
    private InventoryItemHUDViewOverride selectedItem_1;
    private InventoryItemHUDViewOverride selectedItem_2;

    #endregion

    #region Monobehaviors
    private void Start()
    {

      // unlock upgrader only if unlocking level has been reached
      GameParameter starterPack = GameFoundationSdk.catalog.Find<GameParameter>(ProjectDilemmaCatalog.GameParameters.upgrader_unlock_level.key);
      bool unlocked = LauncherScript.CalcCurrentLevel() >= starterPack[ProjectDilemmaCatalog.GameParameters.upgrader_unlock_level.StaticProperties.unlock_level];
      if (unlocked)
      {
        OnUpgraderUnlocked?.Invoke();
      }
    }
    #endregion

    #region public methods

    public void AssignSelectedItemToSlot()
    {
      if (SelectedSlot == 1)
      {

        //disable text in the slot
        this.slot_1_TextObj.SetActive(false);
        // show selected item in slot
        this.slot_1_Image.sprite = selectedItem_1.iconImageField.sprite;
      }
      else if (SelectedSlot == 2)
      {
        //disable text in the slot
        this.slot_2_TextObj.SetActive(false);
        // show selected item in slot
        this.slot_2_Image.sprite = selectedItem_2.iconImageField.sprite;
      }

      // show upgrade button only if both slots are filled
      if (selectedItem_1 != null && selectedItem_2 != null)
      {
        // enable recycling button (should turn from grey to colored)
        this.upgradeButton.interactable = true;
        this.upgradeButton.GetComponent<CanvasGroup>().alpha = 1;
        // if both items are of the same type (e.g. both are pants) 
        if (((RigData)selectedItem_1.whoDis).type == ((RigData)selectedItem_2.whoDis).type)
        {
          // show the appropriate mistery image (e.g. pants with a question mark)
          switch (((RigData)selectedItem_1.whoDis).type)
          {
            case ClothingType.Mask:
              upgradedItemSlotImage.sprite = MiscelaneousSettings.Instance.upgrader_mistery_mask_image;
              break;
            case ClothingType.Chest:
              upgradedItemSlotImage.sprite = MiscelaneousSettings.Instance.upgrader_mistery_chest_image;
              break;
            case ClothingType.Leggings:
              upgradedItemSlotImage.sprite = MiscelaneousSettings.Instance.upgrader_mistery_pants_image;
              break;
            case ClothingType.Gloves:
              upgradedItemSlotImage.sprite = MiscelaneousSettings.Instance.upgrader_mistery_gloves_image;
              break;
            case ClothingType.Boots:
              upgradedItemSlotImage.sprite = MiscelaneousSettings.Instance.upgrader_mistery_shoes_image;
              break;
            default:
              break;
          }
        }
        else
        {
          upgradedItemSlotImage.sprite = MiscelaneousSettings.Instance.upgrader_mistery_random_item_image;
        }
      }

    }

    public void Upgrade()
    {
      // if both items are of the same type (e.g. both are pants) 
      InventoryItemDefinition itemType;
      if (((RigData)selectedItem_1.whoDis).type == ((RigData)selectedItem_2.whoDis).type)
      {
        // give a random next tier item of the same type
        ItemRarity rarityOfUpgrade;
        rarityOfUpgrade = (ItemRarity)((int)selectedItem_1.whoDis.Rarity + 1);
        MyDebug.Log("UpgradeView:: rarity of selected item 1", selectedItem_1.whoDis.Rarity.ToString());
        MyDebug.Log("UpgradeView:: rarityOfUpgrade", rarityOfUpgrade.ToString());
        if (Enum.IsDefined(typeof(ItemRarity), rarityOfUpgrade))
        {
          List<InventoryItemDefinition> itemsOfNexTier = InventoryManager.instance.FetchItemTypesByRarity(rarityOfUpgrade, ParseClothingTypeToTagString(((RigData)selectedItem_1.whoDis).type));
          itemType = HelperFunctions.ChooseRandomItemFromCollection(itemsOfNexTier);
          MyDebug.Log("UpgradeView:: random item is", itemType.displayName);
        }
        else
        {
          rarityOfUpgrade = ItemRarity.Unique;
          List<InventoryItemDefinition> itemsOfNexTier = InventoryManager.instance.FetchItemTypesByRarity(rarityOfUpgrade, ParseClothingTypeToTagString(((RigData)selectedItem_1.whoDis).type));
          itemType = HelperFunctions.ChooseRandomItemFromCollection(itemsOfNexTier);
          MyDebug.Log("UpgradeView:: random item is", itemType.displayName);
        }
      }
      // if items are not of the same type (e.g. pants and masks)
      else
      {
        // give a random next tier item of random type
        MyDebug.Log("UpgradeView:: not coded yet");
        return;

      }

      // actually create the instance of the rewarded item
      InventoryManager.instance.AddItem(itemType.key);

      // I am thinking every time we click Upgrade a new instance of the upgraded item is created and one instance of the component items are discarded until 
      // at least one of the items is depleted at which point that item slot is restored to default values and the upgrade button is hidden

      // discard X amount of duplicates (X being the number of duplicates necessary to produce the upgrade)
      /* ItemList duplicates = GameFoundationSdk.inventory.CreateList();
      int someInt = GameFoundationSdk.inventory.FindItems(this.selectedItem_2.whoDis.inventoryitemDefinition, duplicates, true);
      if (duplicates.Count > 1)
      {
        int limit = duplicates.Count;
        for (int i = 0; i < limit - 1; i++)
        {
          GameFoundationSdk.inventory.Delete(duplicates[0]);
        }
      } */

      // reset fields
      this.slot_1_TextObj.SetActive(true);
      this.slot_1_Image.sprite = this.defaultSlotSprite;
      this.slot_2_TextObj.SetActive(true);
      this.slot_2_Image.sprite = this.defaultSlotSprite;
      this.upgradeButton.interactable = false;
      this.upgradeButton.GetComponent<CanvasGroup>().alpha = 0;
      upgradedItemSlotImage.sprite = defaultSlotSprite;
      // play upgrading feedback
      OnItemUpgraded?.Invoke();
    }



    private string ParseClothingTypeToTagString(ClothingType type)
    {
      string tag = "";
      switch (type)
      {
        case ClothingType.Boots:
          tag = "footware";
          break;
        case ClothingType.Chest:
          tag = "torso";
          break;
        case ClothingType.Gloves:
          tag = "gloves";
          break;
        case ClothingType.Leggings:
          tag = "pants";
          break;
        case ClothingType.Mask:
          tag = "head";
          break;
        default:
          break;
      }

      return tag;
    }
    #endregion

    #region private methods

    #endregion
    public void ItemSelected(InventoryItemHUDViewOverride inventoryItemHUDViewOverride)
    {
      MyDebug.Log("Selected ", inventoryItemHUDViewOverride.name + " for upgrading");
      if (SelectedSlot == 1)
      {
        this.selectedItem_1 = inventoryItemHUDViewOverride;
      }
      else if (SelectedSlot == 2)
      {
        this.selectedItem_2 = inventoryItemHUDViewOverride;
      }
      OnItemSelected?.Invoke();
    }
  }
}
