using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ValueCounter : MonoBehaviour
{
  public UnityEvent<string> OnValueChangedString;
  public UnityEvent<float> OnValueChangedInt;
  public int newAmount = 50;
  public int CountFPS = 30;
  public float Duration = 1f;
  public string NumberFormat = "N0";
  public bool startOnAwake = false;
  private int _value;
  public int Value
  {
    get
    {
      return _value;
    }
    set
    {
      UpdateText(value);
      _value = value;
    }
  }
  private Coroutine CountingCoroutine;

  public void Start()

  {
    if (startOnAwake)
      Value = newAmount;
  }


  private void UpdateText(int newValue)
  {
    if (CountingCoroutine != null)
    {
      StopCoroutine(CountingCoroutine);
    }

    CountingCoroutine = StartCoroutine(CountText(newValue));
  }

  private IEnumerator CountText(int newValue)
  {
    WaitForSeconds Wait = new WaitForSeconds(1f / CountFPS);
    int previousValue = _value;
    int stepAmount;

    if (newValue - previousValue < 0)
    {
      stepAmount = Mathf.FloorToInt((newValue - previousValue) / (CountFPS * Duration)); // newValue = -20, previousValue = 0. CountFPS = 30, and Duration = 1; (-20- 0) / (30*1) // -0.66667 (ceiltoint)-> 0
    }
    else
    {
      stepAmount = Mathf.CeilToInt((newValue - previousValue) / (CountFPS * Duration)); // newValue = 20, previousValue = 0. CountFPS = 30, and Duration = 1; (20- 0) / (30*1) // 0.66667 (floortoint)-> 0
    }

    if (previousValue < newValue)
    {
      while (previousValue < newValue)
      {
        previousValue += stepAmount;
        if (previousValue > newValue)
        {
          previousValue = newValue;
        }

        OnValueChangedString?.Invoke(previousValue.ToString(NumberFormat));
        OnValueChangedInt?.Invoke(previousValue);

        yield return Wait;
      }
    }
    else
    {
      while (previousValue > newValue)
      {
        previousValue += stepAmount; // (-20 - 0) / (30 * 1) = -0.66667 -> -1              0 + -1 = -1
        if (previousValue < newValue)
        {
          previousValue = newValue;
        }

        OnValueChangedString?.Invoke(previousValue.ToString(NumberFormat));
        OnValueChangedInt?.Invoke(previousValue);

        yield return Wait;
      }
    }
  }
}
