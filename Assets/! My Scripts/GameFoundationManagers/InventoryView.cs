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

namespace GuruLaghima.ProjectDilemma
{


  public class InventoryView : MonoBehaviour, ISequenceExecutor
  {

    #region UI references
    /// <summary>
    /// the parent to which we will attach inventory items
    /// </summary>
    public Transform contentFrame;
    /// <summary>
    /// the inventory item template
    /// </summary>
    public Transform inventoryItemPrefab;
    /// <summary>
    /// the item type this inventory should render
    /// </summary>
    public ItemSettings.ItemType itemType;
    public string itemTag;
    /// <summary>
    /// the radial menu which pops up when we select an item
    /// </summary>
    public RadialChooseMenu radialMenu;
    public RadialMenuData radialMenuData;

    public ControllableSequence choosingSlotSequence;
    public ControllableSequence hidingRadialMenuSequence;

    public bool onlyOneItemPerInventory;
    public bool mustHaveOneEquippedAtAllTimes;
    [ShowIf("onlyOneItemPerInventory")]
    [ReadOnly]
    public InventoryItemHUDViewOverride equippedItem;

    public bool InSelection
    {
      get;
      set;
    }

    #endregion

    #region exposed fields

    #endregion

    #region property keys

    [SerializeField] string inventoryItem_HUDIconPropertyKey;

    #endregion

    #region private fields

    List<InventoryItemHUDViewOverride> inventoryItems = new List<InventoryItemHUDViewOverride>();

    #endregion

    #region Monobehaviors

    private void Awake()
    {
      if (radialMenuData)
        radialMenu.radialMenuData = this.radialMenuData;
    }

    // public void ItemWasEquipped(ItemData item)
    // {
    //   // if only one item is allowed to be equppied (from this list) then unequip other items
    //   if(onlyOneItemAllowed){
    //     if(!item.Equipped){

    //     }
    //   }
    // }

    private void OnEnable()
    {
      InventoryManager.InventoryUpdated += RefreshUI;
      if (radialMenu)
        radialMenu.OnSelectionChanged += UpdateSelectedSlotIcon;
    }

    private void UpdateSelectedSlotIcon(Transform lastSelected, Transform newlySelected)
    {
      lastSelectedSlot = newlySelected;
      newlySelected.GetComponent<SelectionMenuContainer>().image.sprite = lastItemSelected.whoDis.ico;
      if (lastSelected)
      {
        if (lastSelected.GetComponent<SelectionMenuContainer>().container)
          lastSelected.GetComponent<SelectionMenuContainer>().image.sprite = ((ItemData)lastSelected.GetComponent<SelectionMenuContainer>().container).ico;
        else
          lastSelected.GetComponent<SelectionMenuContainer>().image.sprite = lastSelected.GetComponent<SelectionMenuContainer>().defaultIcon;
      }
    }

    private void OnDisable()
    {
      InventoryManager.InventoryUpdated -= RefreshUI;
      if (radialMenu)
        radialMenu.OnSelectionChanged -= UpdateSelectedSlotIcon;

    }

    #endregion

    #region public methods
    public void StartSelectionPhase()
    {
      // start selection phase
      StartCoroutine(RadialMenuSelection());
    }

    public void ClearRadialMenuData()
    {
      for (int i = 0; i < radialMenuData.orderedItems.Count; i++)
      {
        radialMenuData.orderedItems[i] = null;
      }
    }

    public void ToggleInventoryVisibility()
    {
      contentFrame.gameObject.SetActive(!contentFrame.gameObject.activeSelf);
    }
    public void SetInventoryVisibility(bool visible)
    {
      contentFrame.gameObject.SetActive(visible);
    }

    public void ShowRadialMenuSequence(InventoryItemHUDViewOverride itemSelected)
    {
      StopAllCoroutines();
      this.lastItemSelected = itemSelected;
      choosingSlotSequence.rootCoroutine = choosingSlotSequence.RunSequence(this);
      StartCoroutine(choosingSlotSequence.rootCoroutine);
    }


    IEnumerator currentCoroutine;

    public IEnumerator SequenceCoroutine(List<ControllableSequence.EventWithDuration> eventSequence, string name)
    {
      MyDebug.Log($"[{name}]", "started");

      foreach (ControllableSequence.EventWithDuration ev in eventSequence)
      {
        if (ev.shouldExecute)
        {
          ev.theEvent?.Invoke();
          if (ev.isCoroutine)
          {
            if (currentCoroutine != null)
            {
              StopCoroutine(currentCoroutine);
              yield return StartCoroutine(currentCoroutine);
            }
            else
            {
              MyDebug.Log("There is no current coroutine. Assign it in the UnityEvent handler by calling a void function");
              yield return new WaitForSeconds(ev.duration);
            }
          }
          else
          {
            yield return new WaitForSeconds(ev.duration);
          }
        }
      }

      MyDebug.Log($"[{name}]", "ended");
    }

