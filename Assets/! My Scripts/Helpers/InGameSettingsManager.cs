using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Workbench.ProjectDilemma;
using UnityEngine.SceneManagement;
using GuruLaghima;
using Michsky.UI.ModernUIPack;

public class InGameSettingsManager : MonoBehaviour
{
  public static InGameSettingsManager Instance;
  public List<InGameSetting> settings = new List<InGameSetting>();

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      if (Instance != this)
        Destroy(this);
    }
    // DontDestroyOnLoad(this.gameObject);
  }

  private void OnEnable()
  {
    RegisterEventListeners();
    MapSettingsToScriptableObjects();

    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
  {
    MyDebug.Log("Loaded scene", arg0.name);
    LoadSettings();
  }

  /// <summary>
  /// We are mapping settings in the InGameSettingsManager to scriptable objects so we can reference specific settings
  /// from anywhere in the game. The Scriptable object holds the key of the PlayerPref.
  /// </summary>
  private void MapSettingsToScriptableObjects()
  {
    foreach (InGameSetting setting in settings)
    {
      setting.settingSO.settingKey = setting.settingKey;
    }
  }


  // load settings
  public void LoadSettings()
  {
    foreach (InGameSetting setting in settings)
    {
      LoadSingleSetting(setting);
    }
  }

  /// <summary>
  /// load single setting from disk and show it in UI
  /// </summary>
  public void LoadSingleSetting(InGameSetting setting)
  {
    FetchDataFromStorage(setting);
    // * mind you, when we apply data to ui elements they also trigger the event listeners that we registered previously
    // * (which means we not only load the settings from disk and show it in the ui but also apply the implementations of those settings )
    // * BUT ONLY IF THE VALUE IS CHANGED!
    ApplyDataToUIElement(setting);
    // * which is why we must apply the settings here as well. so first the settings are loaded then they are applied twice (potentially)
    ApplySetting(setting);
  }

  private void ApplySetting(InGameSetting setting)
  {
    MyDebug.Log("Applying setting", setting.settingKey);
    setting.OnValueChanged?.Invoke(setting.settingKey);
  }

  private void RegisterEventListeners()
  {
    foreach (InGameSetting setting in settings)
    {
      switch (setting.elementType)
      {
        case UIElementType.Slider:
          if (setting.uiElementReference && setting.uiElementReference.GetComponent<Slider>())
          {
            // listener for saving the value to disk
            setting.uiElementReference.GetComponent<Slider>().onValueChanged.AddListener((val) => { SaveSingleSetting(setting); });
            // listener for executing event on change of value
            setting.uiElementReference.GetComponent<Slider>().onValueChanged.AddListener((val) => { setting.OnValueChanged?.Invoke(val); });

          }
          break;
        case UIElementType.Toggle:
          if (setting.uiElementReference && setting.uiElementReference.GetComponent<Toggle>())
          {
            setting.uiElementReference.GetComponent<Toggle>().onValueChanged.AddListener((val) => { SaveSingleSetting(setting); });
            setting.uiElementReference.GetComponent<Toggle>().onValueChanged.AddListener((val) => { setting.OnValueChanged?.Invoke(val); });
          }
          break;
        case UIElementType.InputField:
          if (setting.uiElementReference && setting.uiElementReference.GetComponent<TMP_InputField>())
          {
            setting.uiElementReference.GetComponent<TMP_InputField>().onValueChanged.AddListener((val) => { SaveSingleSetting(setting); });
            setting.uiElementReference.GetComponent<TMP_InputField>().onValueChanged.AddListener((val) => { setting.OnValueChanged?.Invoke(val); });
          }
          break;
        case UIElementType.Dropdown:
          if (setting.uiElementReference && setting.uiElementReference.GetComponent<TMP_Dropdown>())
          {
            setting.uiElementReference.GetComponent<TMP_Dropdown>().onValueChanged.AddListener((val) => { SaveSingleSetting(setting); });
            setting.uiElementReference.GetComponent<TMP_Dropdown>().onValueChanged.AddListener((val) => { setting.OnValueChanged?.Invoke(val); });
          }
          break;
        case UIElementType.HorizontalSelector:
          if (setting.uiElementReference && setting.uiElementReference.GetComponent<HorizontalSelector>())
          {
            // setting.uiElementReference.GetComponent<HorizontalSelector>().selectorEvent.AddListener((val) => { MyDebug.Log("whatever", val); });
            setting.uiElementReference.GetComponent<HorizontalSelector>().selectorEvent.AddListener((val) => { SaveSingleSetting(setting); });
            setting.uiElementReference.GetComponent<HorizontalSelector>().selectorEvent.AddListener((val) => { setting.OnValueChanged?.Invoke(val); });

          }
          break;
        default:
          break;
      }

    }
  }


  public void SomeFunction() { }

  void FetchDataFromStorage(InGameSetting setting)
  {
    // 
    MyDebug.Log("fetch data for setting", setting.settingKey);

    if (PlayerPrefs.HasKey(setting.settingKey))
    {
      switch (setting.valueType)
      {
        case ValueType.Float:
          float valf = PlayerPrefs.GetFloat(setting.settingKey, 0);
          setting.settingValue = valf;
          MyDebug.Log("setting val", setting.settingValue);
          break;
        case ValueType.Int:
          int vali = PlayerPrefs.GetInt(setting.settingKey, 0);
          setting.settingValue = vali;
          MyDebug.Log("setting val", setting.settingValue);
          break;
        case ValueType.String:
          string vals = PlayerPrefs.GetString(setting.settingKey, "");
          setting.settingValue = vals;
          MyDebug.Log("setting val", setting.settingValue);
          break;
        default:
          setting.settingValue = null;
          break;
      }
    }
    else
    {
      switch (setting.valueType)
      {
        case ValueType.Float:
          setting.settingValue = setting.defaultValueFloat;
          MyDebug.Log("setting val", setting.settingValue);
          break;
        case ValueType.Int:
          setting.settingValue = setting.defaultValueInt;
          MyDebug.Log("setting val", setting.settingValue);
          break;
        case ValueType.String:
          setting.settingValue = setting.defaultValueString;
          MyDebug.Log("setting val", setting.settingValue);
          break;
        default:
          setting.settingValue = null;
          break;
      }
    }
  }

  private void ApplyDataToUIElement(InGameSetting setting)
  {
    if (setting.settingValue != null)
    {
      switch (setting.elementType)
      {
        case UIElementType.Slider:
          if (setting.uiElementReference && setting.uiElementReference.GetComponent<Slider>())
          {
            float mappedValue = GuruLaghima.HelperFunctions.remap((float)setting.settingValue,
          setting.min_Value,
          setting.max_Value,
          setting.uiElementReference.GetComponent<Slider>().minValue,
          setting.uiElementReference.GetComponent<Slider>().maxValue,
          true,
          true,
          true,
          true);
            setting.uiElementReference.GetComponent<Slider>().value = mappedValue;
          }
          break;
        case UIElementType.Toggle:
          if (setting.uiElementReference && setting.uiElementReference.GetComponent<Toggle>())
          {
            setting.uiElementReference.GetComponent<Toggle>().isOn = (int)setting.settingValue == 0 ? false : true;
          }
          break;
        case UIElementType.InputField:
          if (setting.uiElementReference && setting.uiElementReference.GetComponent<TMP_InputField>())
          {
            setting.uiElementReference.GetComponent<TMP_InputField>().text = (string)setting.settingValue;
          }
          break;
        case UIElementType.Dropdown:
          if (setting.uiElementReference && setting.uiElementReference.GetComponent<TMP_Dropdown>())
          {
            setting.uiElementReference.GetComponent<TMP_Dropdown>().value = (int)setting.settingValue;
          }
          break;
        case UIElementType.HorizontalSelector:
          if (setting.uiElementReference && setting.uiElementReference.GetComponent<HorizontalSelector>())
          {
            setting.uiElementReference.GetComponent<HorizontalSelector>().defaultIndex = (int)setting.settingValue;
            setting.uiElementReference.GetComponent<HorizontalSelector>().index = (int)setting.settingValue;
            setting.uiElementReference.GetComponent<HorizontalSelector>().UpdateUI();
          }
          break;
        default:
          break;
      }
    }

  }

  // save settings
  public void SaveSettings()
  {
  }


  /// <summary>
  /// Save a setting to disk
  /// </summary>
  /// <param name="setting"></param>
  public void SaveSingleSetting(InGameSetting setting)
  {
    MyDebug.Log("saving setting", setting.settingKey);
    switch (setting.elementType)
    {
      case UIElementType.Slider:
        if (setting.uiElementReference && setting.uiElementReference.GetComponent<Slider>())
        {
          float newValue = setting.uiElementReference.GetComponent<Slider>().value;
          // we need to map the value from the slider to a value that makes sense in AudioMixer context (attenuation goes from -80 to 20)
          float mappedValue = GuruLaghima.HelperFunctions.remap(newValue,
                              setting.uiElementReference.GetComponent<Slider>().minValue,
                              setting.uiElementReference.GetComponent<Slider>().maxValue,
                              setting.min_Value,
                              setting.max_Value,
                              true,
                              true,
                              true,
                              true);
          PlayerPrefs.SetFloat(setting.settingKey, mappedValue);
          setting.settingValue = mappedValue;
        }
        break;
      case UIElementType.Toggle:
        if (setting.uiElementReference && setting.uiElementReference.GetComponent<Toggle>())
        {
          bool newValue = setting.uiElementReference.GetComponent<Toggle>().isOn;
          int mappedValue = newValue ? 1 : 0;
          PlayerPrefs.SetInt(setting.settingKey, mappedValue);
          setting.settingValue = mappedValue;
        }
        break;
      case UIElementType.InputField:
        if (setting.uiElementReference && setting.uiElementReference.GetComponent<TMP_InputField>())
        {
          string newValue = setting.uiElementReference.GetComponent<TMP_InputField>().text;
          PlayerPrefs.SetString(setting.settingKey, newValue);
          setting.settingValue = newValue;
        }
        break;
      case UIElementType.Dropdown:
        if (setting.uiElementReference && setting.uiElementReference.GetComponent<TMP_Dropdown>())
        {
          int newValue = setting.uiElementReference.GetComponent<TMP_Dropdown>().value;
          PlayerPrefs.SetInt(setting.settingKey, newValue);
          setting.settingValue = newValue;
        }
        break;
      case UIElementType.HorizontalSelector:
        if (setting.uiElementReference && setting.uiElementReference.GetComponent<HorizontalSelector>())
        {
          int newValue = setting.uiElementReference.GetComponent<HorizontalSelector>().index;
          MyDebug.Log("saving horizontalselector value", newValue);
          PlayerPrefs.SetInt(setting.settingKey, newValue);
          setting.settingValue = newValue;
        }
        break;
      default:
        break;
    }
  }

}

[System.Serializable]
public class InGameSetting
{
  /** 
   * todo: eventually this class should hold only a reference to the InGameSettingSO and to the ui element, nothing more. 
   * todo: all the data pertinent to the setting should be stored in the InGameSettingSO ScriptableObjects
  */
  public string settingKey;
  public InGameSettingSO settingSO;
  public GameObject uiElementReference;
  // todo: valueType depends on elementType, so with NaughtyAttributes I could assign the proper value to valueType depending on elementType
  public ValueType valueType;
  public UIElementType elementType;
  public object settingValue;
  // todo: if I use NaughtyAttributes I can make it show only the proper default value depending on the valueType
  public float defaultValueFloat;
  public int defaultValueInt;
  public string defaultValueString;
  // todo: min max values work only for float types so they should be hidden unless elementType is slider
  public float min_Value;
  public float max_Value;
  public UnityEvent<object> OnValueChanged;
}

public enum ValueType
{
  Float,
  Int,
  String,
}
public enum UIElementType
{
  Slider,
  Toggle,
  InputField,
  Dropdown,
  HorizontalSelector,

}
