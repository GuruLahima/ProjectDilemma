using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class SelectionMenuContainer : SelectionMenuItem
{
  public ScriptableObject container;
  public UnityEngine.UI.Image image;
}
