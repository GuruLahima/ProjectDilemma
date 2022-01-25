using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using NaughtyAttributes;
#endif

[CreateAssetMenu(fileName = "InputAxisMappings", menuName = "Workbench/ScriptableObjects/InputAxisMappings", order = 1)]
public class InputAxisMappings : SingletonScriptableObject<InputAxisMappings>
{
  #if UNITY_EDITOR
  [InputAxis]
  #endif
  public string EnterChatAxis;

#if UNITY_EDITOR
  [MenuItem("CONTEXT/InputAxisMappings/Reset (Custom)")]
  static void CustomReset(MenuCommand command)
  {
    InputAxisMappings thisObj = (InputAxisMappings)command.context;
    thisObj.Reset();
  }
#endif
}
