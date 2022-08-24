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
using UnityEngine.UI;

public class TransactionItemViewOverride : TransactionItemView
{

  [SerializeField] UnityEvent OnTransactionDisabled; // *should probably rename it to OnTransactionPurchased
  [SerializeField] TooltipTrigger tooltip;
  public Button itemButton;
  public GameObject selectionBox;
  public bool unrepeatable = false;
  public bool owned = false;

  private void Start()
  {
    Invoke("AssignClothingData", 1f);
    Invoke("UpdateVisualStatus", 1f);
    Invoke("AssignTooltip", 1f);
  }

  void NotifyStoreViewHelper()
  {
    SendMessageUpwards("DelayedStart", SendMessageOptions.RequireReceiver);
  }

  void UpdateVisualStatus()
  {
    // making sure this code executes only in play mode (when it executes in edite mode it throws errors)
    if (Application.isPlaying)
    {


      // * I need to check if the transaction for this TransactionView is repeatable 
      // * and if not then check if it has already been made
      // * so I can turn it off in the store or hide it or grey it out or whatever
      // need: place to save transactions. ScriptableObject? But that's just locally. I need to save the none-repeatable transaction keys in a DB
      // * this works (after loading/saving data is taken care of) but not for the already owned items at the start of a game. for that we will need something else
      if (TransactionsData.Instance && this.transaction != null)
        if (TransactionsData.Instance.recordedNonrepeatableTransactions.Contains(this.transaction.key))
        {
          // 
          OnTransactionDisabled?.Invoke();
        }

      // if this transaction is not repeatable,
      if (!this.transaction.GetStaticProperty("repeatable"))
      {
        unrepeatable = true;
        // and if it's only for one specific item (like an emote)
        int exchangesCount = this.transaction.payout.GetExchanges();
        if (exchangesCount == 1)
        {
          // and if we own that item
          InventoryItemDefinition itemDef = (InventoryItemDefinition)this.transaction.payout.GetExchange(0).tradableDefinition;
          if (GameFoundationSdk.inventory.GetTotalQuantity(itemDef) > 0)
          {
            // disable or enable the buy button
            OnTransactionDisabled?.Invoke();

            owned = true;

          }
        }

      }

      // notify Store parent 
      NotifyStoreViewHelper();
    }

  }

  void AssignClothingData()
  {
    if (GetComponentInChildren<ClothingPlaceholder>())
      if (transaction != null)
        // if (transaction.payout.GetExchange(0) != null)
        GetComponentInChildren<ClothingPlaceholder>().Clothing = transaction.payout.GetExchange(0).tradableDefinition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<RigData>();
  }

  void AssignTooltip()
  {
    if (tooltip != null)
    {
      if (transaction != null)
      {

        ItemData item = transaction.payout.GetExchange(0).tradableDefinition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<ItemData>();
        if (item)
        {
          tooltip.icon = item.ico;
          tooltip.title = item.inventoryitemDefinition.displayName;
          tooltip.description = (string)item.inventoryitemDefinition.GetStaticProperty("description") +
            "\n" + (string)item.inventoryitemDefinition.GetStaticProperty("note_description");
        }
      }
    }
  }

  public void RecordTransaction()
  {
    // if this transaction is repeatable don't record it
    if (this.transaction.GetStaticProperty("repeatable"))
      return;

    if (TransactionsData.Instance)
      TransactionsData.Instance.AddTransaction(this.transaction.key);
  }

  public void Equip()
  {
    MyDebug.Log("Trying out store item");

    // unequip other items


    // if this is a clothing item add it to character 
    if (GetComponentInChildren<ClothingPlaceholder>().Clothing)
    {
      GetComponentInChildren<ClothingPlaceholder>().AddToCharacter();
    }
  }


  public void DisableTransaction()
  {
    // if this transaction is repeatable don't disable it
    if (this.transaction.GetStaticProperty("repeatable"))
      return;

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
