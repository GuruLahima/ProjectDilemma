using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using UnityEngine.GameFoundation;
using GuruLaghima.ProjectDilemma;
using TMPro;
using UnityEngine.UI;
using System;
using MoreMountains.Feedbacks;
using GuruLaghima;
using DG.Tweening;


namespace Workbench.ProjectDilemma
{
  public class ThrowablesActivator : BaseActivatorComponent
  {
    public enum ResolutionMultiplier : int { X1 = 1, X2 = 2, X4 = 4, X8 = 8, X16 = 16, X32 = 32 }
    #region public fields

    #endregion

    #region exposed fields
    [HorizontalLine]
    [CustomTooltip("Source from where the projectile is thrown")]
    [SerializeField] GameObject raycastBlocker;
    [SerializeField] public SimpleCameraController canon;
    [SerializeField] Transform startingAngle;
    [SerializeField] LineRenderer aimHelper;
    [SerializeField] Transform startingPoint;
    [SerializeField] Transform startingPointPivot;
    [SerializeField] GameObject selectedItemHUD;
    [SerializeField] TextMeshProUGUI selectedItemCounter;
    [SerializeField] Image selectedItemIcon;
    [HorizontalLine]
    [MinValue(0), MaxValue(100)]
    [CustomTooltip("the base velocity with which we throw the projectile. ")]
    [SerializeField] float baseThrowStrength;
    [SerializeField] float aimingAngle = 0f;
    [CustomTooltip("how much the angle of the aim changes when we move the mouse")]
    [SerializeField] float aimAngleMod = 1f;
    [CustomTooltip("how much the strength of the throw changes when we move the mouse")]
    [SerializeField] float throwStrengthAimingMod;
    [CustomTooltip("how fast does the strength update when we change it with the mouse wheel")]
    [SerializeField] float strengthUpdateSpeed;
    [SerializeField] float aimingCameraTransitionInterval;
    [SerializeField] float aimingCameraTransitionFieldOfView;
    [Min(2)]
    [SerializeField] int resolutionRay;
    [SerializeField] ResolutionMultiplier resolutionLine = ResolutionMultiplier.X1;
    [HorizontalLine]
    [SerializeField] LayerMask layerMask;
    [SerializeField][ReadOnly] ProjectileData selectedProjectile;
    [SerializeField] RadialChooseMenu radialMenu;
    [Space(10, order = -1)]
    [Header("Extras")]
    [SerializeField] Outline iconOutline;
    #endregion

    #region feedbacks
    [HorizontalLine]
    [SerializeField] MMFeedbacks throwablesOnCooldownFeedbacks;
    [SerializeField] MMFeedbacks outOfThrowablesFeedbacks;
    [SerializeField] MMFeedbacks shootThrowableFeedbacks;
    [SerializeField] GameObject decal;

    #endregion

    #region private fields
    SimpleCameraController mainCam;
    List<ProjectileData> ownedProjectiles;
    bool _strengthScalingUp = false;
    private float originalFieldOfView;
    private float newBaseThrowStrength;
    private bool aiming;
    #endregion

    #region  Monobehaviour callbacks

    private void OnEnable()
    {
      InventoryManager.InventoryUpdated += Init;
      mainCam = GetComponent<PlayerSpot>().playerCam.GetComponent<SimpleCameraController>();
      newBaseThrowStrength = baseThrowStrength;
      Init();
    }

    private void OnDisable()
    {
      InventoryManager.InventoryUpdated -= Init;
    }

    private void Update()
    {

    }

    #endregion

    #region public methods
    [ContextMenu("Init")]
    public override void Init()
    {
      ownedProjectiles = ItemSettings.Instance.throwables.Where((obj) => { return obj.Owned && obj.Equipped; }).ToList();
      // generate the new items
      // foreach (ProjectileData projData in ownedProjectiles)
      // {
      //   if (projData.Icon)
      //   {
      //     var ico = Instantiate(projData.Icon, radialMenu.transform);
      //     ico.container = projData;
      //     if (projData.ico) ico.image.sprite = projData.ico;
      //   }
      // }
    }
    public void Pick()
    {
      MyDebug.Log("Pick projectile");
      if (iconOutline)
      {
        iconOutline.effectColor = Color.white;
      }
      ActiveState = true;
      radialMenu.Activate();
      CursorManager.SetLockMode(CursorLockMode.Confined);
      CursorManager.SetVisibility(false);
    }
    public void Set()
    {
      MyDebug.Log("Set projectile");
      ActiveState = false;
      radialMenu.Deactivate();
      CursorManager.SetLockMode(CursorLockMode.Locked);
      CursorManager.SetVisibility(false);
      if (radialMenu.LastSelectedObject != null)
      {
        var container = radialMenu.LastSelectedObject.GetComponent<SelectionMenuContainer>();
        if (container)
        {
          if (container.container is ProjectileData)
          {
            selectedProjectile = container.container as ProjectileData;
            selectedItemCounter.text = selectedProjectile.AmountOwned.ToString();
            selectedItemIcon.sprite = selectedProjectile.ico;
            selectedItemHUD.SetActive(true);
          }
        }
      }
      ResetRotationOfCanon();
    }
    private void DisableCameraControl()
    {
      mainCam.playerHasControlOverCamera = false;
      // unlock mouse
      CursorManager.SetLockMode(CursorLockMode.Confined);

      // also force camera to look straight ahead. maybe increase field of view too for extra effect
      Vector3 position;
      if (PhotonNetwork.IsConnected)
      {
        position = GameMechanic.Instance.otherPlayerSpot.character.transform.position;
      }
      else
      {
        if (this.GetComponent<PlayerSpot>() == GameMechanic.Instance.playerOneSpot)
          position = GameMechanic.Instance.playerTwoSpot.playerModel.transform.position;
        else
          position = GameMechanic.Instance.playerOneSpot.playerModel.transform.position;

      }
      Camera currentPlayerCam = GameMechanic.Instance.localPlayerSpot.playerCam.GetComponent<Camera>();
      currentPlayerCam.transform.DOLookAt(position, aimingCameraTransitionInterval, AxisConstraint.Y, Vector3.up);
      currentPlayerCam.DOFieldOfView(aimingCameraTransitionFieldOfView, aimingCameraTransitionInterval);
    }

