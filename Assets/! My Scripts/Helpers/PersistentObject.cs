using UnityEngine;

namespace Workbench.Wolfsbane.Multiplayer
{
  public class PersistentObject : MonoBehaviour
  {
    private void Awake()
    {
      DontDestroyOnLoad(gameObject);
    }
  }
}
