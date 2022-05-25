using System;
using System.Collections.Generic;
using GuruLaghima;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.GameFoundation;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "TransactionsData", menuName = "Workbench/ScriptableObjects/TransactionsData", order = 1)]
public class TransactionsData : SingletonScriptableObject<TransactionsData>
{

  #region public fields
  public List<string> recordedNonrepeatableTransactions = new List<string>();
  #endregion

  #region private fields
  #endregion

  #region public methods

  /// <summary>
  /// todo: this function should be called at beggining of game and at any update of transactions
  /// todo: it should sync this ScriptableObjects record with the one from the datalayer 
  /// </summary>
  public void LoadTransactionDataFromDataLayer()
  {

  }
  
  public void AddTransaction(string transactionKey)
  {
    recordedNonrepeatableTransactions.Add(transactionKey);
  }
  #endregion


  #region 

  #endregion

}