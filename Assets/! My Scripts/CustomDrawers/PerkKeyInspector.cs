using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Workbench.ProjectDilemma;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(PerkKeyAttribute))]
public class PerkKeyInspector : PropertyDrawer
{
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    if (PerkKeys.CompleteListOfKeys.Length < 1)
    {
      Debug.LogError("No keys are defined");
      return;
    }

    if (property.propertyType == SerializedPropertyType.String)
    {
      string[] displayedOptions = PerkKeys.CompleteListOfKeys; 
      int _selectedIndex = 0;
      if (System.Array.Exists(displayedOptions, s => s == property.stringValue))
      {
        _selectedIndex = System.Array.IndexOf(displayedOptions, property.stringValue);
      }
      _selectedIndex = EditorGUI.Popup(position, _selectedIndex, displayedOptions);
      property.stringValue = displayedOptions[_selectedIndex];
    }
    else
    {
      Debug.LogError("Property " + property.name + " is not of a matching type!");
    }
  }
}
#endif