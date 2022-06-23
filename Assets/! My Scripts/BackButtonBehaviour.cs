using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonBehaviour : SingletonBase<BackButtonBehaviour>
{
  #region public fields
  public static BackButtonBehaviour instance;
  #endregion

  #region private fields

  Stack<UndoableAction> undoableActionsStack = new Stack<UndoableAction>();
  #endregion

  #region public methods
  public void GoBackOneAction()
  {
    var lastAction = undoableActionsStack.Pop();
    lastAction.Execute();
  }
  public void AddOneAction(UndoableAction newAction)
  {
    undoableActionsStack.Push(newAction);
  }


  #endregion

}
