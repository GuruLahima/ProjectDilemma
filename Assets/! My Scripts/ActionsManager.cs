using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GuruLaghima.ControllableSequence;

namespace GuruLaghima.ProjectDilemma
{

  public class ActionsManager : MonoBehaviour
  {

    public List<InvokableAction> possibleActions = new List<InvokableAction>();

    public void InvokeAction(string actionKey)
    {
      InvokableAction aktion = possibleActions.Find((obj) =>
            {
              return obj.name == actionKey;
            });
      if (aktion != null)
        aktion?.action?.Invoke();
    }


  }
}
