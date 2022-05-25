using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GuruLaghima
{

  // [RequireComponent(typeof(Collider))]
  public class OnMouseEvents : MonoBehaviour
  {
    public static float globalMinDistance = 0.5f;

    public UnityEvent _OnMouseEnter;

    [SerializeField] CinemachineBrain cineBrain;

    private void Awake()
    {
      // cineBrain = Camera.main.GetComponent<CinemachineBrain>();
    }

    public void OnMouseEnter()
    {
      // Check if the mouse was clicked over a UI element
      if (EventSystem.current.IsPointerOverGameObject())
      {
        // Debug.Log("There is UI in front of game object");
        return;
      }
      // check distance of main cam to object
      if (cineBrain != null && Vector3.Distance(this.transform.position, cineBrain.OutputCamera.transform.position) > globalMinDistance)
        return;

      _OnMouseEnter?.Invoke();
    }

    public UnityEvent _OnMouseOver;

    public void OnMouseOver()
    {
      // Check if the mouse was clicked over a UI element
      if (EventSystem.current.IsPointerOverGameObject())
      {
        // Debug.Log("There is UI in front of game object");
        return;
      }
      // check distance of main cam to object
      if (cineBrain != null && Vector3.Distance(this.transform.position, cineBrain.OutputCamera.transform.position) > globalMinDistance)
        return;
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
      // check distance of main cam to object
      if (cineBrain != null && Vector3.Distance(this.transform.position, cineBrain.OutputCamera.transform.position) > globalMinDistance)
        return;
      _OnMouseDown?.Invoke();
    }

    public UnityEvent _OnMouseDrag;

    public void OnMouseDrag()
    {
      // Check if the mouse was clicked over a UI element
      if (EventSystem.current.IsPointerOverGameObject())
      {
        // Debug.Log("There is UI in front of game object");
        return;
      }
      // check distance of main cam to object
      if (cineBrain != null && Vector3.Distance(this.transform.position, cineBrain.OutputCamera.transform.position) > globalMinDistance)
        return;
      _OnMouseDrag?.Invoke();
    }

    public UnityEvent _OnMouseUp;

    public void OnMouseUp()
    {
      // Check if the mouse was clicked over a UI element
      if (EventSystem.current.IsPointerOverGameObject())
      {
        // Debug.Log("There is UI in front of game object");
        return;
      }
      // check distance of main cam to object
      if (cineBrain != null && Vector3.Distance(this.transform.position, cineBrain.OutputCamera.transform.position) > globalMinDistance)
        return;
      _OnMouseUp?.Invoke();
    }

    public UnityEvent _OnMouseExit;

    public void OnMouseExit()
    {
      // Check if the mouse was clicked over a UI element
      if (EventSystem.current.IsPointerOverGameObject())
      {
        // Debug.Log("There is UI in front of game object");
        return;
      }
      // check distance of main cam to object
      if (cineBrain != null && Vector3.Distance(this.transform.position, cineBrain.OutputCamera.transform.position) > globalMinDistance)
        return;
      _OnMouseExit?.Invoke();
    }


  }
}