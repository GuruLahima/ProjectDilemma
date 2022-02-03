using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
  public class CameraSwitcher : MonoBehaviour
  {
    #region public fields

    #endregion


    #region exposed fields
    [InputAxis]
    [SerializeField] string switchInput;
    [InputAxis]
    [SerializeField] string switchBackModifierInput;

    [SerializeField] List<Camera> camerasStack;
    [SerializeField] int currentCameraIndex = 0;
    #endregion


    #region private fields
    #endregion


    #region public methods

    #endregion


    #region MonoBehaviour callbacks
    private void Start()
    {
      GameMechanic.GameStarted += SetCamera;


      // register this script's code that needs to run at certain input with the input processor
      // InputProcessor.Instance.SubscribeMethodToInputChecks(SwitchCamera, );
    }

    private void OnDisable()
    {
      GameMechanic.GameStarted -= SetCamera;
    }

    #endregion


    #region private methods

    void SetCamera()
    {
      foreach (Camera cam in camerasStack)
        cam.gameObject.SetActive(false);
      if (currentCameraIndex >= 0 && currentCameraIndex < camerasStack.Count)
        camerasStack[currentCameraIndex].gameObject.SetActive(true);
    }
    public void SwitchCamera()
    {
      if (CanSwitch())
        if (Input.GetButtonDown(switchInput))
        {
          if (Input.GetButton(switchBackModifierInput))
            PrevCam();
          else
            NextCamera();
        }
    }
    private void NextCamera()
    {

      foreach (Camera cam in camerasStack)
        cam.gameObject.SetActive(false);
      if (currentCameraIndex < 0)
        currentCameraIndex = -1;
      if (currentCameraIndex >= camerasStack.Count - 1)
        currentCameraIndex = -1;
      if (camerasStack.Count > 0)
        camerasStack[++currentCameraIndex].gameObject.SetActive(true);
    }

    private void PrevCam()
    {

      foreach (Camera cam in camerasStack)
        cam.gameObject.SetActive(false);
      if (currentCameraIndex >= camerasStack.Count)
        currentCameraIndex = camerasStack.Count;
      if (currentCameraIndex <= 0)
        currentCameraIndex = camerasStack.Count;
      if (camerasStack.Count > 0)
        camerasStack[--currentCameraIndex].gameObject.SetActive(true);
    }

    bool CanSwitch()
    {
      bool result = true;

      result = GameMechanic.Instance.canChoose && !ChatGui.isInChatMode;

      return result;
    }
    #endregion


    #region networking code

    #endregion


  }
}