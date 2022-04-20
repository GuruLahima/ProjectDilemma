using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Workbench.ProjectDilemma
{
  public class MagnifyingGlass : MonoBehaviour
  {
    [SerializeField] List<TextMeshProUGUI> recentGames = new List<TextMeshProUGUI>();
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float displayDuration;
    /// <summary>
    /// Since this will require things such as getting player's last x games CHOICE and the OUTCOME of the game
    /// I will simply leave an empty panel to show for x seconds and we will re-do this script when everything else is done
    /// </summary>
    public void Use()
    {
      string PLAYERNAME = RandomUsername();
      foreach (TextMeshProUGUI txt in recentGames)
      {
        string CHOICE = Random.value > 0.5f ? "cooperated" : "betrayed";
        string OUTCOME = Random.value > 0.5f ? "won" : "lost";
        string text = $"{PLAYERNAME} {CHOICE} and {OUTCOME}";
        txt.text = text;
      }
      StartCoroutine(Display());
    }
    string RandomUsername()
    {
      string username = string.Empty;
      string st = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
      int n = Random.Range(4, 8);
      for (int i = 0; i < n; i++)
      {
        char c = st[Random.Range(0, st.Length)];
        username += c;
      }
      return username;
    }

    IEnumerator Display()
    {
      canvasGroup.alpha = 1f;
      yield return new WaitForSeconds(displayDuration);
      canvasGroup.alpha = 0f;
    }
  }
}