using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor;
using System;
using DG.Tweening;
using UnityEngine.Events;
using NaughtyAttributes;

namespace GuruLaghima
{
  public class HelperFunctions
  {

    public static bool ValidateCollectedData(object[] args)
    {
      // check if parameters are good

      if (args.Length < 2)
      {
        Debug.Log("<color=red>Bad data for item</color>");
        return false;
      }
      else
      {
        return true;
      }
    }

    // Full version, clamping settable on all 4 range elements (in1, in2, out1, out2)
    public static float remap(float val, float in1, float in2, float out1, float out2,
        bool in1Clamped, bool in2Clamped, bool out1Clamped, bool out2Clamped)
    {
      if (in1Clamped == true && val < in1) val = in1;
      if (in2Clamped == true && val > in2) val = in2;

      float result = out1 + (val - in1) * (out2 - out1) / (in2 - in1);

      if (out1Clamped == true && result < out1) result = out1;
      if (out2Clamped == true && result > out2) result = out2;

      return result;
    }


    /// <summary>
    /// Returns a list of values between [0;31].
    /// </summary>
    public static List<int> GetAllLayerMaskInspectorIndex(LayerMask _mask)
    {
      List<int> layers = new List<int>();
      var bitmask = _mask.value;
      for (int i = 0; i < 32; i++)
      {
        if (((1 << i) & bitmask) != 0)
        {
          layers.Add(i);
        }
      }
      return layers;
    }


  }
  public class MyDebug
  {

    public static void Log(string location, string message, Color textColor)
    {
#if ALEK_DEBUG_ON
      Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + location + " >> </color>" + message);
#endif
    }
    public static void Log(string message, Color textColor)
    {
#if ALEK_DEBUG_ON
      Debug.Log("<color=#" + ColorUtility.ToHtmlStringRGB(textColor) + ">" + message + "</color>");
#endif
    }

    public static void Log(string location, string message)
    {
#if ALEK_DEBUG_ON
      Debug.Log("<color=orange>" + location + " >> </color>" + message);
#endif
    }

    public static void Log(string location, string color = "orange", string message = "")
    {
#if ALEK_DEBUG_ON
      Debug.Log("<color=" + color + ">" + location + " >> </color>" + message);
#endif
    }

    public static void LogError(string message, UnityEngine.Object obj)
    {
#if ALEK_DEBUG_ON
      Debug.LogError(message, obj);
#endif
    }

    public static void LogError(string message)
    {
#if ALEK_DEBUG_ON
      Debug.LogError(message);
#endif
    }

    public static void LogFormat(string message, params object[] args)
    {
#if ALEK_DEBUG_ON
      Debug.LogFormat(message, args);
#endif
    }

    public static void LogErrorFormat(string location, short returnCode, string message = "", string color = "orange")
    {
#if ALEK_DEBUG_ON
      Debug.LogErrorFormat(location, returnCode, message);
#endif
    }
    public static void LogWarning(string location, UnityEngine.Object obj)
    {
#if ALEK_DEBUG_ON
      Debug.LogWarning(location, obj);
#endif
    }
    public static void LogWarning(object message)
    {
#if ALEK_DEBUG_ON
      Debug.LogWarning(message);
#endif
    }


