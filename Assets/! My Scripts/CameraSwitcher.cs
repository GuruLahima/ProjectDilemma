using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Cinemachine;

namespace Workbench.ProjectDilemma
{
  public class CameraSwitcher : MonoBehaviour
  {
    #region public fields
    [HideInInspector] public CinemachineVirtualCamera currentCamera;

    #endregion


    #region exposed fields
    [InputAxis]
    [SerializeField] string switchInput;
    [InputAxis]
    [SerializeField] string switchBackModifierInput;

    [SerializeField] List<CinemachineVirtualCamera> camerasStack;
    [SerializeField] int currentCameraIndex = 0;
    #endregion


    #region private fields
    #endregion


    #region public methods

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

    public void DisableGameplayCameras()
    {

      foreach (CinemachineVirtualCamera cam in camerasStack)
      {
        if (cam)
          cam.gameObject.SetActive(false);
      }

    }
    #endregion


    #region MonoBehaviour callbacks
    void Start()
    {
      GameMechanic.GameStarted += SetCamera;
      // GameMechanic.VotingEnded += DisableGameplayCameras;

      SetCamera();


      // register this script's code that needs to run at certain input with the input processor
      // InputProcessor.Instance.SubscribeMethodToInputChecks(SwitchCamera, );
    }

    void OnDisable()
    {
      GameMechanic.GameStarted -= SetCamera;
      // GameMechanic.VotingEnded -= DisableGameplayCameras;

    }

    #endregion


    #region private methods


    void SetCamera()
    {
      foreach (CinemachineVirtualCamera cam in camerasStack)
        cam.gameObject.SetActive(false);
      if (currentCameraIndex >= 0 && currentCameraIndex < camerasStack.Count)
      {
        currentCamera = camerasStack[currentCameraIndex];
        currentCamera.gameObject.SetActive(true);
      }
    }

    void NextCamera()
    {

      foreach (CinemachineVirtualCamera cam in camerasStack)
        cam.gameObject.SetActive(false);
      if (currentCameraIndex < 0)
        currentCameraIndex = -1;
      if (currentCameraIndex >= camerasStack.Count - 1)
        currentCameraIndex = -1;
      if (camerasStack.Count > 0)
      {
        currentCamera = camerasStack[++currentCameraIndex];
        currentCamera.gameObject.SetActive(true);
      }
    }

    void PrevCam()
    {

      foreach (CinemachineVirtualCamera cam in camerasStack)
        cam.gameObject.SetActive(false);
      if (currentCameraIndex >= camerasStack.Count)
        currentCameraIndex = camerasStack.Count;
      if (currentCameraIndex <= 0)
        currentCameraIndex = camerasStack.Count;
      if (camerasStack.Count > 0)
      {
        currentCamera = camerasStack[--currentCameraIndex];
        currentCamera.gameObject.SetActive(true);
      }
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