using UnityEngine;

namespace Workbench.ProjectDilemma
{
  public class DestroyWithDelay : MonoBehaviour
  {
    [SerializeField] public float delay = 1f;
    // Start is called before the first frame update
    void Start()
    {
      Invoke("Destroy", delay);
    }

    void Destroy()
    {
      Destroy(this.gameObject);
    }
  }
}