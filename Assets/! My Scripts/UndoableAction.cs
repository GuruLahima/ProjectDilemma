using UnityEngine.Events;

public class UndoableAction
{
  public UnityEvent action;
  public void Execute()
  {
    action?.Invoke();
  }
}