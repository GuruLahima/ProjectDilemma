using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Workbench.ProjectDilemma
{
  /// <summary>
  /// Handles all perk/objective related stuff
  /// </summary>
  public class PerkActivator : BaseActivatorComponent
  {
    #region Active Modifiers (contains public methods)
    /// <summary>
    /// Dictionary storing predefined string keys from PerkKeys class and BonusModifiers value - list of object values
    /// <para>To Add, Remove or Get a specific key/value, use the AddModifier(), GetModifier() or RemoveModifier() functions below</para>
    /// </summary>
    public Dictionary<string, BonusModifiers> activeModifiers = new Dictionary<string, BonusModifiers>();
    /// <summary>
    /// Class used exclusively by OperatePerk to store list of object values inside a dictionary 
    /// </summary>
    public class BonusModifiers
    {
      public List<object> Mods = new List<object>();
      public BonusModifiers(object value)
      {
        this.Mods.Add(value);
      }
      public void AddValue(object value)
      {
        this.Mods.Add(value);
      }
      public void RemoveValue(object value)
      {
        if (this.Mods.Contains(value))
        {
          this.Mods.Remove(value);
        }
      }
    }
    public void AddModifier(string key, object value)
    {
      if (!activeModifiers.ContainsKey(key))
      {
        activeModifiers.Add(key, new BonusModifiers(value));
      }
      else if (activeModifiers.ContainsKey(key))
      {
        activeModifiers[key].AddValue(value);
      }
    }
    public object GetModifier(string key)
    {
      if (activeModifiers.ContainsKey(key))
      {
        object[] values = new object[activeModifiers[key].Mods.Count];
        for (int i = 0; i < activeModifiers[key].Mods.Count; i++)
        {
          values[i] = activeModifiers[key].Mods[i];
        }
        return values;
      }
      return null;
    }
    public bool RemoveModifier(string key)
    {
      if (activeModifiers.ContainsKey(key))
      {
        activeModifiers.Remove(key);
        return true;
      }
      else
      {
        return false;
      }
    }
    public float CalculateModifierBonuses(string key)
    {
      float totalValue = 0;
      object[] values = (object[])GetModifier(key);
      if (values != null)
      {
        foreach (object val in values)
        {
          if (val is float @float)
          {
            totalValue += @float;
          }
        }
      }
      return totalValue;
    }
    public bool CheckModifierBonuses(string key)
    {
      object[] values = (object[])GetModifier(key);
      if (values != null)
      {
        foreach (object val in values)
        {
          if (val is bool @bool)
          {
            if (@bool)
            {
              return @bool;
            }
          }
        }
      }
      return false;
    }
    #endregion

    #region Exposed Private Fields
    [SerializeField] Transform perkSlot;
    [SerializeField] private PerkData ownedPerk;
    #endregion

    #region Private Fields
    private ObjectiveBase activeQuest;
    private bool bonusAdded = false;
    private bool perkActivated = false;
    #endregion

    #region Public Methods
    public override void Init()
    {
      if (ownedPerk.Icon && perkSlot)
      {
        var ico = Instantiate(ownedPerk.Icon, perkSlot);
        ico.container = ownedPerk;
        if (ownedPerk.ico)
        {
          ico.image.sprite = ownedPerk.ico;
        }
      }
    }
    public void ShowPerk()
    {
      ActiveState = true;
      CursorManager.SetLockMode(CursorLockMode.Confined);
      CursorManager.SetVisibility(true);
    }
    public void HidePerk()
    {
      ActiveState = false;
      CursorManager.SetLockMode(CursorLockMode.Locked);
      CursorManager.SetVisibility(false);
    }
    public void ActivatePerk()
    {
      if (perkActivated)
        return;

      #region LOCAL STUFF
      perkActivated = true;
      activeQuest = Instantiate(ownedPerk.quest);
      #endregion

      #region INTERNET STUFF
      if (PhotonNetwork.IsConnected)
      {
        int viewID = GameMechanic.Instance.localPlayerSpot.GetComponent<PhotonView>().ViewID;
        RPCManager.Instance.photonView.RPC("RPC_PerkActivated", RpcTarget.AllViaServer, viewID);
      }
      else
      {
        SYNC_ActivatePerk();
      }
      #endregion

      Debug.Log("Perk " + activeQuest + " has been activated");
    }
    /// <summary>
    /// This method is invoked through an RPC call
    /// <para>Here we can initialize stuff that we want both players to see ex: animations</para>
    /// </summary>
    public void SYNC_ActivatePerk()
    {
      Debug.Log("RPC TEST PERK ACTIVATION");
    }
    public void AddPerkBonuses(ObjectiveCondition condition)
    {
      if (bonusAdded == true)
      {
        return;
      }
      bonusAdded = true;

      PerkData.PerkBonusModifiers[] mod = ownedPerk.GetPerkModifiers(condition);
      foreach (PerkData.PerkBonusModifiers pm in mod)
      {
        AddModifier(pm.ModifierKey, pm.GetValue());
      }
    }
    #endregion

    #region Private Methods

    #endregion
  }
  /// <summary>
  /// PerkKeys is a class defining custom string keys used as bonus modifiers
  /// <para>To define a new custom key, simply create a new public const string and add it to the array CompleteListOfKeys</para>
  /// <para>Note: keys with matching string values will not show up in the PerkKey property</para>
  /// </summary>
  public static class PerkKeys
  {
    // standard formula for calculating points would look like
    // x where x is the base amount of points
    // x + bonus% + bonus(flat)

    /// <summary>
    /// Used by the PerkKey property to display a dropdown menu of the existing keys
    /// </summary>
    public static string[] CompleteListOfKeys =
    {
      PERCENT_BONUS_EXPERIENCE,
      PERCENT_BONUS_POINTS,
      PERCENT_BONUS_COINS,
      FLAT_BONUS_EXPERIENCE,
      FLAT_BONUS_POINTS,
      FLAT_BONUS_COINS,
      IMMUNITY_TO_LOSS,
      MULTIPLIER_BONUS_CARDS,
      RARITY_IMPROVEMENT_CARDS
    };
    public const string PERCENT_BONUS_EXPERIENCE = "percentBonusExperience";
    public const string PERCENT_BONUS_POINTS = "percentBonusPoints";
    public const string PERCENT_BONUS_COINS = "percentBonusCoins";
    public const string FLAT_BONUS_EXPERIENCE = "flatBonusExperience";
    public const string FLAT_BONUS_POINTS = "flatBonusPoints";
    public const string FLAT_BONUS_COINS = "flatBonusCoins";
    public const string IMMUNITY_TO_LOSS = "immunityToLoss";
    public const string MULTIPLIER_BONUS_CARDS = "mutliplierBonusCards";
    public const string RARITY_IMPROVEMENT_CARDS = "rarityImprovementCards";
  }
  public static class GameEvents
  {
    public static System.Action OnBothPlayerLoaded;
    public static System.Action<Choice> OnLocalPlayerVoted;
    public static System.Action<Choice> OnOtherPlayerVoted;
    public static System.Action<Choice,Choice> OnBothPlayersVoted;
    public static System.Action<Outcome> OnOutcome;
  }
}