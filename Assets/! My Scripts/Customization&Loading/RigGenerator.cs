using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using UnityEditor;
using System.IO;
using GuruLaghima;

public class RigGenerator : MonoBehaviour
{
  #region Class Declaration
  [System.Serializable]
  public class BoneListWrapper
  {
    public BoneListWrapper(SkinnedMeshRenderer smr)
    {
      this.skinnedMeshRenderer = smr;
    }
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public List<Transform> boneListPreTransformation = new List<Transform>();
    public List<Transform> boneListPostTransformation = new List<Transform>();
  }
  #endregion

  //--------Copy bones/Debug bones--------
  #region Exposed Private Fields
  [Foldout("Copy/Debug Bones")]
  [ReadOnly]
  [SerializeField] private List<BoneListWrapper> ListOfBonesAndSMR = new List<BoneListWrapper>();
  [Foldout("Copy/Debug Bones")]
  [ReadOnly]
  [SerializeField] private List<Transform> sourcePositions = new List<Transform>();
  [Foldout("Copy/Debug Bones")]
  [CustomTooltip("The renderer(s) to which we apply the bones")]
  [SerializeField] private SkinnedMeshRenderer[] destinationRenderers;
  [Foldout("Copy/Debug Bones")]
  [CustomTooltip("The renderer from which we copy the bones")]
  [SerializeField] private SkinnedMeshRenderer sourceRenderer;
  [Foldout("Copy/Debug Bones")]
  [CustomTooltip("Debugging will significantly increase process time as more objects are being added!")]
  [SerializeField] private bool debugBones = false;
  #endregion

  #region Private Methods
  private void CopyBonesOfSkinnedMeshRenderer(SkinnedMeshRenderer _destinationRenderer, BoneListWrapper blw = null)
  {
    Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();

    if (debugBones && blw != null)
    {
      DebugBones(out blw.boneListPreTransformation, _destinationRenderer.bones);
    }

    foreach (Transform bone in sourceRenderer.bones)
    {
      boneMap[bone.name] = bone;
    }
    Transform[] boneArray = _destinationRenderer.bones;

    for (int idx = 0; idx < boneArray.Length; ++idx)
    {
      if (boneArray[idx])
      {
        string boneName = boneArray[idx].name;
        if (false == boneMap.TryGetValue(boneName, out boneArray[idx]))
        {
          Debug.LogError("failed to get bone: " + boneName);
        }
      }
    }
    // set the bone map
    _destinationRenderer.bones = boneArray;
    // set the root bone
    _destinationRenderer.rootBone = sourceRenderer.rootBone;
    if (debugBones && blw != null)
    {
      DebugBones(out blw.boneListPostTransformation, _destinationRenderer.bones);
    }
  }
  private void DebugBones(out List<Transform> generateTo, Transform[] generateFrom)
  {
    generateTo = new List<Transform>();
    foreach (Transform t in generateFrom)
    {
      generateTo.Add(t);
    }
  }
  private void CopyBonesInit()
  {
    ListOfBonesAndSMR = new List<BoneListWrapper>();
    if (debugBones)
    {
      DebugBones(out sourcePositions, sourceRenderer.bones);
    }
    foreach (SkinnedMeshRenderer smr in destinationRenderers)
    {
      if (debugBones)
      {
        BoneListWrapper blw = new BoneListWrapper(smr);
        ListOfBonesAndSMR.Add(blw);
        CopyBonesOfSkinnedMeshRenderer(smr, blw);
      }
      else
      {
        CopyBonesOfSkinnedMeshRenderer(smr);
      }
    }

  }
  #endregion
  //--------------------------------------

  //--------Generate new prefab-----------
  #region Exposed Private Fields
  [Foldout("Generate Prefab")]
  [CustomTooltip("Default prefab used for generating new prefabs")]
  [SerializeField] private RiggableSkin Prefab;
  [Foldout("Generate Prefab")]
  [CustomTooltip("All skinned mesh renderers to generate prefabs from")]
  [SerializeField] private List<SkinnedMeshRenderer> SMRTargets = new List<SkinnedMeshRenderer>();
  #endregion

  #region Private Fields
#if UNITY_EDITOR

  private void GeneratePrefabInit()
  {
    List<SkinnedMeshRenderer> successfulyGeneratedPrefabs = new List<SkinnedMeshRenderer>();
    foreach (SkinnedMeshRenderer target in SMRTargets)
    {
      var state = GeneratePrefabOf(target);
      if (state)
      {
        successfulyGeneratedPrefabs.Add(target);
      }
    }
    foreach (SkinnedMeshRenderer gp in successfulyGeneratedPrefabs)
    {
      if (SMRTargets.Contains(gp))
      {
        SMRTargets.Remove(gp);
      }
    }
  }

