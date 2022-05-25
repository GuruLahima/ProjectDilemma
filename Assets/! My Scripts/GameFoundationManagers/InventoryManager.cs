using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.GameFoundation.Components;
using UnityEngine.GameFoundation.DefaultLayers;
using UnityEngine.GameFoundation.DefaultLayers.Persistence;
using UnityEngine.SceneManagement;

namespace GuruLaghima.ProjectDilemma
{
  /// <summary>
  /// This class manages the inventory (update, save, informing other classes of changes)
  /// </summary>
  public class InventoryManager : MonoBehaviour
  {
    /// <summary>
    /// singleton
    /// </summary>
    public static InventoryManager instance;

    #region events
    public static Action InventoryUpdated;
    #endregion

    #region public fields
    /// <summary>
    /// Reference to a list of InventoryItems in the GameFoundationSdk.inventory.
    /// </summary>
    public ItemList m_InventoryItems;
    /// <summary>
    /// Reference to a list of InventoryItems of a certain definition.
    /// </summary>
    public List<InventoryItem> m_ItemsByDefinition = new List<InventoryItem>();
    /// <summary>
    /// Reference in the scene to the Game Foundation Init component.
    /// </summary>
    public GameFoundationInit gameFoundationInit;
    #endregion



    #region private vars
    /// <summary>
    /// Flag for inventory item changed callback events to ensure they are added exactly once when
    /// Game Foundation finishes initialization or when script is enabled.
    /// </summary>
    private bool m_SubscribedFlag = false;
    #endregion

    #region Monobehaviour methods

    private void Awake()
    {
      if (!instance)
        instance = this;
      else
        Destroy(this);

    }

    /// <summary>
    /// Standard starting point for Unity scripts.
    /// </summary>
    private void Start()
    {

    }

    /// <summary>
    /// Called when this object becomes enabled to ensure our callbacks are active, if Game Foundation
    /// has already completed initialization (otherwise they will be enabled at end of initialization).
    /// </summary>
    private void OnEnable()
    {
      SubscribeToGameFoundationEvents();
      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
      UpdateInventory();
    }

    /// <summary>
    /// Disable InventoryManager callbacks if Game Foundation has been initialized and callbacks were added.
    /// </summary>
    private void OnDisable()
    {
      UnsubscribeFromGameFoundationEvents();
    }
    #endregion

    #region callbacks
    /// <summary>
    /// Once Game Foundation completes initialization, we enable buttons, setup callbacks, update GUI, etc.
    /// </summary>
    public void OnGameFoundationInitialized()
    {
      m_InventoryItems = GameFoundationSdk.inventory.CreateList();

      UpdateInventory();

      // Ensure that the inventory item changed callback is connected
      SubscribeToGameFoundationEvents();
    }

    /// <summary>
    /// Listener for changes in GameFoundationSdk.inventory. Will get called whenever an item is added or removed.
    /// Because many items can get added or removed at a time, we will have the listener only set a flag
    /// that changes exist, and on our next update, we will check the flag to see whether changes to the UI
    /// need to be made.
    /// </summary>
    /// <param name="itemChanged">This parameter will not be used, but must exist so the signature is compatible with the inventory callbacks so we can bind it.</param>
    private void OnInventoryItemChanged(InventoryItem itemChanged)
    {
      UpdateInventory();
    }

    void UpdateInventory()
    {
      MyDebug.Log("Updating inventory");
      FetchInventory(m_InventoryItems);
      ParseInventoryToScriptableObject(m_InventoryItems);
      InventoryUpdated?.Invoke();
    }


    /// <summary>
    /// If GameFoundation throws exception, log the error to console.
    /// </summary>
    /// <param name="exception">
    /// Exception thrown by GameFoundation.
    /// </param>
    public void OnGameFoundationException(Exception exception)
    {
      Debug.LogError($"GameFoundation exception: {exception}");
    }
    #endregion

    #region public methods

    /// <summary>
    /// Adds a single item to the main inventory.
    /// </summary>
    public void AddItem(string itemDefinitionKey)
    {
      try
      {
        // This will create a new item inside the InventoryManager.
        var itemDefinition = GameFoundationSdk.catalog.Find<InventoryItemDefinition>(itemDefinitionKey);
        GameFoundationSdk.inventory.CreateItem(itemDefinition);
      }
      catch (Exception exception)
      {
        OnGameFoundationException(exception);
      }
    }