    public void HideItems()
    {
      MyDebug.Log("inventoryItems.Count", inventoryItems.Count);
      foreach (InventoryItemHUDViewOverride item in inventoryItems)
      {
        if (item)
        {

          MyDebug.Log("Hiding item", item.name);
          MyDebug.Log("using feedback", item.feedbacks.Find((obj) => obj.key == "Hide").key);
          item.feedbacks.Find((obj) => obj.key == "Hide")?.element?.PlayFeedbacks();
        }
      }
    }
    public void ShowItems()
    {
      MyDebug.Log("inventoryItems.Count", inventoryItems.Count);
      foreach (InventoryItemHUDViewOverride item in inventoryItems)
      {
        if (item)
        {

          MyDebug.Log("Showing item", item.name);
          MyDebug.Log("using feedback", item.feedbacks.Find((obj) => obj.key == "Show").key);
          item.feedbacks.Find((obj) => obj.key == "Show")?.element?.PlayFeedbacks();
        }
      }
    }
    private bool inSelection = true;
    private InventoryItemHUDViewOverride lastItemSelected;
    private Transform lastSelectedSlot;
    private bool onlyOneItemAllowed;

    public IEnumerator RadialMenuSelection()
    {
      inSelection = true;
      while (inSelection)
      {
        radialMenu.Activate();
        if (Input.GetMouseButtonDown(0))
          inSelection = false;
        yield return null;
      }
      // when selection ends
      // close the radial menu 
      HideRadialMenuSequence();
      // save the selected position in the RadialMenuData
      if (radialMenuData.orderedItems.Count > lastSelectedSlot.GetSiblingIndex())
        radialMenuData.orderedItems[lastSelectedSlot.GetSiblingIndex()] = lastItemSelected.whoDis;
      // update 
      radialMenu.PopulateRadialMenuFromData();
    }
    public void HideRadialMenuSequence()
    {
      StopAllCoroutines();
      hidingRadialMenuSequence.rootCoroutine = hidingRadialMenuSequence.RunSequence(this);
      StartCoroutine(hidingRadialMenuSequence.rootCoroutine);
    }


    public void UnequipPreviousitem()
    {
      if (equippedItem)
      {

        equippedItem.Equip(false);
        equippedItem = null;
      }
    }

    #endregion

    #region private methods

    /// <summary>
    /// This will fill out the main text box with information about the main inventory.
    /// </summary>
    private void RefreshUI()
    {

      ClearUI();

      PopulateUI();
    }