  private bool GeneratePrefabOf(SkinnedMeshRenderer target) 
  {
    if (!target)
    {
      Debug.LogError("Invalid SkinnedMeshRenderer, target should not be null");
      return false;
    }

    #region Creating save directory
    if (!Directory.Exists("Assets/GeneratedStorage"))
      AssetDatabase.CreateFolder("Assets", "GeneratedStorage");
    if (!Directory.Exists("Assets/GeneratedStorage/Prefabs"))
      AssetDatabase.CreateFolder("Assets/GeneratedStorage", "Prefabs");
    if (!Directory.Exists("Assets/GeneratedStorage/RigData"))
      AssetDatabase.CreateFolder("Assets/GeneratedStorage", "RigData");
    #endregion

    string universalName = target.gameObject.name;

    RiggableSkin PrefabDestination = Instantiate(Prefab);
    PrefabDestination.name = universalName + "Prefab";

    #region Creating Rig a.k.a scriptable object RigData
    RigData newRigData = ScriptableObject.CreateInstance<RigData>();
    newRigData.name = universalName + "RigData";


    // path has to start at "Assets"
    string rigDataPath = "Assets/GeneratedStorage/RigData/" + newRigData.name + ".asset";
    AssetDatabase.CreateAsset(newRigData, rigDataPath);
    EditorUtility.SetDirty(newRigData);
    PrefabUtility.RecordPrefabInstancePropertyModifications(newRigData);
    AssetDatabase.SaveAssets();
    AssetDatabase.Refresh();

    EditorUtility.SetDirty(newRigData);
    PrefabUtility.RecordPrefabInstancePropertyModifications(newRigData);
    PrefabDestination.rig = newRigData;
    #endregion

    if (!PrefabDestination.rig)
    {
      Debug.LogError("Failed to generate RigData, proccess canceled");
      return false;
    }

    List<string> boneMap = new List<string>();
    foreach (Transform bone in target.bones)
    {
      if (bone)
        boneMap.Add(bone.name);
      else
        boneMap.Add(null);
    }
    PrefabDestination.rig.BonesData = boneMap;
    var _smr = Instantiate(target, PrefabDestination.transform);
    PrefabDestination.skinMeshRenderer = _smr;
    var _root = Instantiate(target.rootBone, PrefabDestination.transform);
    _root.name = target.rootBone.name;
    PrefabDestination.root = _root;
    Rig.ReassignBones(PrefabDestination.root, PrefabDestination.rig, PrefabDestination.skinMeshRenderer);

    var prefabPath = "Assets/GeneratedStorage/Prefabs/" + PrefabDestination.gameObject.name + ".prefab";
    GameObject newPrefab = PrefabUtility.SaveAsPrefabAsset(PrefabDestination.gameObject, prefabPath, out bool operationStatus);
    if (operationStatus)
    {
      EditorUtility.SetDirty(newPrefab);
      PrefabUtility.RecordPrefabInstancePropertyModifications(newPrefab);
      newRigData.SkinPrefab = newPrefab.GetComponent<RiggableSkin>();
      EditorUtility.SetDirty(newRigData);
      PrefabUtility.RecordPrefabInstancePropertyModifications(newRigData);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      DestroyImmediate(PrefabDestination.gameObject);
      return true;
    }
    else
    {
      Debug.LogError("Failed to create asset with name " + PrefabDestination.name + " at location " + prefabPath);
      return false;
    }
  }
#endif
  #endregion
  //--------------------------------------

  #region Editor Buttons
#if UNITY_EDITOR
  [Button("Copy Bones (Source => Destination)")]
  void Editor_CopyBonesButton()
  {
    CopyBonesInit();
  }

  [Button("Generate new object")]
  void Editor_GeneratePrefabButton()
  {
    GeneratePrefabInit();
  }
#endif
  #endregion
}
public static class Rig
{
  public static void ReassignBones(Transform _root, RigData _rig, SkinnedMeshRenderer _skinMeshRenderer)
  {
    if (!_root || !_rig || !_skinMeshRenderer) return;

    List<Transform> newBones = new List<Transform>();
    Transform[] rigTransforms = _root.GetComponentsInChildren<Transform>();

    for (int i = 0; i < _rig.BonesData.Count; i++)
    {
      bool found = false;
      foreach (Transform t in rigTransforms)
      {
        if (t.name == _rig.BonesData[i])
        {
          newBones.Add(t);
          found = true;
          break;
        }
      }
      if (!found)
      {
        newBones.Add(null);
      }
    }

    _skinMeshRenderer.bones = newBones.ToArray();
    _skinMeshRenderer.rootBone = _root;
  }
}