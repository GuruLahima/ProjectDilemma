using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(InputNameAttribute))]
public class InputNameInspector : PropertyDrawer
{
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    if (property.propertyType == SerializedPropertyType.String)
    {
      string[] displayedOptions = InputManager.ListOfHotkeys().ToArray();
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
