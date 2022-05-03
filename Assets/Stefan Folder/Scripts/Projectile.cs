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
  //[SerializeField] [Layer] int explosionlayerMask;

  [Header("Custom Properties", order = 0)]
  [HorizontalLine(order = 1)]
  [Foldout("Impact Trigger")][SerializeField] private bool hit;
  [Foldout("Impact Trigger")][SerializeField] private bool explode;
  [Foldout("Impact Trigger")][SerializeField] private float explodeRadius;
  [Foldout("Impact Trigger")][SerializeField] private bool bounce;
  [Foldout("Impact Trigger")][SerializeField] private bool isInfinite;
  [Foldout("Impact Trigger")][SerializeField][DisableIf("isInfinite")] private int bounceAmount;
  [InfoBox("If you want to preserve the initial strength leave this field to -1", EInfoBoxType.Normal)]
  [Foldout("Impact Trigger")][SerializeField] private float bounceStrength = -1;
  [InfoBox("Different bounce types require higher collision ignore window in order to work properly!", EInfoBoxType.Warning)]
  [Foldout("Impact Trigger")][SerializeField] private BounceType bounceType;
  [Tooltip("How long should the projectile last after initial bounce? \n" +
      "Main use of this property is for Adhesive type of projectiles")]
  [Foldout("Impact Trigger")][SerializeField] private float projectileAdhesiveDuration = 5f;
  [Tooltip("Total lifetime of the projectile from the moment it was launched")]
  [Foldout("Impact Trigger")][SerializeField] private float projectileLifeLimit = 8f;

  [SerializeField] ProjectileType projectileType;
  [InfoBox("Rigidbody Use Gravity should be false at all times \n" +
      "This script contains built-in gravity", EInfoBoxType.Warning)]
  [SerializeField] float gravity = Physics.gravity.y;
  [SerializeField] List<VFXWrapper> vfxOnThrown = new List<VFXWrapper>();
  [SerializeField] List<VFXWrapper> vfxOnCollision = new List<VFXWrapper>();
  [SerializeField] List<VFXWrapper> vfxOnTrigger = new List<VFXWrapper>();
  [SerializeField] List<VFXWrapper> vfxOnCollisionLocal = new List<VFXWrapper>();
  //[SerializeField] [EnableIf("bounceType", BounceType.Adhesive)] GameObject vfxOnExpire;
  [SerializeField] float ignoreCollisionDuration = 0.05f;
  [SerializeField] float initialCollisionDisabledDuration = 0.05f;
  #endregion

  #region Private Fields
  private int bounceNumber = 0;
  //private int arenaPointsMultiplier = 1;//1 should be default value
  private bool projectileDisabled = false;
  #endregion

  #region Public Methods
  public void SetProjectile(Vector3 startPosition, Vector3 direction, float strength, float? customGravity = null) //int pointsMultiplier = 1/*the value from the arena*/)
  {
    // set gravity (some of these things I will apply in wolfsbane) ! remove this comment later
    if (customGravity != null)
      gravity = (float)customGravity;
    gravity = (projectileType == ProjectileType.PhysicsDriven) ? gravity : 0f;

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
    transform.rotation = Quaternion.LookRotation(direction);

    // vfx on cast
    foreach (VFXWrapper vfx in vfxOnThrown)
    {
      VFXManager.Instance.GenerateVFX(vfx, transform);
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
      _rb.velocity += new Vector3(0, gravity /* 2*/ * Time.fixedDeltaTime, 0);
    }

  }
  #endregion

  #region Private Methods
  private void ProjectileDetection(Collision collision)
  {
    if (projectileDisabled) return;



    //if (projectileOwner != null)
    //{
    //  // we create list for the objects in radius and a single variable for the object hit
    //  Collider[] objectsInRadius = new Collider[0];
    //  Collider objectHit = null;
    //  if (hit)
    //    objectHit = collision.collider;
    //  if (explode)
    //    objectsInRadius = Physics.OverlapSphere(transform.position, explodeRadius, explosionlayerMask);
    //  projectileOwner.SendMessage(ProjectileDataReturn(objectHit, objectsInRadius, this, arenaPointsMultiplier);
    //}


    // local vfx on collision
    if (GameMechanic.Instance.localPlayerSpot.playerModel == collision.transform.gameObject)
    {
      foreach (VFXWrapper vfx in vfxOnCollisionLocal)
      {
        VFXManager.Instance.GenerateVFX(vfx, transform);
      }
    }



    // vfx on collison
    foreach (VFXWrapper vfx in vfxOnCollision)
    {
      VFXManager.Instance.GenerateVFX(vfx, transform);
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

    //if (abilityListener)
    //{
    //  // we create list for the objects in radius and a single variable for the object hit
    //  Collider[] objectsInRadius = new Collider[0];
    //  Collider objectHit = null;
    //  if (hit)
    //    objectHit = collider;
    //  if (explode)
    //    objectsInRadius = Physics.OverlapSphere(transform.position, explodeRadius, explosionlayerMask);
    //  abilityListener.ProjectileDataReturn(objectHit, objectsInRadius, this, arenaPointsMultiplier);
    //}


    // vfx on trigger
    foreach (VFXWrapper vfx in vfxOnTrigger)
    {
      VFXManager.Instance.GenerateVFX(vfx, transform);
    }
  }

  private void InitialCollisionEnable()
  {
    _col.enabled = true;
  }
  #endregion


  // ! i used this for pooling systems in Wolfsbane, if we need to create pooling system here as well simply uncomment this and copy the pooling system from wolfsbane
  /*[System.Serializable]
  public class Pool
  {
    [ReadOnly] public string name;
    [OnValueChanged("OnValueChangedCallback")]
    [AllowNesting]
    public Projectile Prefab;
    public int size;


    private void OnValueChangedCallback()
    {
      if (Prefab)
        name = Prefab.ProjectileName;
    }
  }*/
}

[System.Serializable]
public class VFXWrapper
{
  public GameObject VFXPrefab;
  public float VFXDuration;
  public bool VFXFollowsObject;
}