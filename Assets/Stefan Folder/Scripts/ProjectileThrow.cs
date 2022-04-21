using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

namespace Workbench.ProjectDilemma
{
  public class ProjectileThrow : MonoBehaviourPun
  {
    [Tooltip("Source from where the projectile is thrown")]
    public Transform throwablePrefabPivot;
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
    public SelectionMenu selectionMenu;
    public CursorManager cursorManager;
    private CanvasGroup canvasGroup;

    /// <summary>
    /// Temporary for testing, later we have to move this into a scriptable object of a sort that takes care of ALL projectiles,
    /// and another reference to the invenotry which lets us know which projectile we have equiped and ready to use ! tbd
    /// </summary>
    List<ProjectileData> ownedProjectiles;
    public ProjectileData projectilePrefab;
    private void Start()
    {
      ownedProjectiles = ItemSettings.Instance.throwables.Where((obj) => { return obj.Owned; }).ToList();
      canvasGroup = selectionMenu.GetComponent<CanvasGroup>();
      if (photonView.IsMine)
      {
        foreach (ProjectileData projData in ownedProjectiles)
        {
          if (projData.Icon)
          {
            var ico = Instantiate(projData.Icon, selectionMenu.transform);
            ico.container = projData;
            if (projData.ico) ico.image.sprite = projData.ico;
          }
        }
      }
    }


    public void Pick()
    {
      canvasGroup.alpha = 1; canvasGroup.blocksRaycasts = true; canvasGroup.interactable = true;
      cursorManager.SetLockMode(CursorLockMode.Confined);
      cursorManager.SetVisibility(true);
    }

    public void Set()
    {
      canvasGroup.alpha = 0; canvasGroup.blocksRaycasts = false; canvasGroup.interactable = false;
      cursorManager.SetLockMode(CursorLockMode.Locked);
      cursorManager.SetVisibility(false);
      if (selectionMenu.LastSelectedObject != null)
      {
        var container = selectionMenu.LastSelectedObject.GetComponent<SelectionMenuContainer>();
        if (container)
        {
          if (container.container is ProjectileData)
          {
            projectilePrefab = container.container as ProjectileData;
          }
        }
      }
    }

    public void Aim()
    {
      throwablePrefabPivot.gameObject.SetActive(true);
      angle = Vector3.SignedAngle(throwablePrefabPivot.InverseTransformDirection(throwablePrefabPivot.forward),
        throwablePrefabPivot.InverseTransformDirection(playerCamera.transform.forward), Vector3.right) * -1;
      TrajectoryMotion.AIM(lineRenderer, lineRenderer.transform, angle, strength, resoRay, 2, Physics.DefaultRaycastLayers, null, null, decal);
    }
    public void Throw()
    {
      throwablePrefabPivot.gameObject.SetActive(false);
      float radianAngle = angle * Mathf.Deg2Rad;
      if (projectilePrefab)
      {
        if (Photon.Pun.PhotonNetwork.IsConnected)
        {

          RPCManager.Instance.photonView.RPC("RPC_ThrowProjectile", Photon.Pun.RpcTarget.AllViaServer, MasterData.Instance.GetProjectileIndex(projectilePrefab), lineRenderer.transform.position,
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