    // List<InventoryItemDefinition> addedItemTypes = new List<InventoryItemDefinition>();
    private void PopulateUI()
    {
      MyDebug.Log("Updating Inventory UI");
      List<ItemData> tempList = new List<ItemData>();
      // addedItemTypes.Clear();
      // Loop through every type of item within the inventory and display its name and quantity.
      List<InventoryItemDefinition> allClothesDefinitions = new List<InventoryItemDefinition>();
      GameFoundationSdk.catalog.FindItems<InventoryItemDefinition>(GameFoundationSdk.tags.Find(Keys.CLOTHES_TAG), allClothesDefinitions);
      switch (itemType)
      {
        case ItemSettings.ItemType.Emotes:
          MyDebug.Log("Updating Emotes");
          foreach (ItemData inventoryItemType in ItemSettings.Instance.emotes.Where((obj) => { return obj.inventoryitemDefinition.HasTag(GameFoundationSdk.tags.Find(itemTag)); }))
          {
            // visually we want only one instance of the HUD representation per item type because we specify the quantity of that type there too
            // if (addedItemTypes.Contains(inventoryItemType.inventoryitemDefinition))
            // {
            //   continue;
            // }
            InventoryItemHUDViewOverride newItem = Instantiate(inventoryItemPrefab, contentFrame, false).GetComponent<InventoryItemHUDViewOverride>();
            newItem.parentView = this;
            // addedItemTypes.Add(inventoryItemType.inventoryitemDefinition);
            newItem.SetItemDefinition(inventoryItemType.inventoryitemDefinition);
            newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
            newItem.SetItemTitle(inventoryItemType.inventoryitemDefinition.displayName);
            newItem.SetItemData(inventoryItemType);
            newItem.usesRadialMenu = true;
            // newItem.notificationIcon.SetActive(inventoryItemType.inventoryitemDefinition.);
            inventoryItems.Add(newItem);
            // newItem.GetComponent<ClothingPlaceholder>().Clothing = LoadItemData(inventoryItemType.inventoryitemDefinition.GetStaticProperty("ingame_ScriptableObject"), SetIconSprite, OnSpriteLoadFailed) as RigData;
          }
          break;
        case ItemSettings.ItemType.Throwables:
          MyDebug.Log("Updating Throwables");
          foreach (ItemData inventoryItemType in ItemSettings.Instance.throwables.Where((obj) => { return obj.inventoryitemDefinition.HasTag(GameFoundationSdk.tags.Find(itemTag)); }))
          {
            // visually we want only one instance of the HUD representation per item type because we specify the quantity of that type there too
            // if (addedItemTypes.Contains(inventoryItemType.inventoryitemDefinition))
            // {
            //   continue;
            // }
            InventoryItemHUDViewOverride newItem = Instantiate(inventoryItemPrefab, contentFrame, false).GetComponent<InventoryItemHUDViewOverride>();
            newItem.parentView = this;
            // addedItemTypes.Add(inventoryItemType.inventoryitemDefinition);
            newItem.SetItemDefinition(inventoryItemType.inventoryitemDefinition);
            newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
            newItem.SetItemTitle(inventoryItemType.inventoryitemDefinition.displayName);
            newItem.SetItemData(inventoryItemType);
            newItem.usesRadialMenu = true;
            // newItem.notificationIcon.SetActive(inventoryItemType.NewlyAdded);
            inventoryItems.Add(newItem);

            // newItem.GetComponent<ClothingPlaceholder>().Clothing = LoadItemData(inventoryItemType.inventoryitemDefinition.GetStaticProperty("ingame_ScriptableObject"), SetIconSprite, OnSpriteLoadFailed) as RigData;
          }
          break;
        case ItemSettings.ItemType.Clothes:
          MyDebug.Log("Updating Clothes");
          foreach (ItemData inventoryItemType in ItemSettings.Instance.clothes.Where((obj) => { return obj.inventoryitemDefinition.HasTag(GameFoundationSdk.tags.Find(itemTag)); }))
          {
            // visually we want only one instance of the HUD representation per item type because we specify the quantity of that type there too
            // if (addedItemTypes.Contains(inventoryItemType.inventoryitemDefinition))
            // {
            //   continue;
            // }
            InventoryItemHUDViewOverride newItem = Instantiate(inventoryItemPrefab, contentFrame, false).GetComponent<InventoryItemHUDViewOverride>();
            newItem.parentView = this;
            // addedItemTypes.Add(inventoryItemType.inventoryitemDefinition);
            newItem.SetItemDefinition(inventoryItemType.inventoryitemDefinition);
            newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
            newItem.SetItemTitle(inventoryItemType.inventoryitemDefinition.displayName);
            newItem.SetItemData(inventoryItemType);
            newItem.usesRadialMenu = false;
            // newItem.notificationIcon.SetActive(inventoryItemType.NewlyAdded);
            if (newItem.GetComponentInChildren<ClothingPlaceholder>())
              newItem.GetComponentInChildren<ClothingPlaceholder>().Clothing = LoadItemData(inventoryItemType.inventoryitemDefinition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT), SetIconSprite, OnSpriteLoadFailed) as RigData;
            inventoryItems.Add(newItem);
          }
          break;
        case ItemSettings.ItemType.ActivePerks:
          MyDebug.Log("Updating perks");
          foreach (ItemData inventoryItemType in ItemSettings.Instance.activePerks.Where((obj) => { return obj.inventoryitemDefinition.HasTag(GameFoundationSdk.tags.Find(itemTag)); }))
          {
            // visually we want only one instance of the HUD representation per item type because we specify the quantity of that type there too
            // if (addedItemTypes.Contains(inventoryItemType.inventoryitemDefinition))
            // {
            //   continue;
            // }
            InventoryItemHUDViewOverride newItem = Instantiate(inventoryItemPrefab, contentFrame, false).GetComponent<InventoryItemHUDViewOverride>();
            newItem.parentView = this;
            // addedItemTypes.Add(inventoryItemType.inventoryitemDefinition);
            newItem.SetItemDefinition(inventoryItemType.inventoryitemDefinition);
            newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
            newItem.SetItemTitle(inventoryItemType.inventoryitemDefinition.displayName);
            newItem.SetItemData(inventoryItemType);
            newItem.usesRadialMenu = false;
            // newItem.notificationIcon.SetActive(inventoryItemType.NewlyAdded);
            inventoryItems.Add(newItem);
          }
          break;
        case ItemSettings.ItemType.Abilities:
          MyDebug.Log("Updating abilities");
          foreach (ItemData inventoryItemType in ItemSettings.Instance.abilities.Where((obj) => { return obj.inventoryitemDefinition.HasTag(GameFoundationSdk.tags.Find(itemTag)); }))
          {
            // visually we want only one instance of the HUD representation per item type because we specify the quantity of that type there too
            // if (addedItemTypes.Contains(inventoryItemType.inventoryitemDefinition))
            // {
            //   continue;
            // }
            InventoryItemHUDViewOverride newItem = Instantiate(inventoryItemPrefab, contentFrame, false).GetComponent<InventoryItemHUDViewOverride>();
            newItem.parentView = this;
            // addedItemTypes.Add(inventoryItemType.inventoryitemDefinition);
            newItem.SetItemDefinition(inventoryItemType.inventoryitemDefinition);
            newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
            newItem.SetItemTitle(inventoryItemType.inventoryitemDefinition.displayName);
            newItem.SetItemData(inventoryItemType);
            newItem.usesRadialMenu = false;
            // newItem.notificationIcon.SetActive(inventoryItemType.NewlyAdded);
            inventoryItems.Add(newItem);
          }
          break;
        default:
          break;
      }

    }

