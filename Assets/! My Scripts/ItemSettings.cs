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
public class ItemSettings : SingletonScriptableObject<ItemSettings>
{

  public enum ItemType
  {
    Emotes,
    Clothes,
    Throwables,
    ActivePerks,
    Abilities

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
  #endregion

  #region private fields
  private List<InventoryItemDefinition> throwablesTypes = new List<InventoryItemDefinition>();
  private List<InventoryItemDefinition> emoteTypes = new List<InventoryItemDefinition>();
  private List<InventoryItemDefinition> clothesTypes = new List<InventoryItemDefinition>();
  private List<InventoryItemDefinition> allTypes = new List<InventoryItemDefinition>();
  private List<InventoryItemDefinition> activePerksTypes = new List<InventoryItemDefinition>();
  private List<InventoryItemDefinition> abilitiesTypes = new List<InventoryItemDefinition>();
  #endregion

  #region 
  public void Equip(ItemData item, bool equip)
  {
    MyDebug.Log("Equipping ", item.Key + ": " + equip);
    if (allTypes.Contains(item.inventoryitemDefinition))
    {
      MyDebug.Log("in singleton");
      ItemList allItemsOfThisType = GameFoundationSdk.inventory.CreateList();
      GameFoundationSdk.inventory.FindItems(item.inventoryitemDefinition, allItemsOfThisType);
      foreach (InventoryItem itm in allItemsOfThisType)
      {

        MyDebug.Log("in GFSDK inventory");
        itm.SetMutableProperty(Keys.ITEMPROPERTY_EQUIPPED, equip);
      }
      item.Equipped = equip;
    }

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
    }
  }

  private void PopulateItemDataFromInventoryItem(InventoryItem item)
  {
    // 
    if (allTypes.Contains(item.definition))
    {
      MyDebug.Log("Adding quantity to ", item.definition.displayName);

      // add the quantity of this item to the ProjectileData representation for this item type
      ItemData proj = allItems.Find((obj) => { return obj.inventoryitemDefinition == item.definition; });
      proj.AmountOwned += 1;
      // dont add another entry in the allItems list
      return;
    }
    MyDebug.Log("Populating item data for ", item.definition.displayName);


    allTypes.Add(item.definition);
    ItemData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<ItemData>();
    if (tmpData)
    {
      tmpData.inventoryitemDefinition = item.definition;
      tmpData.AmountOwned = 1;
    }
    allItems.Add(tmpData);

  }

  public List<ProjectileData> ParseThrowables(ItemList allItemsList)
  {
    if (allItemsList != null)
    {

      ItemList throwablesList = GameFoundationSdk.inventory.CreateList();
      // throwablesTypes.Clear();

      allItemsList.Find(GameFoundationSdk.tags.Find(Keys.THROWABLES_TAG), throwablesList);

      foreach (InventoryItem item in throwablesList)
      {
        PopulateProjectileDataFromInventoryItem(item);
      }
    }
    return throwables;
  }

  #region 

  private void PopulateProjectileDataFromInventoryItem(InventoryItem item)
  {
    // 
    if (throwablesTypes.Contains(item.definition))
    {
      MyDebug.Log("Adding quantity to ", item.definition.displayName);

      // add the quantity of this item to the ProjectileData representation for this item type
      ProjectileData proj = throwables.Find((obj) => { return obj.inventoryitemDefinition == item.definition; });
      proj.AmountOwned += 1;
      // dont add another entry in the throwables list
      return;
    }
    MyDebug.Log("Populating projectile data for ", item.definition.displayName);

    throwablesTypes.Add(item.definition);
    ProjectileData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<ProjectileData>();
    tmpData.inventoryitemDefinition = item.definition;
    tmpData.AmountOwned = 1;
    throwables.Add(tmpData);

  }
  private void PopulateEmoteDataFromInventoryItem(InventoryItem item)
  {
    // 
    if (emoteTypes.Contains(item.definition))
    {
      MyDebug.Log("Adding quantity to ", item.definition.displayName);

      // add the quantity of this item to the Emotedata representation for this item type
      EmoteData emote = emotes.Find((obj) => { return obj.inventoryitemDefinition == item.definition; });
      emote.AmountOwned += 1;
      // dont add another entry in the throwables list
      return;
    }
    MyDebug.Log("Populating emote data for ", item.definition.displayName);

    emoteTypes.Add(item.definition);
    EmoteData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<EmoteData>();
    tmpData.inventoryitemDefinition = item.definition;
    tmpData.AmountOwned = 1;
    emotes.Add(tmpData);

  }

