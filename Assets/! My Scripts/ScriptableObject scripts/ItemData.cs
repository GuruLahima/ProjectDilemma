using UnityEngine;
using NaughtyAttributes;
using UnityEngine.GameFoundation;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Workbench/ScriptableObjects/ItemData", order = 1)]
public class ItemData : ScriptableObject
{
  public InventoryItemDefinition inventoryitemDefinition;
  public string Key;
  public int AmountOwned;
  public bool Equipped = false;
  public bool Owned = true; // fetch this data from server
  // for handling notifications (!) when new item is added
  public bool NewlyAdded = false;
  // for handling shop population, if item should not show up
  public bool ShowInShop = true;

}
