using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Workbench.ProjectDilemma;
using System.Reflection;

/// <summary>
/// Abstract class for making reload-proof singletons out of ScriptableObjects
/// Returns the asset created on the editor, or null if there is none
/// Based on https://www.youtube.com/watch?v=VBA1QCoEAX4
/// Edited by GuruLaghima to accomodate default values
/// </summary>
/// <typeparam name="T">Singleton type</typeparam>

public abstract class SingletonScriptableObject<T> : ScriptableObjectWrapper where T : ScriptableObjectWrapper
{
  public T DefaultSettings;

  static T _instance = null;
  public static T Instance
  {
    get
    {
      if (!_instance)
      {
        List<T> objectsOfThisType = new List<T>(Resources.FindObjectsOfTypeAll<T>());
        _instance = objectsOfThisType.Find((obj) => (!obj.IsDefault));
      }
      return _instance;
    }
  }

  public void Reset()
  {
    MyDebug.Log("Resetting object of type ", this.GetType().ToString());
    if (this.DefaultSettings != null)
    {
      foreach (FieldInfo field in this.GetType().GetFields().Where(field => field.IsPublic))
      {
        MyDebug.Log("Resetting field of type ", field.Name);
        field.SetValue(this, field.GetValue(this.DefaultSettings));
      }
    }
    else
    {
      MyDebug.Log("DefaultSettings has not been assigned", Color.red);
    }
  }
}