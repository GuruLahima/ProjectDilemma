using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuruLaghima;
using UnityEngine.GameFoundation;
using System;
using TMPro;
using UnityEngine.UI;
using Workbench.ProjectDilemma;
using UnityEngine.Events;

public class EquipModal : MonoBehaviour
{
  [SerializeField] Image itemImage;
  [SerializeField] TextMeshProUGUI itemTitle;
  [SerializeField] ItemData itemToEquip;
  [SerializeField] UnityEvent OnShowModal;


  public void ShowModal(ItemData newItem)
  {
    itemToEquip = newItem;
    itemTitle.text = itemToEquip.inventoryitemDefinition.displayName;
    itemImage.sprite = itemToEquip.inventoryitemDefinition.GetStaticProperty("inventory_icon").AsAsset<Sprite>();

    OnShowModal?.Invoke();
  }

  public void EquipItem()
  {
    AddToCharacter((RigData)itemToEquip);
    if (InventoryData.Instance)
      InventoryData.Instance.Equip(itemToEquip, true);
  }

  void AddToCharacter(RigData clothing)
  {
    if (CharacterCustomizationManager.Instance)
    {
      if (CharacterCustomizationManager.Instance.ActiveCharacterCustomization)
      {
        CharacterCustomizationManager.Instance.ActiveCharacterCustomization.AddClothing(clothing);
      }
    }
  }
}
