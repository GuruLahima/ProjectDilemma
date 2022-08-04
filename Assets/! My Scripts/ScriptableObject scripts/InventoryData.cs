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

[CreateAssetMenu(fileName = "ItemSettings", menuName = "Workbench/ScriptableObjects/ItemSettings", order = 1)]
public class InventoryData : SingletonScriptableObject<InventoryData>
{

#if UNITY_EDITOR
  [SerializeField] bool restoreValuesOnAwake = true;
  private void OnEnable()
  {
    if (restoreValuesOnAwake)
    {
      ClearData();
    }
  }
#endif

  public enum ItemType
  {
    emotes,
    cosmetics,
    throwables,
    perks,
    abilities,
    relics

  }
  #region public fields

  [ReadOnly]
  [CustomTooltip("This list is populated from backend")]
  public List<ItemData> allItems = new List<ItemData>();
  [ReadOnly]
  [CustomTooltip("This list is populated from backend")]
  public List<ProjectileData> throwables = new List<ProjectileData>();
  [ReadOnly]
  [CustomTooltip("This list is populated from backend")]
  public List<EmoteData> emotes = new List<EmoteData>();
  [ReadOnly]
  [CustomTooltip("This list is populated from backend")]
  public List<RigData> clothes = new List<RigData>();
  [ReadOnly]
  [CustomTooltip("This list is populated from backend")]
  public List<PerkData> activePerks = new List<PerkData>();
  [ReadOnly]
  [CustomTooltip("This list is populated from backend")]
  public List<AbilityData> abilities = new List<AbilityData>();
  [ReadOnly]
  [CustomTooltip("This list is populated from backend")]
  public List<RelicData> relics = new List<RelicData>();
  #endregion

  #region private fields
  private List<InventoryItemDefinition> throwablesTypes = new List<InventoryItemDefinition>();
  private List<InventoryItemDefinition> emoteTypes = new List<InventoryItemDefinition>();
  private List<InventoryItemDefinition> clothesTypes = new List<InventoryItemDefinition>();
  private List<InventoryItemDefinition> allTypes = new List<InventoryItemDefinition>();
  private List<InventoryItemDefinition> activePerksTypes = new List<InventoryItemDefinition>();
  private List<InventoryItemDefinition> abilitiesTypes = new List<InventoryItemDefinition>();
  private List<InventoryItemDefinition> relicsTypes = new List<InventoryItemDefinition>();
  #endregion

  #region 
  public void Equip(ItemData item, bool equip)
  {
    // MyDebug.Log("Equipping ", item.Key + ": " + equip);
    if (allTypes.Contains(item.inventoryitemDefinition))
    {
      ItemList allItemsOfThisType = GameFoundationSdk.inventory.CreateList();
      GameFoundationSdk.inventory.FindItems(item.inventoryitemDefinition, allItemsOfThisType);
      foreach (InventoryItem itm in allItemsOfThisType)
      {

        itm.SetMutableProperty(Keys.ITEMPROPERTY_EQUIPPED, equip);
      }
      item.Equipped = equip;
      // notify any possible inventory views using this item
      item.inventoriesUsingThisItem.ForEach((view) =>
      {
        view.OnItemEquipped(item);
      });
    }

  }

  public void RemoveFromNewlyAdded(ItemData item)
  {
    // MyDebug.Log("RemoveFromNewlyAdded ", item.Key);
    if (allTypes.Contains(item.inventoryitemDefinition))
    {
      ItemList allItemsOfThisType = GameFoundationSdk.inventory.CreateList();
      GameFoundationSdk.inventory.FindItems(item.inventoryitemDefinition, allItemsOfThisType);
      foreach (InventoryItem itm in allItemsOfThisType)
      {
        itm.SetMutableProperty("newlyAdded", false);
      }
    }
    item.NewlyAdded = false;

  }

  [Button]
  public void ClearData()
  {
    allItems.Clear();
    allTypes.Clear();
    throwables.Clear();
    throwablesTypes.Clear();
    emotes.Clear();
    emoteTypes.Clear();
    clothes.Clear();
    clothesTypes.Clear();
    activePerks.Clear();
    activePerksTypes.Clear();
    abilities.Clear();
    abilitiesTypes.Clear();
    relics.Clear();
    relicsTypes.Clear();

  }

  public void ParseInventoryData(ItemList items)
  {
    MyDebug.Log("Parsing inventory data");
    ClearData();
    ParseAllItems(items);
  }
  #endregion

  private void ParseAllItems(ItemList items)
  {
    if (items != null)
    {

      foreach (InventoryItem item in items)
      {
        PopulateItemDataFromInventoryItem(item);
      }

      ParseThrowables(items);
      ParseEmotes(items);
      ParseClothes(items);
      ParseActivePerks(items);
      ParseAbilities(items);
      ParseRelics(items);
    }
  }

  private void PopulateItemDataFromInventoryItem(InventoryItem item)
  {
    // 
    if (allTypes.Contains(item.definition))
    {
      // MyDebug.Log("Adding quantity to ", item.definition.displayName);

      // add the quantity of this item to the ProjectileData representation for this item type
      ItemData proj = allItems.Find((obj) => { return obj.inventoryitemDefinition == item.definition; });
      proj.AmountOwned += 1;
      // dont add another entry in the allItems list
      return;
    }
    // MyDebug.Log("Populating item data for ", item.definition.displayName);
    // MyDebug.Log("UpgraderView:: item.GetMutableProperty(equipped)", (bool)item.GetMutableProperty("equipped"));


    allTypes.Add(item.definition);
    ItemData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<ItemData>();
    if (tmpData)
    {
      tmpData.Key = item.definition.key;
      tmpData.inventoryitemDefinition = item.definition;
      tmpData.AmountOwned = 1;
      if (item.HasMutableProperty("equipped"))
        tmpData.Equipped = (bool)item.GetMutableProperty("equipped");
      if (item.HasMutableProperty("newlyAdded"))
        tmpData.NewlyAdded = (bool)item.GetMutableProperty("newlyAdded");
      // MyDebug.Log(item.definition.displayName + " has mutable property::", item.HasMutableProperty("equipped") + (item.HasMutableProperty("equipped") ? " with value of " + (bool)item.GetMutableProperty("equipped") : ""));
    }
    allItems.Add(tmpData);
  }

