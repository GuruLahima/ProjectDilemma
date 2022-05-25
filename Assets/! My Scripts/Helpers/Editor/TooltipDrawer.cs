// The property drawer class should be placed in an editor script, inside a folder called Editor.

// Tell the RangeDrawer that it is a drawer for properties with the RangeAttribute.
using UnityEngine;
using UnityEditor;
using GuruLaghima;

[CustomPropertyDrawer(typeof(CustomTooltipAttribute))]
public class TooltipDrawer : PropertyDrawer
{
  private Texture2D tooltipNotificationImage;

  private void OnEnable()
  {
  }
  // Draw the property inside the given rect
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    if (!tooltipNotificationImage)
      tooltipNotificationImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/! My Scripts/Helpers/Editor/Tooltip_16x16.png", typeof(Texture2D));

    // First get the attribute
    CustomTooltipAttribute tooltip = attribute as CustomTooltipAttribute;
    label.image = tooltipNotificationImage;
    // label.text = "lalala";
    label.tooltip = tooltip.tooltip;
    EditorGUI.PropertyField(position, property, label);
    // EditorGUILayout.PropertyField(property, );

    // EditorGUILayout.PropertyField()


    // // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
    // if (property.propertyType == SerializedPropertyType.Float)
    //   EditorGUI.Slider(position, property, range.min, range.max, label);
    // else if (property.propertyType == SerializedPropertyType.Integer)
    //   EditorGUI.IntSlider(position, property, Convert.ToInt32(range.min), Convert.ToInt32(range.max), label);
    // else
    //   EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
  }


}