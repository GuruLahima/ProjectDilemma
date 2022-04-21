using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClothingPlaceholder : MonoBehaviour,  IPointerClickHandler
{
  public RigData Clothing;

  public void OnPointerClick(PointerEventData eventData)
  {
    if (CharacterCustomization.Instance)
    {
      CharacterCustomization.Instance.AddClothing(this.Clothing);
    }
  }

  private void OnMouseDown()
  {
    if (CharacterCustomization.Instance)
    {
      CharacterCustomization.Instance.AddClothing(this.Clothing);
    }
  }
}
