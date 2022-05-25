using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Workbench.ProjectDilemma
{
  public class ClothingPlaceholder : MonoBehaviour, IPointerClickHandler
  {
    public RigData Clothing;
    public UnityEvent OnSelected;
    public UnityEvent OnDeselected;

    public void Select()
    {
      OnSelected?.Invoke();
    }
    public void Deselect()
    {
      OnDeselected?.Invoke();
    }

    private void OnEnable()
    {
      if (CharacterCustomizationManager.Instance)
      {
        CharacterCustomizationManager.Instance.RefreshUI();
      }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
      if (CharacterCustomizationManager.Instance)
      {
        if (CharacterCustomizationManager.Instance.ActiveCharacterCustomization)
        {
          CharacterCustomizationManager.Instance.ActiveCharacterCustomization.AddClothing(this.Clothing, this);
        }
      }
    }
  }
}
