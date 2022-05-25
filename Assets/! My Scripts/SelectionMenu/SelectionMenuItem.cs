using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class SelectionMenuItem : SelectionMenuContainer, IPointerEnterHandler, IPointerExitHandler
{
  public int Index 
  { 
    get
    {
      return transform.GetSiblingIndex();
    }
  }
  //can only have one master at a given time
  public SelectionMenu Master;
  public void Set(SelectionMenu master)
  {
    this.Master = master;
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    Master.OnTriggerEnterChild(Index);
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    Master.OnTriggerExitChild(Index);
  }
}
