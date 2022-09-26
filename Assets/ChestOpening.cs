using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Workbench.ProjectDilemma;

public class ChestOpening : MonoBehaviour
{
  public UnityEvent OnChestOpened;
  public UnityEvent OnChestOpenDelay;
  public UnityEvent OnChestRevealed;
  public UnityEvent OnChestClosed;
  public UnityEvent OnClaimChest;
  public Image image0;
  public Image image1;
  public Image image2;
  public Canvas canvas;
  [SerializeField] float openDelay;
  [HideInInspector]
  public ChestView chestView;

  private int revealedCards = 0;

  public void Open()
  {
    if (chestView)
    {
      chestView.OpenChest();
    }
    OnChestOpened?.Invoke();
    Invoke("OpenDelay", openDelay);

  }

  public void Reveal(int i)
  {
    revealedCards += i;
    if (revealedCards >= 3)
    {
      OnChestRevealed?.Invoke();
    }
  }

  public void Cancel()
  {
    if (chestView)
    {
      chestView.Cancel();
    }
    OnChestClosed?.Invoke();
  }

  private void OpenDelay()
  {
    OnChestOpenDelay?.Invoke();
  }

  public void ClaimReward()
  {
    if (chestView)
    {
      chestView.ClaimChest();
    }
    OnClaimChest?.Invoke();
  }
}
