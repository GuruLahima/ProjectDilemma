using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOptionsCanvas : MonoBehaviour
{

  public void Load(){
    InGameSettingsManager.Instance.GetComponent<Animator>().SetTrigger("SettingsEnable");
  }
}
