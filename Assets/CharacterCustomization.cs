using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClothingType : byte { Chest, Leggings, Boots, Mask, Gloves }
public class CharacterCustomization : MonoBehaviour
{
  #region Singleton
  public static CharacterCustomization Instance;
  private void Awake()
  {
    Instance = this;
  }
  #endregion

  private void Start()
  {
    Clothes = currentClothingData.Clothes;
    GenerateCharacter();
  }

  public List<ClothingTree> Clothes = new List<ClothingTree>();

  //rigRoot should be assigned in a different way
  public Transform rigRoot;
  public GameObject localCharacter;
  public ClothingData currentClothingData;

  public List<RigData> allExistingSkins = new List<RigData>();
  public int GetIndexOfSkin(RigData rigData)
  {
    int index = allExistingSkins.IndexOf(rigData);
    return index;
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
            ct.RiggableSkin = Instantiate(ct.Clothing.SkinPrefab, localCharacter.transform);
            RigManipulator.ReassignBones(rigRoot, ct.Clothing, ct.RiggableSkin.skinMeshRenderer);
            Destroy(ct.RiggableSkin.root.gameObject);
            ct.RiggableSkin.root = null;
          }
          else
          {
            Debug.LogWarning("No clothes are currently equiped!");
          }
        }
        else
        {
          Debug.LogWarning("No changes were made to this item");
        }
      }
      else
      {
        if (ct.Clothing)
        {
          ct.RiggableSkin = Instantiate(ct.Clothing.SkinPrefab, localCharacter.transform);
          RigManipulator.ReassignBones(rigRoot, ct.Clothing, ct.RiggableSkin.skinMeshRenderer);
          Destroy(ct.RiggableSkin.root.gameObject);
          ct.RiggableSkin.root = null;
        }
        else
        {
          Debug.LogWarning("No clothes are currently equiped!");
        }
      }
    }
  }
  public void AddClothing(RigData rigToAdd)
  {
    if (!Application.isPlaying)
    {
      Debug.LogError("Are you trying to call this function outside of Play Mode?");
      return;
    }

    ClothingTree tree = GetClothingOfType(rigToAdd.type);
    if (tree.Clothing == rigToAdd)
    {
      // if players tries to equip the same piece after being equiped, remove that piece instead
      RemoveClothing(tree);
      return;
    }
    if (tree.equiped)
    {
      // ask the player if he is sure to replace the clothes
      RequestConfirm();
      tree.Clothing = rigToAdd;
    }
    else
    {
      tree.Clothing = rigToAdd;
      tree.equiped = true;
    }
    currentClothingData.SaveClothes(this.Clothes);
    GenerateCharacter();
  }

  public System.Action AreYouSure;
  private void RequestConfirm()
  {
    AreYouSure?.Invoke();
  }

  private RigData _cachedForConfirmation;
  public void ConfirmSelection(bool state)
  {
    if (state && _cachedForConfirmation)
    {
      ClothingTree tree = GetClothingOfType(_cachedForConfirmation.type);
      tree.Clothing = _cachedForConfirmation;
      currentClothingData.SaveClothes(this.Clothes);
      GenerateCharacter();
    }
  }

  private void RemoveClothing(ClothingTree tree)
  {
    tree.equiped = false;
    tree.Clothing = null;
    Destroy(tree.RiggableSkin.gameObject);
    tree.RiggableSkin = null;
    currentClothingData.SaveClothes(this.Clothes);
    GenerateCharacter();
  }
}


[System.Serializable]
public class ClothingTree
{
  [HideInInspector] public bool equiped = false;
  public ClothingType Type;
  public RigData Clothing;
  public RiggableSkin RiggableSkin;
}