    public static void DrawRay(Ray ray, Color drawColor, float duration)
    {
#if ALEK_DEBUG_ON
      Debug.DrawRay(ray.origin, ray.direction, drawColor, duration);
#endif
    }


  }

  /*   public static class MainCamFinder
    {
      public static Transform Get()
      {
        if (SceneOverlord.activeMainCam != null)
          return SceneOverlord.activeMainCam.transform;

        // this MainCam search assumes there is only ever one camera tagged MainCamera
        GameObject[] mainCams = GameObject.FindGameObjectsWithTag("MainCamera");
        if (mainCams.Length > 0)
          return mainCams[0].transform;

        return null;
      }
    }
   */

  [Serializable]
  public class SpeedBoost
  {
    public float boostValue;
    public BoostType boostType;
    public BoostMode boostMode;

    public SpeedBoost(BoostMode mode, BoostType type, float value)
    {
      this.boostMode = mode;
      this.boostType = type;
      this.boostValue = value;
    }
    public SpeedBoost()
    {
      this.boostMode = BoostMode.All;
      this.boostType = BoostType.Flat;
      this.boostValue = 0f;
    }

    public float ApplyBoost(float origVal, bool isWalking)
    {
      if (isWalking)
      {
        if (boostMode == BoostMode.Walk || boostMode == BoostMode.All)
          switch (this.boostType)
          {
            case BoostType.Flat:
              origVal += this.boostValue;
              break;
            case BoostType.Percentage:
              origVal += this.boostValue;
              break;
            default:
              break;
          }
      }
      else
      {
        if (boostMode == BoostMode.Run || boostMode == BoostMode.All)
          switch (this.boostType)
          {
            case BoostType.Flat:
              origVal += this.boostValue;
              break;
            case BoostType.Percentage:
              origVal += origVal * (this.boostValue / 100f);
              break;
            case BoostType.Multiplicative:
              origVal *= this.boostValue;
              break;
            default:
              break;
          }
      }

      return origVal;
    }

    public enum BoostType
    {
      Flat,
      Percentage,
      Multiplicative
    }
    public enum BoostMode
    {
      Walk,
      Run,
      All
    }

    public static object Deserialize(byte[] data)
    {
      var result = new SpeedBoost();
      result.boostMode = (BoostMode)data[0];
      result.boostType = (BoostType)data[1];
      result.boostValue = (float)data[2];
      return result;
    }

    public static byte[] Serialize(object customType)
    {
      var c = (SpeedBoost)customType;
      return new byte[] { (byte)c.boostMode, (byte)c.boostType, (byte)c.boostValue };
    }
  }

  public class LerpDerp
  {

    /* parameters */
    float m_lerpTime = 1f;
    float m_start;
    float m_finish;

    /* private vars used for calcs */
    float m_currentLerpTime;
    float m_perc = 0;

    public LerpDerp(float start, float finish, float lerpTime)
    {
      this.m_start = start;
      this.m_finish = finish;
      this.m_lerpTime = lerpTime;
    }

    public float Lerp()
    {
      float perc = 0;
      if (m_lerpTime == 0)
      {
        perc = 1;
      }
      else
      {
        //increment timer once per frame
        m_currentLerpTime += Time.deltaTime;
        if (m_currentLerpTime > m_lerpTime)
        {
          m_currentLerpTime = m_lerpTime;
        }

        //lerp!
        perc = m_currentLerpTime / m_lerpTime;
      }

      return Mathf.Lerp(m_start, m_finish, perc);
    }

    public static Vector3 Lerp(Vector3 startPos, Vector3 endPos, ValueWrapper<float> lerpTime, ValueWrapper<float> currentLerpTime)
    {

      //increment timer once per frame
      currentLerpTime.Value += Time.deltaTime;
      if (currentLerpTime.Value > lerpTime.Value)
      {
        currentLerpTime.Value = lerpTime.Value;
      }

      //lerp!
      float perc = currentLerpTime.Value / lerpTime.Value;
      return Vector3.Lerp(startPos, endPos, perc);
    }
    public static Quaternion Lerp(Quaternion startRot, Quaternion endRot, ValueWrapper<float> lerpTime, ValueWrapper<float> currentLerpTime)
    {

      //increment timer once per frame
      currentLerpTime.Value += Time.deltaTime;
      if (currentLerpTime.Value > lerpTime.Value)
      {
        currentLerpTime.Value = lerpTime.Value;
      }

      //lerp!
      float perc = currentLerpTime.Value / lerpTime.Value;
      return Quaternion.Lerp(startRot, endRot, perc);
    }

  }

  [Serializable]
  public class DictWrapper
  {
    public string key;
    public GameObject element;
  }

  [Serializable]
  public class MyDictionary
  {
    public List<ItemModPair> collectableList = new List<ItemModPair>();
    public Dictionary<GameObject, float> collectableDict = new Dictionary<GameObject, float>();

    public Dictionary<GameObject, float> GetItems()
    {
      if (collectableDict.Count > 0)
        return collectableDict;

      foreach (ItemModPair entry in collectableList)
      {
        collectableDict.Add(entry.item, entry.mod);
      }
      return collectableDict;
    }
    public void AddRange(MyDictionary mods)
    {
      collectableList.AddRange(mods.collectableList);
    }

    [Serializable]
    public class ItemModPair
    {
      public GameObject item;
      public float mod;

    }
  }

  [Serializable]
  public class MyGenericDictionary<K, V>
  {
    [ReorderableList]
    [SerializeField] List<KeyValuePair> list = new List<KeyValuePair>();

    private Dictionary<K, V> dictionary = new Dictionary<K, V>();

    public Dictionary<K, V> GetItems()
    {
      if (dictionary.Count > 0)
        return dictionary;

      foreach (KeyValuePair entry in list)
      {
        if (!dictionary.ContainsKey(entry.key))
          dictionary.Add(entry.key, entry.value);
      }
      return dictionary;
    }
    public void AddRange(MyGenericDictionary<K, V> dic)
    {
      list.AddRange(dic.list);
    }

    [Serializable]
    public class KeyValuePair
    {
      public K key;
      public V value;

    }
  }


  [Serializable]
  public class MyEventsDictionary
  {
    public List<ItemModPair> items = new List<ItemModPair>();
    public Dictionary<string, UnityEngine.Events.UnityEvent> dictionary = new Dictionary<string, UnityEngine.Events.UnityEvent>();

    public Dictionary<string, UnityEngine.Events.UnityEvent> GetItems()
    {
      if (dictionary.Count > 0)
        return dictionary;

      foreach (ItemModPair entry in items)
      {
        dictionary.Add(entry.name, entry.item);
      }
      return dictionary;
    }
    public void AddRange(MyEventsDictionary mods)
    {
      items.AddRange(mods.items);
    }

    [Serializable]
    public class ItemModPair
    {
      public string name;
      public UnityEngine.Events.UnityEvent item;

    }
  }


  /// <summary>
  /// I wanted to write an extension method for the Sequence class but it's sealed so now I just have this here for reference purposes
  /// </summary>
  namespace DG.Tweening
  {
    public static class ExtensionSequence
    {
      public delegate void MyCallback();
      public static void Kill(Sequence seq, MyCallback del)
      {
        del();
      }
    }
  }

  [Serializable]
  public class BoolWrapper
  {
    [SerializeField] bool value;
    public bool Value { get { return this.value; } set { this.value = value; } }
    public BoolWrapper(bool value) { this.Value = value; }
  }

  public class ValueWrapper<T> where T : struct
  {
    public T Value { get; set; }
    public ValueWrapper(T value) { this.Value = value; }
  }

  [Serializable]
  public class EventEnclosure
  {
    public string name;
    public float BeforePause;
    public UnityEvent OnStarted;
    public float Interval;
    public UnityEvent OnEnded;
    public float AfterPause;

  }

  /*   [Serializable]
    public class ControllableSequence
    {
      public string name;
      public BoolReference condition;
      public bool inParallel;
  #if UNITY_EDITOR
      [ReorderableList]
  #endif
      public List<EventWithDuration> eventSequence;
      public List<ControllableSequence> nextSequences;

      public IEnumerator RunSequence()
      {
        if (condition && condition.boolWrapper.Value)
        {
          yield return SequenceCoroutine();

          // ruun next sequences (in sequence)
          foreach (ControllableSequence seq in nextSequences)
          {
            if (seq.inParallel)
            {
              seq.RunSequence();
            }
            else
            {
              yield return seq.RunSequence();
            }
          }
        }


      }

      IEnumerator currentCoroutine;
      IEnumerator SequenceCoroutine()
      {
        MyDebug.Log($"[{name}]", "started");

        foreach (EventWithDuration ev in eventSequence)
        {
          ev.theEvent?.Invoke();
          if (ev.hasCoroutine)
          {

            yield return currentCoroutine.MoveNext();
          }
          else
          {

            yield return new WaitForSeconds(ev.duration);
          }
        }

        MyDebug.Log($"[{name}]", "ended");

      }

      [Serializable]
      public class EventWithDuration
      {
        public bool hasCoroutine;
        public UnityEvent theEvent;
        [HideIf("hasCoroutine")]
        public float duration;
      }

    } */

  [Serializable]
  public class EventSequence
  {
    public float BeforePause;
    public UnityEvent OnStarted;
#if UNITY_EDITOR
    [ReorderableList]
#endif

    public List<EventEnclosure> eventSequence;
    public UnityEvent OnEnded;
    public float AfterPause;


  }
}
