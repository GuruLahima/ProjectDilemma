using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuruLaghima;

namespace Workbench.ProjectDilemma
{
  public class SwitchRagdoll : MonoBehaviour
  {
    [SerializeField] bool ragdollAtStart;
    [SerializeField] bool ragdollAtImpact;
    [SerializeField] Animator anim;
    private bool ragdollOn;


    private void Start()
    {
      // anim = GetComponent<Animator>();
      Toggle(ragdollAtStart);

      if (ragdollAtImpact)
      {
        // then toggle rigidbodies
        foreach (Rigidbody rb in transform.GetComponentsInChildren<Rigidbody>())
        {
          rb.gameObject.AddComponent<CollisionEmitter>();
        }
      }
    }


    public void Toggle(bool value)
    {
      ragdollOn = value;
      PerformSwitch();
    }
    public void Toggle()
    {
      ragdollOn = !ragdollOn;
      PerformSwitch();
    }
    public void TurnToRigidbodyOnImpact()
    {
      if (ragdollAtImpact)
      {
        ragdollOn = true;
        PerformSwitch();
      }
    }

    void PerformSwitch()
    {
      // first toggle animator
      if (anim)
        anim.enabled = !ragdollOn;

      // then toggle rigidbodies
      foreach (Rigidbody rb in transform.GetComponentsInChildren<Rigidbody>())
      {
        rb.isKinematic = !ragdollOn;
      }
    }
  }
}