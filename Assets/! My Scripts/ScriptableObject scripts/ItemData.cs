using UnityEngine;
using NaughtyAttributes;
using UnityEngine.GameFoundation;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Workbench/ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{

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
  // how much does this item give when recycled
  public ItemRarity Rarity;
  public float recyclingRewardAmount = 10f;


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
      Debug.Log((_activatedCardQuest ? "Added quest for: " : "Removed quest for: ") + (RewardData != null ? RewardData.name : "empty"));
    }
  }
  [Foldout("Earning this item")]
  [SerializeField] private bool _activatedCardQuest;
  [Foldout("Earning this item")]
  public RewardData RewardData;
}

public enum ItemRarity : byte { None = default, Common = 0, Uncommon = 1, Rare = 2, VeryRare = 3, Epic = 4, Legendary = 5, Unique = 6 }