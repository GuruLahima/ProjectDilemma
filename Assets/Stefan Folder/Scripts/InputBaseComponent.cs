using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
  public class InputBaseComponent : MonoBehaviourPun
  {
    [SerializeField] protected UnityEngine.UI.Image Icon;
    [SerializeField] protected UnityEngine.UI.Outline IconOutline;
    public bool ActiveState
    {
      set
      {
        if (value)
        {
          IconOutline.enabled = true;
        }
        else
        {
          IconOutline.enabled = false;
        }
      }
    }
    public float InputCooldown;
    [HideInInspector] public float NextInputReady;
    public bool OnCooldown
    {
      get
      {
        if (NextInputReady > Time.time)
        {
          return true;
        }
        else
        {
          return false;
        }
      }
    }


    public virtual void Init()
    {

    }

    /// <summary>
    /// Add custom <paramref name="amount"/> of seconds to the cooldown
    /// </summary>
    /// <param name="amount"></param>
    protected void AddCooldown(float amount)
    {
      NextInputReady = amount + Time.time;
      if (Icon != null)
      {
        Icon.fillAmount = 0f;
        Tween myTween;
        myTween = DOTween.To(() => Icon.fillAmount, x => Icon.fillAmount = x, 1f, amount);
      }
    }

    /// <summary>
    /// Add this ability's base cooldown to the cooldown
    /// </summary>
    protected void AddCooldown()
    {
      NextInputReady = InputCooldown + Time.time;
      if (Icon != null)
      {
        Icon.fillAmount = 0f;
        Tween myTween;
        myTween = DOTween.To(() => Icon.fillAmount, x => Icon.fillAmount = x, 1f, InputCooldown);
      }
    }
  }
}