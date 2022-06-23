using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.GameFoundation;
using UnityEngine.UI;
using TMPro;
using GuruLaghima;
using Workbench.ProjectDilemma;

public class ActivePerksChooser : MonoBehaviour
{
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
    // fetch data about perks
    List<InventoryItemDefinition> allPerkDefinitions = new List<InventoryItemDefinition>();
    int count = GameFoundationSdk.catalog.FindItems(GameFoundationSdk.tags.Find("perks"), allPerkDefinitions);
    MyDebug.Log("Perks Count", count);

    foreach (InventoryItemDefinition item in allPerkDefinitions)
    {
      if (!card_1_Data)
        if (item.GetStaticProperty("ingame_ScriptableObject").AsAsset<ItemData>().Equipped)
        {

          card_1_Data = item.GetStaticProperty("ingame_ScriptableObject").AsAsset<ItemData>();
          MyDebug.Log("Equipped", item.key);
          continue;
        }
      if (!card_2_Data)
        if (item.GetStaticProperty("ingame_ScriptableObject").AsAsset<ItemData>().Equipped)
        {
          card_2_Data = item.GetStaticProperty("ingame_ScriptableObject").AsAsset<ItemData>();
          MyDebug.Log("Equipped", item.key);
          continue;
        }
      if (!card_3_Data)
        if (item.GetStaticProperty("ingame_ScriptableObject").AsAsset<ItemData>().Equipped)
        {
          card_3_Data = item.GetStaticProperty("ingame_ScriptableObject").AsAsset<ItemData>();
          MyDebug.Log("Equipped", item.key);
          break;
        }
    }

    if (card_1_Data)
    {
      card_1_Image.sprite = card_1_Data.ico;
      card_1_Desc.text = (string)card_1_Data.inventoryitemDefinition.GetStaticProperty("description");
      card_1_Title.text = (string)card_1_Data.inventoryitemDefinition.displayName;
      card_1_Button.onClick.AddListener(() =>
      {
        MyDebug.Log("Perks window activating perk 1");
        playerPerkFeature.ownedPerk = (PerkData)card_1_Data;
        playerPerkFeature.DisablePerkWindow();
        playerPerkFeature.HidePerk();
        playerPerkFeature.ActivatePerk();
      });
    }
    if (card_2_Data)
    {
      card_2_Image.sprite = card_2_Data.ico;
      card_2_Desc.text = (string)card_2_Data.inventoryitemDefinition.GetStaticProperty("description");
      card_2_Title.text = (string)card_2_Data.inventoryitemDefinition.displayName;
      card_2_Title.text = (string)card_2_Data.inventoryitemDefinition.displayName;
      card_2_Button.onClick.AddListener(() =>
      {
        MyDebug.Log("Perks window activating perk 2");
        playerPerkFeature.ownedPerk = (PerkData)card_2_Data;
        playerPerkFeature.DisablePerkWindow();
        playerPerkFeature.HidePerk();
        playerPerkFeature.ActivatePerk();
      });
    }
    if (card_3_Data)
    {
      card_3_Image.sprite = card_3_Data.ico;
      card_3_Desc.text = (string)card_3_Data.inventoryitemDefinition.GetStaticProperty("description");
      card_3_Title.text = (string)card_3_Data.inventoryitemDefinition.displayName;
      card_3_Title.text = (string)card_3_Data.inventoryitemDefinition.displayName;
      card_3_Button.onClick.AddListener(() =>
      {
        MyDebug.Log("Perks window activating perk 3");
        playerPerkFeature.ownedPerk = (PerkData)card_3_Data;
        playerPerkFeature.DisablePerkWindow();
        playerPerkFeature.HidePerk();
        playerPerkFeature.ActivatePerk();
      });
    }


  }

  // Update is called once per frame
  void Update()
  {

  }
}
