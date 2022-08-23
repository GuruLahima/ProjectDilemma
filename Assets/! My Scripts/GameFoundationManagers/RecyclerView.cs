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
using Michsky.UI.ModernUIPack;

namespace GuruLaghima.ProjectDilemma
{


  public class RecyclerView : MonoBehaviour
  {

    #region public fields

    #endregion

    #region UI references
    [SerializeField] Image slotImage;
    [SerializeField] Sprite defaultSlotSprite;
    [SerializeField] GameObject slotTextObj;
    [SerializeField] Button recycleButton;
    [SerializeField] TextMeshProUGUI recycleRewardAmountText;
    [SerializeField] SliderManager quantitySlider;

    #endregion

    #region exposed fields

    [SerializeField] UnityEvent OnItemSelected;
    [SerializeField] UnityEvent OnItemRecycled;
    [SerializeField] UnityEvent OnRecyclerUnlocked;
    #endregion

    #region property keys


    #endregion

    #region private fields
    private InventoryItemHUDViewOverride selectedItem;
    private static int amountToRecycle;


    #endregion

    #region Monobehaviors
    private void Start()
    {

      // unlock recycler only if unlocking level has been reached
      GameParameter starterPack = GameFoundationSdk.catalog.Find<GameParameter>(ProjectDilemmaCatalog.GameParameters.recycler_unlock_level.key);
      bool unlocked = LauncherScript.CalcCurrentLevel() >= starterPack[ProjectDilemmaCatalog.GameParameters.recycler_unlock_level.StaticProperties.unlock_level];
      if (unlocked)
      {
        OnRecyclerUnlocked?.Invoke();
      }
    }
    #endregion

    #region public methods
    public void UpdateAmount(float value)
    {
      amountToRecycle = (int)Mathf.Round(value * 1.0f);
      if (recycleRewardAmountText)
        if (selectedItem)
          if (selectedItem.whoDis)
            recycleRewardAmountText.text = "You get \n" + selectedItem.whoDis.recyclingRewardAmount * amountToRecycle + " dilemma points";

    }
    public void AssignSelectedItemToSlot()
    {
      //disable text in the slot
      this.slotTextObj.SetActive(false);
      // show selected item in slot
      this.slotImage.sprite = selectedItem.iconImageField.sprite;
      // enable recycling button (should turn from grey to colored)
      this.recycleButton.interactable = true;
      this.recycleButton.GetComponent<CanvasGroup>().alpha = 1;
      // set up the quantity chooser
      quantitySlider.mainSlider.minValue = 0;
      // quantitySlider.mainSlider.maxValue = selectedItem.whoDis.AmountOwned - 1; // ! deprecated (duplicates-only recycling)
      quantitySlider.mainSlider.maxValue = selectedItem.whoDis.AmountOwned;
      quantitySlider.mainSlider.value = quantitySlider.mainSlider.maxValue;
      amountToRecycle = (int)Mathf.Round(quantitySlider.mainSlider.value * 1.0f);
      // calculate currency exchange rate
      recycleRewardAmountText.text = "You get \n" + selectedItem.whoDis.recyclingRewardAmount * amountToRecycle + " dilemma points";
    }

    public void Recycle()
    {
      MyDebug.Log("Recycling ", selectedItem.whoDis.name);

      // receive the reward (duplicates count x currency reward per item)
      Currency m_CoinDefinition = GameFoundationSdk.catalog.Find<Currency>(ProjectDilemmaCatalog.Currencies.currency_dilemmaPoints.key);
      GameFoundationSdk.wallet.Add(m_CoinDefinition, (long)(this.selectedItem.whoDis.recyclingRewardAmount * (amountToRecycle)));

      MyDebug.Log("New amount", GameFoundationSdk.wallet.Get(m_CoinDefinition));

      // discard duplicates
      ItemList duplicates = GameFoundationSdk.inventory.CreateList();
      int someInt = GameFoundationSdk.inventory.FindItems(this.selectedItem.whoDis.inventoryitemDefinition, duplicates, true);
      // if (duplicates.Count > 1) // ! deprecated (duplicates-only recycling)
      int limit = amountToRecycle;
      for (int i = 0; i < limit; i++)
      {
        GameFoundationSdk.inventory.Delete(duplicates[0]);
      }

      // reset fields
      this.slotTextObj.SetActive(true);
      this.slotImage.sprite = this.defaultSlotSprite;
      this.recycleButton.interactable = false;
      this.recycleButton.GetComponent<CanvasGroup>().alpha = 0;
      recycleRewardAmountText.text = "";
      // play recycling feedback
      OnItemRecycled?.Invoke();
    }
    #endregion

    #region private methods

    #endregion
    public void ItemSelected(InventoryItemHUDViewOverride inventoryItemHUDViewOverride)
    {
      MyDebug.Log("Selected ", inventoryItemHUDViewOverride.name + " for recycling");
      this.selectedItem = inventoryItemHUDViewOverride;
      OnItemSelected?.Invoke();
    }
  }
}
