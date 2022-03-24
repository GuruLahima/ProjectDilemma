using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GuruLaghima;
using Workbench.ProjectDilemma;

public class SimpleCameraController : MonoBehaviour
{
  Vector2 rotation = Vector2.zero;
  public float speed = 3;
  public float yMin = 3;
  public float yMax = 3;
  public float xMin = 3;
  public float xMax = 3;

  [SerializeField] InGameSettingSO mouseSensitivitySetting;

  float sensitivityMod = 1;
  InGameSetting setting;
  private void Start()
  {
    rotation = transform.eulerAngles;
    MyDebug.Log(rotation.ToString());
    yMin = rotation.y + yMin;
    yMax = rotation.y + yMax;
    xMin = rotation.x + xMin;
    xMax = rotation.x + xMax;

    if (InGameSettingsManager.Instance)
      setting = InGameSettingsManager.Instance.settings.Find((obj) =>
      {
        return obj.settingKey == mouseSensitivitySetting.settingKey;
      });
  }

  void Update()
  {
    // fetch mouse sensitivity from settings
    if (setting != null && setting.settingValue != null)
    {
      sensitivityMod = (float)setting.settingValue;
    }

    // get mouse input and apply mouse sensitivity to it
    rotation.y += Input.GetAxis("Mouse X") * sensitivityMod;
    rotation.x += -Input.GetAxis("Mouse Y") * sensitivityMod;

    // clamp rotation
    rotation.y = Mathf.Clamp(rotation.y, yMin, yMax);
    rotation.x = Mathf.Clamp(rotation.x, xMin, xMax);

    // transform.eulerAngles = (Vector2)rotation * speed;
    transform.rotation = Quaternion.AngleAxis(rotation.y, Vector3.up) * Quaternion.AngleAxis(rotation.x, Vector3.right);
  }
}