using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class OutfitItemUpgradePanel : MonoBehaviour
{
  public RigData currentTierItem;
  public RigData nextTierItem;
  public TextMeshProUGUI itemTypeLabel;
  public Image currentTierItemImage;
  public Image nextTierItemImage;
  public TextMeshProUGUI currentTierItemAmount;
  public TextMeshProUGUI nextTierItemRequiredAmount;
  public Button upgradeButton;

  public UnityEvent OnUpgrade;
}