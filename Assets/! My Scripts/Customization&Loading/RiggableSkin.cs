using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiggableSkin : MonoBehaviour
{
  public SkinnedMeshRenderer skinMeshRenderer;
  public Transform root;
  public RigData rig;

  public bool isVisibleToPlayerWearingIt = true;

  [Button("Reassign bones")]
  public void ReassignBones()
  {
    Rig.ReassignBones(this.root, this.rig, this.skinMeshRenderer);
    #region DEPRECATED
    /*
    if (!root || !rig || !skinMeshRenderer) return;

    List<Transform> newBones = new List<Transform>();

    Transform[] rigTransforms = root.GetComponentsInChildren<Transform>();

    for  (int i = 0; i < rig.BonesData.Count; i++)
    {
      bool found = false;
      foreach (Transform t in rigTransforms)
      {
        if (t.name == rig.BonesData[i])
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

    skinMeshRenderer.bones = newBones.ToArray();
    skinMeshRenderer.rootBone = root;
    */
    #endregion
  }

}