    /// <summary>
    /// Removes a single item from the main inventory.
    /// </summary>
    public void RemoveItem(string itemDefinitionKey)
    {
      try
      {
        // To remove a single item from the InventoryManager, you need a specific instance of that item. Since we only know the 
        // InventoryItemDefinition of the item we want to remove, we'll first look for all items with that definition.
        // We'll use the version of FindItems that lets us pass in a collection to be filled to reduce allocations.
        var itemDefinition = GameFoundationSdk.catalog.Find<InventoryItemDefinition>(itemDefinitionKey);
        var itemCount = GameFoundationSdk.inventory.FindItems(itemDefinition, m_InventoryItems);

        // Make sure there actually is an item available to return
        if (itemCount > 0)
        {
          // We'll remove the first instance in the list of items
          GameFoundationSdk.inventory.Delete(m_InventoryItems[0]);
        }
      }
      catch (Exception exception)
      {
        OnGameFoundationException(exception);
      }
    }

    /// <summary>
    /// Removes all items from the inventory WITHOUT reinitializing.
    /// </summary>
    public void RemoveAllInventoryItems()
    {
      try
      {
        GameFoundationSdk.inventory.DeleteAllItems();
      }
      catch (Exception exception)
      {
        OnGameFoundationException(exception);
      }
    }

    /// <summary>
    /// Uninitializes Game Foundation, deletes persistence data, then re-initializes Game Foundation.
    /// Note: Because Game Foundation is initialized again, all Initial Allocation items will be added again.
    /// </summary>
    public void DeleteAndReinitializeGameFoundation()
    {
      try
      {

        // Stop watching item events (events reconnected after initialization completes).
        UnsubscribeFromGameFoundationEvents();

        // Get the reference to the Data Layer used to initialize Game Foundation before GameFoundationSdk gets uninitialized.
        if (!(GameFoundationSdk.dataLayer is PersistenceDataLayer dataLayer))
          return;

        // Using the Data Layer get the local persistence layer.
        if (!(dataLayer.persistence is LocalPersistence localPersistence))
          return;

        // Uninitialize GameFoundation so we can delete the local persistence data file.
        gameFoundationInit.Uninitialize();

        // Delete local persistence data file.  Once finished, we will ReInitializeGameFoundation.
        // Note: if we fail to delete for any reason, exception will be sent to 
        //       OnGameFoundationInitializeException method for logging and buttons will remain disabled.
        localPersistence.Delete(() => gameFoundationInit.Initialize(),
            OnGameFoundationException);
      }
      catch (Exception exception)
      {
        OnGameFoundationException(exception);
      }
    }

    #endregion


    #region private methods

    /// <summary>
    /// Enable InventoryManager callbacks for our UI refresh method.
    /// These callbacks will automatically be invoked anytime an inventory item is added, or removed.
    /// This prevents us from having to manually invoke RefreshUI every time we perform one of these actions.
    /// </summary>
    private void SubscribeToGameFoundationEvents()
    {
      // If inventory has not yet been initialized the ignore request.
      if (!GameFoundationSdk.IsInitialized)
      {
        return;
      }

      // If app has been disabled then ignore the request
      if (!enabled)
      {
        return;
      }

      // Here we bind our UI refresh method to callbacks on the inventory manager.
      // These callbacks will automatically be invoked anytime an inventory item is added, or removed.
      // This allows us to refresh the UI as soon as the changes are applied.
      // Note: this ignores repeated requests to add callback if already properly set up.
      if (!m_SubscribedFlag)
      {
        GameFoundationSdk.inventory.itemAdded += OnInventoryItemChanged;
        GameFoundationSdk.inventory.itemDeleted += OnInventoryItemChanged;

        m_SubscribedFlag = true;
      }
    }

    /// <summary>
    /// Disable InventoryManager callbacks.
    /// </summary>
    private void UnsubscribeFromGameFoundationEvents()
    {
      // If inventory has not yet been initialized the ignore request.
      if (!GameFoundationSdk.IsInitialized)
      {
        return;
      }

      // If callbacks have been added then remove them.
      if (m_SubscribedFlag)
      {
        GameFoundationSdk.inventory.itemAdded -= OnInventoryItemChanged;
        GameFoundationSdk.inventory.itemDeleted -= OnInventoryItemChanged;

        m_SubscribedFlag = false;
      }
    }

    private void FetchInventory(ItemList inv)
    {
      // We'll use the version of GetItems that lets us pass in a collection to be filled to reduce allocations
      GameFoundationSdk.inventory.GetItems(inv);
      MyDebug.Log("Inventory items count:", inv.Count.ToString());
    }

    private void ParseInventoryToScriptableObject(ItemList items)
    {
      ItemSettings.Instance.ParseInventoryData(items);
    }
    #endregion
  }
}
