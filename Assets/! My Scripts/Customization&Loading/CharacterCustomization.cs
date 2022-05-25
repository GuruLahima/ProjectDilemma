using GuruLaghima;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Workbench.ProjectDilemma
{
  public enum ClothingType : byte { Chest, Leggings, Boots, Mask, Gloves }
  /// <summary>
  /// CharacterCustomization is only used locally, it saves the character on a scriptable object that is later loaded in the game 
  /// </summary>
  public class CharacterCustomization : MonoBehaviour
  {
    #region Public Fields
    public UnityEvent OnClothesEquiped;
    public UnityEvent OnClothesUnequiped;
    public List<ClothingTree> Clothes = new List<ClothingTree>();
    public List<ClothingPlaceholder> clothingPlaceholders = new List<ClothingPlaceholder>();
    #endregion

    #region Exposed Private Fields
    [SerializeField] private Transform rigRoot;
    [SerializeField] private ClothingData currentClothingData;
    [SerializeField] private bool setActiveOnEnable;
    #endregion

    #region Private Fields
    private Dictionary<ClothingType, ClothingPlaceholder> placeholdersAllocation = new Dictionary<ClothingType, ClothingPlaceholder>();
    #endregion

    #region MonoBehavior Callbacks
    private void Start()
    {
      RefreshPlaceholders();
      ReInitialize();
    }
    private void OnEnable()
    {
      if (setActiveOnEnable)
      {
        CharacterCustomizationManager.Instance.SetActiveCharacterCustomization(this);
      }
    }
    private void OnDisable()
    {
      if (CharacterCustomizationManager.Instance.ActiveCharacterCustomization == this)
      {
        CharacterCustomizationManager.Instance.SetActiveCharacterCustomization(null);
      }
    }
    #endregion

    #region Public Methods
    public void RefreshPlaceholders()
    {
      clothingPlaceholders = new List<ClothingPlaceholder>();
      clothingPlaceholders = GetComponentsInChildren<ClothingPlaceholder>(true).ToList();
    }
    public void ReInitialize()
    {
      foreach (ClothingTree ct in Clothes)
      {
        DestroyClothing(ct);
      }
      Clothes = new List<ClothingTree>();
      if (currentClothingData.IsPreviewClothing)
      {
        currentClothingData.SaveClothes(currentClothingData.defaultClothing.Clothes);
      }
      foreach (ClothingTree ct in currentClothingData.Clothes)
      {
        var newTree = new ClothingTree(ct.Type);
        Clothes.Add(newTree);
      }

      // we search for the placeholder with a rig matching our equiped rig
      foreach (ClothingTree ct in currentClothingData.Clothes)
      {
        // first we search local placeholders i.e placeholders that are child/grandchild objects of this gameObject
        bool found = false;
        foreach (ClothingPlaceholder cp in clothingPlaceholders)
        {
          if (ct.Clothing == cp.Clothing)
          {
            MyDebug.Log("Clothing placeholder found in the local list of clothing placeholders!", Color.green);
            // if we find the object we stop the search and set found = true;
            AddClothing(cp.Clothing, cp);
            found = true;
            break;
          }
        }

        if (!found)
        {
          MyDebug.Log("Error: No matching clothing placeholder for " + ct
            + " on this character customization :: " + this.name, Color.red);
          MyDebug.Log("Doing a deep search on the CC manager...", Color.cyan);
          // if we fail to find the object in the current branch, we initialize a deep search
          foreach (ClothingPlaceholder cp in CharacterCustomizationManager.Instance.FullListOfClothingPlaceholders)
          {
            if (ct.Clothing == cp.Clothing)
            {
              MyDebug.Log("Clothing placeholder found in the full list of clothing placeholders!", Color.green);
              AddClothing(cp.Clothing, cp);
              found = true;
              break;
            }
          }
        }

        // lastly if we didnt find the item anywhere, we throw an error
        if (!found)
        {
          MyDebug.Log("Error: clothing placeholder could not be found in the full list of placeholders", Color.red);
        }
      }
    }
    public ClothingTree GetClothingOfType(ClothingType type)
    {
      foreach (ClothingTree ct in Clothes)
      {
        if (ct.Type == type)
        {
          return ct;
        }
      }
      return null;
    }
    public void GenerateCharacter()
    {
      foreach (ClothingTree ct in Clothes)
      {
        if (ct.RiggableSkin != null)
        {
          if (ct.RiggableSkin.rig != ct.Clothing)
          {
            Destroy(ct.RiggableSkin.gameObject);
            if (ct.Clothing)
            {
              ct.RiggableSkin = Instantiate(ct.Clothing.SkinPrefab, rigRoot.parent);
              Rig.ReassignBones(rigRoot, ct.Clothing, ct.RiggableSkin.skinMeshRenderer);
              if (ct.RiggableSkin.root)
              {
                Destroy(ct.RiggableSkin.root.gameObject);
              }
              ct.RiggableSkin.root = null;
            }
            else
            {
              MyDebug.Log("No clothes are currently equiped!", Color.yellow);
            }
          }
          else
          {
            MyDebug.Log("No changes were made to this item", Color.yellow);
          }
        }
        else
        {
          if (ct.Clothing)
          {
            ct.RiggableSkin = Instantiate(ct.Clothing.SkinPrefab, rigRoot.parent);
            Rig.ReassignBones(rigRoot, ct.Clothing, ct.RiggableSkin.skinMeshRenderer);
            if (ct.RiggableSkin.root)
            {
              Destroy(ct.RiggableSkin.root.gameObject);
            }
            ct.RiggableSkin.root = null;
          }
          else
          {
            MyDebug.Log("No clothes are currently equiped!", Color.yellow);
          }
        }
      }
    }

    public void AddClothing(RigData rigToAdd, ClothingPlaceholder cp)
    {
      if (rigToAdd == null) return;
      if (cp)
      {
        if (placeholdersAllocation.ContainsKey(rigToAdd.type))
        {
          if (placeholdersAllocation[rigToAdd.type] != null)
          {
            // nothing happens when we press on the same item
            if (placeholdersAllocation[rigToAdd.type] != cp)
            {
              placeholdersAllocation[rigToAdd.type].Deselect();
              placeholdersAllocation[rigToAdd.type] = cp;
              cp.Select();
            }
          }
          else
          {

          }
        }
        else
        {
          placeholdersAllocation.Add(rigToAdd.type, cp);
          cp.Select();
        }
      }

      ClothingTree tree = GetClothingOfType(rigToAdd.type);
      if (tree == null)
      {
        tree = new ClothingTree(rigToAdd.type);
        Clothes.Add(tree);
      }
      if (tree.Clothing == rigToAdd)
      {
        // we return the function, since to unequip you need to simply select "None"
        //DEPRECATED
        // if players tries to equip the same piece after being equiped, remove that piece instead
        //RemoveClothing(tree);
        return;
      }
      tree.Clothing = rigToAdd;
      currentClothingData.SaveClothes(this.Clothes);
      OnClothesEquiped?.Invoke();
      GenerateCharacter();
    }
    #endregion

    #region Private Methods

    private void RemoveClothing(ClothingTree tree)
    {
      tree.Clothing = null;
      if (tree.RiggableSkin)
      {
        Destroy(tree.RiggableSkin.gameObject);
      }
      tree.RiggableSkin = null;
      currentClothingData.SaveClothes(this.Clothes);
      OnClothesUnequiped?.Invoke();
      GenerateCharacter();
    }

    private void DestroyClothing(ClothingTree tree)
    {
      tree.Clothing = null;
      if (tree.RiggableSkin)
      {
        Destroy(tree.RiggableSkin.gameObject);
        tree.RiggableSkin = null;
      }
    }
    #endregion
  }


  [System.Serializable]
  public class ClothingTree
  {
    public ClothingTree(ClothingType _type, RigData _clothing = null, RiggableSkin _riggableSkin = null)
    {
      this.Type = _type;
      this.Clothing = _clothing;
      this.RiggableSkin = _riggableSkin;
    }
    public ClothingType Type;
    public RigData Clothing;
    public RiggableSkin RiggableSkin;
  }
}