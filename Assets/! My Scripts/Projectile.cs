using GuruLaghima;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Workbench.ProjectDilemma;

public class Projectile : MonoBehaviour
{
  #region Enum Declaration
  public enum ProjectileType : byte { PhysicsDriven, Bullet }
  public enum BounceType : byte { MaterialDriven, ReverseImpact, PointBreak, Adhesive }
  #endregion

  #region Public Fields
  //[HideInInspector] public GameObject projectileOwner;
  #endregion

  #region Exposed Private Fields
  [Header("Technical Paramters", order = 0)]
  [HorizontalLine(order = 1)]
  [SerializeField] Rigidbody _rb;
  [SerializeField] Collider _col;


  [Header("Custom Properties", order = 0)]
  [HorizontalLine(order = 1)]
  [Foldout("Impact Trigger")][SerializeField] private bool bounce;
  [Foldout("Impact Trigger")][SerializeField] private bool isInfinite;
  [Foldout("Impact Trigger")][SerializeField][DisableIf("isInfinite")] private int bounceAmount;
  [InfoBox("If you want to preserve the initial strength leave this field to -1", EInfoBoxType.Normal)]
  [Foldout("Impact Trigger")][SerializeField] private float bounceStrength = -1;
  [InfoBox("Different bounce types require higher collision ignore window in order to work properly!", EInfoBoxType.Warning)]
  [Foldout("Impact Trigger")][SerializeField] private BounceType bounceType;
  [CustomTooltip("How long should the projectile last after initial bounce? \n" +
      "Main use of this property is for Adhesive type of projectiles")]
  [Foldout("Impact Trigger")][SerializeField] private float projectileAdhesiveDuration = 5f;
  [CustomTooltip("Total lifetime of the projectile from the moment it was launched")]
  [Foldout("Impact Trigger")][SerializeField] private float projectileLifeLimit = 8f;

  [SerializeField] ProjectileType projectileType;
  [InfoBox("Rigidbody Use Gravity should be false at all times \n" +
      "This script contains built-in gravity", EInfoBoxType.Warning)]
  [SerializeField] float gravity = Physics.gravity.y;
  [SerializeField] bool hasRandomRotation = true;
  [SerializeField] List<FXWrapper> fxOnThrown = new List<FXWrapper>();
  [SerializeField] List<FXWrapper> fxOnCollision = new List<FXWrapper>();
  [SerializeField] List<FXWrapper> fxOnTrigger = new List<FXWrapper>();
  [SerializeField] List<FXWrapper> fxOnCollisionLocal = new List<FXWrapper>();
  [SerializeField] float ignoreCollisionDuration = 0.05f;
  [SerializeField] float initialCollisionDisabledDuration = 0.05f;
  #endregion

  #region Private Fields
  private int bounceNumber = 0;
  private bool projectileDisabled = false;
  private int ownerId = -1; //setting -1 for default since viewId cant be negative
  #endregion

  #region Public Methods
  public void SetProjectile(int viewId, Vector3 startPosition, Vector3 direction, float strength, float? customGravity = null) //int pointsMultiplier = 1/*the value from the arena*/)
  {
    // set gravity (some of these things I will apply in wolfsbane) ! remove this comment later
    if (customGravity != null)
      gravity = (float)customGravity;
    gravity = (projectileType == ProjectileType.PhysicsDriven) ? gravity : 0f;

    // set the owner id
    ownerId = viewId;

    // initialy we disable the collider, so that player doesnt collide with it
    _col.enabled = false;

    // restart the velocity to zero
    _rb.velocity = Vector3.zero;
    _rb.angularVelocity = Vector3.zero;

    // enable this gameObject
    gameObject.SetActive(true);

    // set the projectile
    transform.position = startPosition;
    _rb.velocity = strength * direction;
    if (hasRandomRotation)
    {
      transform.rotation = Random.rotation;
      _rb.angularVelocity = strength / 2 * direction;
    }
    else
    {
      transform.rotation = Quaternion.LookRotation(direction);
    }

    // fx on cast
    foreach (FXWrapper fx in fxOnThrown)
    {
      FXManager.Instance.GenerateFX(fx, transform);
    }

    //
    if ((int)bounceStrength == -1) bounceStrength = strength;

    if (!isInfinite)
      Invoke("ResetProjectile", projectileLifeLimit);

    Invoke("InitialCollisionEnable", initialCollisionDisabledDuration);
    Invoke("EnableProjectile", ignoreCollisionDuration);
  }


