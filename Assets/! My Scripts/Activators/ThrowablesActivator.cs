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
using Cinemachine;

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
    [SerializeField] Image selectedItemInteractablesIcon;
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
    [SerializeField] DiamonMenuEntry menuEntry;

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
      GameMechanic.DiscussionEnded += OnDiscussionEnded;
      mainCam = GetComponent<PlayerSpot>().firstPersonPlayerCam.GetComponent<SimpleCameraController>();
      newBaseThrowStrength = baseThrowStrength;
      Init();
    }

    private void OnDiscussionEnded()
    {
      // when discussion ends the aiming helper should hide (if it was on)
      aimHelper.gameObject.SetActive(false);
      canon.playerHasControlOverCamera = false;
    }

    private void OnDisable()
    {
      InventoryManager.InventoryUpdated -= Init;
      GameMechanic.DiscussionEnded -= OnDiscussionEnded;
    }

    private void Update()
    {

    }

    #endregion

    #region public methods
    [ContextMenu("Init")]
    public override void Init()
    {
      // maybe we should have a reference to the 

    }
    public void Pick()
    {
      MyDebug.Log("Pick projectile");
      if (OnCooldown) return;
      if (iconOutline)
      {
        //we call this only once per activation (to avoid unnecessary tasks)
        iconOutline.effectColor = Color.white;
      }
      if (!ActiveState)
      {
        radialMenu.RegenerateSnapPoints();
      }
      ActiveState = true;

      PlayerInputManager.Instance.OnEnterRadialMenu();
      radialMenu.ActivateRadialMenu(Set);

    }
    public void Set()
    {
      MyDebug.Log("Set projectile");
      PlayerInputManager.Instance.OnExitRadialMenu();
      ActiveState = false;
      radialMenu.Deactivate();
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
            // selectedItemHUD.SetActive(true);

            // diamond menu icon change
            // first swap the default icon with the previous icon to complete the illusion of rotation of the items (because that's how the MMfeedbacks are set up)
            if (menuEntry.chosenItemIcon.sprite)
              menuEntry.defaultIcon.sprite = menuEntry.chosenItemIcon.sprite;
            menuEntry.chosenItemIcon.sprite = selectedProjectile.ico;
            menuEntry.switchToChosenIconFeedbacks.PlayFeedbacks();
          }
        }
      }

      ResetRotationOfCanon();
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
        MyDebug.Log("Start aiming projectile");
        if (OnCooldown)
        {
          MyDebug.Log("projectile on cooldown");
          // notify user there this ability is on cooldown
          ThrowablesOnCooldownFeedback();
          PlayerInputManager.Instance.OnFinishedAiming();
          return;
        }
        if (selectedProjectile == null || selectedProjectile.AmountOwned <= 0)
        {
          MyDebug.Log("we don't have a valid non-zero quantity projectile selected");
          OutOfThrowablesFeedback();
          PlayerInputManager.Instance.OnFinishedAiming();
          return;
        }

        aiming = true;

        PlayerInputManager.Instance.OnStartedAiming();


        // switch to first person camera for aiming
        GameMechanic.Instance.localPlayerSpot.playerCam = GameMechanic.Instance.localPlayerSpot.firstPersonPlayerCam;
        GameMechanic.Instance.localPlayerSpot.GetComponent<CameraSwitcher>().SwitchToCamByReference(
          GameMechanic.Instance.localPlayerSpot.playerCam.GetComponent<CinemachineVirtualCamera>());

        // prevent switching while aiming
        GameMechanic.Instance.localPlayerSpot.GetComponent<CameraSwitcher>().canSwitch = false;

        // remember the original field of view because we will be animating it
        CinemachineVirtualCamera currentPlayerCam = GameMechanic.Instance.localPlayerSpot.playerCam.GetComponent<CinemachineVirtualCamera>();
        originalFieldOfView = currentPlayerCam.m_Lens.FieldOfView;

        // prevent any accidental clicks on the UI
        raycastBlocker.SetActive(true);

        DisableCameraControl();
        AnimateFieldOfViewAndRotationOfCamera();


        // initialize throwables canon and give control to the player
        canon.Init();
        canon.playerHasControlOverCamera = true;
        aimHelper.gameObject.SetActive(true);

        if (iconOutline)
        {
          iconOutline.effectColor = Color.red;
        }
      }

      // aiming
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

    private void DisableCameraControl()
    {
      mainCam.playerHasControlOverCamera = false;
      // unlock mouse
      CursorManager.SetLockMode(CursorLockMode.Locked);
      CursorManager.SetVisibility(false);
    }

    void AnimateFieldOfViewAndRotationOfCamera()
    {
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
      CinemachineVirtualCamera currentPlayerCam = GameMechanic.Instance.localPlayerSpot.playerCam.GetComponent<CinemachineVirtualCamera>();
      if (currentPlayerCam)
      {
        currentPlayerCam.transform.DOLookAt(position, aimingCameraTransitionInterval, AxisConstraint.Y, Vector3.up);
        DOTween.To(() => currentPlayerCam.m_Lens.FieldOfView, x => currentPlayerCam.m_Lens.FieldOfView = x, aimingCameraTransitionFieldOfView, aimingCameraTransitionInterval);
      }
    }

    private void RestoreCameraControl()
    {
      MyDebug.Log("Control restored");
      mainCam.playerHasControlOverCamera = true;
      // lock mouse
      CursorManager.SetLockMode(CursorLockMode.Confined);
      CursorManager.SetVisibility(true);


      // restore field of view
      CinemachineVirtualCamera currentPlayerCam = GameMechanic.Instance.localPlayerSpot.playerCam.GetComponent<CinemachineVirtualCamera>();
      if (currentPlayerCam)
        DOTween.To(() => currentPlayerCam.m_Lens.FieldOfView, x => currentPlayerCam.m_Lens.FieldOfView = x, originalFieldOfView, aimingCameraTransitionInterval);

    }

    public void Throw()
    {
      MyDebug.Log("Throw projectile");
      aiming = false;
      raycastBlocker.SetActive(false);

      // allow switching while aiming
      GameMechanic.Instance.localPlayerSpot.GetComponent<CameraSwitcher>().canSwitch = true;

      if (OnCooldown) return;
      if (!selectedProjectile) return;
      if (!(selectedProjectile.AmountOwned > 0)) return;

      if (selectedProjectile && selectedProjectile.AmountOwned > 0)
      {
        // expend an item
        if (!PhotonNetwork.IsConnected || !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(Keys.PRIVATE_GAME))
          if (InventoryManager.instance != null && selectedProjectile.inventoryitemDefinition != null)
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

          RPCManager.Instance.photonView.RPC("RPC_ThrowProjectile", Photon.Pun.RpcTarget.AllViaServer, photonView.ViewID, MasterData.Instance.GetProjectileIndex(selectedProjectile), startingPoint.position,
            startingPoint.TransformDirection(new Vector3(0, Mathf.Sin(radianAngle), Mathf.Cos(radianAngle))), baseThrowStrength, Physics.gravity.y);
        }
        else
        {
          var proj = Instantiate(selectedProjectile.Prefab);
          proj.SetProjectile(photonView.ViewID, startingPoint.position,
           startingPoint.TransformDirection(new Vector3(0, Mathf.Sin(radianAngle), Mathf.Cos(radianAngle))), baseThrowStrength, Physics.gravity.y);
        }
        GameEvents.OnThrowableUsed?.Invoke(selectedProjectile);
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
