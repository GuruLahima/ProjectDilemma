using System.Collections;
using System.Collections.Generic;

namespace GuruLaghima
{
  public interface ISequenceExecutor
  {
    public IEnumerator SequenceCoroutine(List<ControllableSequence.EventWithDuration> eventSequence, string name);
  }
}