  private void PopulateClothesDataFromInventoryItem(InventoryItem item)
  {
    // 
    if (clothesTypes.Contains(item.definition))
    {
      MyDebug.Log("Adding quantity to ", item.definition.displayName);

      // add the quantity of this item to the Emotedata representation for this item type
      RigData clothing = clothes.Find((obj) => { return obj.inventoryitemDefinition == item.definition; });
      clothing.AmountOwned += 1;
      // dont add another entry in the throwables list
      return;
    }
    MyDebug.Log("Populating clothe data for ", item.definition.displayName);

    clothesTypes.Add(item.definition);
    RigData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<RigData>();
    tmpData.inventoryitemDefinition = item.definition;
    tmpData.AmountOwned = 1;
    clothes.Add(tmpData);

  }

  private void PopulatePerksDataFromInventoryItem(InventoryItem item)
  {
    // 
    if (activePerksTypes.Contains(item.definition))
    {
      MyDebug.Log("Adding quantity to ", item.definition.displayName);

      // add the quantity of this item to the perkdata representation for this item type
      PerkData perk = activePerks.Find((obj) => { return obj.inventoryitemDefinition == item.definition; });
      perk.AmountOwned += 1;
      // dont add another entry in the perks list
      return;
    }
    MyDebug.Log("Populating perk data for ", item.definition.displayName);

    activePerksTypes.Add(item.definition);
    PerkData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<PerkData>();
    tmpData.inventoryitemDefinition = item.definition;
    tmpData.AmountOwned = 1;
    activePerks.Add(tmpData);
  }

  private void PopulateAbilityDataFromInventoryItem(InventoryItem item)
  {
    // 
    if (abilitiesTypes.Contains(item.definition))
    {
      MyDebug.Log("Adding quantity to ", item.definition.displayName);

      // add the quantity of this item to the perkdata representation for this item type
      AbilityData ability = abilities.Find((obj) => { return obj.inventoryitemDefinition == item.definition; });
      ability.AmountOwned += 1;
      // dont add another entry in the perks list
      return;
    }
    MyDebug.Log("Populating ability data for ", item.definition.displayName);

    abilitiesTypes.Add(item.definition);
    AbilityData tmpData = item.definition.GetStaticProperty(Keys.ITEMPROPERTY_INGAMESCRIPTABLEOBJECT).AsAsset<AbilityData>();
    tmpData.inventoryitemDefinition = item.definition;
    tmpData.AmountOwned = 1;
    abilities.Add(tmpData);
  }

  private List<EmoteData> ParseEmotes(ItemList allItemsList)
  {
    if (allItemsList != null)
    {

      ItemList emotesList = GameFoundationSdk.inventory.CreateList();
      // throwablesTypes.Clear();

      allItemsList.Find(GameFoundationSdk.tags.Find(Keys.EMOTES_TAG), emotesList);

      foreach (InventoryItem item in emotesList)
      {
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
      // throwablesTypes.Clear();

      allItemsList.Find(GameFoundationSdk.tags.Find(Keys.CLOTHES_TAG), clothesList);

      foreach (InventoryItem item in clothesList)
      {
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
      // throwablesTypes.Clear();

      allItemsList.Find(GameFoundationSdk.tags.Find(Keys.PERKS_TAG), perksList);

      foreach (InventoryItem item in perksList)
      {
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
      // throwablesTypes.Clear();

      allItemsList.Find(GameFoundationSdk.tags.Find(Keys.ABILITIES_TAG), abilityList);

      foreach (InventoryItem item in abilityList)
      {
        PopulateAbilityDataFromInventoryItem(item);
      }
    }
    return abilities;
  }
  #endregion

}