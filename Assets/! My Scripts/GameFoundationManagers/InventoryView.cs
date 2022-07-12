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
namespace GuruLaghima.ProjectDilemma
{


  public class InventoryView : MonoBehaviour, ISequenceExecutor
  {

    #region public fields


    public bool onlyOneItemPerInventory;
    public bool mustHaveOneEquippedAtAllTimes;
    [ShowIf("onlyOneItemPerInventory")]
    [ReadOnly]
    public InventoryItemHUDViewOverride equippedItem;

    public ControllableSequence choosingSlotSequence;
    public ControllableSequence hidingRadialMenuSequence;
    #endregion

    #region UI references
    /// 
    [CustomTooltip("the parent to which we will attach inventory items")]
    public Transform contentFrame;
    /// 
    [CustomTooltip("the inventory item template")]
    public Transform inventoryItemPrefab;
    /// 
    [CustomTooltip("the item type this inventory should render")]
    public InventoryData.ItemType itemType;
    public string itemTag;
    /// 
    [HorizontalLine]
    [CustomTooltip("the radial menu which pops up when we select an item")]
    public RadialChooseMenu radialMenu;
    public RadialMenuData radialMenuData;
    [HorizontalLine]
    // 
    [CustomTooltip("the New Item notification for this inventory")]
    public GameObject inventoryNotification;


    public bool InSelection
    {
      get;
      set;
    }

    [HorizontalLine]
    public Image hoveredItemIcon;
    public TextMeshProUGUI hoveredItemTitle;
    public TextMeshProUGUI hoveredItemDesc;
    public string defaultItemTitle;
    public string defaultItemDesc;
    public Sprite defaultItemIcon;

    [HorizontalLine]

    #endregion

    #region exposed fields
    [SerializeField] bool usedForRecycler;
    [ShowIf("usedForRecycler")]
    [SerializeField] RecyclerView recyclerView;

    #endregion

    #region property keys

    [HorizontalLine]
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
      // clear data
      for (int i = 0; i < radialMenuData.orderedItems.Count; i++)
      {
        radialMenuData.orderedItems[i] = null;
      }
      // unequip items
      foreach (var item in inventoryItems)
      {
        item.Equip(false);
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
      radialMenu.RegenerateSnapPoints();
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
    // are we in the Radial Menu selecting a slot?
    private bool inSelection = false;
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
        {
          // when selection ends
          // close the radial menu 
          HideRadialMenuSequence();
          inSelection = false;

        }
        yield return null;
      }

      // save the selected position in the RadialMenuData
      if (radialMenuData.orderedItems.Count > lastSelectedSlot.GetSiblingIndex())
        radialMenuData.orderedItems[lastSelectedSlot.GetSiblingIndex()] = lastItemSelected.whoDis;
      // update 
      radialMenu.PopulateRadialMenuFromData();
      // select the inventory item in the Prep board view
      // lastItemSelected.Equip(true);
      // make sure selected item is marked equipped now and vice versa 
      // *(we have to go through all the items in the data because we might have overriden one of them so we have to unequipp them)
      foreach (InventoryItemHUDViewOverride item in inventoryItems)
      {
        ItemData data = item.itemDefinition.GetStaticProperty(ProjectDilemmaCatalog.Items.throwable_egg.StaticProperties.ingame_ScriptableObject).AsAsset<ItemData>();
        bool contains = radialMenuData.orderedItems.Contains(data);
        item.Equip(contains);
      }

    }
    public void HideRadialMenuSequence()
    {
      // hide the radial menu only if it is shown
      if (inSelection)
      {
        StopAllCoroutines();
        hidingRadialMenuSequence.rootCoroutine = hidingRadialMenuSequence.RunSequence(this);
        StartCoroutine(hidingRadialMenuSequence.rootCoroutine);
        inSelection = false;
      }
    }


