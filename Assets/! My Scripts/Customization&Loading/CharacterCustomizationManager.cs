using GuruLaghima;
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
      SetActiveCharacterCustomization(_defaultCharacterCustomization);
    }
    #endregion

    #region Public Fields
    /// <summary>
    /// To change the value call SetActiveCharacterCustomization() function
    /// </summary>
    [HideInInspector]
    public CharacterCustomization ActiveCharacterCustomization
    {
      get
      {
        return _characterCustomization;
      }
      private set
      {
        _characterCustomization = value;
        MyDebug.Log("Current active CharacterCustomization is: " + (_characterCustomization ? _characterCustomization.gameObject.name : "none"), Color.yellow);
      }
    }
    #endregion

    #region Exposed Private Fields
    [CustomTooltip("The CharacterCustomization that becomes active upon start")]
    [SerializeField] CharacterCustomization _defaultCharacterCustomization;
    #endregion

    #region Private Fields
    private CharacterCustomization _characterCustomization;
    #endregion

    #region Public Methods
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