using System.Collections;
using System.Collections.Generic;
using GuruLaghima.ProjectDilemma;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.GameFoundation;
using UnityEngine.UI;
using Workbench.ProjectDilemma;

public class OutfitsView : MonoBehaviour
{

  public Transform outfitsContainer;

  public UpgraderView upgraderView;

  public UnityEvent OnOutfitSelected;
  public Transform lastSelectedOutfit;

  List<Transform> outfits = new List<Transform>();

  #region exposed fields

  [SerializeField] UnityEvent OnItemSelected;
  [SerializeField] UnityEvent OnItemUpgraded;
  [SerializeField] UnityEvent OnUpgraderUnlocked;
  [SerializeField] GameObject emptyViewLabel;
  #endregion

  #region Monobehaviors

  private void Awake()
  {
    // get all outfits in a list
    if (outfitsContainer)
    {
      foreach (Transform child in outfitsContainer)
      {
        outfits.Add(child);
      }
    }
    else
    {
      foreach (Transform child in this.transform)
      {
        outfits.Add(child);
      }
    }

    // iterate through the list and set it up
    foreach (Transform outfit in outfits)
    {
      if (outfit.GetComponent<Button>())
      {
        // on click of outfit select it and invoke unity event that should call the UpgraderView
        outfit.GetComponent<Button>().onClick.AddListener(() =>
        {
          lastSelectedOutfit = outfit;
          OnOutfitSelected?.Invoke();
        });
      }
    }

    if (outfits.Count < 1)
    {
      emptyViewLabel.SetActive(true);
    }
  }

  private void Start()
  {

    // unlock upgrader only if unlocking level has been reached
    GameParameter starterPack = GameFoundationSdk.catalog.Find<GameParameter>(ProjectDilemmaCatalog.GameParameters.upgrader_unlock_level.key);
    bool unlocked = LauncherScript.CalcCurrentLevel() >= starterPack[ProjectDilemmaCatalog.GameParameters.upgrader_unlock_level.StaticProperties.unlock_level];
    if (unlocked)
    {
      OnUpgraderUnlocked?.Invoke();
    }
  }
  #endregion

  public void ShowUpgraderViewWithSelectedOutfit()
  {
    upgraderView.ShowOutfitTiers(lastSelectedOutfit.GetComponent<OutfitUICard>().itemSet);
  }
}