    public void UnequipPreviousitem()
    {
      if (equippedItem)
      {
        var item = equippedItem;
        equippedItem = null;
        item.Equip(false);
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
      // Loop through every type of item within the inventory and display its name and quantity.
      List<InventoryItemDefinition> allClothesDefinitions = new List<InventoryItemDefinition>();
      GameFoundationSdk.catalog.FindItems<InventoryItemDefinition>(GameFoundationSdk.tags.Find(Keys.CLOTHES_TAG), allClothesDefinitions);
      switch (itemType)
      {
        case InventoryData.ItemType.Emotes:
          MyDebug.Log("Updating Emotes");
          PopulateUIForItemType(itemTag, InventoryData.Instance.emotes);
          break;
        case InventoryData.ItemType.Throwables:
          MyDebug.Log("Updating Throwables");
          PopulateUIForItemType(itemTag, InventoryData.Instance.throwables);
          break;
        case InventoryData.ItemType.Clothes:
          MyDebug.Log("Updating Clothes");
          PopulateUIForItemType(itemTag, InventoryData.Instance.clothes);
          break;
        case InventoryData.ItemType.ActivePerks:
          MyDebug.Log("Updating perks");
          PopulateUIForItemType(itemTag, InventoryData.Instance.activePerks);
          break;
        case InventoryData.ItemType.Abilities:
          MyDebug.Log("Updating abilities");
          PopulateUIForItemType(itemTag, InventoryData.Instance.abilities);
          break;
        case InventoryData.ItemType.Relics:
          MyDebug.Log("Updating relics");
          PopulateUIForItemType(itemTag, InventoryData.Instance.relics);
          break;
        default:
          break;
      }

    }

    void PopulateUIForItemType(string itemType, IEnumerable<ItemData> itemList)
    {
      foreach (ItemData inventoryItemType in itemList.Where((obj) => { return itemType == "" ? true : obj.inventoryitemDefinition.HasTag(GameFoundationSdk.tags.Find(itemType)); }))
      {
        if (usedForRecycler)
          if (inventoryItemType.AmountOwned < 2)
            continue;
        InventoryItemHUDViewOverride newItem = Instantiate(inventoryItemPrefab, contentFrame, false).GetComponent<InventoryItemHUDViewOverride>();
        newItem.parentView = this;
        newItem.notificationHandler = this.GetComponent<NewItemsTracker>();
        newItem.SetItemDefinition(inventoryItemType.inventoryitemDefinition);
        newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
        newItem.SetItemTitle(inventoryItemType.inventoryitemDefinition.displayName);
        newItem.SetItemData(inventoryItemType);
        newItem.Equip(inventoryItemType.Equipped);
        newItem.usesRadialMenu = false;
        if (itemType == "emotes" || itemType == "throwables") newItem.usesRadialMenu = true; else newItem.usesRadialMenu = false;
        newItem.notificationIcon.SetActive(inventoryItemType.NewlyAdded);
        if (inventoryItemType.NewlyAdded)
        {
          // this.NewItems++;
          if (newItem.notificationHandler)
            newItem.notificationHandler.NewItems++;
        }
        else
        {
          newItem.notificationPopFeedback.enabled = false;
          Destroy(newItem.notificationPopFeedback);
        }
        // show visually that this item has duplicates
        if (inventoryItemType.AmountOwned > 1)
        {
          newItem.feedbacks.Find((obj) => obj.key == "Duplicates")?.element?.PlayFeedbacks();
        }
        // if this view is used for the recycler feature then this item should have the information to which object to send data about itself
        if (usedForRecycler)
        {
          newItem.usedInRecycler = true;
          newItem.recyclerView = this.recyclerView;
        }
        if (newItem.GetComponentInChildren<ClothingPlaceholder>())
          newItem.GetComponentInChildren<ClothingPlaceholder>().Clothing = LoadItemData(inventoryItemType.inventoryitemDefinition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT), SetIconSprite, OnSpriteLoadFailed) as RigData;
        inventoryItems.Add(newItem);

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

      // NewItems = 0;
      if (GetComponent<NewItemsTracker>())
        GetComponent<NewItemsTracker>().NewItems = 0;
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
