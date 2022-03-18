using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DelayedLoadTemp : MonoBehaviour
{
  public float delay;
  public string sceneToLoad;

  public void LoadLevelWithDelay()
  {
    Invoke("Delayed", delay);
  }

  private void Delayed()
  {
    SceneManager.LoadScene(sceneToLoad);
  }

}
