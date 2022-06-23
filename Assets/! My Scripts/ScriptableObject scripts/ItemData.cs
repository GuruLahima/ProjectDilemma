using UnityEngine;
using NaughtyAttributes;
using UnityEngine.GameFoundation;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Workbench/ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{

  [SerializeField] bool restoreValuesOnAwake = true;
  private void OnEnable()
  {
    if (restoreValuesOnAwake)
    {
      Equipped = false;
      AmountOwned = 0;
      ActivatedCardQuest = false;
    }
  }

  public InventoryItemDefinition inventoryitemDefinition;
  public string Key;
  public int AmountOwned = 0;
  public bool Equipped = false;
  public bool Owned => AmountOwned > 0;
  //public bool Owned = true; // fetch this data from server
  // for handling notifications (!) when new item is added
  public bool NewlyAdded = false;
  // for handling shop population, if item should not show up
  public bool ShowInShop = true;

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
