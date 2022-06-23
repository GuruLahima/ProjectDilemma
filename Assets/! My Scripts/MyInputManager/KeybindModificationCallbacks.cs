using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class KeybindModificationCallbacks : MonoBehaviour
{
  public void OnEnable()
  {
    InputManager.OnKeyChanged += OnKeyChanged;
    InputManager.OnKeyAdded += OnKeyAdded;
    InputManager.OnKeyRemoved += OnKeyRemoved;
  }
  public void OnDisable()
  {
    InputManager.OnKeyChanged -= OnKeyChanged;
    InputManager.OnKeyAdded -= OnKeyAdded;
    InputManager.OnKeyRemoved -= OnKeyRemoved;
  }
  public abstract void OnKeyChanged(string buttonName);
  public abstract void OnKeyAdded(string buttonName);
  public abstract void OnKeyRemoved(string buttonName);
}