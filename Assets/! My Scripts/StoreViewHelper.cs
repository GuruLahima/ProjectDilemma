using System.Collections;
using System.Collections.Generic;
using GuruLaghima;
using UnityEngine;
using UnityEngine.GameFoundation.Components;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;
using UnityEngine.GameFoundation;
using UnityEngine.Events;

public class StoreViewHelper : MonoBehaviour
{
  [SerializeField] Button purchaseButton;
  [SerializeField] TextMeshProUGUI purchaseButtonAmount;
  [SerializeField] TextMeshProUGUI purchaseButtonDescription;
  [SerializeField] UnityEvent OnPurchasedUnrepeatableTransaction;
  [SerializeField] UnityEvent OnSelectedUnrepeatableAndOwnedTransaction;
  [SerializeField] UnityEvent OnSelectedTransaction;
  [SerializeField] UnityEvent OnPurchasedTransaction;

  StoreView storeV;
  private bool alreadyInitialized;

  // Start is called before the first frame update
  void Start()
  {
    storeV = GetComponent<StoreView>();
    // Invoke("DelayedStart", 2f);
  }

  // I am calling this function via SendMessageUpwards from the TransactionItemViewOverride
  void DelayedStart()
  {
    if (!alreadyInitialized)
    {
      alreadyInitialized = true;
      foreach (TransactionItemViewOverride item in storeV.itemContainer.GetComponentsInChildren<TransactionItemViewOverride>())
      {
        if (item.itemButton)
        {
          item.itemButton.onClick.AddListener(() =>
          {
            foreach (TransactionItemViewOverride it in storeV.itemContainer.GetComponentsInChildren<TransactionItemViewOverride>())
            {
              it.selectionBox.SetActive(false);
            }
            item.selectionBox.SetActive(true);

            // allow purchasing of this item only if it is not owned and not repeatable
            if (!(item.owned && item.unrepeatable))
            {

              // also don't show the selection feedbacks of it is owned so as the user don't mistake it for a a sign that they can purchase it
              OnSelectedTransaction?.Invoke();

              purchaseButtonAmount.text = item.purchaseButton.priceTextField.text;
              purchaseButton.onClick.RemoveAllListeners();
              purchaseButton.onClick.AddListener(() =>
              {
                OnPurchasedTransaction?.Invoke();
                item.purchaseButton.Purchase();
              });
            }
            else
            {
              // if we select an owned item it should clear the purchase button 
              purchaseButton.onClick.RemoveAllListeners();
              // purchaseButtonAmount.text = "";
              OnSelectedUnrepeatableAndOwnedTransaction?.Invoke();
            }

          });
        }
      }
    }

  }

  public void OnTransactionSucceeded(BaseTransaction transact)
  {
    if (IsTransactionUnrepeatableAndOwned(transact))
    {

      purchaseButton.onClick.RemoveAllListeners();
      // purchaseButtonAmount.text = "";

      OnPurchasedUnrepeatableTransaction?.Invoke();
    }

  }

  bool IsTransactionUnrepeatableAndOwned(BaseTransaction transact)
  {
    // if  we bought an one-time purhcase item (doesn't have quantity) then don't allow buying it again
    if (!transact.GetStaticProperty("repeatable"))
    {
      // and if it's only for one specific item (like an emote)
      int exchangesCount = transact.payout.GetExchanges();
      if (exchangesCount == 1)
      {
        // and if we own that item
        InventoryItemDefinition itemDef = (InventoryItemDefinition)transact.payout.GetExchange(0).tradableDefinition;
        if (GameFoundationSdk.inventory.GetTotalQuantity(itemDef) > 0)
        {

          return true;

        }
      }
    }
    return false;
  }

}
