using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Workbench.ProjectDilemma
{
  public class RPCManager : MonoBehaviourPunCallbacks
  {
    #region Singleton
    public static RPCManager Instance;
    private void Awake()
    {
      if (!Instance)
      {
        Instance = this;
      }
      else if (Instance != this)
      {
        Destroy(this);
      }
    }
    #endregion

    #region Pun RPCs
    [PunRPC]
    void RPC_ThrowProjectile(string projectileKey, Vector3 startPosition, Vector3 direction, float strength, float customGravity)
    {
      /*
      Projectile projectile = null; // <== allocate the projectile via character inventory using projectileKey
      if (projectile)
      {
        var proj = Instantiate(projectile);
        proj.SetProjectile(startPosition, direction, strength, gravity);
      }
      */
    }
    #endregion

  }
}