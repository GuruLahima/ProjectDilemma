using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GuruLaghima;
using Workbench.ProjectDilemma;

public class SimpleCameraController : MonoBehaviour
{
  #region public fields
  public bool rotateByY = true;
  public bool rotateByX = true;
  public bool reverseY = false;
  public bool reverseX = false;
  public bool playerHasControlOverCamera = true;
  public float speed = 3;
  public bool limitsInLocalSpace = true;
  public float yMin = 3;
  public float yMax = 3;
  public float xMin = 3;
  public float xMax = 3;
  #endregion

  #region private fields
  float _yMin = 3;
  float _yMax = 3;
  float _xMin = 3;
  float _xMax = 3;
  #endregion

  [SerializeField] InGameSettingSO mouseSensitivitySetting;

  float sensitivityMod = 1;
  InGameSetting setting;
  Vector2 rotation = Vector2.zero;
  Vector2 origRotation;

  private void Start()
  {
    origRotation = transform.eulerAngles;
    rotation = origRotation;
    if (limitsInLocalSpace)
    {
      _yMin = origRotation.y + yMin;
      _yMax = origRotation.y + yMax;
      _xMin = origRotation.x + xMin;
      _xMax = origRotation.x + xMax;
    }
    else
    {
      _yMin = yMin;
      _yMax = yMax;
      _xMin = xMin;
      _xMax = xMax;
    }

    if (InGameSettingsManager.Instance)
      setting = InGameSettingsManager.Instance.settings.Find((obj) =>
      {
        return obj.settingKey == mouseSensitivitySetting.settingKey;
      });
  }

  void Update()
  {

    if (playerHasControlOverCamera)
      MoveCamera();

  }

  void MoveCamera()
  {

    // fetch mouse sensitivity from settings
    if (setting != null && setting.settingValue != null)
    {
      sensitivityMod = (float)setting.settingValue;
    }

    // get mouse input and apply mouse sensitivity to it
    rotation.y += Input.GetAxis("Mouse X") * sensitivityMod * (reverseY ? -1 : 1);
    rotation.x += -Input.GetAxis("Mouse Y") * sensitivityMod * (reverseX ? -1 : 1);

    // update limits (in case they are changed in runtime)
    if (limitsInLocalSpace)
    {

      _yMin = origRotation.y + yMin;
      _yMax = origRotation.y + yMax;
      _xMin = origRotation.x + xMin;
      _xMax = origRotation.x + xMax;
    }
    else
    {
      _yMin = yMin;
      _yMax = yMax;
      _xMin = xMin;
      _xMax = xMax;
    }

    // clamp rotation
    rotation.y = Mathf.Clamp(rotation.y, _yMin, _yMax);
    rotation.x = Mathf.Clamp(rotation.x, _xMin, _xMax);

    Quaternion smoothRotation;
    if (rotateByX && rotateByY)
    {

      smoothRotation = Quaternion.AngleAxis(rotation.y, Vector3.up) * Quaternion.AngleAxis(rotation.x, Vector3.right);
    }
    else if (rotateByX)
    {
      smoothRotation = Quaternion.AngleAxis(rotation.x, Vector3.right);
    }
    else if (rotateByY)
    {
      smoothRotation = Quaternion.AngleAxis(rotation.y, Vector3.up);
    }
    else
    {
      smoothRotation = Quaternion.identity;
    }


    // smooth out rotation
    transform.rotation = Quaternion.Slerp(transform.rotation, smoothRotation, Time.deltaTime * speed);
  }
}