    private void RestoreCameraControl()
    {
      MyDebug.Log("Control restored");
      mainCam.playerHasControlOverCamera = true;
      // lock mouse
      CursorManager.SetLockMode(CursorLockMode.Locked);

      // restore field of view
      Camera currentPlayerCam = GameMechanic.Instance.localPlayerSpot.playerCam.GetComponent<Camera>();
      currentPlayerCam.DOFieldOfView(originalFieldOfView, aimingCameraTransitionInterval);
    }

    IEnumerator FacePositionSmoothly(Vector3 position, float v)
    {
      yield return null;
    }

    public void Aim()
    {
      MyDebug.Log("Aim projectile");
      if (!aiming)
      {
        if (OnCooldown)
        {
          // notify user there this ability is on cooldown
          ThrowablesOnCooldownFeedback();
          return;
        }
        if (selectedProjectile == null || selectedProjectile.AmountOwned <= 0)
        {
          OutOfThrowablesFeedback();
          return;
        }
        MyDebug.Log("Start aiming");

        aiming = true;
        Camera currentPlayerCam = GameMechanic.Instance.localPlayerSpot.playerCam.GetComponent<Camera>();
        originalFieldOfView = currentPlayerCam.fieldOfView;
        raycastBlocker.SetActive(true);

        DisableCameraControl();
        canon.playerHasControlOverCamera = true;

        aimHelper.gameObject.SetActive(true);

        if (iconOutline)
        {
          iconOutline.effectColor = Color.red;
        }
        // ActiveState = true;
      }
      CalculateStrengthAndAngle();
      TrajectoryMotion.AIM(aimHelper, startingPoint, aimingAngle, baseThrowStrength, resolutionRay, (int)resolutionLine, layerMask.value, null, null, decal);

    }

    private void ResetRotationOfCanon()
    {
      canon.transform.LookAt(startingAngle);
    }

    private void CalculateStrengthAndAngle()
    {
      aimingAngle = -startingPointPivot.eulerAngles.x * aimAngleMod;
      newBaseThrowStrength += Input.mouseScrollDelta.y * throwStrengthAimingMod;
      baseThrowStrength = Mathf.Lerp(baseThrowStrength, newBaseThrowStrength, Time.deltaTime * strengthUpdateSpeed);
    }

    public void Throw()
    {
      MyDebug.Log("Throw projectile");
      aiming = false;
      raycastBlocker.SetActive(false);

      if (OnCooldown) return;
      if (!selectedProjectile) return;
      if (!(selectedProjectile.AmountOwned > 0)) return;

      // ActiveState = false;
      if (selectedProjectile && selectedProjectile.AmountOwned > 0)
      {
        // expend an item
        InventoryManager.instance.RemoveItem(selectedProjectile.inventoryitemDefinition.key);

        // update hud
        selectedItemCounter.text = selectedProjectile.AmountOwned.ToString();
        selectedItemIcon.sprite = selectedProjectile.ico;
        if (GameFoundationSdk.inventory.GetTotalQuantity(selectedProjectile.inventoryitemDefinition) <= 0)
        {
          selectedItemIcon.sprite = null;
          selectedItemCounter.text = "0";
        }

        AddCooldown();

        float radianAngle = aimingAngle * Mathf.Deg2Rad;
        if (PhotonNetwork.IsConnected)
        {

          RPCManager.Instance.photonView.RPC("RPC_ThrowProjectile", Photon.Pun.RpcTarget.AllViaServer, MasterData.Instance.GetProjectileIndex(selectedProjectile), startingPoint.position,
            startingPoint.TransformDirection(new Vector3(0, Mathf.Sin(radianAngle), Mathf.Cos(radianAngle))), baseThrowStrength, Physics.gravity.y);
        }
        else
        {
          var proj = Instantiate(selectedProjectile.Prefab);
          proj.SetProjectile(startingPoint.position,
           startingPoint.TransformDirection(new Vector3(0, Mathf.Sin(radianAngle), Mathf.Cos(radianAngle))), baseThrowStrength, Physics.gravity.y);
        }

        ShootThrowableFeedback();

      }

      aimHelper.gameObject.SetActive(false);
      canon.playerHasControlOverCamera = false;
      RestoreCameraControl();

    }

    #endregion

    #region private methods
    private void ShootThrowableFeedback()
    {
      shootThrowableFeedbacks?.PlayFeedbacks();
    }

    private void OutOfThrowablesFeedback()
    {
      outOfThrowablesFeedbacks?.PlayFeedbacks();
    }

    private void ThrowablesOnCooldownFeedback()
    {
      throwablesOnCooldownFeedbacks?.PlayFeedbacks();
    }
    #endregion
  }
}
