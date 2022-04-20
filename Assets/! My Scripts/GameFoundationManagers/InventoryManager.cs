using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.GameFoundation.Components;

namespace GuruLaghima.ProjectDilemma
{
  /// <summary>
  /// This class manages the scene and serves as an example for inventory basics.
  /// </summary>
  public class InventoryManager : MonoBehaviour
  {

    #region property keys

    [SerializeField] string inventoryItem_HUDIconPropertyKey;

    #endregion

    #region UI references
    /// <summary>
    /// the parent to which we will attach inventory items
    /// </summary>
    public Transform contentFrame;
    /// <summary>
    /// the inventory item template
    /// </summary>
    public Transform inventoryItemPrefab;

    public GameObject canvas;

    #endregion


    #region private vars
    /// <summary>
    /// Flag for inventory item changed callback events to ensure they are added exactly once when
    /// Game Foundation finishes initialization or when script is enabled.
    /// </summary>
    private bool m_SubscribedFlag = false;
    /// <summary>
    /// Reference to a list of InventoryItems in the GameFoundationSdk.inventory.
    /// </summary>
    private readonly List<InventoryItem> m_InventoryItems = new List<InventoryItem>();

    /// <summary>
    /// Reference to a list of InventoryItems of a certain definition.
    /// </summary>
    private readonly List<InventoryItem> m_ItemsByDefinition = new List<InventoryItem>();
    #endregion

    #region Monobehaviour methods

    /// <summary>
    /// Standard starting point for Unity scripts.
    /// </summary>
    private void Start()
    {


      // Game Foundation Initialization is being managed by GameFoundationInit Game Object
      if (!GameFoundationSdk.IsInitialized)
      {
        // Disable all buttons while initializing
        // EnableAllButtons(false);
      }
    }

    /// <summary>
    /// Called when this object becomes enabled to ensure our callbacks are active, if Game Foundation
    /// has already completed initialization (otherwise they will be enabled at end of initialization).
    /// </summary>
    private void OnEnable()
    {
      SubscribeToGameFoundationEvents();
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
      // Show list of inventory items and update the button interactability.
      RefreshUI();

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
      MyDebug.Log("Updating inventory UI");
      RefreshUI();
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

    public void ToggleInventoryVisibility()
    {
      canvas.SetActive(!canvas.activeSelf);
    }
    public void SetInventoryVisibility(bool visible)
    {
      canvas.SetActive(visible);
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

    /// <summary>
    /// This will fill out the main text box with information about the main inventory.
    /// </summary>
    private void RefreshUI()
    {
      ClearUI();

      FetchInventory();

      PopulateUI();

    }

    List<InventoryItemDefinition> addedItemTypes = new List<InventoryItemDefinition>();
    private void PopulateUI()
    {

      addedItemTypes.Clear();
      // Loop through every type of item within the inventory and display its name and quantity.
      foreach (InventoryItem inventoryItem in m_InventoryItems)
      {
        // visually we want only one instance of the HUD representaion per item type because we specify the quantity of that type there too
        if (addedItemTypes.Contains(inventoryItem.definition))
        {
          continue;
        }
        InventoryItemHudView newItem = Instantiate(inventoryItemPrefab, contentFrame, false).GetComponent<InventoryItemHudView>();
        addedItemTypes.Add(inventoryItem.definition);
        newItem.SetItemDefinition(inventoryItem.definition);
        newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
      }
    }

    private void ClearUI()
    {
      InventoryItemHudView[] children = contentFrame.GetComponentsInChildren<InventoryItemHudView>();
      foreach (InventoryItemHudView child in children)
      {
        Destroy(child.gameObject);
      }
    }

    private void FetchInventory()
    {
      // We'll use the version of GetItems that lets us pass in a collection to be filled to reduce allocations
      GameFoundationSdk.inventory.GetItems(m_InventoryItems);
    }
    #endregion
  }
}
