using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class OnMouseEvents : MonoBehaviour
{
  public UnityEvent _OnMouseEnter;

  private void OnMouseEnter()
  {
    _OnMouseEnter?.Invoke();
  }

  public UnityEvent _OnMouseOver;

  private void OnMouseOver()
  {
    _OnMouseOver?.Invoke();
  }

  public UnityEvent _OnMouseDown;

  private void OnMouseDown()
  {
    _OnMouseDown?.Invoke();
  }

  public UnityEvent _OnMouseDrag;

  private void OnMouseDrag()
  {
    _OnMouseDrag?.Invoke();
  }

  public UnityEvent _OnMouseUp;

  private void OnMouseUp()
  {
    _OnMouseUp?.Invoke();
  }

  public UnityEvent _OnMouseExit;

  private void OnMouseExit()
  {
    _OnMouseExit?.Invoke();
  }


}
