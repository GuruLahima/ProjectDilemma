using System.Collections;
using System.Collections.Generic;
using GuruLaghima;
using UnityEngine;
using UnityEngine.GameFoundation.Components;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class StoreViewHelper : MonoBehaviour
{
  [SerializeField] Button purchaseButton;
  [SerializeField] TextMeshProUGUI purchaseButtonAmount;
  [SerializeField] TextMeshProUGUI purchaseButtonDescription;
  [SerializeField] MMFeedbacks selectedItemFeedback;
  [SerializeField] MMFeedbacks boughtItemFeedback;

  StoreView storeV;
  // Start is called before the first frame update
  void Start()
  {
    storeV = GetComponent<StoreView>();
    Invoke("DelayedStart", 2f);
  }

  void DelayedStart()
  {
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
          selectedItemFeedback.PlayFeedbacks();

          purchaseButtonAmount.text = item.purchaseButton.priceTextField.text;
          purchaseButton.onClick.RemoveAllListeners();
          purchaseButton.onClick.AddListener(() =>
          {
            boughtItemFeedback.PlayFeedbacks();
            item.purchaseButton.Purchase();
          });
        });
      }
    }
  }
}
