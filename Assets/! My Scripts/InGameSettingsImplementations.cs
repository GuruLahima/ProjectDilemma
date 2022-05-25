using System;
using System.Collections.Generic;
using System.Linq;
using DigitalRuby.SimpleLUT;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using GuruLaghima;
using Workbench.ProjectDilemma;
using UnityEngine.SceneManagement;
using Michsky.UI.ModernUIPack;

public class InGameSettingsImplementations : MonoBehaviour
{

  [SerializeField] InGameSettingsManager settingsManager;
  [SerializeField] AudioMixer mainMixer;
  // [SerializeField] TMP_Dropdown resolutionDropdown;
  [SerializeField] HorizontalSelector resolutionSelector;


  public static List<Camera> allCamsInScene = new List<Camera>();

  private void Awake()
  {
    //set up resolution list
    InitResolution();
    SceneManager.sceneLoaded += OnSceneLoaded;

  }

  private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
  {
    UpdateAllCamsWithLUTList();
  }

  private void UpdateAllCamsWithLUTList()
  {
    // allCamsInScene.AddRange(Camera.allCameras.Where(c => c.GetComponent<SimpleLUT>() != null).ToList<Camera>());
    // allCamsInScene = allCamsInScene.Distinct().ToList<Camera>();

    List<Camera> camerasWithLUT = new List<Camera>(FindObjectsOfType<Camera>(true).Where(c => c.GetComponent<SimpleLUT>() != null));
    allCamsInScene.AddRange(camerasWithLUT);

  }


  /// <summary>
  /// Applies the audio setting based on the commonKey parameter. CommonKey is both the key of the setting in 
  /// InGameSettingsManager and the name of the exposed property in the Main AudioMixer
  /// </summary>
  /// <param name="commonKey"></param>
  public void ApplyAudioSetting(string commonKey)
  {
    // I will need to do a transformation from the linear slider of the UI to the logarithmic slider of the audio mixer
    mainMixer.SetFloat(commonKey, (float)settingsManager.settings.Find((obj) =>
    {
      return obj.settingKey == commonKey;
    }).settingValue);
  }

  /// <summary>
  /// Applies mouse sensitivity
  /// </summary>
  /// <param name="commonKey"></param>
  public void ApplyMouseSensitivitySetting(string commonKey)
  {
    // 
    InGameSetting setting = settingsManager.settings.Find((obj) =>
    {
      return obj.settingKey == commonKey;
    });

    if (setting != null && setting.settingValue != null)
    {
      // * there might not be a need for doing anything here because in order to implement mouse sensitivity we just need to reference the value from mouse look scripts
      // InGameSettings.Instance.mouseSensitivity = (float)setting.settingValue;
    }
  }

  /// <summary>
  /// Inverts the mouse axis
  /// </summary>
  /// <param name="commonKey"></param>
  public void ApplyInvertAxisSetting(string commonKey)
  {
    // 
    InGameSetting setting = settingsManager.settings.Find((obj) =>
        {
          return obj.settingKey == commonKey;
        });

    // finds all mouse-driven controllers and inverts their y axis
    // unless this is the main menu. we don't want that behaviour there
    if (SceneManager.GetActiveScene().name != "MenuV2")
    {
      SimpleCameraController[] allcontrollerInScene = GameObject.FindObjectsOfType<SimpleCameraController>();
      foreach (SimpleCameraController item in allcontrollerInScene)
      {
        item.reverseY = !item.reverseY;
      }
    }


  }


  Resolution currentResolution = new Resolution();
  List<Resolution> uniqResolutionsList = new List<Resolution>();
  /// <summary>
  /// Changes the resolution of the game
  /// </summary>
  /// <param name="commonKey"></param>
  public void ApplyResolutionSetting(string commonKey)
  {
    // 
    InGameSetting resSetting = settingsManager.settings.Find((obj) =>
    {
      return obj.settingKey == commonKey;
    });

    if (resSetting != null && resSetting.settingValue != null)
    {
      MyDebug.Log("resSetting selector value:", resSetting.settingValue);

      // save setting
      if (uniqResolutionsList.Count > (int)resSetting.settingValue)
      {
        currentResolution = uniqResolutionsList[(int)resSetting.settingValue];
        Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
      }
      else
      {
        currentResolution = uniqResolutionsList[uniqResolutionsList.Count - 1];
        Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
      }
    }

  }

