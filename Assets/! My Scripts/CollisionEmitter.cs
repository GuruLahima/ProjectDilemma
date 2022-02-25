
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuruLaghima;

namespace Workbench.ProjectDilemma
{
  public class CollisionEmitter : MonoBehaviour
  {
    private void OnTriggerEnter(Collider other)
    {
      if (other.transform.CompareTag("DeathObject"))
      {
        MyDebug.Log("Well surely there is collision?");
        SendMessageUpwards("TurnToRigidbodyOnImpact", SendMessageOptions.DontRequireReceiver);
      }
    }
  }
}