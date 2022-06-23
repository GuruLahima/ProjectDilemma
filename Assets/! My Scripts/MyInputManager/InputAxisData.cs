using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
    ListOfHotkeys = new List<string>();
    foreach (InputGroup ig in inputs)
    {
      ListOfHotkeys.Add(ig.AxisName);
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

  public const string _customInputManagerPath = "Assets/Resources/MyInputManager.asset";

#if UNITY_EDITOR
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
#endif
}
