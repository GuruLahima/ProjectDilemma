using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GuruLaghima
{
  public class InputProcessor : SingletonBase<InputProcessor>
  {
    [Serializable]
    public class InputCheck{
      
    }

    List<Action> registeredMethods = new List<Action>();
    List<InputCheck> inputChecks = new List<InputCheck>();
    public void SubscribeMethodToInputChecks(Action method)
    {
      if (!registeredMethods.Contains(method))
        registeredMethods.Add(method);
    }

    // public static bool InputCheck(Func<bool> inputAxis)
    // {
    //   if (inputAxis())
    //     return true;
    //   return false;
    // }


    private void Update()
    {
      foreach (Action method in registeredMethods)
      {

      }
    }


  }
  
}