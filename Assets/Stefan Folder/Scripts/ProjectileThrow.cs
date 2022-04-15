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
    public float strength
    {
      get
      {
        _strength += (_strengthScalingUp) ? strengthScale * Time.deltaTime : -strengthScale * Time.deltaTime;
        _strength = Mathf.Clamp(_strength, strengthExtrema.x, strengthExtrema.y);
        if (_strength >= strengthExtrema.y || _strength <= strengthExtrema.x)
          _strengthScalingUp = !_strengthScalingUp;
        return _strength;
      }
    }
    public float strengthScale;
    private float _strength;
    private bool _strengthScalingUp = false;
    public Vector2 strengthExtrema = new Vector2(10f, 20f);
    public int resoRay;
    public GameObject decal;
    float angle = 0f;

    /// <summary>
    /// Temporary for testing, later we have to move this into a scriptable object of a sort that takes care of ALL projectiles,
    /// and another reference to the invenotry which lets us know which projectile we have equiped and ready to use ! tbd
    /// </summary>
    public List<ProjectileData> ownedProjectiles;
    public ProjectileData projectilePrefab //temporary
    {
      get
      {
        if (ownedProjectiles.Count > 0)
        {
          return ownedProjectiles[Random.Range(0, ownedProjectiles.Count)];
        }
        else
        {
          return null;
        }
      }
    }
    public void Aim()
    {
      gameObject.SetActive(true);
      angle = Vector3.SignedAngle(transform.InverseTransformDirection(transform.forward),
        transform.InverseTransformDirection(playerCamera.transform.forward), Vector3.right) * -1;
      TrajectoryMotion.AIM(lineRenderer, lineRenderer.transform, angle, strength, resoRay, 2, Physics.DefaultRaycastLayers, null, null, decal);
    }
    public void Throw()
    {
      gameObject.SetActive(false);
      float radianAngle = angle * Mathf.Deg2Rad;
      if (projectilePrefab)
      {
        if (Photon.Pun.PhotonNetwork.IsConnected)
        {

         RPCManager.Instance.photonView.RPC("RPC_ThrowProjectile", Photon.Pun.RpcTarget.AllViaServer, ProjectileManager.Instance.GetProjectileIndex(projectilePrefab), lineRenderer.transform.position,
           lineRenderer.transform.TransformDirection(new Vector3(0, Mathf.Sin(radianAngle), Mathf.Cos(radianAngle))), strength, Physics.gravity.y);
        }
        else
        {
          var proj = Instantiate(projectilePrefab.Prefab);
          proj.SetProjectile(lineRenderer.transform.position,
           lineRenderer.transform.TransformDirection(new Vector3(0, Mathf.Sin(radianAngle), Mathf.Cos(radianAngle))), strength, Physics.gravity.y);
        }
      }
    }
  }
}
