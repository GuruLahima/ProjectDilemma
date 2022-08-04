using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using GuruLaghima;

namespace Workbench.ProjectDilemma
{
  public class ClothingPlaceholder : MonoBehaviour
  {
    public RigData Clothing;
    public void AddToCharacter()
    {
      MyDebug.Log("AddCLothing::AddToCharacter called for", this.Clothing.SkinPrefab.name);
      if (CharacterCustomizationManager.Instance)
      {
        // MyDebug.Log("AddCLothing:: CharacterCustomizationManager.Instance present");
        if (CharacterCustomizationManager.Instance.ActiveCharacterCustomization)
        {
          // MyDebug.Log("AddCLothing:: ActiveCharacterCustomization present");
          CharacterCustomizationManager.Instance.ActiveCharacterCustomization.AddClothing(this.Clothing);
        }
      }
    }
  }
}
