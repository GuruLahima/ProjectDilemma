using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.UI;
using TMPro;
using GuruLaghima;
using Workbench.ProjectDilemma;
using UnityEngine.EventSystems;

public class ActivePerksChooser : MonoBehaviour
{
  [SerializeField] RadialMenuData perksData;
  [SerializeField] ItemData card_1_Data;
  [SerializeField] ItemData card_2_Data;
  [SerializeField] ItemData card_3_Data;

  [SerializeField] Image card_1_Image;
  [SerializeField] TextMeshProUGUI card_1_Desc;
  [SerializeField] TextMeshProUGUI card_1_Title;
  [SerializeField] Button card_1_Button;
  [SerializeField] Image card_2_Image;
  [SerializeField] TextMeshProUGUI card_2_Desc;
  [SerializeField] TextMeshProUGUI card_2_Title;
  [SerializeField] Button card_2_Button;
  [SerializeField] Image card_3_Image;
  [SerializeField] TextMeshProUGUI card_3_Desc;
  [SerializeField] TextMeshProUGUI card_3_Title;
  [SerializeField] Button card_3_Button;

  public PerkActivator playerPerkFeature;

  // Start is called before the first frame update
  public void Init()
  {

    // fetch perk data from a scriptable object
    if (perksData)
    {
      foreach (PerkData item in perksData.orderedItems)
      {
        if (item)
        {
          if (!card_1_Data)
          {
            card_1_Data = item;
            MyDebug.Log("Equipped perk ", item.Name);
            continue;
          }
          if (!card_2_Data)
          {
            card_2_Data = item;
            MyDebug.Log("Equipped perk ", item.Name);
            continue;
          }
          if (!card_3_Data)
          {
            card_3_Data = item;
            MyDebug.Log("Equipped perk ", item.Name);
            break;
          }
        }
      }
    }

    PopulatePerkCardFromData(card_1_Image, card_1_Desc, card_1_Title, card_1_Button, card_1_Data);
    PopulatePerkCardFromData(card_2_Image, card_2_Desc, card_2_Title, card_2_Button, card_2_Data);
    PopulatePerkCardFromData(card_3_Image, card_3_Desc, card_3_Title, card_3_Button, card_3_Data);

  }

  void PopulatePerkCardFromData(Image cardIcon, TextMeshProUGUI cardDesc, TextMeshProUGUI cardTitle, Button cardButton, ItemData cardData)
  {
    if (cardData)
    {
      cardIcon.sprite = cardData.ico;
      cardDesc.text = (string)cardData.inventoryitemDefinition.GetStaticProperty("description");
      cardTitle.text = (string)cardData.inventoryitemDefinition.displayName;
      cardButton.GetComponent<EventTrigger>().enabled = true;
      cardButton.onClick.AddListener(() =>
      {
        playerPerkFeature.ownedPerk = (PerkData)cardData;
        playerPerkFeature.DisablePerkWindow();
        playerPerkFeature.HidePerk();
        playerPerkFeature.ActivatePerk();
      });
    }
    else
    {
      cardButton.GetComponent<EventTrigger>().enabled = false;
    }
  }

}
