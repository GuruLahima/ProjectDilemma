using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace Workbench.ProjectDilemma
{
  public class BaseActivatorComponent : MonoBehaviourPun
  {
    #region Public Events
    [Foldout("Events")]
    [Header("These events are updated and called every frame when on cooldown", order = 0)]
    [HorizontalLine(order = 1)]
    [Label("Event that takes the cooldown parameter with value between 0 and 1")]
    public UnityEvent<float> OnCooldownNormalized;
    [Foldout("Events")]
    [Label("Event that takes the cooldown parameter as it is")]
    public UnityEvent<float> OnCooldownRaw;

    [Header("These events are called once, when the player clicks the hotkey", order = 0)]
    [HorizontalLine (order = 1)]
    [Foldout("Events")]
    public UnityEvent OnSelected;
    [Foldout("Events")]
    public UnityEvent OnDeselected;
    #endregion

    /// <summary>
    /// Used by DOTweens mainly to call UnityEvents with this value as parameter
    /// </summary>
    public float CooldownNormalized
    {
      get
      {
        return _cooldownNormalized;
      }
      set
      {
        _cooldownNormalized = value;
        OnCooldownNormalized?.Invoke(_cooldownNormalized);
      }
    }
    private float _cooldownNormalized;
    public bool ActiveState
    {
      set
      {
        if (value)
        {
          OnSelected?.Invoke();
        }
        else
        {
          OnDeselected?.Invoke();
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
      Tween myTween;
      myTween = DOTween.To(() => CooldownNormalized = 0, x => CooldownNormalized = x, 1f, amount);
    }
    /// <summary>
    /// Add this ability's base cooldown to the cooldown
    /// </summary>
    protected void AddCooldown()
    {
      NextInputReady = InputCooldown + Time.time;
      Tween myTween;
      CooldownNormalized = 0;
      myTween = DOTween.To(() => CooldownNormalized = 0, x => CooldownNormalized = x, 1f, InputCooldown);
    }
  }
}