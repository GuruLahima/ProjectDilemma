using UnityEngine;
using Workbench.ProjectDilemma;
/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : Singleton<MyClassName> {}
/// </summary>
public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
  // Check to see if we're about to be destroyed.
  private static bool m_ShuttingDown = false;
  private static object m_Lock = new object();
  private static T m_Instance;

  /// <summary>
  /// Access singleton instance through this propriety.
  /// </summary>
  public static T Instance
  {
    get
    {
      if (m_ShuttingDown)
      {
        MyDebug.LogWarning("[Singleton] Instance '" + typeof(T) +
            "' already destroyed. Returning null.");
        return null;
      }

      lock (m_Lock)
      {
        if (m_Instance == null)
        {
          // Search for existing instance.
          m_Instance = (T)FindObjectOfType(typeof(T));

          // Create new instance if one doesn't already exist.
          if (m_Instance == null)
          {
            // Need to create a new GameObject to attach the singleton to.
            var singletonObject = new GameObject();
            m_Instance = singletonObject.AddComponent<T>();
            singletonObject.name = typeof(T).ToString() + " (Singleton)";

            // Make instance persistent.
            DontDestroyOnLoad(singletonObject);
          }
        }

        return m_Instance;
      }
    }
  }

  protected virtual void Awake()
  {
    MyDebug.Log("singleton for " + transform.name);
    if (m_Instance == null)
    {
      MyDebug.Log("No instance of this type yet " + transform.name);
      m_Instance = gameObject.GetComponent<T>();
      DontDestroyOnLoad(gameObject);
    }
    else if (m_Instance.GetInstanceID() != GetInstanceID())
    {
      MyDebug.Log("Duplicate instance of this type " + transform.name);
      Destroy(gameObject);
      // throw new System.Exception(string.Format("Instance of {0} already exists, removing {1}", GetType().FullName, ToString()));
    }
  }


  private void OnApplicationQuit()
  {
    m_ShuttingDown = true;
  }


  private void OnDestroy()
  {
    m_ShuttingDown = true;
  }
}