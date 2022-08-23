using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GuruLaghima.ControllableSequence;

namespace GuruLaghima.ProjectDilemma
{

  public class BackButtonBehaviour : SingletonBase<BackButtonBehaviour>
  {
    #region public fields

    #endregion

    #region exposed fields

    [SerializeField] ActionsManager amgr;

    #endregion

    #region private fields

    // Stack<UndoableAction> undoableActionsStack = new Stack<UndoableAction>();
    [SerializeField][ReadOnly] Stack<InvokableAction> undoableActionsStack = new Stack<InvokableAction>();
    #endregion

    #region public methods
    public void GoBackOneAction()
    {
      if (undoableActionsStack.Count > 0)
      {
        var lastAction = undoableActionsStack.Pop();
        lastAction.Execute();
      }
    }
    // ? I haven't found a way to make this version work. I know I can make a custom UnityEvent type that takes an object parameter but I have no
    // ? way of  implementing that custom type in the already established classes without erasing all the data associated with the UnityEvent
    public void AddOneAction(InvokableAction newAction)
    {
      if (newAction != null)
        undoableActionsStack.Push(newAction);
    }

    public void AddOneAction(string actionKey)
    {
      InvokableAction action = amgr.possibleActions.Find((obj) =>
      {
        return obj.name == actionKey;
      });
      if (action != null)
        undoableActionsStack.Push(action);
    }

    #endregion

  }

}