  /// <summary>
  /// Switches between fullscreen or windowed mode
  /// </summary>
  /// <param name="commonKey"></param>
  public void ApplyFullscreenSetting(string commonKey)
  {
    // 
    InGameSetting setting = settingsManager.settings.Find((obj) =>
        {
          return obj.settingKey == commonKey;
        });

    if (setting != null && setting.settingValue != null)
    {

      Screen.fullScreen = (int)setting.settingValue == 1 ? true : false;
    }
  }

  /// <summary>
  /// Toggles V-Sync on or off
  /// </summary>
  /// <param name="commonKey"></param>
  public void ApplyVScreenSetting(string commonKey)
  {
    // 
    InGameSetting setting = settingsManager.settings.Find((obj) =>
        {
          return obj.settingKey == commonKey;
        });

    if (setting != null && setting.settingValue != null)
    {

      QualitySettings.vSyncCount = (int)setting.settingValue;
    }
  }

  /// <summary>
  /// Applies brightness level
  /// </summary>
  /// <param name="commonKey"></param>
  public void ApplyBrightnessSetting(string commonKey)
  {
    // 
    // 
    InGameSetting setting = settingsManager.settings.Find((obj) =>
        {
          return obj.settingKey == commonKey;
        });

    if (setting != null && setting.settingValue != null)
    {
      MyDebug.Log("Brightness applied");
      foreach (Camera cam in allCamsInScene)
      {
        if (cam != null)
          if (cam.GetComponent<SimpleLUT>())
            cam.GetComponent<SimpleLUT>().Brightness = (float)setting.settingValue;
      }
    }

  }

  #region private funcs
  private void InitResolution()
  {
    //Filters out all resolutions with low refresh rate:
    Resolution[] resolutions = Screen.resolutions;
    HashSet<Tuple<int, int>> uniqResolutions = new HashSet<Tuple<int, int>>();
    Dictionary<Tuple<int, int>, int> maxRefreshRates = new Dictionary<Tuple<int, int>, int>();
    for (int i = 0; i < resolutions.GetLength(0); i++)
    {
      //Add resolutions (if they are not already contained)
      Tuple<int, int> resolution = new Tuple<int, int>(resolutions[i].width, resolutions[i].height);
      uniqResolutions.Add(resolution);
      //Get highest framerate:
      if (!maxRefreshRates.ContainsKey(resolution))
      {
        maxRefreshRates.Add(resolution, resolutions[i].refreshRate);
      }
      else
      {
        maxRefreshRates[resolution] = resolutions[i].refreshRate;
      }
    }
    //Build resolution list:
    uniqResolutionsList = new List<Resolution>(uniqResolutions.Count);
    foreach (Tuple<int, int> resolution in uniqResolutions)
    {
      Resolution newResolution = new Resolution();
      newResolution.width = resolution.Item1;
      newResolution.height = resolution.Item2;
      if (maxRefreshRates.TryGetValue(resolution, out int refreshRate))
      {
        newResolution.refreshRate = refreshRate;
      }
      uniqResolutionsList.Add(newResolution);
    }

    // resolutionDropdown.ClearOptions(); // if using a Unity dropdown

    List<string> resolutionOptions = new List<string>();

    int currentResolutionIndex = 0;
    string resolutionOption = "";

    for (int i = 0; i < uniqResolutionsList.Count; i++)
    {
      resolutionOption = uniqResolutionsList[i].width + " x " + uniqResolutionsList[i].height;
      resolutionOptions.Add(resolutionOption);
      resolutionSelector.CreateNewItem(resolutionOption);

      if (uniqResolutionsList[i].width == Screen.currentResolution.width && uniqResolutionsList[i].height == Screen.currentResolution.height)
      {
        currentResolutionIndex = i;
      }
    }
    // resolutionDropdown.AddOptions(resolutionOptions); // if using a Unity dropdown
    resolutionSelector.SetupSelector(); // if using a ModernUI HorizontalSelector

    // set default resolution
    // 
    InGameSetting resSetting = settingsManager.settings.Find((obj) =>
    {
      return obj.settingKey == "resolution_setting";
    });

    if (!PlayerPrefs.HasKey("resolution_setting"))
    {
      PlayerPrefs.SetInt("resolution_setting", currentResolutionIndex);
      if (resSetting != null && resSetting.settingValue == null)
      {
        resSetting.settingValue = (int)currentResolutionIndex;

        MyDebug.Log("resSetting selector value:", resSetting.settingValue);

        // save setting
        if (uniqResolutionsList.Count > (int)resSetting.settingValue)
        {
          currentResolution = uniqResolutionsList[(int)resSetting.settingValue];
          Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
        }
        else
        {
          currentResolution = uniqResolutionsList[uniqResolutionsList.Count - 1];
          Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
        }
      }
    }

  }
  #endregion
}
