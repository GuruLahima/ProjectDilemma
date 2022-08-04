using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.GameFoundation.Components;
using UnityEngine.Events;
using System;
using GuruLaghima;
using static Workbench.ProjectDilemma.GameMechanic;
using Workbench.ProjectDilemma;

public class TransactionItemViewOverride : TransactionItemView
{

  [SerializeField] UnityEvent OnTransactionDisabled;

  private void Start()
  {
    Invoke("AssignClothingData", 1f);
    Invoke("UpdateVisualStatus", 1f);
  }

  void UpdateVisualStatus()
  {
    // * I need to check if the transaction for this TransactionView is repeatable 
    // * and if not then check if it has already been made
    // * so I can turn it off in the store or hide it or grey it out or whatever
    // need: place to save transactions. ScriptableObject? But that's just locally. I need to save the none-repeatable transaction keys in a DB
    if (TransactionsData.Instance && this.transaction != null)
      if (TransactionsData.Instance.recordedNonrepeatableTransactions.Contains(this.transaction.key))
      {
        // 
        OnTransactionDisabled?.Invoke();
      }

  }

  void AssignClothingData()
  {
    if (GetComponentInChildren<ClothingPlaceholder>())
      if (transaction != null)
        // if (transaction.payout.GetExchange(0) != null)
        GetComponentInChildren<ClothingPlaceholder>().Clothing = transaction.payout.GetExchange(0).tradableDefinition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<RigData>();
  }

  public void RecordTransaction()
  {
    if (TransactionsData.Instance)
      TransactionsData.Instance.AddTransaction(this.transaction.key);
  }

  public void Equip()
  {
    MyDebug.Log("Trying out store item");

    // if this is a clothing item add it to character 
    if (GetComponentInChildren<ClothingPlaceholder>().Clothing)
    {
      GetComponentInChildren<ClothingPlaceholder>().AddToCharacter();
    }
  }


  public void DisableTransaction()
  {
    OnTransactionDisabled?.Invoke();
  }
  public void UpdateNewlyAddedInfoForAllBoughtItems()
  {
    // for (int i = 0; i < this.transaction.payout.GetExchanges(); i++){
    //   // inventoryItemType.NewlyAdded
    //   this.transaction.payout.GetExchange(i).tradableDefinition.;
    // }
    //   foreach (var item in this.transaction.payout.GetExchange)
    //   {

    //   }
  }
}
