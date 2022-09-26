using UnityEngine;
using NaughtyAttributes;
using UnityEngine.GameFoundation;
using Workbench.ProjectDilemma;
using System.Collections.Generic;
using GuruLaghima.ProjectDilemma;
using GuruLaghima;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Workbench/ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
  [Header("Item data")]

  [SerializeField] protected bool restoreValuesOnAwake = true;
  public virtual void OnEnable()
  {
    if (restoreValuesOnAwake)
    {
      Equipped = false;
      AmountOwned = 0;
      ActivatedCardQuest = false;
      NewlyAddedInBook = true;
      NewlyAdded = true;
    }
  }


  [Header("Mutable data")]
  [HorizontalLine]
  public bool Equipped = false;
  // for handling notifications (!) when new item is added
  public bool NewlyAdded = false;
  // for handling notifications (!) when new item is added
  public bool NewlyAddedInBook = true;
  // for handling shop population, if item should not show up
  public int AmountOwned = 0;
  public bool Owned => AmountOwned > 0;
  [Header("Constant data")]
  [HorizontalLine]
  public string Name;
  public string Key;
  public InventoryItemDefinition inventoryitemDefinition;
  public ItemRarity Rarity;
  [CustomTooltip("how much does this item give when recycled")]
  public float recyclingRewardAmount = 10f;
  public int amountNeededTorUpgrade;
  [CustomTooltip("can we buy this item")]
  public bool ShowInShop = true;
  [CustomTooltip("can we combine this item with something else")]
  public bool ShowInCombiner = true;
  [CustomTooltip("can we recycle this item")]
  public bool ShowInRecycler = true;
  [CustomTooltip("Is this item one of the default items given to player from the very start (should be only one per category. e.g. one mask, one pants, one shoes etc)")]
  public bool isDefaultItem = false;

  public List<InventoryView> inventoriesUsingThisItem = new List<InventoryView>();


  [ShowAssetPreview]
  public Sprite ico;

  //----------------earning this item via quest-------------------------//
  [Foldout("Earning this item")]
  public bool ActivatedCardQuest;
  [Foldout("Earning this item")]
  public RewardData RewardData;
}

public enum ItemRarity : byte { Common = 0, Rare = 1, Epic = 2, Legendary = 3, Unique = 4 }