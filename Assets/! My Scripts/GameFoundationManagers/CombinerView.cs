using UnityEngine;
using UnityEngine.GameFoundation;
using Workbench.ProjectDilemma;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using NaughtyAttributes;

namespace GuruLaghima.ProjectDilemma
{


  public class CombinerView : MonoBehaviour
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
    [HorizontalLine]
    [SerializeField] EquipModal equipModal;


    #endregion

    #region exposed fields

    [SerializeField] UnityEvent OnItemSelected;

    [SerializeField] UnityEvent OnItemSelected_ForSlot_1;
    [SerializeField] UnityEvent OnItemSelected_ForSlot_2;
    [SerializeField] UnityEvent OnItemUpgraded;
    [SerializeField] UnityEvent OnUpgraderUnlocked;
    [SerializeField] UnityEvent OnMismatchOfRarity;
    [SerializeField] UnityEvent OnBothCorrectItemsInSlots;
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
      GameParameter starterPack = GameFoundationSdk.catalog.Find<GameParameter>(ProjectDilemmaCatalog.GameParameters.combiner_unlock_level.key);
      bool unlocked = LauncherScript.CalcCurrentLevel() >= starterPack[ProjectDilemmaCatalog.GameParameters.combiner_unlock_level.StaticProperties.unlock_level];
      if (unlocked)
      {
        OnUpgraderUnlocked?.Invoke();
      }
    }
    #endregion

    #region public methods

    public void ItemSelected(InventoryItemHUDViewOverride selectedItem)
    {
      MyDebug.Log("Selected ", selectedItem.name + " for upgrading");
      if (SelectedSlot == 1)
      {
        // if the other slot is already filled
        if (this.selectedItem_2)
        {
          //  check if the rarity matches. 
          if (selectedItem.whoDis.Rarity != selectedItem_2.whoDis.Rarity)
          {
            // if rarity doesn't match cancel the selection and show feedback why it was cancelled
            this.selectedItem_1 = null;
            // reset text in the slot
            this.slot_1_TextObj.SetActive(true);
            // show default sprite in slot
            this.slot_1_Image.sprite = defaultSlotSprite;

            OnItemSelected?.Invoke();
            OnMismatchOfRarity?.Invoke();
            return;
          }
        }
        OnItemSelected_ForSlot_1?.Invoke();
        this.selectedItem_1 = selectedItem;
      }
      else if (SelectedSlot == 2)
      {
        // if the other slot is already filled
        if (this.selectedItem_1)
        {
          //  check if the rarity matches. 
          if (selectedItem.whoDis.Rarity != selectedItem_1.whoDis.Rarity)
          {
            // if rarity doesn't match cancel the selection and show feedback why it was cancelled
            this.selectedItem_2 = null;
            // reset text in the slot
            this.slot_2_TextObj.SetActive(true);
            // show default sprite in slot
            this.slot_2_Image.sprite = defaultSlotSprite;

            OnItemSelected?.Invoke();
            OnMismatchOfRarity?.Invoke();
            return;
          }
        }
        OnItemSelected_ForSlot_2?.Invoke();
        this.selectedItem_2 = selectedItem;
      }
      OnItemSelected?.Invoke();
    }

    public void AssignSelectedItemToSlot()
    {
      if (SelectedSlot == 1)
      {
        // if the selected slot was not filled (mismatch of rarity for example) don't do anything
        if (!selectedItem_1)
          return;

        //disable text in the slot
        this.slot_1_TextObj.SetActive(false);
        // show selected item in slot
        this.slot_1_Image.sprite = selectedItem_1.iconImageField.sprite;

      }
      else if (SelectedSlot == 2)
      {
        // if the selected slot was not filled (mismatch of rarity for example) don't do anything
        if (!selectedItem_2)
          return;

        //disable text in the slot
        this.slot_2_TextObj.SetActive(false);
        // show selected item in slot
        this.slot_2_Image.sprite = selectedItem_2.iconImageField.sprite;
      }

      // show upgrade button only if both slots are filled
      if (selectedItem_1 != null && selectedItem_2 != null)
      {
        // enable recycling button (should turn from grey to colored)
        // ! this should be done via inspector, not code
        /*         this.upgradeButton.interactable = true;
                this.upgradeButton.GetComponent<CanvasGroup>().alpha = 1; */

        // 
        upgradedItemSlotImage.sprite = MiscelaneousSettings.Instance.upgrader_mistery_random_item_image;

        OnBothCorrectItemsInSlots?.Invoke();
      }

    }

    public void Upgrade()
    {
      // if both items are of the same type (both are pants) 
      // give a random next tier item of the same type

      // if both items are of the same type (e.g. both are pants) 
      InventoryItemDefinition itemType;
      // give a random next tier item of the same type
      ItemRarity rarityOfUpgrade;
      rarityOfUpgrade = (ItemRarity)((int)selectedItem_1.whoDis.Rarity + 1);
      MyDebug.Log("UpgradeView:: rarity of selected item 1", selectedItem_1.whoDis.Rarity.ToString());
      MyDebug.Log("UpgradeView:: rarityOfUpgrade", rarityOfUpgrade.ToString());
      if (System.Enum.IsDefined(typeof(ItemRarity), rarityOfUpgrade))
      {
        List<InventoryItemDefinition> itemsOfNexTier = InventoryManager.instance.FetchItemTypesByRarity(rarityOfUpgrade, "cosmetics");
        if (itemsOfNexTier.Count > 0)
        {

          itemType = HelperFunctions.ChooseRandomItemFromCollection(itemsOfNexTier);
          MyDebug.Log("UpgradeView:: random item is", itemType.displayName);
        }
        else
        {
          MyDebug.Log("UpgradeView:: no available items of this rarity", rarityOfUpgrade);
          return;
        }
      }
      else
      {
        rarityOfUpgrade = ItemRarity.Unique;
        List<InventoryItemDefinition> itemsOfNexTier = InventoryManager.instance.FetchItemTypesByRarity(rarityOfUpgrade, "cosmetics");
        if (itemsOfNexTier.Count > 0)
        {
          itemType = HelperFunctions.ChooseRandomItemFromCollection(itemsOfNexTier);
          MyDebug.Log("UpgradeView:: random item is", itemType.displayName);
        }
        else
        {
          MyDebug.Log("UpgradeView:: no available items of this rarity", rarityOfUpgrade);
          return;
        }
      }


      // actually create the instance of the rewarded item
      InventoryItem newItem = InventoryManager.instance.AddItem(itemType.key);
      ItemData itemData = newItem.definition.GetStaticProperty("ingame_ScriptableObject").AsAsset<ItemData>();
      itemData.inventoryitemDefinition = newItem.definition;

      // discard X amount of duplicates (X being the number of duplicates necessary to produce the upgrade)
      ItemList duplicates = GameFoundationSdk.inventory.CreateList();
      int someInt = GameFoundationSdk.inventory.FindItems(this.selectedItem_2.whoDis.inventoryitemDefinition, duplicates, true);
      if (duplicates.Count > 1)
      {
        int limit = duplicates.Count;
        for (int i = 0; i < limit - 1; i++)
        {
          GameFoundationSdk.inventory.Delete(duplicates[0]);
        }
      }

      // show result of combination
      upgradedItemSlotImage.sprite = newItem.definition.GetStaticProperty("inventory_icon").AsAsset<Sprite>();


      // reset fields
      this.slot_1_Image.sprite = this.defaultSlotSprite;
      this.slot_2_Image.sprite = this.defaultSlotSprite;
      this.slot_1_TextObj.SetActive(true);
      this.slot_2_TextObj.SetActive(true);
      this.upgradedItemSlotImage.sprite = this.defaultSlotSprite;

      // show equip modal while passing the necessary information
      equipModal.ShowModal(itemData);

      // play upgrading feedback
      OnItemUpgraded?.Invoke();
    }

    public void ClearSlot(int slotIndex)
    {
      if (slotIndex == 1)
      {
        this.slot_1_Image.sprite = this.defaultSlotSprite;
        this.slot_1_TextObj.SetActive(true);
        selectedItem_1 = null;
      }
      else if (slotIndex == 2)
      {
        this.slot_2_Image.sprite = this.defaultSlotSprite;
        this.slot_2_TextObj.SetActive(true);
        selectedItem_2 = null;
      }
    }
    #endregion

    #region private methods

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
        case ClothingType.Skin:
          tag = "skin";
          break;
        default:
          break;
      }

      return tag;
    }

    #endregion
  }
}