  public void ResetProjectile()
  {
    bounceNumber = 0;
    //abilityListener = null;
    this.gameObject.SetActive(false);
    _rb.velocity = Vector3.zero;
    _rb.angularVelocity = Vector3.zero;
    //if (vfxOnExpire && SceneOverlord.Instance) SceneOverlord.Instance.PlayInteractionEffect(vfxOnExpire, transform, 1f);
  }

  public void EnableProjectile()
  {
    projectileDisabled = false;
  }

  public void DisableProjectile()
  {
    projectileDisabled = true;
  }
  #endregion

  #region MonoBehavior Callbacks
  private void OnCollisionEnter(Collision collision)
  {
    ProjectileDetection(collision);
  }

  private void OnTriggerEnter(Collider other)
  {
    ProjectileDetection(other);
  }


  private void FixedUpdate()
  {

    if (projectileType == ProjectileType.PhysicsDriven)
    {
      _rb.velocity += new Vector3(0, gravity * Time.fixedDeltaTime, 0);
    }

  }
  #endregion

  #region Private Methods
  private void ProjectileDetection(Collision collision)
  {
    if (projectileDisabled) return;

    
    TargetPlaceholder target = collision.gameObject.GetComponent<TargetPlaceholder>();
    if (target != null && GameMechanic.Instance.localPlayerSpot.GetComponent<Photon.Pun.PhotonView>().ViewID == ownerId)
    {
      //only activate this event for the owner of this projectile
      GameEvents.OnTargetHit?.Invoke(target.TargetData);
    }

    MyDebug.Log("we hit " + collision.gameObject.name);
    // local fx on collision
    if (GameMechanic.Instance.localPlayerSpot.rigRoot.gameObject == collision.transform.gameObject)
    {
      foreach (FXWrapper fx in fxOnCollisionLocal)
      {
        FXManager.Instance.GenerateFX(fx, transform);
      }
    }
    if (GameMechanic.Instance.otherPlayerSpot.rigRoot.gameObject == collision.transform.gameObject)
    {
      MyDebug.Log("We HIT the other player!", Color.yellow);
    }



    // fx on collison
    foreach (FXWrapper fx in fxOnCollision)
    {
      FXManager.Instance.GenerateFX(fx, transform);
    }

    if (bounce && (bounceNumber < bounceAmount || isInfinite))
    {
      bounceNumber++;
      switch (bounceType)
      {
        case BounceType.MaterialDriven:
          //we let the physics handle it and hope for the best

          // small invulnerability frame
          Invoke("EnableProjectile", ignoreCollisionDuration);
          break;
        case BounceType.ReverseImpact:
          //return to sender
          _rb.velocity = bounceStrength * -_rb.velocity.normalized;

          // small invulnerability frame
          Invoke("EnableProjectile", ignoreCollisionDuration);
          break;
        case BounceType.PointBreak:
          //return from the point of impact to the center of the body
          Vector3 dir = (collision.GetContact(0).point - transform.position).normalized;
          _rb.velocity = bounceStrength * -dir;

          // small invulnerability frame
          Invoke("EnableProjectile", ignoreCollisionDuration);
          break;
        case BounceType.Adhesive:
          //stick to the wall
          _rb.velocity = Vector3.zero;
          _rb.angularVelocity = Vector3.zero;
          //this.enabled = false;
          DisableProjectile();

          //destroy this object after certain time
          if (!isInfinite)
            Invoke("ResetProjectile", projectileAdhesiveDuration);
          break;
        default:
          break;
      }
    }
    else
    {
      ResetProjectile();
    }
  }
  private void ProjectileDetection(Collider collider)
  {
    if (projectileDisabled) return;

    // fx on trigger
    foreach (FXWrapper fx in fxOnTrigger)
    {
      FXManager.Instance.GenerateFX(fx, transform);
    }
  }

  private void InitialCollisionEnable()
  {
    _col.enabled = true;
  }
  #endregion
}