  #region 

  private void PopulateProjectileDataFromInventoryItem(InventoryItem item)
  {
    // MyDebug.Log("Populating projectile data for ", item.definition.displayName);
    throwablesTypes.Add(item.definition);
    ProjectileData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<ProjectileData>();
    throwables.Add(tmpData);

  }
  private void PopulateEmoteDataFromInventoryItem(InventoryItem item)
  {
    // MyDebug.Log("Populating emote data for ", item.definition.displayName);
    emoteTypes.Add(item.definition);
    EmoteData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<EmoteData>();
    emotes.Add(tmpData);
  }

  private void PopulateClothesDataFromInventoryItem(InventoryItem item)
  {
    // MyDebug.Log("Populating clothe data for ", item.definition.displayName);
    clothesTypes.Add(item.definition);
    RigData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<RigData>();
    // tmpData.Equipped = (bool)item.GetDefaultValueOfMutableProperty("equipped");
    clothes.Add(tmpData);
  }

  private void PopulatePerksDataFromInventoryItem(InventoryItem item)
  {
    // MyDebug.Log("Populating perk data for ", item.definition.displayName);
    activePerksTypes.Add(item.definition);
    PerkData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<PerkData>();
    activePerks.Add(tmpData);
  }

  private void PopulateAbilityDataFromInventoryItem(InventoryItem item)
  {
    // MyDebug.Log("Populating ability data for ", item.definition.displayName);
    abilitiesTypes.Add(item.definition);
    AbilityData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<AbilityData>();
    abilities.Add(tmpData);
  }

  private void PopulateRelicDataFromInventoryItem(InventoryItem item)
  {
    // MyDebug.Log("Populating relic data for ", item.definition.displayName);
    relicsTypes.Add(item.definition);
    RelicData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<RelicData>();
    relics.Add(tmpData);
  }

  public List<ProjectileData> ParseThrowables(ItemList allItemsList)
  {
    if (allItemsList != null)
    {

      ItemList throwablesList = GameFoundationSdk.inventory.CreateList();

      allItemsList.Find(GameFoundationSdk.tags.Find(Keys.THROWABLES_TAG), throwablesList);

      foreach (InventoryItem item in throwablesList)
      {
        if (throwablesTypes.Contains(item.definition))
        {
          continue;
        }
        PopulateProjectileDataFromInventoryItem(item);
      }
    }
    return throwables;
  }

  private List<EmoteData> ParseEmotes(ItemList allItemsList)
  {
    if (allItemsList != null)
    {

      ItemList emotesList = GameFoundationSdk.inventory.CreateList();

      allItemsList.Find(GameFoundationSdk.tags.Find(Keys.EMOTES_TAG), emotesList);

      foreach (InventoryItem item in emotesList)
      {
        if (emoteTypes.Contains(item.definition))
        {
          continue;
        }
        PopulateEmoteDataFromInventoryItem(item);
      }
    }
    return emotes;
  }
  private List<RigData> ParseClothes(ItemList allItemsList)
  {
    if (allItemsList != null)
    {

      ItemList clothesList = GameFoundationSdk.inventory.CreateList();

      allItemsList.Find(GameFoundationSdk.tags.Find(Keys.CLOTHES_TAG), clothesList);

      foreach (InventoryItem item in clothesList)
      {
        if (clothesTypes.Contains(item.definition))
        {
          continue;
        }
        PopulateClothesDataFromInventoryItem(item);
      }
    }
    return clothes;
  }

  private List<PerkData> ParseActivePerks(ItemList allItemsList)
  {
    if (allItemsList != null)
    {

      ItemList perksList = GameFoundationSdk.inventory.CreateList();

      allItemsList.Find(GameFoundationSdk.tags.Find(Keys.PERKS_TAG), perksList);

      foreach (InventoryItem item in perksList)
      {
        if (activePerksTypes.Contains(item.definition))
        {
          continue;
        }
        PopulatePerksDataFromInventoryItem(item);
      }
    }
    return activePerks;
  }
  private List<AbilityData> ParseAbilities(ItemList allItemsList)
  {
    if (allItemsList != null)
    {

      ItemList abilityList = GameFoundationSdk.inventory.CreateList();

      allItemsList.Find(GameFoundationSdk.tags.Find(Keys.ABILITIES_TAG), abilityList);

      foreach (InventoryItem item in abilityList)
      {
        if (abilitiesTypes.Contains(item.definition))
        {
          continue;
        }
        PopulateAbilityDataFromInventoryItem(item);
      }
    }
    return abilities;
  }

  private List<RelicData> ParseRelics(ItemList allItemsList)
  {
    if (allItemsList != null)
    {

      ItemList relicList = GameFoundationSdk.inventory.CreateList();

      allItemsList.Find(GameFoundationSdk.tags.Find(Keys.RELICS_TAG), relicList);

      foreach (InventoryItem item in relicList)
      {
        if (relicsTypes.Contains(item.definition))
        {
          continue;
        }
        PopulateRelicDataFromInventoryItem(item);
      }
    }
    return relics;
  }
  #endregion

}