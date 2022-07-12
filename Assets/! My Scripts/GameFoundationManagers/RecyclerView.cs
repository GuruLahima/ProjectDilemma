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

    #endregion

    #region exposed fields

    [SerializeField] UnityEvent OnItemSelected;
    [SerializeField] UnityEvent OnItemRecycled;
    #endregion

    #region property keys


    #endregion

    #region private fields
    private InventoryItemHUDViewOverride selectedItem;


    #endregion

    #region Monobehaviors

    #endregion

    #region public methods

    public void AssignSelectedItemToSlot()
    {
      //disable text in the slot
      this.slotTextObj.SetActive(false);
      // show selected item in slot
      this.slotImage.sprite = selectedItem.iconImageField.sprite;
      // enable recycling button (should turn from grey to colored)
      this.recycleButton.interactable = true;
      this.recycleButton.GetComponent<CanvasGroup>().alpha = 1;
      // calculate currency exchange rate
      recycleRewardAmountText.text = "You get \n" + selectedItem.whoDis.recyclingRewardAmount * selectedItem.whoDis.AmountOwned + " dilemma points";
    }

    public void Recycle()
    {
      // receive the reward
      Currency m_CoinDefinition = GameFoundationSdk.catalog.Find<Currency>(ProjectDilemmaCatalog.Currencies.currency_dilemmaPoints.key);
      GameFoundationSdk.wallet.Add(m_CoinDefinition, (long)(this.selectedItem.whoDis.recyclingRewardAmount * this.selectedItem.whoDis.AmountOwned));

      MyDebug.Log("New amount", GameFoundationSdk.wallet.Get(m_CoinDefinition));

      // discard one copy of the object. wait, how do we tell how any copies we want discarded? or all duplicates?
      ItemList duplicates = GameFoundationSdk.inventory.CreateList();
      int someInt = GameFoundationSdk.inventory.FindItems(this.selectedItem.whoDis.inventoryitemDefinition, duplicates, true);
      if (duplicates.Count > 1)
      {
        int limit = duplicates.Count;
        for (int i = 0; i < limit - 1; i++)
        {
          GameFoundationSdk.inventory.Delete(duplicates[0]);
        }
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
