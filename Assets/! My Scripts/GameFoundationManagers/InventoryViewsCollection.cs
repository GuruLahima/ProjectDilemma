using System.Collections;
using System.Collections.Generic;
using GuruLaghima;
using GuruLaghima.ProjectDilemma;
using UnityEngine;

public class InventoryViewsCollection : MonoBehaviour
{
  [SerializeField] List<InventoryView> inventoryViews = new List<InventoryView>();

  // the "New Item" notification for this collection
  public GameObject inventoryCollectionNotification;
  public GameObject inventoryCollectionNotificationAlternative;
  public InventoryViewsCollection parentCollection;

  public int newItems;

  public void UpdateNewItemsCount()
  {
    newItems = 0;
    foreach (InventoryView item in inventoryViews)
    {
      newItems += item.NewItems;
    }

    if (newItems <= 0)
    {
      if (inventoryCollectionNotification)
        inventoryCollectionNotification.SetActive(false);
      if (inventoryCollectionNotificationAlternative)
        inventoryCollectionNotificationAlternative.SetActive(false);
    }
    else if (newItems > 0)
    {
      if (inventoryCollectionNotification)
        inventoryCollectionNotification.SetActive(true);
      if (inventoryCollectionNotificationAlternative)
        inventoryCollectionNotificationAlternative.SetActive(true);
    }

    if (parentCollection)
    {
      parentCollection.UpdateNewItemsCount();
      MyDebug.Log("parentCollection " + parentCollection.name, parentCollection.newItems);

    }
  }

  // // Start is called before the first frame update
  // void Awake()
  // {
  //   inventoryCollectionNotification.SetActive(false);
  // }

}
