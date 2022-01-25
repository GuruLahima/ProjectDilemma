using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace Workbench.ProjectDilemma
{
  public class GameLevelObject : MonoBehaviour
  {
    private void Awake()
    {
      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
      SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
      if (scene.name == MiscelaneousSettings.Instance.mainMenuScene)
        Destroy(gameObject);
    }
  }
}
