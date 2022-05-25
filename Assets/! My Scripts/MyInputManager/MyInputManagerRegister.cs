using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

// Register a SettingsProvider using IMGUI for the drawing framework:
static class MyInputManagerRegister
{
#if UNITY_EDITOR
  public enum InputManagerSelectionLabel { InputManager, CustomKeyMap, HotkeysList}
  
  [SettingsProvider]
  public static SettingsProvider CreateCustomInputManager()
  {
    InputManagerSelectionLabel selectButton = InputManagerSelectionLabel.InputManager;
    bool autoSaveButton = false;
    // First parameter is the path in the Settings window.
    // Second parameter is the scope of this setting: it only appears in the Project Settings window.
    var provider = new SettingsProvider("Project/Stefan's Input Manager", SettingsScope.Project)
    {
      // By default the last token of the path is used as display name if no label is provided.
      label = "Stefan's Input Manager",
      // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
      guiHandler = (searchContext) =>
      {
        var settings = InputAxisData.GetSerializedSettings();
        autoSaveButton = EditorGUILayout.Toggle("Auto Generate Hotkeys", autoSaveButton);
        settings.FindProperty("autoGenerateHotkeys").boolValue = autoSaveButton;
        selectButton = (InputManagerSelectionLabel)EditorGUILayout.EnumPopup(selectButton);
        switch (selectButton)
        {
          case InputManagerSelectionLabel.InputManager:

            EditorGUILayout.LabelField("An improved version of Unity's Input Manager");
            EditorGUILayout.PropertyField(settings.FindProperty("inputs"), new GUIContent("My Inputs"));
            break;
          case InputManagerSelectionLabel.CustomKeyMap:
            EditorGUILayout.LabelField("Custom key names and icons");
            EditorGUILayout.PropertyField(settings.FindProperty("customKeyMapping"), new GUIContent("Custom Key Map"));
            break;
          case InputManagerSelectionLabel.HotkeysList:
            EditorGUILayout.LabelField("Auto-Generated list of hotkeys from Input Manager");
            EditorGUILayout.PropertyField(settings.FindProperty("ListOfHotkeys"), new GUIContent("Hotkeys List"));
            break;
        }
      },

      // Populate the search keywords to enable smart search filtering and label highlighting:
      keywords = new HashSet<string>(InputManager.ListOfHotkeys().ToArray())
    };

    return provider;
  }
#endif
}