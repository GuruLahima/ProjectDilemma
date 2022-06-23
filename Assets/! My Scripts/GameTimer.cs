using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Workbench.ProjectDilemma
{
  public class GameTimer : MonoBehaviour
  {
    public float CurrentTime
    {
      get
      {
        return incTimer;
      } 
    }

    [SerializeField] TextMeshProUGUI Timer;

    private float incTimer;
    private float decTimer;
    private string timerString;

    // Start is called before the first frame update
    public void StartTimer(float time)
    {
      if (Timer == null)
        Destroy(this);
      StartCoroutine(UpdateTimer(time));
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