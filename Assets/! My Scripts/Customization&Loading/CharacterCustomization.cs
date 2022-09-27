using GuruLaghima;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Workbench.ProjectDilemma
{
  public enum ClothingType : byte { Chest, Leggings, Boots, Mask, Gloves, Skin }
  /// <summary>
  /// CharacterCustomization is only used locally, it saves the character on a scriptable object that is later loaded in the game 
  /// </summary>
  public class CharacterCustomization : MonoBehaviour
  {
    #region Public Fields
    public UnityEvent OnClothesEquiped;
    public UnityEvent OnClothesUnequiped;
    public List<ClothingTree> Clothes = new List<ClothingTree>();
    #endregion

    #region Exposed Private Fields
    [SerializeField] private Transform rigRoot;
    [SerializeField] private ClothingData currentClothingData;
    [SerializeField] private bool setActiveOnEnable;
    [Header("Default clothes")]
    [SerializeField] RigData defaultMask;
    [SerializeField] RigData defaultChest;
    [SerializeField] RigData defaultGloves;
    [SerializeField] RigData defaultLegs;
    [SerializeField] RigData defaultShoes;
    [SerializeField] RigData defaultSkin;
    #endregion

    #region Private Fields
    #endregion

    #region MonoBehavior Callbacks
    private void Start()
    {
      ReInitialize();
    }
    private void OnEnable()
    {
      //if setActiveOnEnable is false then use the public methods instead
      if (setActiveOnEnable)
      {
        CharacterCustomizationManager.Instance.SetActiveCharacterCustomization(this);
      }
    }
    private void OnDisable()
    {
      //we always want to check and remove from active if the object is disabled
      if (CharacterCustomizationManager.Instance.ActiveCharacterCustomization == this)
      {
        CharacterCustomizationManager.Instance.SetActiveCharacterCustomization(null);
      }
    }
    #endregion

    #region Public Methods
    public void SetAsActiveCharacterCustomization()
    {
      CharacterCustomizationManager.Instance.SetActiveCharacterCustomization(this);
    }
    public void RemoveAsActiveCharacterCustomization()
    {
      if (CharacterCustomizationManager.Instance.ActiveCharacterCustomization == this)
      {
        CharacterCustomizationManager.Instance.SetActiveCharacterCustomization(null);
      }
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
        currentClothingData.SaveClothes(currentClothingData.currentClothing.Clothes);
      }
      foreach (ClothingTree ct in currentClothingData.Clothes)
      {
        var newTree = new ClothingTree(ct.Type);
        Clothes.Add(newTree);
        AddClothing(ct.Clothing);
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

    public void AddClothing(RigData rigToAdd)
    {
      if (rigToAdd == null)
      {
        MyDebug.Log("AddCLothing: Trying to add clothing with a null value");
        return;
      }

      ClothingTree tree = GetClothingOfType(rigToAdd.type);
      if (tree == null)
      {
        MyDebug.Log("AddCLothing:: tree is not null");
        tree = new ClothingTree(rigToAdd.type);
        Clothes.Add(tree);
      }
      if (tree.Clothing == rigToAdd)
      {
        MyDebug.Log("AddClothing:: tried equipping the same clothes");
        return;
      }
      tree.Clothing = rigToAdd;
      currentClothingData.SaveClothes(this.Clothes);
      OnClothesEquiped?.Invoke();
      GenerateCharacter();
    }
    public void RemoveClothing(RigData rigToRemove)
    {
      if (rigToRemove == null)
      {
        MyDebug.Log("Trying to remove clothing with a null value");
        return;
      }

      ClothingTree tree = GetClothingOfType(rigToRemove.type);
      if (tree == null)
      {
        tree = new ClothingTree(rigToRemove.type);
        Clothes.Add(tree);
      }
      if (tree.Clothing == rigToRemove)
      {
        MyDebug.Log("removing clothes of type ", rigToRemove.type.ToString());
        AddDefaultClothing(rigToRemove.type);
        return;
      }
      // // tree.Clothing = rigToRemove;
      // currentClothingData.SaveClothes(this.Clothes);
      // OnClothesEquiped?.Invoke();
      // GenerateCharacter();
    }

    private void AddDefaultClothing(ClothingType type)
    {
      switch (type)
      {
        case ClothingType.Mask:
          AddClothing(defaultMask);
          break;
        case ClothingType.Chest:
          AddClothing(defaultChest);
          break;
        case ClothingType.Gloves:
          AddClothing(defaultGloves);
          break;
        case ClothingType.Leggings:
          AddClothing(defaultLegs);
          break;
        case ClothingType.Boots:
          AddClothing(defaultShoes);
          break;
        case ClothingType.Skin:
          AddClothing(defaultSkin);
          break;
        default:
          break;
      }
    }
    #endregion

    #region Private Methods
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