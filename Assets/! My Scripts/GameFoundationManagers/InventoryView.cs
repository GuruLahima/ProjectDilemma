using UnityEngine;
using GuruLaghima;
using System.Collections.Generic;
using UnityEngine.GameFoundation;
using UnityEngine.GameFoundation.Components;
using System.Linq;
using Workbench.ProjectDilemma;
using System;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GuruLaghima.ProjectDilemma
{


  public class InventoryView : MonoBehaviour
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


    #endregion

    #region property keys

    [SerializeField] string inventoryItem_HUDIconPropertyKey;

    #endregion

    #region MOnobehaviors

    private void OnEnable()
    {
      InventoryManager.InventoryUpdated += RefreshUI;

    }

    private void OnDisable()
    {
      InventoryManager.InventoryUpdated -= RefreshUI;
    }

    #endregion

    #region public methods

    public void ToggleInventoryVisibility()
    {
      contentFrame.gameObject.SetActive(!contentFrame.gameObject.activeSelf);
    }
    public void SetInventoryVisibility(bool visible)
    {
      contentFrame.gameObject.SetActive(visible);
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
            // addedItemTypes.Add(inventoryItemType.inventoryitemDefinition);
            newItem.SetItemDefinition(inventoryItemType.inventoryitemDefinition);
            newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
            newItem.SetItemTitle(inventoryItemType.inventoryitemDefinition.displayName);
            newItem.SetItemData(inventoryItemType);

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
            // addedItemTypes.Add(inventoryItemType.inventoryitemDefinition);
            newItem.SetItemDefinition(inventoryItemType.inventoryitemDefinition);
            newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
            newItem.SetItemTitle(inventoryItemType.inventoryitemDefinition.displayName);
            newItem.SetItemData(inventoryItemType);

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
            // addedItemTypes.Add(inventoryItemType.inventoryitemDefinition);
            newItem.SetItemDefinition(inventoryItemType.inventoryitemDefinition);
            newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
            newItem.SetItemTitle(inventoryItemType.inventoryitemDefinition.displayName);
            newItem.SetItemData(inventoryItemType);
            if (newItem.GetComponentInChildren<ClothingPlaceholder>())
              newItem.GetComponentInChildren<ClothingPlaceholder>().Clothing = LoadItemData(inventoryItemType.inventoryitemDefinition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT), SetIconSprite, OnSpriteLoadFailed) as RigData;
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
            // addedItemTypes.Add(inventoryItemType.inventoryitemDefinition);
            newItem.SetItemDefinition(inventoryItemType.inventoryitemDefinition);
            newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
            newItem.SetItemTitle(inventoryItemType.inventoryitemDefinition.displayName);
            newItem.SetItemData(inventoryItemType);
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
            // addedItemTypes.Add(inventoryItemType.inventoryitemDefinition);
            newItem.SetItemDefinition(inventoryItemType.inventoryitemDefinition);
            newItem.SetIconSpritePropertyKey(inventoryItem_HUDIconPropertyKey);
            newItem.SetItemTitle(inventoryItemType.inventoryitemDefinition.displayName);
            newItem.SetItemData(inventoryItemType);
          }
          break;
        default:
          break;
      }

    }

    private void ClearUI()
    {
      InventoryItemHUDViewOverride[] children = contentFrame.GetComponentsInChildren<InventoryItemHUDViewOverride>();
      foreach (InventoryItemHUDViewOverride child in children)
      {
        Destroy(child.gameObject);
      }
    }

    #endregion

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
    public static ItemData LoadItemData(Property scriptableObjectProperty, Action<ItemData> onLoadSucceeded, Action<string> onLoadFailed)
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
  }
}
