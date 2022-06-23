using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GuruLaghima
{

  public class ActionOnDelay : MonoBehaviour
  {
    public float delay;
    public UnityEvent ActionToTriggerOnDelay;

    void Start(){
      Invoke("InvokeDelayedACtion", delay);
    }

    void InvokeDelayedACtion(){
      ActionToTriggerOnDelay?.Invoke();
    }

  }
}