    private void ClearUI()
    {
      MyDebug.Log("Clearing UI for", this.name);
      foreach (InventoryItemHUDViewOverride child in inventoryItems)
      {
        Destroy(child.gameObject);
      }
      inventoryItems.Clear();
    }


    /// <summary>
    ///     Triggers one of the two callbacks specified in the parameters, either onLoadSucceeded or on LoadFailed,
    ///     depending on whether an itemdata is found in the specified item data Property.
    /// </summary>
    /// <param name="scriptableObjectProperty">
    ///     The <see cref="Property"/> that maps to the desired itemdata.
    /// </param>
    /// <param name="onLoadSucceeded">
    ///     Callback for if a itemdata is successfully found from the given Property.
    /// </param>
    /// <param name="onLoadFailed">
    ///     Callback for if a itemdata could not be found in the given Property.
    /// </param>
    static ItemData LoadItemData(Property scriptableObjectProperty, Action<ItemData> onLoadSucceeded, Action<string> onLoadFailed)
    {
      if (Application.isPlaying)
      {
        switch (scriptableObjectProperty.type)
        {
          case PropertyType.ResourcesAsset:
            {
              if (scriptableObjectProperty.AsAsset<ItemData>() is ItemData data)
              {
                onLoadSucceeded?.Invoke(data);
                return data;
              }
              else
              {
                onLoadFailed?.Invoke($"No itemdata was found in the {scriptableObjectProperty.type} " +
                                     $"{nameof(Property)}.");
              }

              return null;
            }
          case PropertyType.Addressables:
            {
              var dataHandle = scriptableObjectProperty.AsAddressable<ItemData>();
              dataHandle.Completed += handle =>
              {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                  onLoadSucceeded?.Invoke(handle.Result);
                }
                else
                {
                  onLoadFailed?.Invoke($"A itemdata could not be found in the {nameof(Property)} " +
                                                 $"({scriptableObjectProperty.type}). Addressables exception: " +
                                                 $"{handle.OperationException}");
                }
              };
              return null;
            }
        }
      }
#if UNITY_EDITOR
      else
      {
        switch (scriptableObjectProperty.type)
        {
          case PropertyType.ResourcesAsset:
            {
              if (scriptableObjectProperty.AsAsset<ItemData>() is ItemData data)
              {
                onLoadSucceeded?.Invoke(data);
              }
              else
              {
                onLoadFailed?.Invoke($"No ItemData was found in the {nameof(PropertyType.ResourcesAsset)} " +
                                     $"{nameof(Property)}.");
              }
              return null;
            }
          case PropertyType.Addressables:
            onLoadFailed?.Invoke($"ItemDatas associated with Properties of type {scriptableObjectProperty.type} " +
                                 $"cannot be loaded at editor time.");
            return null;
        }
      }
#endif
      onLoadFailed?.Invoke($"PropertyType {scriptableObjectProperty.type} is unsupported for displaying ItemData.");
      return null;
    }

    /// <summary>
    ///     Sets ItemData of item in display.
    /// </summary>
    /// <param name="icon">
    ///     The new ItemData to display.
    /// </param>
    void SetIconSprite(ItemData data)
    {

    }

    /// <summary>
    ///     Callback for if there is an error while trying to load a sprite from its Property.
    /// </summary>
    /// <param name="errorMessage">
    ///     The error message returned by <see cref="PrefabTools.LoadSprite"/>.
    /// </param>
    void OnSpriteLoadFailed(string errorMessage)
    {
      MyDebug.LogWarning(errorMessage);
    }
    #endregion

  }
}
