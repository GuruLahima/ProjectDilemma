using UnityEngine;
using NaughtyAttributes;
using UnityEngine.GameFoundation;
using Workbench.ProjectDilemma;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Workbench/ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
#if UNITY_EDITOR
  [SerializeField] bool restoreValuesOnAwake = true;
  private void Awake()
  {
    if (restoreValuesOnAwake)
    {
      Equipped = false;
      Owned = false;
    }
  }
#endif

  public InventoryItemDefinition inventoryitemDefinition;
  public string Key;
  public int AmountOwned;
  public bool Equipped = false;
  public bool Owned = true; // fetch this data from server
  // for handling notifications (!) when new item is added
  public bool NewlyAdded = false;
  // for handling shop population, if item should not show up
  public bool ShowInShop = true;

  [ShowAssetPreview]
  public Sprite ico;

  //
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
  [HorizontalLine]
  [SerializeField] private bool _activatedCardQuest;
  public RewardData RewardData;
}
