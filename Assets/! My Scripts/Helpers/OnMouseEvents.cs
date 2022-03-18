using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GuruLaghima
{

  // [RequireComponent(typeof(Collider))]
  public class OnMouseEvents : MonoBehaviour
  {
    public UnityEvent _OnMouseEnter;

    public void OnMouseEnter()
    {
      // Check if the mouse was clicked over a UI element
      if (EventSystem.current.IsPointerOverGameObject())
      {
        Debug.Log("There is UI in front of game object");
        return;
      }
      _OnMouseEnter?.Invoke();
    }

    public UnityEvent _OnMouseOver;

    public void OnMouseOver()
    {
      // Check if the mouse was clicked over a UI element
      if (EventSystem.current.IsPointerOverGameObject())
      {
        Debug.Log("There is UI in front of game object");
        return;
      }
      _OnMouseOver?.Invoke();
    }

    public UnityEvent _OnMouseDown;

    public void OnMouseDown()
    {
      // Check if the mouse was clicked over a UI element
      if (EventSystem.current.IsPointerOverGameObject())
      {
        Debug.Log("There is UI in front of game object");
        return;
      }
      _OnMouseDown?.Invoke();
    }

    public UnityEvent _OnMouseDrag;

    public void OnMouseDrag()
    {
      // Check if the mouse was clicked over a UI element
      if (EventSystem.current.IsPointerOverGameObject())
      {
        Debug.Log("There is UI in front of game object");
        return;
      }
      _OnMouseDrag?.Invoke();
    }

    public UnityEvent _OnMouseUp;

    public void OnMouseUp()
    {
      // Check if the mouse was clicked over a UI element
      if (EventSystem.current.IsPointerOverGameObject())
      {
        Debug.Log("There is UI in front of game object");
        return;
      }
      _OnMouseUp?.Invoke();
    }

    public UnityEvent _OnMouseExit;

    public void OnMouseExit()
    {
      // Check if the mouse was clicked over a UI element
      if (EventSystem.current.IsPointerOverGameObject())
      {
        Debug.Log("There is UI in front of game object");
        return;
      }
      _OnMouseExit?.Invoke();
    }


  }
}