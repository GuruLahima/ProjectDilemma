using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace GuruLaghima
{


  public class OnKeyEvents : MonoBehaviour
  {
    [InputAxis]
    [SerializeField] string inputAxis;

    public UnityEvent _OnButtonDown;

    public UnityEvent _OnButton;

    public UnityEvent _OnButtonUp;


    private void Update()
    {
      if (Input.GetButtonDown(inputAxis))
        _OnButtonDown?.Invoke();
      if (Input.GetButton(inputAxis))
        _OnButton?.Invoke();
      if (Input.GetButtonUp(inputAxis))
        _OnButtonUp?.Invoke();

    }


  }
}
