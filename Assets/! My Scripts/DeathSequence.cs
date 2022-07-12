using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using GuruLaghima;

namespace Workbench.ProjectDilemma
{
  public class DeathSequence : MonoBehaviour
  {
    #region Enum Declaration
    public enum OutcomeSequence : byte { Both = 0, PlayerOneWin = 1, PlayerTwoWin = 2}
    #endregion

    #region Public Fields
    // public DeathSequence_UI_Item ui_for_deathBook;
    public string labelText;
    public float duration;
    public Sprite backgroundSprite;
    public DeathSequenceData DeathSequenceData;
    #endregion
    /*/// <summary>
    /// Override to default (death sequences where both cooperate or betray)
    /// </summary>
    [SerializeField] Camera deathCam;*/

    #region Exposed Private Fields
    [Space(7, order = 0)]
    [Header("Outcomes:", order = 1)]
    [HorizontalLine(order = 2)]
    [SerializeField] private GameObject both;
    [SerializeField] private GameObject player1Win;
    [SerializeField] private GameObject player2Win;
    #endregion

    #region MonoBehavior
    private void Start()
    {
      Invoke("SelfDestruct", duration);
      if (DeathSequenceData)
      {
        GameEvents.OnDeathSequenceActivated?.Invoke(DeathSequenceData);
      }
    }
    #endregion

    #region Public Methods
    public void ActivateOutcome(OutcomeSequence outcome)
    {
      switch (outcome)
      {
        case OutcomeSequence.Both:
          if (both)
            both.SetActive(true);
          break;
        case OutcomeSequence.PlayerOneWin:
          if (player1Win)
            player1Win.SetActive(true);
          break;
        case OutcomeSequence.PlayerTwoWin:
          if (player2Win)
            player2Win.SetActive(true);
          break;
      }
    }
    #endregion

    #region Private Methods
    void SelfDestruct()
    {
      Destroy(this.gameObject);
    }
    #endregion
  }
}