using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;
using UnityEditor;
using System.IO;

public class RigManipulator : MonoBehaviour
{
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

  [ReadOnly]
  public List<BoneListWrapper> ListOfBonesAndSMR = new List<BoneListWrapper>();

  [ReadOnly]
  public List<Transform> sourcePositions = new List<Transform>();

  [Header("The renderer(s) to which we apply the bones")]
  [SerializeField] SkinnedMeshRenderer[] destinationRenderers;
  [Header("The renderer from which we copy the bones")]
  [SerializeField] SkinnedMeshRenderer sourceRenderer;

  [InfoBox("Debugging will significantly increase process time as more objects are being added!",EInfoBoxType.Warning)]
  [SerializeField] private bool _debugBones = false;


  private void CopyBonesOfSkinnedMeshRenderer(SkinnedMeshRenderer _destinationRenderer, BoneListWrapper blw = null)
  {
    Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();

    if (_debugBones && blw != null)
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
    if (_debugBones && blw != null)
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
    if (_debugBones)
    {
      DebugBones(out sourcePositions, sourceRenderer.bones);
    }
    foreach (SkinnedMeshRenderer smr in destinationRenderers)
    {
      if (_debugBones)
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


  [Button("Copy Bones (Source => Destination)")]
  void Editor_CopyBonesButton()
  {
    CopyBonesInit();
  }

  public RiggableSkin Prefab;

  [Space(20)]
  //[HideInInspector] public RiggableSkinTest PrefabDestination;
  public List<SkinnedMeshRenderer> SMRTargets = new List<SkinnedMeshRenderer>();
#if UNITY_EDITOR
  [Button("Generate new object")]
  void GenerateObject()
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

  bool GeneratePrefabOf(SkinnedMeshRenderer target) 
  {
    if (!target)
    {
      Debug.LogError("Invalid SkinnedMeshRenderer, target should not be null");
      return false;
    }


    if (!Directory.Exists("Assets/GeneratedStorage"))
      AssetDatabase.CreateFolder("Assets", "GeneratedStorage");
    if (!Directory.Exists("Assets/GeneratedStorage/Prefabs"))
      AssetDatabase.CreateFolder("Assets/GeneratedStorage", "Prefabs");
    if (!Directory.Exists("Assets/GeneratedStorage/RigData"))
      AssetDatabase.CreateFolder("Assets/GeneratedStorage", "RigData");

    string universalName = target.gameObject.name;

    RiggableSkin PrefabDestination = Instantiate(Prefab);
    PrefabDestination.name = universalName + "Prefab";

    #region Creating Rig a.k.a scriptable object RigData
    // MyClass is inheritant from ScriptableObject base class
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

    //EditorUtility.FocusProjectWindow();
    //Selection.activeObject = example;
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
    RigManipulator.ReassignBones(PrefabDestination.root, PrefabDestination.rig, PrefabDestination.skinMeshRenderer);


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
  #region Static Methods
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
  #endregion
}



