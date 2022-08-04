using UnityEngine;
using NaughtyAttributes;
using UnityEngine.GameFoundation;
using Workbench.ProjectDilemma;
using System.Collections.Generic;
using GuruLaghima.ProjectDilemma;

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
    }
  }
  [HorizontalLine]
  public InventoryItemDefinition inventoryitemDefinition;
  public string Key;
  [HorizontalLine]
  public int AmountOwned = 0;
  public bool Owned => AmountOwned > 0;
  [HorizontalLine]
  public bool Equipped = false;
  // for handling notifications (!) when new item is added
  public bool NewlyAdded = false;
  // for handling notifications (!) when new item is added
  public bool NewlyAddedInBook = true;
  // for handling shop population, if item should not show up
  public bool ShowInShop = true;
  [HorizontalLine]
  public ItemRarity Rarity;
  // how much does this item give when recycled
  public float recyclingRewardAmount = 10f;
  public int amountNeededTorUpgrade;

  public List<InventoryView> inventoriesUsingThisItem = new List<InventoryView>();


  [ShowAssetPreview]
  public Sprite ico;

  //----------------earning this item via quest-------------------------//
  public bool ActivatedCardQuest
  {
    get
    {
      return _activatedCardQuest;
    }
    set
    {
      _activatedCardQuest = value;
      // Debug.Log((_activatedCardQuest ? "Added quest for: " : "Removed quest for: ") + (RewardData != null ? RewardData.name : "empty"));
    }
  }
  [Foldout("Earning this item")]
  [SerializeField] private bool _activatedCardQuest;
  [Foldout("Earning this item")]
  public RewardData RewardData;
}

public enum ItemRarity : byte { Common = 0, Rare = 1, Epic = 2, Legendary = 3, Unique = 4}