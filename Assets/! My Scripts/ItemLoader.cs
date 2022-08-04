using System;
using System.Collections.Generic;
using GuruLaghima;
using UnityEngine;
using UnityEngine.GameFoundation;
using Workbench.ProjectDilemma;

public class ItemLoader : MonoBehaviour
{

  // we call this function from a UnityEvent in the GameFoundationInit object
  public void OnGameFoundationInitialized()
  {
    List<InventoryItemDefinition> list = new List<InventoryItemDefinition>();

    FetchAllItems(list);
    ParseAllItemsToScriptableObject(list);
  }

  private void FetchAllItems(List<InventoryItemDefinition> list)
  {
    GameFoundationSdk.catalog.GetItems<InventoryItemDefinition>(list);
  }

  private void ParseAllItemsToScriptableObject(List<InventoryItemDefinition> list)
  {
    PopulateItemDataForEachItemDefinition(list);
  }

  private void PopulateItemDataForEachItemDefinition(List<InventoryItemDefinition> list)
  {
    // 
    foreach (InventoryItemDefinition item in list)
    {
      PopulateItemDataForOneItemDefinition(item);
    }

  }

  private void PopulateItemDataForOneItemDefinition(InventoryItemDefinition itemDefinition)
  {
    // MyDebug.Log("Populating item data for ", itemDefinition.displayName);

    ItemData tmpData = itemDefinition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<ItemData>();
    if (tmpData)
    {
      tmpData.Key = itemDefinition.key;
      tmpData.inventoryitemDefinition = itemDefinition;
      if (itemDefinition.HasStaticProperty("inventory_icon"))
        tmpData.ico = itemDefinition.GetStaticProperty("inventory_icon").AsAsset<Sprite>();
    }
  }
}