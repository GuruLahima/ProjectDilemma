using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenBehaviour : MonoBehaviour
{
  public void LoadMenu()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }
}
