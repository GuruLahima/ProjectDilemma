using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class NumberCounter : MonoBehaviour
{
  public TextMeshProUGUI Text;
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
  [SerializeField] bool customFormatting = true;

  private void Awake()
  {
    Text = GetComponent<TextMeshProUGUI>();
  }

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

        if (customFormatting)
        {
          Text.SetText($"{NumberFormating.Format((long)previousValue)}");
        }
        else
        {
          Text.SetText(previousValue.ToString(NumberFormat));
        }

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

        if (customFormatting)
        {
          Text.SetText($"{NumberFormating.Format((long)previousValue)}");
        }
        else
        {
          Text.SetText(previousValue.ToString(NumberFormat));
        }

        yield return Wait;
      }
    }
  }
}