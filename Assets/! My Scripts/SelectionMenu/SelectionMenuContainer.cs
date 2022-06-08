using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class SelectionMenuContainer : MonoBehaviour
{
  public ScriptableObject container;
  public UnityEngine.UI.Image image;
  public Sprite defaultIcon;
  public UnityEvent OnSelected;
  public UnityEvent OnDeselected;
}
