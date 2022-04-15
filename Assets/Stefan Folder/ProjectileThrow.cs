using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
  public class ProjectileThrow : MonoBehaviour
  {
    public LineRenderer lineRenderer;
    public Camera playerCamera;
    public float strength;
    public int resoRay;
    [InputAxis] public string Hotkey;
    [Layer] public int layerMask;
    float angle = 0f;

    /// <summary>
    /// Temporary for testing, later we have to move this into a scriptable object of a sort that takes care of ALL projectiles,
    /// and another reference to the invenotry which lets us know which projectile we have equiped and ready to use ! tbd
    /// </summary>
    public List<ProjectileData> allProjectiles;
    public ProjectileData projectilePrefab //temporary
    {
      get
      {
        if (allProjectiles.Count > 0)
        {
          return allProjectiles[Random.Range(0, allProjectiles.Count)];
        }
        else
        {
          return null;
        }
      }
    }

    private void Update()
    {
      if (Input.GetButton(Hotkey))
      {
        angle = Vector3.SignedAngle(transform.InverseTransformDirection(transform.forward),
  transform.InverseTransformDirection(playerCamera.transform.forward), Vector3.right) * -1;
        TrajectoryMotion.AIM(lineRenderer, lineRenderer.transform, angle, strength, resoRay, 2, layerMask);
      }
      if (Input.GetButton(Hotkey))
      {
        float radianAngle = angle * Mathf.Deg2Rad;
        if (projectilePrefab)
        {
          RPCManager.Instance.photonView.RPC("RPC_ThrowProjectile", Photon.Pun.RpcTarget.AllViaServer, projectilePrefab.Key, lineRenderer.transform.position,
            lineRenderer.transform.TransformDirection(new Vector3(0, Mathf.Sin(radianAngle), Mathf.Cos(radianAngle))), strength, Physics.gravity.y);
        }
      }
    }
  }

}
