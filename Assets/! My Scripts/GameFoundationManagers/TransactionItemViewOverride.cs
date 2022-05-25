using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.GameFoundation.Components;
using UnityEngine.Events;
using System;
using GuruLaghima;
using static Workbench.ProjectDilemma.GameMechanic;

public class TransactionItemViewOverride : TransactionItemView
{

  [SerializeField] UnityEvent OnTransactionDisabled;

  private void Awake()
  {
    Invoke("UpdateVisualStatus", 1f);

  }

  void UpdateVisualStatus()
  {
    MyDebug.Log(this.transaction.key);
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

  public void RecordTransaction()
  {
    if (TransactionsData.Instance)
      TransactionsData.Instance.AddTransaction(this.transaction.key);
  }

  public void DisableTransaction()
  {
    OnTransactionDisabled?.Invoke();
  }
}
