using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class InputManager
{
  public const string _customInputManagerPath = "MyInputManager";

  public static System.Action<string> OnKeyAdded;
  public static System.Action<string> OnKeyRemoved;
  public static System.Action<string> OnKeyChanged;

  public static InputAxisData InputAxisData
  {
    get
    {
      if (_inputAxisData)
      {
        return _inputAxisData;
      }
      else
      {
        //this should only happen ever once
#if UNITY_EDITOR
        _inputAxisData = Resources.Load<InputAxisData>(_customInputManagerPath);
#endif
#if !UNITY_EDITOR
        _inputAxisData = Resources.Load<InputAxisData>(_customInputManagerPath);
#endif
        if (_inputAxisData)
        {
          Debug.Log("File found in the directory " + _customInputManagerPath + ", successfuly initialized InputManager");
          return _inputAxisData;
        }
        else
        {
          Debug.LogError("Missing file for InputAxisData, could NOT be found in the directory " + _customInputManagerPath);
          return null;
        }
      }
    }
  }
  private static InputAxisData _inputAxisData;
  /// <summary>
  /// Returns true during the frame the user pressed down the virtual button identified by buttonName.
  /// </summary>
  public static bool GetButtonDown(string buttonName)
  {
    if (InputAxisData)
    {
      return InputAxisData.GetButtonDown(buttonName);
    }
    return false;
  }
  /// <summary>
  /// Returns true the first frame the user releases the virtual button identified by buttonName.
  /// </summary>
  public static bool GetButtonUp(string buttonName)
  {
    if (InputAxisData)
    {
      return InputAxisData.GetButtonUp(buttonName);
    }
    return false;
  }
  /// <summary>
  /// Returns true while the virtual button identified by buttonName is held down.
  /// </summary>
  /// <returns>True when an axis has been pressed and not released.</returns>
  public static bool GetButton(string buttonName)
  {
    if (InputAxisData)
    {
      return InputAxisData.GetButton(buttonName);
    }
    return false;
  }
  /// <summary>
  /// Returns the value of the virtual axis identified by axisName with no smoothing applied.
  /// </summary>
  public static float GetAxisRaw(string axisName)
  {
    if (InputAxisData)
    {
      return InputAxisData.GetAxisRaw(axisName);
    }
    return 0;
  }
  /// <summary>
  /// Returns the value of the virtual axis identified by axisName with smoothing.
  /// </summary>
  public static float GetAxis(string axisName)
  {
    if (InputAxisData)
    {
      return InputAxisData.GetAxis(axisName);
    }
    return 0;
  }

  public static bool ChangeKeyProperty(KeyCode key, string buttonName)
  {
    if (InputAxisData)
    {
      InputAxisData.InputGroup IG = InputAxisData.GetInputGroup(buttonName);
      if (IG != null)
      {
        int i = IG.FindBind(key);
        if (i >= 0)
        {
          InputAxisData.InputGroup.KeyBinds bind = IG.Binds[i];
          bind.Key = key;
          OnKeyChanged?.Invoke(buttonName);
          return true;
        }
      }
    }
    return false;
  }
  public static bool ChangeKeyProperty(KeyCode key, string buttonName, InputAxisData.InputGroup.KeyBinds.KeyCodeValue value)
  {
    if (InputAxisData)
    {
      InputAxisData.InputGroup IG = InputAxisData.GetInputGroup(buttonName);
      if (IG != null)
      {
        int i = IG.FindBind(key);
        if (i >= 0)
        {
          InputAxisData.InputGroup.KeyBinds bind = IG.Binds[i];
          bind.Key = key;
          bind.Value = value;
          OnKeyChanged?.Invoke(buttonName);
          return true;
        }
      }
    }
    return false;
  }
  public static bool AddKeyProperty(KeyCode key, string buttonName, InputAxisData.InputGroup.KeyBinds.KeyCodeValue value = InputAxisData.InputGroup.KeyBinds.KeyCodeValue.Positive)
  {
    if (InputAxisData)
    {
      InputAxisData.InputGroup IG = InputAxisData.GetInputGroup(buttonName);
      if (IG != null)
      {
        // check if we already have bind to that key
        if (IG.ContainsKey(key))
        {
          return false;
        }
        else
        {
          InputAxisData.InputGroup.KeyBinds newKeyBind = new InputAxisData.InputGroup.KeyBinds();
          newKeyBind.Key = key;
          newKeyBind.Value = value;
          IG.Binds.Add(newKeyBind);
          OnKeyAdded?.Invoke(buttonName);
          return true;
        }
      }
    }
    return false;
  }
  public static bool RemoveKeyProperty(KeyCode key, string buttonName)
  {
    if (InputAxisData)
    {
      InputAxisData.InputGroup IG = InputAxisData.GetInputGroup(buttonName);
      if (IG != null)
      {
        int i = IG.FindBind(key);
        if (i >= 0)
        {
          IG.Binds.RemoveAt(i);
          OnKeyRemoved?.Invoke(buttonName);
          return true;
        }
      }
    }
    return false;
  }
  /// <summary>
  /// Searches through predefined list of mapped keys with their corresponding key name
  /// </summary>
  /// <returns>The custom name that matches the <paramref name="key"/> parameter, otherwise returns the KeyCode as a string</returns>
  public static string GetKeyName(KeyCode key)
  {
    if (InputAxisData)
    {
      return InputAxisData.GetKeyName(key);
    }
    return key.ToString();
  }
  /// <summary>
  /// Searches through predefined list of mapped keys with their corresponding key icon
  /// </summary>
  /// <returns>The icon that matches the <paramref name="key"/> parameter</returns>
  public static Sprite GetKeyIcon(KeyCode key)
  {
    if (InputAxisData)
    {
      return InputAxisData.GetKeyIcon(key);
    }
    return null;
  }
  /// <summary>
  /// Searches through predefined list of mapped keys, and assigns <paramref name="text"/> and <paramref name="sprite"/> for the matching item in the list
  /// </summary>
  /// <returns>True if the <paramref name="key"/> parameter matches item in the list</returns>
  public static bool GetKeyMap(KeyCode key, out string text, out Sprite sprite)
  {
    text = key.ToString();
    sprite = null;
    if (InputAxisData)
    {
      if (InputAxisData.GetKeyMap(key, out text, out sprite))
      {
        return true;
      }
    }
    return false;
  }
  /// <summary>
  /// Finds an input that matches <paramref name="buttonName"/>, fetches the first key if any 
  /// <para>Searches through predefined list of mapped keys with their corresponding key name</para>
  /// </summary>
  /// <returns>The custom name that matches the key, otherwise returns the KeyCode as a string</returns>
  public static string GetKeyNameByInput(string buttonName)
  {
    if (InputAxisData)
    {
      InputAxisData.InputGroup IG = InputAxisData.GetInputGroup(buttonName);
      if (IG != null)
      {
        if (IG.Binds.Count > 0)
        {
          return InputAxisData.GetKeyName(IG.Binds[0].Key);
        }
      }
    }
    return string.Empty;
  }
  /// <summary>
  /// Finds an input that matches <paramref name="buttonName"/>, fetches the first key if any 
  /// <para>Searches through predefined list of mapped keys with their corresponding key icon</para>
  /// </summary>
  /// <returns>The icon that matches the key</returns>
  public static Sprite GetKeyIconByInput(string buttonName)
  {
    if (InputAxisData)
    {
      InputAxisData.InputGroup IG = InputAxisData.GetInputGroup(buttonName);
      if (IG != null)
      {
        if (IG.Binds.Count > 0)
        {
          return InputAxisData.GetKeyIcon(IG.Binds[0].Key);
        }
      }
    }
    return null;
  }
  /// <summary>
  /// Finds an input that matches <paramref name="buttonName"/>, fetches the first key if any 
  /// <para>Searches through predefined list of mapped keys, and assigns <paramref name="text"/> and <paramref name="sprite"/> for the matching item in the list</para>
  /// </summary>
  /// <returns>True if the key matches item in the list</returns>
  public static bool GetKeyMapByInput(string buttonName, out string text, out Sprite sprite)
  {
    text = string.Empty;
    sprite = null;
    if (InputAxisData)
    {
      InputAxisData.InputGroup IG = InputAxisData.GetInputGroup(buttonName);
      if (IG != null)
      {
        if (IG.Binds.Count > 0)
        {
          return InputAxisData.GetKeyMap(IG.Binds[0].Key, out text, out sprite);
        }
      }
    }
    return false;
  }
  /// <returns>
  /// Copy of a predefined list of hotkeys
  /// </returns>
  public static List<string> ListOfHotkeys()
  {
    if (InputAxisData)
    {
      return InputAxisData.ListOfHotkeys;
    }
    else
    {
      return new List<string>();
    }
  }
}


