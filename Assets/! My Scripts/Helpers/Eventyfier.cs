
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Workbench.ProjectDilemma
{
  public class Eventyfier : MonoBehaviour
  {
    #region private fields

    private bool genericState;
    BoolWrapper genericStateWrapper = new BoolWrapper(false);

    private Coroutine stateTypeCoroutine;

    public static Action OnStartOfPress;
    public static Action OnEndOfPress;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
      /* example usage */
      /*       OnStartOfPress += () => { Debug.Log("OnStartOfPress triggered"); };
            OnEndOfPress += () => { Debug.Log("OnEndOfPress triggered"); };
            stateTypeCoroutine = StartCoroutine(EventifyFlagChange(genericStateWrapper, OnStartOfPress, OnEndOfPress)); */

    }

    // Update is called once per frame
    void Update()
    {
      /* example usage */
      // the check needed to be performed every frame
      /*       if (Input.GetButton("Vertical"))
            {

              genericState = true;
              genericStateWrapper.Value = true;
            }
            else
            {
              genericState = false;
              genericStateWrapper.Value = false;
            } */


    }

    #region private methods

    public static void Eventify(BoolWrapper state, Action OnStart = null, Action OnFinish = null, Action PreStart = null, Action WhileTrue = null, Action WhileFalse = null)
    {
      // StartCoroutine(EventifyFlagChange(state, OnStart, OnFinish, PreStart, WhileTrue, WhileFalse));
    }

    /// <summary>
    /// Use this function to send start and end event based on continuous input,
    /// use case: We might need to call some code ONLY the first frame some state changed in the game, while this state is continually sending, like a keypress
    /// more specifically: 
    /// </summary>
    /// <param name="state"></param>
    /// <param name="OnStart"></param>
    /// <param name="OnFinish"></param>
    /// <param name="PreStart"></param>
    /// <param name="WhileTrue"></param>
    /// <param name="WhileFalse"></param>
    /// <returns></returns>
    public static IEnumerator EventifyFlagChange(BoolWrapper state, Action OnStart = null, Action OnFinish = null, Action PreStart = null, Action WhileTrue = null, Action WhileFalse = null)
    {
      bool firstFrame = true;
      bool lastFrame = false;


      while (state != null)
      {
        if (state.Value == true)
        {
          if (firstFrame)
          {
            // Debug.Log("flag changed to true");
            firstFrame = false;
            lastFrame = true;

            // event code
            if (OnStart != null)
            {
              if (PreStart != null)
                PreStart();
              OnStart();
            }
          }
          else
          {
            // Debug.Log("Flag is constantly true");
            if (WhileTrue != null)
              WhileTrue();
          }
          yield return null;
        }
        else
        {

          if (lastFrame)
          {
            // Debug.Log("Flag reverted back to false");
            lastFrame = false;
            firstFrame = true;

            // event code
            if (OnFinish != null)
              OnFinish();
          }
          else
          {
            // Debug.Log("Flag is constantly false");
            if (WhileFalse != null)
              WhileFalse();
          }
          yield return null;
        }

      }

      yield return null;
    }

    #endregion
  }

  /*   public class BoolWrapper
    {
      public bool Value { get; set; }
      public BoolWrapper(bool value) { this.Value = value; }
    }

    public class ValueWrapper<T> where T : struct
    {
      public T Value { get; set; }
      public ValueWrapper(T value) { this.Value = value; }
    } */
}