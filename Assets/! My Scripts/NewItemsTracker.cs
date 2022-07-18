using System.Collections;
using System.Collections.Generic;
using GuruLaghima;
using UnityEngine;

public class NewItemsTracker : MonoBehaviour
{
  [CustomTooltip("the New Item notification for this inventory")]
  public GameObject inventoryNotification;
  [CustomTooltip("// the collection that tracks the new items in this inventory for notification purposes")]
  public NewItemsTracker parentCollection;

  int newItems;

  public int NewItems
  {
    get
    {
      return newItems;
    }
    set
    {
      if (value <= 0)
      {
        if (inventoryNotification)
          inventoryNotification.SetActive(false);
      }
      else if (value > 0 && newItems <= 0)
      {
        if (inventoryNotification)
          inventoryNotification.SetActive(true);
      }

      if (parentCollection)
      {
        parentCollection.NewItems += value - newItems;
      }

      newItems = value;

    }
  }
}
