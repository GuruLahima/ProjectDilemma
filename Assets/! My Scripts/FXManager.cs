using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
  public class FXManager : MonoBehaviour
  {
    #region Singleton
    public static FXManager Instance;
    private void Awake()
    {
      Instance = this;
    }
    #endregion

    public void GenerateFX(FXWrapper fx, Transform castFrom, float customDuration)
    {
      Transform fxParent = transform;
      if (fx.FXFollowsObject) fxParent = castFrom;
      var obj = Instantiate(fx.FXPrefab, castFrom.position, castFrom.rotation, fxParent);
      if (customDuration > 0)
      {
        StartCoroutine(DestroyWithDelay(obj, customDuration));
      }
    }

    public void GenerateFX(FXWrapper fx, Transform castFrom)
    {
      Transform fxParent = transform;
      if (fx.FXFollowsObject) fxParent = castFrom;
      var obj = Instantiate(fx.FXPrefab, castFrom.position, castFrom.rotation, fxParent);
      if (fx.FXDuration > 0)
      {
        StartCoroutine(DestroyWithDelay(obj, fx.FXDuration));
      }
    }

    public void GenerateFX(FXWrapper fx, Transform castFrom, Vector3 position, Quaternion rotation)
    {
      Transform fxParent = transform;
      if (fx.FXFollowsObject) fxParent = castFrom;
      var obj = Instantiate(fx.FXPrefab, position, rotation, fxParent);
      if (fx.FXDuration > 0)
      {
        StartCoroutine(DestroyWithDelay(obj, fx.FXDuration));
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


  [System.Serializable]
  public class FXWrapper
  {
    public GameObject FXPrefab;
    public float FXDuration;
    public bool FXFollowsObject;
  }
}