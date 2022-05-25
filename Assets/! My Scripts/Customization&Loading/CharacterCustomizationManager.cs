using GuruLaghima.ProjectDilemma;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
  public class CharacterCustomizationManager : MonoBehaviour
  {
    #region Singleton 
    public static CharacterCustomizationManager Instance;
    private void Awake()
    {
      Instance = this;
    }
    #endregion

    #region Public Fields
    /// <summary>
    /// To change the value call SetActiveCharacterCustomization() function
    /// </summary>
    [HideInInspector] public CharacterCustomization ActiveCharacterCustomization
    {
      get
      {
        return _characterCustomization;
      }
      private set
      {
        _characterCustomization = value;
      }
    }
    //this is only used for deep searches
     public List<ClothingPlaceholder> FullListOfClothingPlaceholders = new List<ClothingPlaceholder>();
    #endregion

    #region Exposed Private Fields
    [SerializeField] private List<CharacterCustomization> CharacterCustomizationsToUpdate = new List<CharacterCustomization>();
    #endregion

    #region Private Fields
    private CharacterCustomization _characterCustomization;
    #endregion

    private void Start()
    {
      RefreshUI();
    }

    #region Public Methods

    public void RefreshUI()
    {
      FullListOfClothingPlaceholders = new List<ClothingPlaceholder>();
      foreach (CharacterCustomization cc in CharacterCustomizationsToUpdate)
      {
        cc.RefreshPlaceholders();
        //cc.ReInitialize();
        //someArray = FullListOfClothingPlaceholders.ToArray();
        FullListOfClothingPlaceholders.AddRange(cc.clothingPlaceholders);
      }
    }

    public void SetActiveCharacterCustomization(CharacterCustomization cc)
    {
      if (ActiveCharacterCustomization)
      {
        ActiveCharacterCustomization.enabled = false;
      }
      if (cc)
      {
        ActiveCharacterCustomization = cc;
        ActiveCharacterCustomization.ReInitialize();
      }
    }
    #endregion

  }
}