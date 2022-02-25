using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace GuruLaghima
{

  [RequireComponent(typeof(Collider))]
  public class OnMouseEvents : MonoBehaviour
  {
    public UnityEvent _OnMouseEnter;

    public void OnMouseEnter()
    {
      _OnMouseEnter?.Invoke();
    }

    public UnityEvent _OnMouseOver;

    public void OnMouseOver()
    {
      _OnMouseOver?.Invoke();
    }

    public UnityEvent _OnMouseDown;

    public void OnMouseDown()
    {
      _OnMouseDown?.Invoke();
    }

    public UnityEvent _OnMouseDrag;

    public void OnMouseDrag()
    {
      _OnMouseDrag?.Invoke();
    }

    public UnityEvent _OnMouseUp;

    public void OnMouseUp()
    {
      _OnMouseUp?.Invoke();
    }

    public UnityEvent _OnMouseExit;

    public void OnMouseExit()
    {
      _OnMouseExit?.Invoke();
    }


  }
}