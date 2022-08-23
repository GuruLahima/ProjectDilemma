using UnityEngine.Events;

public class UndoableAction
{
  public UnityEvent theReverseAction;
  public void Execute()
  {
    theReverseAction?.Invoke();
  }
}