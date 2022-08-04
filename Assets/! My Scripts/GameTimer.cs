using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Workbench.ProjectDilemma
{
  public class GameTimer : MonoBehaviour
  {
    [System.Serializable]
    public class TimerEvent
    {
      public float TriggerTime;
      public UnityEngine.Events.UnityEvent TriggerEvent;
    }

    public float CurrentTime
    {
      get
      {
        return incTimer;
      } 
    }

    [SerializeField] List<TimerEvent> OnTimerEvents = new List<TimerEvent>();
    private TimerEvent _lastTimerEvent;

    [SerializeField] TextMeshProUGUI Timer;

    private float incTimer;
    private float decTimer;
    private string timerString;
    Coroutine timerCoroutine;
    // Start is called before the first frame update
    public void StartTimer(float time)
    {
      if (Timer == null)
        Destroy(this);
      timerCoroutine = StartCoroutine(UpdateTimer(time));
    }

    public void ResetTimer(float time)
    {
      StopCoroutine(timerCoroutine);
      StartTimer(time);
    }

    private IEnumerator UpdateTimer(float time)
    {
      while (decTimer >= 0)
      {
        // yield return new WaitWhile(() => { return false; });
        // Example for a increasing timer
        incTimer += Time.deltaTime;

        // Example for a decreasing timer
        decTimer = time - incTimer;

        // gameTimer -= Time.deltaTime;
        FormatTimeForUI(decTimer);
        yield return null;
        var timerEvent = OnTimerEvents.Find((x) => Mathf.Abs(decTimer - x.TriggerTime) <= 1); //1 is threshold for checking
        if (timerEvent != null && timerEvent != _lastTimerEvent)
        {
          timerEvent.TriggerEvent?.Invoke();
        }
      }
      FormatTimeForUI(0f);
    }

    private void FormatTimeForUI(double time)
    {
      int seconds = (int)(time % 60);
      int minutes = (int)(time / 60) % 60;

      timerString = string.Format("{0:00}:{1:00}", minutes, seconds);

      Timer.text = timerString;
    }

  }
}