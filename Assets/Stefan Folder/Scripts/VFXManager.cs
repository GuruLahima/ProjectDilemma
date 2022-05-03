using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
  public class VFXManager : MonoBehaviour
  {
    #region Singleton
    public static VFXManager Instance;
    private void Awake()
    {
      Instance = this;
    }
    #endregion

    public void GenerateVFX(VFXWrapper vfx, Transform castFrom, float customDuration)
    {
      Transform vfxParent = transform;
      if (vfx.VFXFollowsObject) vfxParent = castFrom;
      var obj = Instantiate(vfx.VFXPrefab, castFrom.position, castFrom.rotation, vfxParent);
      if (customDuration > 0)
      {
        StartCoroutine(DestroyWithDelay(obj, customDuration));
      }
    }

    public void GenerateVFX(VFXWrapper vfx, Transform castFrom)
    {
      Transform vfxParent = transform;
      if (vfx.VFXFollowsObject) vfxParent = castFrom;
      var obj = Instantiate(vfx.VFXPrefab, castFrom.position, castFrom.rotation, vfxParent);
      if (vfx.VFXDuration > 0)
      {
        StartCoroutine(DestroyWithDelay(obj, vfx.VFXDuration));
      }
    }

    public void GenerateVFX(VFXWrapper vfx, Transform castFrom, Vector3 position, Quaternion rotation)
    {
      Transform vfxParent = transform;
      if (vfx.VFXFollowsObject) vfxParent = castFrom;
      var obj = Instantiate(vfx.VFXPrefab, position, rotation, vfxParent);
      if (vfx.VFXDuration > 0)
      {
        StartCoroutine(DestroyWithDelay(obj, vfx.VFXDuration));
      }
    }

    /// <summary>
    /// We use this coroutine instead of Destroy(GameObject obj, float time) since we are instantiating some objects
    /// inside a parent that could potentially get destroyed before the <paramref name="objectToDestroy"/> itself,
    /// this way we can avoid potential errors
    /// </summary>
    private IEnumerator DestroyWithDelay(GameObject objectToDestroy, float destroyDelay)
    {
      yield return new WaitForSeconds(destroyDelay);
      if (objectToDestroy)
      {
        Destroy(objectToDestroy);
      }
    }
  }
}