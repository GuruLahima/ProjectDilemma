using UnityEngine;

namespace GuruLaghima
{
  public class PersistentObject : MonoBehaviour
  {
    private void Awake()
    {
      DontDestroyOnLoad(gameObject);
    }
  }
}
