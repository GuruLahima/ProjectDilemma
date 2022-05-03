using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
[CreateAssetMenu(fileName = "New InputAxis", menuName = "Workbench/InputAxis")]
public class InputAxisData : ScriptableObject
{

  [System.Serializable]
  public class InputGroup
  {
    public string AxisName;
    public List<KeyBinds> Binds = new List<KeyBinds>();
    public AnimationCurve inputCurve;
    public float SmoothingStep = 0.1f;

    private float deltaSmoothing;
    private float initValue;
    private float initTime;

    public bool GetKeyDown()
    {
      int value = 0;
      foreach (KeyBinds bind in Binds)
      {
        if (Input.GetKeyDown(bind.Key))
        {
          value++;
        }
      }
      return (value == 1) ? true : false;
    }
    public bool GetKeyUp()
    {
      int value = 0;
      foreach (KeyBinds bind in Binds)
      {
        if (Input.GetKeyUp(bind.Key))
        {
          value++;
        }
      }
      return (value == 1) ? true : false;
    }
    public bool GetKey()
    {
      int value = 0;
      foreach (KeyBinds bind in Binds)
      {
        if (Input.GetKey(bind.Key))
        {
          value++;
        }
      }
      return (value == 1) ? true : false;
    }
    public float GetAxis()
    {
      // !: initialization
      if (initTime > Time.time)
      {
        initTime = Time.time;
        initValue = 0f;
        deltaSmoothing = 0f;
      }

      float value = 0;
      foreach (KeyBinds bind in Binds)
      {
        if (Input.GetKey(bind.Key))
        {
          value += (int)bind.Value;
        }
      }
      if (value == 0)
      {
        deltaSmoothing -= SmoothingStep;
        initTime = Time.time - 1 + deltaSmoothing;
      }
      else
      {
        deltaSmoothing = Mathf.Clamp01(deltaSmoothing + SmoothingStep);
        initTime = Time.time - 1 + deltaSmoothing;
        initValue = Mathf.Clamp(value, -1f, 1f);
      }
      var f = 1 - Mathf.Clamp01(Time.time - initTime);
      value = inputCurve.Evaluate(f) * initValue;
      return Mathf.Clamp(value, -1f, 1f);
    }
    public int GetAxisRaw()
    {
      int value = 0;
      foreach (KeyBinds bind in Binds)
      {
        if (Input.GetKey(bind.Key))
        {
          value += (int)bind.Value;
        }
      }
      return Mathf.Clamp(value, -1, 1);
    }
    public bool ContainsKey(KeyCode key)
    {
      foreach (KeyBinds bind in Binds)
      {
        if (bind.Key == key)
        {
          return true;
        }
      }
      return false;
    }
    public int FindBind(KeyCode key)
    {
      for (int i = 0; i < Binds.Count; i++)
      {
        if (Binds[i].Key == key)
        {
          return i;
        }
      }
      return -1;
    }


    [System.Serializable]
    public struct KeyBinds
    {
      public enum KeyCodeValue : sbyte { Positive = 1, Negative = -1}
      public KeyCode Key;
      public KeyCodeValue Value;
    }
  }
  [SerializeField] List<InputGroup> inputs = new List<InputGroup>();
  public bool GetButtonDown(string keyName)
  {
    foreach (InputGroup _inpt in inputs)
    {
      if (string.Equals(_inpt.AxisName, keyName))
      {
        return _inpt.GetKeyDown();
      }
    }
    return false;
  }
  public bool GetButton(string keyName)
  {
    foreach (InputGroup _inpt in inputs)
    {
      if (string.Equals(_inpt.AxisName, keyName))
      {
        return _inpt.GetKey();
      }
    }
    return false;
  }
  public bool GetButtonUp(string keyName)
  {
    foreach (InputGroup _inpt in inputs)
    {
      if (string.Equals(_inpt.AxisName, keyName))
      {
        return _inpt.GetKeyUp();
      }
    }
    return false;
  }
  public int GetAxisRaw(string keyName)
  {
    foreach (InputGroup _inpt in inputs)
    {
      if (string.Equals(_inpt.AxisName, keyName))
      {
        return _inpt.GetAxisRaw();
      }
    }
    return 0;
  }
  public float GetAxis(string keyName)
  {
    foreach (InputGroup _inpt in inputs)
    {
      if (string.Equals(_inpt.AxisName, keyName))
      {
        return _inpt.GetAxis();
      }
    }
    return 0;
  }
  public InputGroup GetInputGroup(string buttonName)
  {
    foreach (InputGroup ig in inputs)
    {
      if (string.Equals(ig.AxisName, buttonName))
      {
        return ig;
      }
    }
    return null;
  }

  [ReadOnly]
  [SerializeField]
  public List<string> ListOfHotkeys = new List<string>();
  [SerializeField] bool autoGenerateHotkeys;
#if UNITY_EDITOR
  private void OnValidate()
  {
    if (autoGenerateHotkeys)
    {
      ListOfHotkeys = new List<string>();
      foreach (InputGroup ig in inputs)
      {
        ListOfHotkeys.Add(ig.AxisName);
      }
    }
  }
#endif

  [System.Serializable]
  public class KeyPortrayal
  {
    public string TextDisplay;
    public Sprite IconDisplay;
    public KeyCode Key;
  }
  [SerializeField] List<KeyPortrayal> customKeyMapping = new List<KeyPortrayal>();

  public string GetKeyName(KeyCode key)
  {
    foreach (KeyPortrayal kp in customKeyMapping)
    {
      if (kp.Key == key)
      {
        return kp.TextDisplay;
      }
    }
    return key.ToString();
  }
  public Sprite GetKeyIcon(KeyCode key)
  {
    foreach (KeyPortrayal kp in customKeyMapping)
    {
      if (kp.Key == key)
      {
        return kp.IconDisplay;
      }
    }
    return null;
  }
  public bool GetKeyMap(KeyCode key, out string text, out Sprite sprite)
  {
    foreach (KeyPortrayal kp in customKeyMapping)
    {
      if (kp.Key == key)
      {
        text = kp.TextDisplay;
        sprite = kp.IconDisplay;
        return true;
      }
    }
    text = key.ToString();
    sprite = null;
    return false;
  }

  public const string _customInputManagerPath = "Assets/Stefan Folder/CustomInputManager/MyInputManager.asset";
  internal static InputAxisData GetOrCreateSettings()
  {
    var settings = AssetDatabase.LoadAssetAtPath<InputAxisData>(_customInputManagerPath);
    if (settings == null)
    {
      settings = ScriptableObject.CreateInstance<InputAxisData>();
      AssetDatabase.CreateAsset(settings, _customInputManagerPath);
      AssetDatabase.SaveAssets();
    }
    return settings;
  }

  internal static SerializedObject GetSerializedSettings()
  {
    return new SerializedObject(GetOrCreateSettings());
  }
}

public static class InputManager
{
  public const string _customInputManagerPath = "Assets/Stefan Folder/CustomInputManager/MyInputManager.asset";

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
        _inputAxisData = AssetDatabase.LoadAssetAtPath<InputAxisData>(_customInputManagerPath);
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


