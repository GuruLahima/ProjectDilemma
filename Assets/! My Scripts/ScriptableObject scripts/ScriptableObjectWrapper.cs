using GuruLaghima;
using UnityEngine;

public class ScriptableObjectWrapper : ScriptableObject
{
  [CustomTooltip(@"this is how I filter all objects of this type. when the singleton finds an instance which is marked as not default it uses that one
   there can mbre multiple default ones but only one non-default instance which is used in the game")]
  [SerializeField] private bool isDefault = true;

  public bool IsDefault
  {
    get
    {
      return isDefault;
    }
  }
}